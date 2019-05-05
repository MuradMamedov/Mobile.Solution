using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.iOS.Renderers;
using Mobile.Solution.UI.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EnumPicker<PeriodSection>), typeof(EnumPickerRenderer))]

namespace Mobile.Solution.iOS.Renderers
{
	public class EnumPickerRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            Control.VerticalAlignment = UIKit.UIControlContentVerticalAlignment.Bottom;
        }
    }
}
