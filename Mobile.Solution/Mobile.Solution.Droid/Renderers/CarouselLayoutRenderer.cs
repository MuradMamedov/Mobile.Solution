using System.ComponentModel;
using System.Reflection;
using System.Timers;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Mobile.Solution.Droid.Renderers;
using Mobile.Solution.Infrastructure.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CarouselLayout), typeof(CarouselLayoutRenderer))]

namespace Mobile.Solution.Droid.Renderers
{
    public class CarouselLayoutRenderer : ScrollViewRenderer
    {
        private int _deltaX;
        private Timer _deltaXResetTimer;

        private bool _initialized;
        private bool _motionDown;
        private int _prevScrollX;
        private Timer _scrollStopTimer;
        private HorizontalScrollView _scrollView;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null) return;

            _deltaXResetTimer = new Timer(100) {AutoReset = false};
            _deltaXResetTimer.Elapsed += (sender, args) => _deltaX = 0;

            _scrollStopTimer = new Timer(200) {AutoReset = false};
            _scrollStopTimer.Elapsed += (sender, args2) => UpdateSelectedIndex();

            e.NewElement.PropertyChanged += ElementPropertyChanged;
        }

        private void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "Renderer")
                {
                    _scrollView = (HorizontalScrollView) typeof(ScrollViewRenderer)
                        .GetField("_hScrollView", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.GetValue(this);

                    System.Diagnostics.Debug.Assert(_scrollView != null, "_scrollView != null");
                    _scrollView.HorizontalScrollBarEnabled = false;
                    _scrollView.Touch += HScrollViewTouch;
                }
                if (e.PropertyName == CarouselLayout.SelectedIndexProperty.PropertyName && !_motionDown)
                    ScrollToIndex(((CarouselLayout) Element).SelectedIndex);
            }
            catch
            {
                //ignored
            }
        }

        private void HScrollViewTouch(object sender, TouchEventArgs e)
        {
            e.Handled = false;

            switch (e.Event.Action)
            {
                case MotionEventActions.Move:
                    _deltaXResetTimer.Stop();
                    _deltaX = _scrollView.ScrollX - _prevScrollX;
                    _prevScrollX = _scrollView.ScrollX;

                    UpdateSelectedIndex();

                    _deltaXResetTimer.Start();
                    break;
                case MotionEventActions.Down:
                    _motionDown = true;
                    _scrollStopTimer.Stop();
                    break;
                case MotionEventActions.Up:
                    _motionDown = false;
                    SnapScroll();
                    _scrollStopTimer.Start();
                    break;
            }
        }

        private void UpdateSelectedIndex()
        {
            var center = _scrollView.ScrollX + _scrollView.Width / 2;
            var carouselLayout = (CarouselLayout) Element;
            carouselLayout.SelectedIndex = center / _scrollView.Width;
        }

        private void SnapScroll()
        {
            var roughIndex = (float) _scrollView.ScrollX / _scrollView.Width;

            var targetIndex =
                _deltaX < 0
                    ? Math.Floor(roughIndex)
                    : _deltaX > 0
                        ? Math.Ceil(roughIndex)
                        : Math.Round(roughIndex);

            ScrollToIndex((int) targetIndex);
        }

        private void ScrollToIndex(int targetIndex)
        {
            var targetX = targetIndex * _scrollView.Width;
            _scrollView.Post(new Runnable(() => { _scrollView.SmoothScrollTo(targetX, 0); }));
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            if (_initialized) return;
            _initialized = true;
            var carouselLayout = (CarouselLayout) Element;
            _scrollView.ScrollTo(carouselLayout.SelectedIndex * Width, 0);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            if (_initialized && w != oldw)
                _initialized = false;
            base.OnSizeChanged(w, h, oldw, oldh);
        }
    }
}