using FieldGuide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Entry = FieldGuide.Models.Entry;

namespace FieldGuide.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EntryView : ContentPage
    {
        public EntryView(Entry e)
        {
            InitializeComponent();
            BindingContext = new EntryViewViewModel(Navigation, e);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}