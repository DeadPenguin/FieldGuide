using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FieldGuide.Utilities
{
    static class AlertManager
    {
        private static ResourceManager rm = null;

        public static void CheckInitialized()
        {
            if(rm == null)
                rm = new ResourceManager("FieldGuide.Resources.AppResources", 
                    Assembly.GetExecutingAssembly());
        }

        public static async Task<bool> DiscardBook()
        {
            CheckInitialized();

            bool result = await App.Current.MainPage.DisplayAlert(
                rm.GetString("Attention"),
                rm.GetString("DeleteAlertBook"),
                rm.GetString("Continue"),
                rm.GetString("Cancel"));

            return result;
        }

        public static async Task<bool> Return()
        {
            CheckInitialized();

            bool result = await App.Current.MainPage.DisplayAlert(
                rm.GetString("Attention"),
                rm.GetString("ReturnAlert"),
                rm.GetString("Continue"),
                rm.GetString("Cancel"));

            return result;
        }
    
        public static async Task<bool> DiscardEntry()
        {
            CheckInitialized();

            bool result = await App.Current.MainPage.DisplayAlert(
                rm.GetString("Attention"),
                rm.GetString("DeleteAlertEntry"),
                rm.GetString("Continue"),
                rm.GetString("Cancel"));

            return result;
        }

        public static async void IncompleteInfo()
        {
            CheckInitialized();

            await App.Current.MainPage.DisplayAlert(
                rm.GetString("Attention"),
                rm.GetString("IncompleteInfoAlert"),
                rm.GetString("Ok"));
        }

        public static async Task<bool> NoEntries()
        {
            CheckInitialized();

            bool result = await App.Current.MainPage.DisplayAlert(
                rm.GetString("Attention"),
                rm.GetString("NoEntries"),
                rm.GetString("Ok"),
                rm.GetString("Cancel"));

            return result;
        }

        public static async void IncompleteEntry()
        {
            CheckInitialized();

            await App.Current.MainPage.DisplayAlert(
                rm.GetString("Attention"),
                rm.GetString("IncompleteEntryAlert"),
                rm.GetString("Ok"));
        }

        public static async void NoTags()
        {
            CheckInitialized();

            await App.Current.MainPage.DisplayAlert(
                rm.GetString("Attention"),
                rm.GetString("IncompleteTagsAlert"),
                rm.GetString("Ok"));
        }

        public static async Task<bool> NoDescription()
        {
            CheckInitialized();

            bool result = await App.Current.MainPage.DisplayAlert(
                rm.GetString("Attention"),
                rm.GetString("IncompleteDescriptionAlert"),
                rm.GetString("Ok"),
                rm.GetString("Cancel"));

            return result;
        }

        public static async void ExportPath(String path)
        {
            CheckInitialized();

            await App.Current.MainPage.DisplayAlert(
                rm.GetString("Attention"),
                $"{rm.GetString("ExportPath")}:\n{path}",
                rm.GetString("Ok"));
        }
    }
}
