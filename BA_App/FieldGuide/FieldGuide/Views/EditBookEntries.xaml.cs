using FieldGuide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static FieldGuide.Models.WorkModel;

namespace FieldGuide.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditBookEntries : ContentPage
    {
        public EditBookEntries(WorkBook book)
        {
            InitializeComponent();
            BindingContext = new NewEntryViewModel(Navigation, book);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public EditBookEntries(WorkBook book, string entry)
        {
            InitializeComponent();
            BindingContext = new EntryEditViewModel(Navigation, book, entry);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}