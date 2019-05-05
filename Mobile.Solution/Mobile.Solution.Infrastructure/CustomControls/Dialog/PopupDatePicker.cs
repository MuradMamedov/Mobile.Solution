using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Syncfusion.SfCalendar.XForms;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure
{
	public class PopupDatePicker : PopupPage
	{
        public PopupDatePicker(DateTime? selectedDate = null, DateTime? minDate = null, DateTime? maxDate = null, string okText = "Ок")
		{ 
            SelectedDate = selectedDate ?? DateTime.Now;
			var calendar = new SfCalendar
            {
                SelectedDate = SelectedDate,
                MinDate = minDate.HasValue ? minDate.Value : DateTime.MinValue,
                MaxDate = maxDate.HasValue ? maxDate.Value : DateTime.MaxValue,
                ViewMode = ViewMode.MonthView,
                BindingContext = this,
                FirstDayofWeek = 1,
                Locale = new System.Globalization.CultureInfo("ru-RU"),
                MonthViewSettings = new MonthViewSettings
                {
					HeaderFont = Font.SystemFontOfSize(18),
					HeaderTextColor = (Color)Application.Current.Resources["CalendarHeaderColor"],
					HeaderBackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],

                    DayCellFont = Font.SystemFontOfSize(18),
                    DayHeaderTextColor = (Color)Application.Current.Resources["CalendarDateCellColor"],
                    DateSelectionColor = (Color)Application.Current.Resources["CalendarDateSelectedCellColor"], 
                    SelectedDayTextColor = Color.White
				},
                ShowNavigationButtons = true,
                BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],
            };
            calendar.SetBinding(SfCalendar.SelectedDateProperty, nameof(SelectedDate), BindingMode.TwoWay);
            calendar.SelectionChanged += async (sender, args) =>
            {
                await PopupNavigation.PopAsync();
            };

            var okButton = new Button
            {
                Text = okText,
                BackgroundColor= (Color)Application.Current.Resources["AccentColor"],
                Style = (Style)Application.Current.Resources["MainButtonStyle"]
            };
            okButton.Clicked += async (sender, e) => 
            {
				await PopupNavigation.PopAsync();
			};

            Content = new StackLayout
            {
                Padding = new Thickness(0, 0, 0, 0),
				Margin = new Thickness(5, 0, 5, 5),
				Spacing = 50,
                Children =
                {
                    new Frame
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        HasShadow = false,
                        CornerRadius = 5,
						BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],
                        Content = new StackLayout
                        {
                            Padding = new Thickness(0, 0, 0, 0),
                            Spacing = 0,
                            VerticalOptions = LayoutOptions.End,
                            Children =
                            {
                                calendar,
                            },
                        }
                    },
                    okButton
                }
            };
		}

		public DateTime SelectedDate { get; set; }
	}
}
