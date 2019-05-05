using Android.Text;
using Mobile.Solution.Droid.Renderers;
using Mobile.Solution.Infrastructure.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FormattedPhoneNumberEntry), typeof(FormattedPhoneNumberEntryRenderer))]

namespace Mobile.Solution.Droid.Renderers
{
    public class FormattedPhoneNumberEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
                Control.AfterTextChanged -= Control_AfterTextChanged;
            if (e.NewElement != null)
                Control.AfterTextChanged += Control_AfterTextChanged;
        }

        private void Control_AfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            var element = (FormattedPhoneNumberEntry) Element;
            if (!element.ShouldReactToTextChanges) return;
            element.ShouldReactToTextChanges = false;
            try
            {
                var cursorPosition = Control.SelectionStart;
                var oldText = Control.Text;
                var number = FormattedPhoneNumberEntry.GetNumber(oldText);
                var newText = number > 999999999 ? $"{number:(###)-###-##-#0}" : $"{number:### ### ## #0}";
                Control.Text = number == 0 ? "" : newText;
                var change = oldText.Length - newText.Length;
                var index = cursorPosition - change;
                Control.SetSelection(index);
            }
            catch
            {
                // ignored
            }
            finally
            {
                element.ShouldReactToTextChanges = true;
            }
        }
    }
}