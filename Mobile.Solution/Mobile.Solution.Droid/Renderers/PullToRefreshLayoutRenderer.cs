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
using System.Reflection;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Mobile.Solution.Droid.Renderers;
using Mobile.Solution.Infrastructure.CustomControls.PullToRefreshLayout;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ScrollView = Android.Widget.ScrollView;
using WebView = Android.Webkit.WebView;

[assembly: ExportRenderer(typeof(PullToRefreshLayout), typeof(PullToRefreshLayoutRenderer))]

namespace Mobile.Solution.Droid.Renderers
{
    /// <summary>
    ///     Pull to refresh layout renderer.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class PullToRefreshLayoutRenderer : SwipeRefreshLayout,
        IVisualElementRenderer,
        SwipeRefreshLayout.IOnRefreshListener
    {
        private bool _init;
        private IVisualElementRenderer _packed;

        private bool _refreshing;

        private BindableProperty _rendererProperty;

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="PullToRefreshLayoutRenderer" /> class.
        /// </summary>
        public PullToRefreshLayoutRenderer()
            : base(Forms.Context)
        {
        }

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

                var type = Type.GetType("Xamarin.Forms.Platform.Android.Platform, Xamarin.Forms.Platform.Android");
                if (type != null)
                {
                    var prop = type.GetField("RendererProperty",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (prop != null)
                    {
                        var val = prop.GetValue(null);
                        _rendererProperty = val as BindableProperty;
                    }
                }

                return _rendererProperty;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this
        ///     <see cref="PullToRefreshLayoutRenderer" /> is refreshing.
        /// </summary>
        /// <value><c>true</c> if refreshing; otherwise, <c>false</c>.</value>
        public override bool Refreshing
        {
            get { return _refreshing; }
            set
            {
                try
                {
                    _refreshing = value;
                    //this will break binding :( sad panda we need to wait for next version for this
                    //right now you can't update the binding.. so it is 1 way
                    if (RefreshView != null && RefreshView.IsRefreshing != _refreshing)
                        RefreshView.IsRefreshing = _refreshing;

                    if (base.Refreshing == _refreshing)
                        return;

                    base.Refreshing = _refreshing;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }


        /// <summary>
        ///     Helpers to cast our element easily
        ///     Will throw an exception if the Element is not correct
        /// </summary>
        /// <value>The refresh view.</value>
        public PullToRefreshLayout RefreshView => (PullToRefreshLayout) Element;


        /// <summary>
        ///     The refresh view has been refreshed
        /// </summary>
        public void OnRefresh()
        {
            //someone pulled down to refresh or it is done

            var command = RefreshView?.RefreshCommand;

            command?.Execute(null);
        }

        /// <summary>
        ///     Occurs when element changed.
        /// </summary>
        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        /// <summary>
        ///     Setup our SwipeRefreshLayout and register for property changed notifications.
        /// </summary>
        /// <param name="element">Element.</param>
        public void SetElement(VisualElement element)
        {
            var oldElement = Element;

            //unregister old and re-register new
            if (oldElement != null)
                oldElement.PropertyChanged -= HandlePropertyChanged;

            Element = element;
            if (Element != null)
            {
                UpdateContent();
                Element.PropertyChanged += HandlePropertyChanged;
            }

            if (!_init)
            {
                _init = true;
                //sizes to match the forms view
                //updates properties, handles visual element properties
                Tracker = new VisualElementTracker(this);
                SetOnRefreshListener(this);
            }

            UpdateColors();
            UpdateIsRefreshing();
            UpdateIsSwipeToRefreshEnabled();

            ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(oldElement, Element));
        }

        /// <summary>
        ///     Gets the size of the desired.
        /// </summary>
        /// <returns>The desired size.</returns>
        /// <param name="widthConstraint">Width constraint.</param>
        /// <param name="heightConstraint">Height constraint.</param>
        public SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            _packed.ViewGroup.Measure(widthConstraint, heightConstraint);

            //Measure child here and determine size
            return new SizeRequest(new Size(_packed.ViewGroup.MeasuredWidth, _packed.ViewGroup.MeasuredHeight));
        }

        /// <summary>
        ///     Updates the layout.
        /// </summary>
        public void UpdateLayout()
        {
            Tracker?.UpdateLayout();
        }

        /// <summary>
        ///     Gets the tracker.
        /// </summary>
        /// <value>The tracker.</value>
        public VisualElementTracker Tracker { get; private set; }

        /// <summary>
        ///     Gets the view group.
        /// </summary>
        /// <value>The view group.</value>
        public ViewGroup ViewGroup => this;

        /// <summary>
        ///     Gets the element.
        /// </summary>
        /// <value>The element.</value>
        public VisualElement Element { get; private set; }

        /// <summary>
        ///     Used for registration with dependency service
        /// </summary>
        public static void Init()
        {
            var temp = DateTime.Now;
        }

        /// <summary>
        ///     Managest adding and removing the android viewgroup to our actual swiperefreshlayout
        /// </summary>
        private void UpdateContent()
        {
            if (RefreshView.Content == null)
                return;

            if (_packed != null)
                RemoveView(_packed.ViewGroup);

            _packed = Platform.CreateRenderer(RefreshView.Content);

            try
            {
                RefreshView.Content.SetValue(RendererProperty, _packed);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to sent renderer property, maybe an issue: " + ex);
            }

            AddView(_packed.ViewGroup, LayoutParams.MatchParent);
        }

        private void UpdateColors()
        {
            if (RefreshView == null)
                return;
            if (RefreshView.RefreshColor != Color.Default)
                SetColorSchemeColors(RefreshView.RefreshColor.ToAndroid());
            if (RefreshView.RefreshBackgroundColor != Color.Default)
                SetProgressBackgroundColorSchemeColor(RefreshView.RefreshBackgroundColor.ToAndroid());
        }

        private void UpdateIsRefreshing()
        {
            Refreshing = RefreshView.IsRefreshing;
        }

        private void UpdateIsSwipeToRefreshEnabled()
        {
            Enabled = RefreshView.IsPullToRefreshEnabled;
        }


        /// <summary>
        ///     Determines whether this instance can child scroll up.
        ///     We do this since the actual swipe refresh can't figure it out
        /// </summary>
        /// <returns><c>true</c> if this instance can child scroll up; otherwise, <c>false</c>.</returns>
        public override bool CanChildScrollUp()
        {
            return CanScrollUp(_packed.ViewGroup);
        }

        private bool CanScrollUp(ViewGroup viewGroup)
        {
            if (viewGroup == null)
                return base.CanChildScrollUp();

            var sdk = (int) Build.VERSION.SdkInt;
            if (sdk >= 16)
                if (viewGroup.IsScrollContainer)
                    return base.CanChildScrollUp();

            //if you have something custom and you can't scroll up you might need to enable this
            //for instance on a custom recycler view where the code above isn't working!
            for (var i = 0; i < viewGroup.ChildCount; i++)
            {
                var child = viewGroup.GetChildAt(i);
                if (child is AbsListView)
                {
                    var list = child as AbsListView;
                    if (list.FirstVisiblePosition == 0)
                    {
                        var subChild = list.GetChildAt(0);

                        return subChild != null && subChild.Top != 0;
                    }

                    //if children are in list and we are scrolled a bit... sure you can scroll up
                    return true;
                }
                if (child is ScrollView)
                {
                    var scrollview = child as ScrollView;
                    return scrollview.ScrollY <= 0.0;
                }
                if (child is WebView)
                {
                    var webView = child as WebView;
                    return webView.ScrollY > 0.0;
                }
                if (child is SwipeRefreshLayout)
                    return CanScrollUp(child as ViewGroup);
                //else if something else like a recycler view?
            }

            return false;
        }

        /// <summary>
        ///     Handles the property changed.
        ///     Update the control and trigger refreshing
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content")
                UpdateContent();
            else if (e.PropertyName == PullToRefreshLayout.IsPullToRefreshEnabledProperty.PropertyName)
                UpdateIsSwipeToRefreshEnabled();
            else if (e.PropertyName == PullToRefreshLayout.IsRefreshingProperty.PropertyName)
                UpdateIsRefreshing();
            else if (e.PropertyName == PullToRefreshLayout.RefreshColorProperty.PropertyName)
                UpdateColors();
            else if (e.PropertyName == PullToRefreshLayout.RefreshBackgroundColorProperty.PropertyName)
                UpdateColors();
        }

        /// <summary>
        ///     Cleanup layout.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (Element != null)
                Element.PropertyChanged -= HandlePropertyChanged;
            /*if(packed != null)
            {
                packed.Dispose();
                packed = null;
            }

            if (rendererProperty != null)
            {
                rendererProperty = null;
            }*/

            _init = false;
        }
    }
}