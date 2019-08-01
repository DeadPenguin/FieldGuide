using FieldGuide.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static FieldGuide.Models.WorkModel;

namespace FieldGuide.Models
{
    class SearchResultsViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public ObservableCollection<Tag> SearchTags { get; set; }
        public ObservableCollection<Result> MatchingEntries { get; set; }
        public List<Entry> Entries { get; set; }
        public int Root { get; set; }

        public ICommand ReturnCommand { get; }
        public ICommand ListView { get; }
        public ICommand TileView { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new
                PropertyChangedEventArgs(propertyName));
        }

        private Result _selectedResult;
        public Result SelectedResult
        {
            get { return _selectedResult; }
            set
            {
                if (_selectedResult != value)
                    _selectedResult = value;
                OnPropertyChanged(nameof(SelectedResult));
                HandleSelectedItem();
            }
        }

        private async void HandleSelectedItem()
        {
            if (SelectedResult != null)
            {
                foreach (Entry e in Entries)
                    if (e.Name == _selectedResult.Name) {
                        await Navigation.PushAsync(new EntryView(e));
                        break;
                    }
                _selectedResult = null;
            }
        }
        public SearchResultsViewModel(INavigation navigation, List<Entry> entries, List<Result> results)
        {
            Navigation = navigation;
            ReturnCommand = new Command(OnReturnCommand);
            Entries = entries;
            MatchingEntries = new ObservableCollection<Result>();

            foreach (Result r in results)
                MatchingEntries.Add(r);
        }

        private async void OnReturnCommand(object obj)
        {
            await Navigation.PopAsync();
        }
    }
}
