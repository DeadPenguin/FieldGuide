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
    public partial class BookView : ContentPage
    {
        public BookView(int Id)
        {
            InitializeComponent();
            BindingContext = new BookViewViewModel(Navigation, Id);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}