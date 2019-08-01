using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using FieldGuide.Models;
using System.Linq;
using System.Diagnostics;
using Realms.Exceptions;
using Rg.Plugins.Popup.Services;
using FieldGuide.Modals;
using Xamarin.Essentials;
using FieldGuide.Views;
using System.IO;
using System.Reflection;
using System.Resources;
using FieldGuide.Utilities;

namespace FieldGuide.Models
{
    public class FileBrowser : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new
                PropertyChangedEventArgs(propertyName));
        }

        //holds id of current root-element of FileSystem
        public static int Root { get; set; }

        //holds all children of current root-element to be displayed
        public ObservableCollection<FileItem> Files { get; set; }

        public ICommand PlusCommand { get; }
        public ICommand ReturnCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand MenuCommand { get; }
        public ICommand Help { get; }
        public ICommand NewFolder { get; }
        public ICommand Import { get; }

        private FileItem selectedItem;
        public FileItem SelectedItem {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                    selectedItem = value;
                HandleSelectedItem();
            }
        }

        private async void HandleSelectedItem()
        {
            if (SelectedItem != null)
            {
                if (SelectedItem.Type == 0)
                {
                    Root = SelectedItem.Id;
                    Caption = SelectedItem.Title;
                    RefreshBrowser();
                } else
                {
                    await PopupNavigation.Instance.PushAsync(new BookAction(this));
                }
            }
        }

        private bool menu;
        private bool list;

        public bool Menu
        {
            get { return menu; }
            set
            {
                menu = value;
                OnPropertyChanged(nameof(Menu));
            }
        }

        public bool List
        {
            get { return list; }
            set
            {
                list = value;
                OnPropertyChanged(nameof(List));
            }
        }

        private String caption;
        public String Caption
        {
            get { return caption; }
            set
            {
                if (caption != value)
                    caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        //Receives Name for new File from Popup
        public String NewFolderName { get; set; }

        public FileBrowser(INavigation navigation)
        {
            Debug.WriteLine($"BaseDir: {FileSystem.AppDataDirectory}");
            Root = 0;
            Navigation = navigation;
            PlusCommand = new Command(OnPlusCommand);
            ReturnCommand = new Command(OnReturnCommand);
            SearchCommand = new Command(OnSearchCommand);
            MenuCommand = new Command(OnMenuCommand);
            Help = new Command(OnHelpCommand);
            NewFolder = new Command(OnNewFolder);
            Import = new Command(OnImport);
            Menu = false;
            List = true;
            Caption = "Field Guide";
            Files = new ObservableCollection<FileItem>();

            //Delete last Databse for testing purposes
            //var config = new RealmConfiguration();
            //Realm.DeleteRealm(config);

            Realm LocalRealm = Realm.GetInstance();

            //retrieve all Folders and Books in the root-file
            RefreshBrowser();
        }

        private async void OnPlusCommand() {
            //change View to Book Creation
            await Navigation.PushAsync(new EditBook(Root, false));
        }
        private void OnReturnCommand()
        {
            if (Root != 0)
            {
                Folder f = RealmManager.GetParent(Root);

                if (f == null)
                {
                    Root = 0;
                    Caption = "FieldGuide";
                } else
                {
                    Root = f.Id;
                    Caption = f.Title;
                }
                RefreshBrowser();
            }  
        }

        private async void OnSearchCommand()
        {
            await Navigation.PushAsync(new Search(Root));
        }
        private void OnMenuCommand()
        {
            Menu = !Menu;
            List = !List;
        }

        public async void OnHelpCommand()
        {
            await Browser.OpenAsync("https://github.com/DeadPenguin/FieldGuide");
            OnMenuCommand();
        }

        private async void OnNewFolder()
        {
            await PopupNavigation.Instance.PushAsync(new NewFolder(this));
        }

        private async void OnImport()
        {
            await FileManager.Import(Root);
            OnMenuCommand();
            RefreshBrowser();
        }

        public void CreateNewFolder()
        {
            Folder newFolder = new Folder(Root, NewFolderName);
            RealmManager.AddFolder(newFolder);
            OnMenuCommand();
            RefreshBrowser();
        }

        public void RefreshBrowser()
        {
            Realm LocalRealm = RealmManager.DirectAccess();
            Files.Clear();
            List<Folder> Folders = LocalRealm.All<Folder>().Where(f => f.ParentId == Root).ToList();
            List<Book> Books = LocalRealm.All<Book>().Where(b => b.ParentId == Root).ToList();
            
            foreach (Folder f in Folders)
            {
                Files.Add(new FileItem(f.Id, f.Title, 0));
            }
            foreach (Book b in Books)
            {
                Files.Add(new FileItem(b.Id, b.Title, 1));
            }
        }
    }
}
