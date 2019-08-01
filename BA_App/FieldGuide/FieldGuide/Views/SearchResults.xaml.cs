using FieldGuide.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static FieldGuide.Models.WorkModel;
using Entry = FieldGuide.Models.Entry;

namespace FieldGuide.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchResults : ContentPage
    {
        public SearchResults(List<Entry> entries, List<Result> results)
        {
            InitializeComponent();
            BindingContext = new SearchResultsViewModel(Navigation, entries, results);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}