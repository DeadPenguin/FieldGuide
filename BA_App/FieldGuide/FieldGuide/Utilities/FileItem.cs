using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace FieldGuide.Models
{
    enum Types : int { FILE = 0, BOOK = 1 }
    public class FileItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public string Icon { get; set; }

        public FileItem(int id, string title, int type)
        {
            this.Id = id;
            this.Title = title;
            this.Type = type;
            this.Icon = type == 0 ? "Folder_klein.png" : "Book_klein.png";
        }
    }
}
