using System;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.Data.Extensions;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
	public class EnumPicker<T> : Picker
        where T : struct
    {
        public new static BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IDictionary<T, string>), typeof(EnumPicker<T>),
                default(IDictionary<T, string>), propertyChanged: OnItemsSourceChanged);

        public new static BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(KeyValuePair<T, string>), typeof(EnumPicker<T>),
                default(KeyValuePair<T, string>), BindingMode.TwoWay,
                propertyChanged: OnSelectedItemChanged);

        public static BindableProperty DisplayMemberPathProperty =
            BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(EnumPicker<T>), default(string));

        public EnumPicker()
        {
            SelectedIndexChanged += OnSelectedIndexChanged;
        }

        public new IDictionary<T, string> ItemsSource
        {
            get => (IDictionary<T, string>) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public new KeyValuePair<T, string> SelectedItem
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
            var picker = bindable as EnumPicker<T>;
            if (picker == null) return;
            picker.Items.Clear();
            if (newvalue != null)
                foreach (var item in (IDictionary<T, string>) newvalue)
                    picker.Items.Add(GetDeepPropertyValue(item, picker.DisplayMemberPath)?.ToString());
        }

        private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (SelectedIndex < 0 || SelectedIndex > Items.Count - 1)
                SelectedItem = new KeyValuePair<T, string>();
            else
                SelectedItem = ItemsSource.ElementAt(SelectedIndex);
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as EnumPicker<T>;
            if (newvalue == null) return;
            if (picker != null)
                picker.SelectedIndex = picker.Items.IndexOf(((KeyValuePair<T, string>) newvalue).Value);
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