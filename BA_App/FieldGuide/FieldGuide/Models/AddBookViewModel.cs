using FieldGuide.Utilities;
using FieldGuide.Views;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;
using static FieldGuide.Models.WorkModel;

namespace FieldGuide.Models
{    public class AddBookViewModel : INotifyPropertyChanged
    {
        public string Caption { get; set; }
        public INavigation Navigation { get; set; }
        private int Folder { get; set; }

        public ICommand SaveBook { get; }
        public ICommand DiscardBook { get; }
        public ICommand ToggleViews { get; }
        public ICommand NewBookEntry { get; }
        public ICommand Return { get; }

        //Variables for book info
        private WorkBook _newBook;
        private String _bookTags;

        public WorkBook NewBook
        {
            get { return _newBook; }
            set
            {
                _newBook = value;
                OnPropertyChanged(nameof(NewBook));
            }
        }
        public String BookTags
        {
            get { return _bookTags; }
            set
            {
                _bookTags = value;
                OnPropertyChanged(nameof(BookTags));
            }
        }

        private WorkEntry _selectedEntry;
        public WorkEntry SelectedEntry
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
                string name = SelectedEntry.Name;
                _selectedEntry = null;
                await Navigation.PushAsync(new EditBookEntries(NewBook, name));
            }
        }

        //Variables for book entry-list
        public ObservableCollection<WorkEntry> Entries { get; set; }

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

        public AddBookViewModel(INavigation navigation, int id)
        {
            Navigation = navigation;
            SaveBook = new Command(OnSaveBook);
            DiscardBook = new Command(OnDiscardBook);
            ToggleViews = new Command(OnToggleViews);
            NewBookEntry = new Command(OnNewBookEntry);
            Return = new Command(OnReturn);
            Entries = new ObservableCollection<WorkEntry>();

            NewBook = new WorkBook();
            NewBook.Id = RealmManager.CreateId();
            NewBook.ParentId = id;
            NewBook.BookTags = new List<string>();
            NewBook.Entries = new List<WorkEntry>();

            BookInfoVisible = true;
            BookEntriesVisible = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new
                PropertyChangedEventArgs(propertyName));
        }

        private async void OnSaveBook()
        {
            bool checkTitle, checkAuthor, checkTags, checkEntries;
            string[] tags;

            checkTitle = NewBook.Title == "" || NewBook.Title == null ? false : true;
            checkAuthor = NewBook.Author == "" || NewBook.Author == null ? false : true;
            checkTags = BookTags == null || BookTags == "" ? false : true;

            if (!(checkTitle && checkAuthor && checkTags))
            {
                AlertManager.IncompleteInfo();
                return;
            }

            if (NewBook.Entries == null || NewBook.Entries.Count == 0)
            {
                checkEntries = await AlertManager.NoEntries();
                if (!checkEntries)
                    return;
            }

            tags = Regex.Split(BookTags, ", ");
            NewBook.BookTags.Clear();
            foreach (String s in tags)
                NewBook.BookTags.Add(s);

            HashSet<WorkTag> entryTags = new HashSet<WorkTag>();
            NewBook.EntryTags.Clear();

            foreach (WorkEntry e in NewBook.Entries)
            {
                if (e.ImageChanged)
                {
                    await FileManager.AddImage(e.ImagePath, e.NewImageData);
                }
                foreach (WorkTag t in e.Tags)
                    NewBook.EntryTags.Add(t.Name);
            }

            Debug.WriteLine(NewBook.ToString());
            RealmManager.AddBook(NewBook);

            await Navigation.PopAsync();
        }

        private async void OnDiscardBook()
        {
            bool result = await AlertManager.DiscardBook();

            if(result)
                await Navigation.PopAsync();
        }

        private void OnToggleViews()
        {
            BookInfoVisible = !BookInfoVisible;
            BookEntriesVisible = !BookEntriesVisible;
        }

        private async void OnNewBookEntry()
        {
            await Navigation.PushAsync(new EditBookEntries(NewBook));
        }

        private async void OnReturn()
        {
            OnDiscardBook();
        }

        public void update()
        {
            Entries.Clear();
            foreach (WorkEntry e in NewBook.Entries)
            {
                Entries.Add(e);
            }
        }
    }
}
