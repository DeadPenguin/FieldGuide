using FieldGuide.Views;
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
    public class SearchViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }

        public ICommand AddTag { get; }
        public ICommand RemoveLastTag { get; }
        public ICommand ReturnCommand { get; }
        public ICommand StartSearch { get; }
        public ICommand NewSearch { get; }

        public ObservableCollection<Tag> CurrentTags { get; set; }
        public ObservableCollection<string> AvailableTags { get; set; }
        private int Root { get; set; }
        private string _newTag;
        private string _newValue;

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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new
                PropertyChangedEventArgs(propertyName));
        }

        public SearchViewModel(INavigation navigation, int root)
        {
            Navigation = navigation;
            Root = root;
            CurrentTags = new ObservableCollection<Tag>();
            ReturnCommand = new Command(OnReturnCommand);
            AddTag = new Command(OnAddTag);
            RemoveLastTag = new Command(OnRemoveLastTag);
            StartSearch = new Command(OnStartSeach);
            NewSearch = new Command(OnNewSearch);

            //Search Realm for all available Tags in the current subtree
            List<string> temp = RealmManager.FindAvailableTags(root);
            AvailableTags = new ObservableCollection<string>();
            foreach (string s in temp)
                AvailableTags.Add(s);
            Debug.WriteLine(AvailableTags.Count);

        }

        private void OnNewSearch(object obj)
        {
            CurrentTags.Clear();
        }

        private async void OnStartSeach(object obj)
        {
            List<Entry> entries = RealmManager.GetResults(CurrentTags, Root);
            List<Result> results = new List<Result>();

            foreach (Entry e in entries)
            {
                Result r = await Result.CreateResult(e);
                results.Add(r);
            }
            await Navigation.PushAsync(new SearchResults(entries, results));
        }

        private async void OnReturnCommand()
        {
            await Navigation.PopAsync();
        }

        private void OnRemoveLastTag()
        {
            if(CurrentTags.Count > 0)
                CurrentTags.RemoveAt(CurrentTags.Count - 1);
        }

        private void OnAddTag()
        {
            Tag tag = new Tag(NewTag, NewValue);
            CurrentTags.Add(tag);
            NewTag = "";
            NewValue = "";
        }

    }
}
