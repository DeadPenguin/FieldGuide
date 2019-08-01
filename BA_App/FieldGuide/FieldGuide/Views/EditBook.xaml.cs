using FieldGuide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FieldGuide
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditBook : ContentPage
    {
        private bool Edit;

        public EditBook(int id, bool edit)
        {
            Edit = edit;
            InitializeComponent();
            if (edit)
                BindingContext = new EditBookViewModel(Navigation, id);
            else
                BindingContext = new AddBookViewModel(Navigation, id);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            EditBookViewModel temp1;
            AddBookViewModel temp2;
            if (Edit)
            {
                temp1 = (EditBookViewModel)BindingContext;
                temp1.Update();
            }
            else
            {
                temp2 = (AddBookViewModel)BindingContext;
                temp2.update();
            }

        }
    }
}