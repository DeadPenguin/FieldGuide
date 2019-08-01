using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Realms;
using Xamarin.Forms;
using static FieldGuide.Models.WorkModel;

namespace FieldGuide.Models
{
    public class IdManager : RealmObject
    {
        public int NextId { get; set; }
        public int IdCount { get; set; }

        public IdManager()
        {
            NextId = 1;
            IdCount = 0;
        }

        public void Inc()
        {
            NextId++;
            IdCount++;
        }
    }
    //Different classes for Folder and Book as Realm doesn't support casting between classes and subclasses
    public class Folder : RealmObject
    {
        [PrimaryKey, Indexed]
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }

        public Folder() { }
        public Folder(int parentId, string title)
        {
            ParentId = parentId;
            Title = title;
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class Book : RealmObject
    {
        [PrimaryKey, Indexed, JsonProperty]
        public int Id { get; set; }
        [JsonProperty("parentId")]
        public int ParentId { get; set; }
        [Indexed, JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("bookTags")]
        public IList<string> BookTags { get;}
        [JsonProperty("entryTags")]
        public IList<string> EntryTags { get;}
        [JsonProperty("entries")]
        public IList<Entry> Entries { get;}

        public WorkBook ToWork()
        {
            WorkBook work = new WorkBook();

            work.Id = this.Id;
            work.ParentId = this.ParentId;
            work.Title = this.Title;
            work.Author = this.Author;

            foreach (string s in BookTags)
                work.BookTags.Add(s);

            foreach (string s in EntryTags)
                work.EntryTags.Add(s);

            foreach (Entry e in Entries)
                work.Entries.Add(e.ToWork());

            return work;
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

    [JsonObject(MemberSerialization.OptIn)]
    public class Entry : RealmObject
    {
        [Indexed, JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("imagePath")]
        public string ImagePath { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        [Backlink(nameof(Book.Entries))]
        public IQueryable<Book> BookLink { get; }
        [JsonProperty("tags")]
        public IList<Tag> Tags { get; }

        public WorkEntry ToWork()
        {
            WorkEntry e = new WorkEntry();
            e.Name = Name;
            e.Description = Description;
            e.ImagePath = ImagePath;
            foreach (Tag t in Tags)
                e.Tags.Add(new WorkTag(t.Name, t.Value));

            return e;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Tag : RealmObject
    {
        [Indexed, JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [Backlink (nameof(Entry.Tags))]
        public IQueryable<Entry> EntryLink { get; }
        public Tag() { }
        public Tag (string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
