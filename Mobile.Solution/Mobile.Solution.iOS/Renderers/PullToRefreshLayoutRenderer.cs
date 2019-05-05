/*
 * Copyright (C) 2015 Refractored LLC & James Montemagno: 
 * http://github.com/JamesMontemagno
 * http://twitter.com/JamesMontemagno
 * http://refractored.com
 * 
 * The MIT License (MIT) see GitHub For more information
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using Foundation;
using Mobile.Solution.iOS.Renderers;
using Mobile.Solution.Infrastructure.CustomControls.PullToRefreshLayout;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PullToRefreshLayout), typeof(PullToRefreshLayoutRenderer))]

namespace Mobile.Solution.iOS.Renderers
{
    /// <summary>
    ///     Pull to refresh layout renderer.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class PullToRefreshLayoutRenderer : ViewRenderer<PullToRefreshLayout, UIView>
    {
        private bool _isRefreshing;

        private UIRefreshControl _refreshControl;

        private BindableProperty _rendererProperty;

        /// <summary>
        ///     Gets the bindable property.
        /// </summary>
        /// <returns>The bindable property.</returns>
        private BindableProperty RendererProperty
        {
            get
            {
                if (_rendererProperty != null)
                    return _rendererProperty;

                var type = Type.GetType("Xamarin.Forms.Platform.iOS.Platform, Xamarin.Forms.Platform.iOS");
                if (type != null)
                {
                    var prop = type.GetField("RendererProperty");
                    var val = prop.GetValue(null);
                    _rendererProperty = val as BindableProperty;
                }

                return _rendererProperty;
            }
        }


        /// <summary>
        ///     Helpers to cast our element easily
        ///     Will throw an exception if the Element is not correct
        /// </summary>
        /// <value>The refresh view.</value>
        public PullToRefreshLayout RefreshView => Element;

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is refreshing.
        /// </summary>
        /// <value><c>true</c> if this instance is refreshing; otherwise, <c>false</c>.</value>
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                if (_isRefreshing)
                    _refreshControl.BeginRefreshing();
                else
                    _refreshControl.EndRefreshing();
            }
        }

        /// <summary>
        ///     Used for registration with dependency service
        /// </summary>
        public new static void Init()
        {
            var temp = DateTime.Now;
        }

        /// <summary>
        ///     Raises the element changed event.
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnElementChanged(
            ElementChangedEventArgs<PullToRefreshLayout> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;


            _refreshControl = new UIRefreshControl();

            _refreshControl.ValueChanged += OnRefresh;

            try
            {
                TryInsertRefresh(this);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("View is not supported in PullToRefreshLayout: " + ex);
            }


            UpdateColors();
            UpdateIsRefreshing();
            UpdateIsSwipeToRefreshEnabled();
        }

        private bool TryInsertRefresh(UIView view, int index = 0)
        {
            if (view is UITableView)
            {
                view.InsertSubview(_refreshControl, index);
                return true;
            }

            if (view is UICollectionView)
            {
                view.InsertSubview(_refreshControl, index);
                return true;
            }

            var uiWebView = view as UIWebView;
            if (uiWebView != null)
            {
                uiWebView.ScrollView.InsertSubview(_refreshControl, index);
                return true;
            }

            var uIScrollView = view as UIScrollView;
            if (uIScrollView != null)
            {
                view.InsertSubview(_refreshControl, index);
                uIScrollView.AlwaysBounceVertical = true;
                return true;
            }

            if (view.Subviews == null)
                return false;

            for (var i = 0; i < view.Subviews.Length; i++)
            {
                var control = view.Subviews[i];
                if (TryInsertRefresh(control, i))
                    return true;
            }

            return false;
        }

        private void UpdateColors()
        {
            if (RefreshView == null)
                return;
            if (RefreshView.RefreshColor != Color.Default)
                _refreshControl.TintColor = RefreshView.RefreshColor.ToUIColor();
            if (RefreshView.RefreshBackgroundColor != Color.Default)
                _refreshControl.BackgroundColor = RefreshView.RefreshBackgroundColor.ToUIColor();
        }

        private void UpdateIsRefreshing()
        {
            IsRefreshing = RefreshView.IsRefreshing;
        }

        private void UpdateIsSwipeToRefreshEnabled()
        {
            _refreshControl.Enabled = RefreshView.IsPullToRefreshEnabled;
        }

        /// <summary>
        ///     The refresh view has been refreshed
        /// </summary>
        private void OnRefresh(object sender, EventArgs e)
        {
            //someone pulled down to refresh or it is done

            var command = RefreshView?.RefreshCommand;

            command?.Execute(null);
        }

        /// <summary>
        ///     Raises the element property changed event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == PullToRefreshLayout.IsPullToRefreshEnabledProperty.PropertyName)
                UpdateIsSwipeToRefreshEnabled();
            else if (e.PropertyName == PullToRefreshLayout.IsRefreshingProperty.PropertyName)
                UpdateIsRefreshing();
            else if (e.PropertyName == PullToRefreshLayout.RefreshColorProperty.PropertyName)
                UpdateColors();
            else if (e.PropertyName == PullToRefreshLayout.RefreshBackgroundColorProperty.PropertyName)
                UpdateColors();
        }

        /// <summary>
        ///     Dispose the specified disposing.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_refreshControl != null)
                _refreshControl.ValueChanged -= OnRefresh;
        }
    }
}