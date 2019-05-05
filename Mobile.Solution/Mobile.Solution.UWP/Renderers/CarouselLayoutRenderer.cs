using System;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.UWP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;

[assembly: ExportRenderer(typeof(CarouselLayout), typeof(CarouselLayoutRenderer))]

namespace Mobile.Solution.UWP.Renderers
{
    public sealed class CarouselLayoutRenderer : ScrollViewRenderer
    {
        private double _deltaX;
        private Timer _deltaXResetTimer;
        private Timer _scrollStopTimer;

        private bool _initialized;
        private bool _disableSelecting;
        private double _prevScrollX;
        private ScrollView _scrollView;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            try
            {
                if (Control.HorizontalScrollBarVisibility != ScrollBarVisibility.Hidden)
                {
                    Control.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    Control.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    Control.ViewChanged += ControlOnViewChanged;
                    Control.SizeChanged += async (o, args) => await ScrollToSelection();
                }
            }
            catch
            {
                // ignored
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ScrollView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null) return;
            _scrollView = e.NewElement;

            _deltaXResetTimer = new Timer(state => _deltaX = 0, null, TimeSpan.FromMilliseconds(-1),
                TimeSpan.FromMilliseconds(-1));
            _scrollStopTimer = new Timer(state => UpdateSelectedIndex(), null, TimeSpan.FromMilliseconds(-1),
                TimeSpan.FromMilliseconds(-1));
            e.NewElement.PropertyChanged += ElementPropertyChanged;
        }

        private async void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == CarouselLayout.SelectedIndexProperty.PropertyName && !_disableSelecting)
                {
                    _disableSelecting = true;
                    await ScrollToIndex(((CarouselLayout) Element).SelectedIndex);
                    _disableSelecting = false;
                }
            }
            catch
            {
                //ignored
            }
        }

        private async void ControlOnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (e.IsIntermediate)
            {
                _deltaX = _scrollView.ScrollX - _prevScrollX;
                _prevScrollX = _scrollView.ScrollX;

                UpdateSelectedIndex();

                _scrollStopTimer.Change(0, 100);
            }
            else
            {
                await ScrollToSelection();
                _scrollStopTimer.Change(0, 200);
            }
        }

        private void UpdateSelectedIndex()
        {
            if(Element == null)
                return;

            var center = _scrollView.ScrollX + _scrollView.Bounds.Width / 2;
            var newIndex = (int) center / (int) _scrollView.Bounds.Width;
            ((CarouselLayout) Element).SelectedIndex = newIndex;
        }

        private async Task ScrollToIndex(int targetIndex)
        {
            if (Element == null) return;
            var d = _scrollView.Bounds.Width * Math.Max(0, targetIndex);
            await _scrollView.ScrollToAsync(d, _scrollView.ScrollY, true);
        }

        private async Task ScrollToSelection()
        {
            if (Element == null) return;
            var d = _scrollView.Bounds.Width *
                    Math.Max(0, ((CarouselLayout) Element).SelectedIndex);
            await _scrollView.ScrollToAsync(d, _scrollView.ScrollY, true);
        }
    }
}