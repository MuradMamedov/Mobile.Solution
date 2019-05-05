using UIKit;
using System;
using Mobile.Solution.iOS;
using Mobile.Solution.Infrastructure.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FormattedPhoneNumberEntry), typeof(FormattedPhoneNumberEntryRenderer))]

namespace Mobile.Solution.iOS
{
    public class FormattedPhoneNumberEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
                Control.EditingChanged -= Control_EditingChanged;
            if (e.NewElement != null)
                Control.EditingChanged += Control_EditingChanged;
            
			Control.BorderStyle = UITextBorderStyle.None;
        }

        private void Control_EditingChanged(object sender, EventArgs e)
        {
            var element = (FormattedPhoneNumberEntry) Element;
            if (!element.ShouldReactToTextChanges) return;
            element.ShouldReactToTextChanges = false;
            var selectedRange = Control.SelectedTextRange;
            var oldText = Control.Text;
            var number = FormattedPhoneNumberEntry.GetNumber(oldText);
            var newText = number > 999999999 ? $"{number:(###)-###-##-#0}" : $"{number:### ### ## #0}";
            Control.Text = number == 0 ? "" : newText;
            var change = -1 * (oldText.Length - newText.Length);
            var newPosition = Control.GetPosition(selectedRange.Start, change);
            if (newPosition != null)
                Control.SelectedTextRange = Control.GetTextRange(newPosition, newPosition);
            element.ShouldReactToTextChanges = true;
        }
    }
}