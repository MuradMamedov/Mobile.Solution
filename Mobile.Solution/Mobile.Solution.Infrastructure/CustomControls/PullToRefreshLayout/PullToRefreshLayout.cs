﻿/*
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

using System.Windows.Input;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls.PullToRefreshLayout
{
    /// <summary>
    ///     Pull to refresh layout.
    /// </summary>
    public class PullToRefreshLayout : ContentView
    {
        /// <summary>
        ///     The is refreshing property.
        /// </summary>
        public static readonly BindableProperty IsRefreshingProperty =
            BindableProperty.Create(nameof(IsRefreshing), typeof(bool), typeof(PullToRefreshLayout), false);

        /// <summary>
        ///     The is pull to refresh enabled property.
        /// </summary>
        public static readonly BindableProperty IsPullToRefreshEnabledProperty =
            BindableProperty.Create(nameof(IsPullToRefreshEnabled), typeof(bool), typeof(PullToRefreshLayout), true);

        /// <summary>
        ///     The refresh command property.
        /// </summary>
        public static readonly BindableProperty RefreshCommandProperty =
            BindableProperty.Create(nameof(RefreshCommand), typeof(ICommand), typeof(PullToRefreshLayout));

        /// <summary>
        ///     Color property of refresh spinner color
        /// </summary>
        public static readonly BindableProperty RefreshColorProperty =
            BindableProperty.Create(nameof(RefreshColor), typeof(Color), typeof(PullToRefreshLayout), Color.Default);


        /// <summary>
        ///     Color property of refresh background color
        /// </summary>
        public static readonly BindableProperty RefreshBackgroundColorProperty =
            BindableProperty.Create(nameof(RefreshBackgroundColor), typeof(Color), typeof(PullToRefreshLayout),
                Color.Default);

        /// <summary>
        ///     Initializes a new instance of the <see cref="PullToRefreshLayout" /> class.
        /// </summary>
        public PullToRefreshLayout()
        {
            IsClippedToBounds = true;
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is refreshing.
        /// </summary>
        /// <value><c>true</c> if this instance is refreshing; otherwise, <c>false</c>.</value>
        public bool IsRefreshing
        {
            get => (bool) GetValue(IsRefreshingProperty);
            set
            {
                if ((bool) GetValue(IsRefreshingProperty) == value)
                    OnPropertyChanged(nameof(IsRefreshing));

                SetValue(IsRefreshingProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is pull to refresh enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is pull to refresh enabled; otherwise, <c>false</c>.</value>
        public bool IsPullToRefreshEnabled
        {
            get => (bool) GetValue(IsPullToRefreshEnabledProperty);
            set => SetValue(IsPullToRefreshEnabledProperty, value);
        }

        /// <summary>
        ///     Gets or sets the refresh command.
        /// </summary>
        /// <value>The refresh command.</value>
        public ICommand RefreshCommand
        {
            get => (ICommand) GetValue(RefreshCommandProperty);
            set => SetValue(RefreshCommandProperty, value);
        }

        /// <summary>
        ///     Refresh  color
        /// </summary>
        public Color RefreshColor
        {
            get => (Color) GetValue(RefreshColorProperty);
            set => SetValue(RefreshColorProperty, value);
        }

        /// <summary>
        ///     Refresh background color
        /// </summary>
        public Color RefreshBackgroundColor
        {
            get => (Color) GetValue(RefreshBackgroundColorProperty);
            set => SetValue(RefreshBackgroundColorProperty, value);
        }


        /// <param name="widthConstraint">The available width for the element to use.</param>
        /// <param name="heightConstraint">The available height for the element to use.</param>
        /// <summary>
        ///     Optimization as we can get the size here of our content all in DIP
        /// </summary>
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (Content == null)
                return new SizeRequest(new Size(100, 100));

            return base.OnMeasure(widthConstraint, heightConstraint);
        }
    }
}