using FieldGuide.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static FieldGuide.Models.WorkModel;

namespace FieldGuide.Models
{
    class BookViewViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }

        public ICommand Return { get; }
        public ICommand ToggleViews { get; }

        private Book _currentBook { get; set; }
        public Book CurrentBook
        {
            get { return _currentBook; }
            set
            {
                if (_currentBook != value)
                    _currentBook = value;
                OnPropertyChanged(nameof(CurrentBook));
                HandleSelectedItem();
            }
        }

        private String _bookTags { get; set; }
        public String BookTags {
            get { return _bookTags; }
            set
            {
                if (_bookTags != value)
                    _bookTags = value;
                OnPropertyChanged(nameof(BookTags));
                HandleSelectedItem();
            }
        }

        public ObservableCollection<Result> Entries { get; set; }

        private Result _selectedEntry;
        public Result SelectedEntry
        {
            get { return _selectedEntry; }
            set
            {
                if (_selectedEntry != value)
                    _selectedEntry = value;
                OnPropertyChanged(nameof(SelectedEntry));
                HandleSelectedItem();
            }
        }

        private async void HandleSelectedItem()
        {
            if (SelectedEntry != null)
            {
                Result temp = _selectedEntry;
                _selectedEntry = null;

                foreach(Entry e in CurrentBook.Entries)
                {
                    if (e.Name == temp.Name)
                    {
                        await Navigation.PushAsync(new EntryView(e));
                        break;
                    }
                }
            }
        }

        //Variables to manage view-visibility
        private bool _bookInfoVisible;
        private bool _bookEntriesVisible;
        public bool BookInfoVisible
        {
            get { return _bookInfoVisible; }
            set
            {
                _bookInfoVisible = value;
                OnPropertyChanged(nameof(BookInfoVisible));
            }
        }
        public bool BookEntriesVisible
        {
            get { return _bookEntriesVisible; }
            set
            {
                _bookEntriesVisible = value;
                OnPropertyChanged(nameof(BookEntriesVisible));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new
                PropertyChangedEventArgs(propertyName));
        }

        public BookViewViewModel(INavigation navigation, int id)
        {
            Navigation = navigation;

            Return = new Command(OnReturn);
            ToggleViews = new Command(OnToggleViews);

            CurrentBook = RealmManager.FindBookById(id);
            Entries = new ObservableCollection<Result>();
            BookInfoVisible = true;
            BookEntriesVisible = false;

            BookTags = "";
            int n = CurrentBook.BookTags.Count;
            for (int i = 0; i < n - 1; i++)
                BookTags += ($"{CurrentBook.BookTags[i]}, ");
            BookTags += CurrentBook.BookTags[n - 1];

            foreach (Entry e in CurrentBook.Entries)
                Entries.Add(new Result(e));
        }

        private void OnToggleViews()
        {
            BookInfoVisible = !BookInfoVisible;
            BookEntriesVisible = !BookEntriesVisible;
        }

        private async void OnReturn()
        {
                await Navigation.PopAsync();
        }
    }
}
