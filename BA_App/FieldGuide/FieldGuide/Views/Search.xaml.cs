using FieldGuide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FieldGuide.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Search : ContentPage
    {
        public Search(int root)
        {
            InitializeComponent();
            BindingContext = new SearchViewModel(Navigation, root);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}