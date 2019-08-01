using FieldGuide.Utilities;
using Newtonsoft.Json;
using PCLStorage;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Xamarin.Forms;
using FileAccess = PCLStorage.FileAccess;
using FileSystem = PCLStorage.FileSystem;

namespace FieldGuide.Models
{
    public static class FileManager
    {
        private static IFolder RootDir = null;
        private static IFolder ImageDir = null;

        private static async Task CheckInitialized()
        {
            if(RootDir == null)
                RootDir = FileSystem.Current.LocalStorage;
            if(ImageDir == null)
                ImageDir = await RootDir.CreateFolderAsync("images", 
                    CreationCollisionOption.OpenIfExists);
        }

        public static async Task AddImage(string imageName, byte[] data)
        {
            await CheckInitialized();

            IFile image = await ImageDir.CreateFileAsync(
                imageName, CreationCollisionOption.ReplaceExisting);

            using (var fileHandler = await image.OpenAsync(
                FileAccess.ReadAndWrite))
            {
                await fileHandler.WriteAsync(data, 0, data.Length);
            }
        }

        public static async Task RemoveImage(string name)
        {
            await CheckInitialized();

            IFile image = await ImageDir.GetFileAsync(name);
            if (image != null)
            {
                await image.DeleteAsync();
            }
        }

        public static async Task<StreamImageSource> GetImage(string filename)
        {
            await CheckInitialized();

            IFile image = await ImageDir.GetFileAsync(filename);
            Stream stream = await image.OpenAsync(FileAccess.Read);
            return (StreamImageSource)ImageSource.FromStream(() => stream);
        }

        public static async Task ReplaceImage(string oldName, string newName, byte[] data)
        {
            await CheckInitialized();

            RemoveImage(oldName);
            AddImage(newName, data);
        }

        public static string CombineImage(string formerName, int bookId, string entryName)
        {
            string[] splits = formerName.Split('.');
            string img = $"{bookId}_{entryName}.{splits[1]}";
            return img;
        }

        // Following functions are necesarry for exporting and importing.

        private static string BookToJson(Book book)
        {
            string json = JsonConvert.SerializeObject(book);
            return json;
        }

        private static Book JsonToBook(string json)
        {
            Book book = JsonConvert.DeserializeObject<Book>(json);
            return book;
        }

        private static async Task<string> Zip(string sourcePath, string bookTitle)
        {
            await CheckInitialized();

            ExistenceCheckResult exists = await RootDir.CheckExistsAsync($"{bookTitle}.zip");
            if (exists == ExistenceCheckResult.FileExists)
            {
                IFile temp = await RootDir.GetFileAsync($"{bookTitle}.zip");
                await temp.DeleteAsync();
            }
            string targetPath = RootDir.Path + $"\\{bookTitle}.zip";
            ZipFile.CreateFromDirectory(sourcePath, targetPath);

            return targetPath;
        }

        private static async Task<Tuple<string, string>> Unzip(string sourcePath)
        {
            await CheckInitialized();

            char[] splitChars = { '\\', '.' };
            string[] split = sourcePath.Split(splitChars);
            string name = split[split.Length - 2];
            string newPath = $"{RootDir.Path}\\{name}";
            Debug.WriteLine($"RootDir: {RootDir.Path}, Unzip Path: {newPath}");
            
            if(await RootDir.CheckExistsAsync(name) == ExistenceCheckResult.FolderExists)
            {
                IFolder old = await RootDir.GetFolderAsync(name);
                await old.DeleteAsync();
            }
            ZipFile.ExtractToDirectory(sourcePath, newPath);

            return new Tuple<string, string>(name, newPath);
        }

        public static async Task<IFile> CopyZip(string zipName, byte[] data)
        {
            await CheckInitialized();

            IFile zip = await RootDir.CreateFileAsync(zipName, CreationCollisionOption.ReplaceExisting);
            using (var fileHandler = await zip.OpenAsync(FileAccess.ReadAndWrite))
            {
                await fileHandler.WriteAsync(data, 0, data.Length);
            }
            return zip;
        }

        public static async Task Export(Book book)
        {
            await CheckInitialized();

            string json = BookToJson(book);
            IFolder folder = await RootDir.CreateFolderAsync(book.Title, CreationCollisionOption.ReplaceExisting);
           
            foreach(Entry e in book.Entries)
            {
                IFile image = await ImageDir.GetFileAsync(e.ImagePath);
                IFile target = await folder.CreateFileAsync(e.ImagePath, CreationCollisionOption.ReplaceExisting);
                using (var targetStream = await target.OpenAsync(FileAccess.ReadAndWrite))
                using (var imageStream = await image.OpenAsync(FileAccess.Read))
                    await imageStream.CopyToAsync(targetStream);
            }

            IFile jsonFile = await folder.CreateFileAsync("content.json", CreationCollisionOption.ReplaceExisting);
            await jsonFile.WriteAllTextAsync(json);

            string exportPath = await Zip(folder.Path, book.Title);
            await folder.DeleteAsync();

            AlertManager.ExportPath(exportPath);
        }

        public static async Task Import(int parentId)
        {
            await CheckInitialized();

            string[] types = {".zip"};
            FileData zipFile = await CrossFilePicker.Current.PickFile(types);

            if (zipFile == null)
                return;

            IFile newZip = await CopyZip(zipFile.FileName, zipFile.DataArray);

            Tuple<string, string> values = await Unzip(newZip.Path);

            IFolder import = await RootDir.GetFolderAsync(values.Item1);
            IFile json = await import.GetFileAsync("content.json");
            string jsonText = await json.ReadAllTextAsync();

            Book b = JsonToBook(jsonText);
            Debug.WriteLine($"NewBook: {b.ToString()}");
            RealmManager.ImportBook(b, parentId);
            IFile old;

            foreach (Entry e in b.Entries)
            {
                string[] splits = e.ImagePath.Split('_');
                string oldPath = $"{b.Id}_{splits[1]}";
                ExistenceCheckResult exists = await import.CheckExistsAsync(oldPath);
                if (exists == ExistenceCheckResult.NotFound)
                {
                    Debug.WriteLine($"Image not Found at: {e.ImagePath}");
                    continue;
                }
                old = await import.GetFileAsync(oldPath);
                await old.MoveAsync($"{ImageDir.Path}\\{old.Name}", NameCollisionOption.ReplaceExisting);
            }

            await import.DeleteAsync();
            await newZip.DeleteAsync();
        }
    }
}
