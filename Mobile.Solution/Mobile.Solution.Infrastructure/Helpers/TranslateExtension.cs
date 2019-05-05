using System;
using System.Diagnostics;
using Mobile.Solution.Infrastructure.Properties;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile.Solution.Infrastructure.Helpers
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            var translation = Resources.ResourceManager.GetString(Text);

            if (translation == null)
            {
                Debug.WriteLine($"Key '{Text}' was not found in resources.");

                translation = Text;
            }

            return translation;
        }
    }
}