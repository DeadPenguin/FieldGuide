using FieldGuide.Models;
using Rg.Plugins.Popup.Services;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FieldGuide.Modals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewFolder
    {
        public string FolderName;
        public FileBrowser Browser;

        public NewFolder(FileBrowser browser)
        {
            InitializeComponent();
            BindingContext = this;
            Browser = browser;
        }

        [Obsolete]
        public async void OnAccept(object sender, EventArgs args)
        {
            Browser.NewFolderName = FileName.Text;
            Browser.CreateNewFolder();
            await PopupNavigation.PopAsync();
        }

        [Obsolete]
        public async void OnCancel(object sender, EventArgs args)
        {
            await PopupNavigation.PopAsync();
        }
    }
}