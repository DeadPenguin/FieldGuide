using FieldGuide.Utilities;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static FieldGuide.Models.WorkModel;

namespace FieldGuide.Models
{
    public class EntryEditViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }

        public ICommand AddImage { get; }
        public ICommand AddTag { get; }
        public ICommand RemoveLastTag { get; }
        public ICommand SaveEntry { get; }
        public ICommand DiscardEntry { get; }
        public ICommand ToggleView { get; }
        public ICommand Return { get; }

        private bool _tagsVisible;
        private bool _descriptionVisible;
        public bool TagsVisible
        {
            get { return _tagsVisible; }
            set
            {
                _tagsVisible = value;
                OnPropertyChanged(nameof(TagsVisible));
            }
        }
        public bool DescriptionVisible
        {
            get { return _descriptionVisible; }
            set
            {
                _descriptionVisible = value;
                OnPropertyChanged(nameof(DescriptionVisible));
            }
        }

        private WorkBook NewBook { get; set; }
        private WorkEntry NewEntry { get; set; }
        public ObservableCollection<WorkTag> CurrentTags { get; set; }
        private string _newName;
        private string _newTag;
        private string _newValue;
        private string _description;

        private StreamImageSource _imageSource;
        private FileData Image { get; set; }

        public string NewName
        {
            get { return _newName; }
            set
            {
                _newName = value;
                OnPropertyChanged(nameof(NewName));
            }
        }

        public string NewTag
        {
            get { return _newTag; }
            set
            {
                _newTag = value;
                OnPropertyChanged(nameof(NewTag));
            }
        }

        public string NewValue
        {
            get { return _newValue; }
            set
            {
                _newValue = value;
                OnPropertyChanged(nameof(NewValue));
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public StreamImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                Debug.WriteLine("Source changed");
                OnPropertyChanged(nameof(ImageSource));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new
                PropertyChangedEventArgs(propertyName));
        }

        public EntryEditViewModel(INavigation navigation, WorkBook book, string entry)
        {
            Navigation = navigation;
            NewBook = book;
            CurrentTags = new ObservableCollection<WorkTag>();
            AddImage = new Command(OnAddImage);
            AddTag = new Command(OnAddTag);
            RemoveLastTag = new Command(OnRemoveLastTag);
            SaveEntry = new Command(OnSaveEntry);
            DiscardEntry = new Command(OnDiscardEntry);
            ToggleView = new Command(OnToggleView);
            Return = new Command(OnReturn);

            TagsVisible = true;
            DescriptionVisible = false;

            RecoverValues(entry);
        }

        private async void RecoverValues(string entry)
        {
            foreach (WorkModel.WorkEntry e in NewBook.Entries)
                if (entry == e.Name)
                {
                    NewEntry = e;
                    break;
                }

            if (NewEntry.ImageChanged)
            {
                ImageSource = (StreamImageSource)Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(NewEntry.NewImageData));
            }
            else
            {
                NewEntry.OldImagePath = NewEntry.ImagePath;
                ImageSource = await FileManager.GetImage(NewEntry.ImagePath);
            }
            
            NewName = NewEntry.Name;
            Description = NewEntry.Description;
            foreach (WorkTag t in NewEntry.Tags)
                CurrentTags.Add(t);
        }

        private async void OnAddImage()
        {
            string[] types = { ".jpg", ".png" };
            Image = await CrossFilePicker.Current.PickFile(types);
            if (Image == null)
            {
                return;
            }

            ImageSource = (StreamImageSource)Xamarin.Forms.ImageSource.FromStream(() => Image.GetStream());
        }

        private void OnRemoveLastTag()
        {
            if (NewEntry.Tags.Count > 0)
            {
                WorkTag temp = NewEntry.Tags[NewEntry.Tags.Count - 1];
                NewBook.EntryTags.Remove(temp.Name);
                NewEntry.Tags.RemoveAt(NewEntry.Tags.Count - 1);
                CurrentTags.RemoveAt(CurrentTags.Count - 1);
            }
        }

        private void OnAddTag()
        {
            WorkTag tag = new WorkTag(NewTag, NewValue);
            NewEntry.Tags.Add(tag);
            CurrentTags.Add(tag);
            NewTag = "";
            NewValue = "";
        }

        private async void OnSaveEntry()
        {
            bool checkName, checkDescription, checkTags, checkImage;

            checkName = NewName == "" || NewName == null ? false : true;
            checkTags = NewEntry.Tags == null || NewEntry.Tags.Count == 0 ? false : true;
            checkImage = Image == null ? false : true;

            if (!(checkName && checkImage))
            {
                AlertManager.IncompleteEntry();
                return;
            }

            if (!checkTags)
            {
                AlertManager.NoTags();
                return;
            }

            if (Description == "" || Description == null)
            {
                checkDescription = await AlertManager.NoDescription();
                if (!checkDescription)
                    return;
            }

            NewEntry.Name = NewName;
            NewEntry.Description = Description;

            if (checkImage)
            {
                NewEntry.NewImageData = Image.DataArray;
                NewEntry.ImagePath = FileManager.CombineImage(Image.FileName, NewBook.Id, NewEntry.Name);
                NewEntry.ImageChanged = true;
            }

            await Navigation.PopAsync();
        }

        private async void OnDiscardEntry()
        {
            bool result = await AlertManager.DiscardEntry();

            if (result)
            {
                NewBook.RemovedImages.Add(NewEntry.ImagePath);
                NewBook.Entries.RemoveAt(NewBook.Entries.IndexOf(NewEntry));
                await Navigation.PopAsync();
            }
        }

        private void OnToggleView()
        {
            DescriptionVisible = !DescriptionVisible;
            TagsVisible = !TagsVisible;
        }

        private async void OnReturn()
        {
            bool result = await AlertManager.Return();

            if (result)
                await Navigation.PopAsync();
        }
    }
}
