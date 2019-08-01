using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FieldGuide.Models.WorkModel;

namespace FieldGuide.Models
{
    public static class RealmManager
    {
        private static Realm LocalRealm = null;

        private static void CheckInitialized()
        {
            if(LocalRealm == null)
                LocalRealm = Realm.GetInstance();
        } 

        public static int CreateId()
        {
            return DateTime.Now.GetHashCode();
        }

        public static void AddBook(WorkBook newBook)
        {
            CheckInitialized();

            Book realmBook = newBook.ToRealm();
            
            LocalRealm.Write(() =>
            {
                LocalRealm.Add(realmBook);
            });
        }
        public static void UpdateBook(WorkBook editedBook)
        {
            CheckInitialized();

            Book newBook = editedBook.ToRealm();

            LocalRealm.Write(() =>
            {
                LocalRealm.Add(newBook, update: true);
            });
        }
        public static void ImportBook(Book book, int parentId)
        {
            CheckInitialized();

            book.ParentId = parentId;

            foreach (Entry e in book.Entries)
            {
                string[] parts = e.ImagePath.Split('_');
                e.ImagePath = $"{book.Id}_{parts[1]}";
            }

            LocalRealm.Write(() =>
            {
                LocalRealm.Add(book);
            });
        }
        public static void RemoveBookById(int id)
        {
            CheckInitialized();

            Book oldBook = FindBookById(id);
            LocalRealm.Write(() =>
            {
                LocalRealm.Remove(oldBook);
            });
        }
        public static Realm DirectAccess()
        {
            CheckInitialized();

            return LocalRealm;
        }

        public static void AddFolder(Folder newFolder)
        {
            CheckInitialized();

            newFolder.Id = CreateId();

            LocalRealm.Write(() =>
            {
                LocalRealm.Add(newFolder);
            });
        }

        public static List<Book> FindBook(String title)
        {
            CheckInitialized();

            List<Book> temp = LocalRealm.All<Book>().Where(b => b.Title == title).ToList();
            return temp;
        }

        public static Book FindBookById(int id)
        {
            CheckInitialized();

            List<Book> temp = LocalRealm.All<Book>().Where(b => b.Id == id).ToList();
            if (temp.Count < 1)
                return null;
            return temp[0];
        }

        public static Folder GetParent(int id)
        {
            CheckInitialized();

            Folder temp = LocalRealm.Find<Folder>(id);
            if (temp.ParentId == 0)
                return null;
            else
            {
                temp = LocalRealm.Find<Folder>(temp.ParentId);
                return temp;
            }
        }

        //Recursively get all books in all subfolders from current root
        public static List<Book> GetSubtree(int root)
        {
            CheckInitialized();

            List<Book> books = LocalRealm.All<Book>().Where(b => b.ParentId == root).ToList();
            List<Folder> folders = LocalRealm.All<Folder>().Where(f => f.ParentId == root).ToList();

            foreach (Folder f in folders)
                books.AddRange(GetSubtree(f.Id));

            if (books.Count == 0 && folders.Count == 0)
                return new List<Book>();

            return books;
        }

        //Get a set of all available entry tags in the current subtree
        public static List<string> FindAvailableTags(int root)
        {
            CheckInitialized();

            List<Book> books = GetSubtree(root);

            HashSet<string> tags = new HashSet<string>();
            foreach (Book b in books) {
                foreach (String s in b.EntryTags) {
                    tags.Add(s);
                }
            }

            return tags.ToList();
        }

        public static List<Entry> GetResults(ObservableCollection<Tag> tags, int root)
        {
            CheckInitialized();

            int count;
            bool check;
            List<Book> bookstemp = GetSubtree(root);
            List<Book> books = new List<Book>();
            List<Entry> entries = new List<Entry>();

            List<string> tagNames = new List<String>();
            foreach(Tag t in tags)
                tagNames.Add(t.Name);

            //filter books by EntryTags
            foreach (Book b in bookstemp)
            {
                if (b.EntryTags.Intersect(tagNames).Count() == tags.Count)
                    books.Add(b);
            }

            //find matching Entries in books
            foreach (Book b in books)
            {
                foreach (Entry e in b.Entries)
                {
                    count = 0;
                    foreach (Tag t1 in tags)
                    {
                        check = false;
                        foreach (Tag t2 in e.Tags)
                        {

                            if (t1.Name == t2.Name && t1.Value == t2.Value)
                            {
                                check = true;
                                break;
                            }
                        }
                        if (check)
                            count++;
                    }
                    if (count == tags.Count)
                        entries.Add(e);
                }
                Debug.WriteLine($"Matched Entries: {entries.Count}");
            }

            return entries;
        }
    }
}
