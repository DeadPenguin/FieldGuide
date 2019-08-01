using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FieldGuide.Models
{
    //A class providing value-type equivalents of the RealmModel-classes which can be manipulated outside the Realm-scope.
    public class WorkModel
    {
        public class WorkFolder
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public string Title { get; set; }
        }
        public class WorkBook
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public List<string> BookTags { get; set; }
            public HashSet<string> EntryTags { get; set; }
            public List<WorkEntry> Entries { get; set; }
            public List<string> RemovedImages { get; set; }

            public WorkBook()
            {
                BookTags = new List<string>();
                EntryTags = new HashSet<string>();
                Entries = new List<WorkEntry>();
            }
            public Book ToRealm()
            {
                Book b = new Book();
                b.Id = this.Id;
                b.ParentId = this.ParentId;
                b.Title = this.Title;
                b.Author = this.Author;
                foreach (string s in BookTags)
                    b.BookTags.Add(s);
                foreach (string s in EntryTags)
                    b.EntryTags.Add(s);
                foreach (WorkEntry e in Entries)
                    b.Entries.Add(e.ToRealm());

                return b;
            }

            public override string ToString()
            {
                string s = $@"Id: {Id}
                    Parent: {ParentId}
                    Title: {Title}
                    BookTags: {BookTags.Count}
                    EntryTags: {EntryTags.Count}
                    Entries: {Entries.Count}";
                return s;
            }
        }

        public class WorkEntry
        {
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public string Description { get; set; }
            public List<WorkTag> Tags { get; }
            public bool ImageChanged { get; set; }
            public byte[] NewImageData { get; set; }
            public string OldImagePath { get; set; }
            public WorkEntry()
            {
                Tags = new List<WorkTag>();
            }
            public Entry ToRealm()
            {
                Entry e = new Entry();
                e.Name = Name;
                e.Description = Description;
                e.ImagePath = ImagePath;
                foreach (WorkTag t in Tags)
                    e.Tags.Add(t.ToRealm());

                return e;
            }
        }

        public class WorkTag
        {
            public WorkTag(string name, string val)
            {
                Name = name;
                Value = val;
            }

            public string Name { get; set; }
            public string Value { get; set; }

            public Tag ToRealm()
            {
                Tag t = new Tag(Name, Value);
                return t;
            }
        }

        public class Result
        {
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public StreamImageSource Source { get; set; }

            public Result(Entry e)
            {
                Name = e.Name;
                ImagePath = e.ImagePath;
            }

            private async Task<Result> InitializeAsync()
            {
                Source = await FileManager.GetImage(ImagePath);
                return this;
            }

            public static Task<Result> CreateResult(Entry e)
            {
                Result result = new Result(e);
                return result.InitializeAsync();
            }
        }
    }
}
