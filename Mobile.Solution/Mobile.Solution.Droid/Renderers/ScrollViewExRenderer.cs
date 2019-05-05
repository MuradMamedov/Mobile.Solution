using System.ComponentModel;
using Mobile.Solution.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ScrollView), typeof(ScrollViewExRenderer))]

namespace Mobile.Solution.Droid.Renderers
{
    public class ScrollViewExRenderer : ScrollViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;

            e.NewElement.PropertyChanged += OnElementPropertyChanged;
        }

        private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (ChildCount > 0)
                {
                    GetChildAt(0).HorizontalScrollBarEnabled = false;
                    GetChildAt(0).VerticalScrollBarEnabled = false;
                }
            }
            catch
            {
                var visualElement = sender as VisualElement;
                if (visualElement != null)
                    visualElement.PropertyChanged -= OnElementPropertyChanged;
            }
        }
    }
}