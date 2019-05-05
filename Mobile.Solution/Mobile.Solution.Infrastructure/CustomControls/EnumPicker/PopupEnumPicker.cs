using System;
using System.Collections.Generic;
using System.Linq;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Syncfusion.Data.Extensions;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public class PopupEnumPicker<T> : PopupPage
        where T : struct
    {
        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IDictionary<T, string>), typeof(PopupEnumPicker<T>),
                default(IDictionary<T, string>), propertyChanged: OnItemsSourceChanged);

        public static BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(KeyValuePair<T, string>), typeof(PopupEnumPicker<T>),
                default(KeyValuePair<T, string>), BindingMode.TwoWay,
                propertyChanged: OnSelectedItemChangedAsync);

        public static BindableProperty DisplayMemberPathProperty =
            BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(EnumPicker<T>), default(string));

        private StackLayout _stack;

        public PopupEnumPicker()
        {
            Content = _stack = new StackLayout
            {
                VerticalOptions = LayoutOptions.EndAndExpand,
                Spacing = 5,
                Margin = new Thickness(10,0,10,15)
            };
        }

        public IDictionary<T, string> ItemsSource
        {
            get => (IDictionary<T, string>) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public KeyValuePair<T, string> SelectedItem
        {
            get => (KeyValuePair<T, string>) GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public string DisplayMemberPath
        {
            get => (string) GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as PopupEnumPicker<T>;
            if (picker == null) return;
            picker._stack.Children.Clear();
            var color = (Color)Application.Current.Resources["FrameBorderColor"];
            var style = Application.Current.Resources["PopupButtonStyle"] as Style;
            if (newvalue != null)
                foreach (var item in (IDictionary<T, string>) newvalue)
                {
                    var button = new Button
                    {
                        Text = item.Value,
                        BorderRadius = 16,
                        Style = style,
                    };
                    button.Clicked += async (sender, e) =>
                    {
                        await PopupNavigation.PopAsync();
                        picker.SelectedItem = item;
                    };
                    picker._stack.Children.Add(new Frame
                    {
                        BindingContext = item,
                        Content = button,
                        Padding = 0,
                        CornerRadius = 16,
                        BackgroundColor = Color.Transparent,
                        OutlineColor = color,
                        HasShadow = false
                    });
                }
        }

        private static void OnSelectedItemChangedAsync(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as PopupEnumPicker<T>;
            if (newvalue == null) return;
            if (picker != null)
            {
                foreach (Frame child in picker._stack.Children)
                {
                    (child.Content as Button).IsEnabled = true;
                    if(((KeyValuePair<T, string>)child.BindingContext).Key.Equals(picker.SelectedItem.Key))
                    {
                        (child.Content as Button).IsEnabled = false;
                    }
                }
            }
        }

        public static object GetDeepPropertyValue(object instance, string path)
        {
            var pp = path.Split('.');
            var t = instance.GetType();
            foreach (var prop in pp)
            {
                var propInfo = t.GetProperty(prop);
                if (propInfo != null)
                {
                    instance = propInfo.GetValue(instance, null);
                    t = propInfo.PropertyType;
                }
                else
                {
                    throw new ArgumentException("Properties path is not correct");
                }
            }
            return instance;
        }
    }
}