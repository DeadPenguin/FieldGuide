using FieldGuide.Models;
using FieldGuide.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FieldGuide.Modals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookAction
    {
        private int Id;

        public BookAction(FileBrowser browser)
        {
            InitializeComponent();
            BindingContext = this;
            Id = browser.SelectedItem.Id;
            browser.SelectedItem = null;
        }

        [Obsolete]
        public async void OnEdit(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new EditBook(Id, true));
            await PopupNavigation.PopAsync();
        }

        [Obsolete]
        public async void OnExport(object sender, EventArgs args)
        {
            Book book = RealmManager.FindBookById(Id);
            FileManager.Export(book);
            await PopupNavigation.PopAsync();
        }

        [Obsolete]
        public async void OnCancel(object sender, EventArgs args)
        {
            await PopupNavigation.PopAsync();
        }

        public async void OnSearchBook(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new BookView(Id));
            await PopupNavigation.PopAsync();
        }
    }
}