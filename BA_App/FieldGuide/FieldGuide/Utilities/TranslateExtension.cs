using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FieldGuide.Utilities
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        const string ResourceId = "FieldGuide.Resources.AppResources";
        public static ResourceManager Translator = null;
        public string Text { get; set; }

        private static void CheckInitialized()
        {
            if(Translator == null)
                Translator = new ResourceManager(ResourceId, Assembly.GetExecutingAssembly());
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            CheckInitialized();

            if (Text == null)
                return null;
            ResourceManager resourceManager = new ResourceManager(
                ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);

            return resourceManager.GetString(Text, CultureInfo.CurrentCulture);
        }
    }
}
