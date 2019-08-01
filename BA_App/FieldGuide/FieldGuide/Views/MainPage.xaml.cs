using FieldGuide.Models;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace FieldGuide
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new FileBrowser(Navigation);
            NavigationPage.SetHasNavigationBar(this, false);

            //necessary to deselect and item after selection
            FileSystem.ItemSelected += (sender, e) =>
            {
                ((ListView)sender).SelectedItem = null;
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            FileBrowser temp = (FileBrowser)BindingContext;
            temp.RefreshBrowser();
        }
    }
}
