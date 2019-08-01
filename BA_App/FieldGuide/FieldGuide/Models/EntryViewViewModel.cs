using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static FieldGuide.Models.WorkModel;

namespace FieldGuide.Models
{
    class EntryViewViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public ICommand ToggleView { get; }
        public ICommand Return { get; }

        public ObservableCollection<Tag> Tags { get; set; }
        private StreamImageSource _imageSource;

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _description;
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
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new
                PropertyChangedEventArgs(propertyName));
        }

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

        public EntryViewViewModel(INavigation navigation, Entry e)
        {
            Navigation = navigation;
            ToggleView = new Command(OnToggleView);
            Return = new Command(OnReturn);
            Tags = new ObservableCollection<Tag>();
            Name = e.Name;
            Description = e.Description;

            foreach (Tag t in e.Tags)
                Tags.Add(t);

            TagsVisible = true;
            DescriptionVisible = false;

            RecoverImage(e.ImagePath);
        }

        private async void RecoverImage(string path)
        {
            ImageSource = await FileManager.GetImage(path);
        }

        private async void OnReturn()
        {
            await Navigation.PopAsync();
        }

        private void OnToggleView()
        {
            TagsVisible = !TagsVisible;
            DescriptionVisible = !DescriptionVisible;
        }
    }
}
