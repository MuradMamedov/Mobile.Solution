using System;
using System.Collections;
using System.Linq;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public enum PickerType
    {
        Month,
        Year
    }

    public sealed class YearMonthPicker : Picker
    {
        public new static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(int), typeof(YearMonthPicker), default(int),
                BindingMode.TwoWay);

        public static readonly BindableProperty PickerTypeProperty =
            BindableProperty.Create(nameof(SelectionType), typeof(PickerType), typeof(YearMonthPicker),
                default(PickerType), BindingMode.TwoWay);

        public static readonly BindableProperty MinYearProperty =
            BindableProperty.Create(nameof(MinYear), typeof(int), typeof(YearMonthPicker), DateTime.Now.Year - 10);

        public static readonly BindableProperty MaxYearProperty =
            BindableProperty.Create(nameof(MaxYear), typeof(int), typeof(YearMonthPicker), DateTime.Now.Year);

        public static readonly BindableProperty EnabledMonthsProperty =
            BindableProperty.Create(nameof(EnabledMonths), typeof(IList), typeof(YearMonthPicker),
                propertyChanged: EnabledMonthsPropertyChanged);

        public YearMonthPicker()
        {
            SelectedIndexChanged += CustomDatePicker_SelectedIndexChanged;
        }

        public PickerType SelectionType
        {
            get => (PickerType) GetValue(PickerTypeProperty);
            set => SetValue(PickerTypeProperty, value);
        }

        public new int SelectedItem
        {
            get => (int) GetValue(SelectedItemProperty);
            set
            {
                SetValue(SelectedItemProperty, value);
                string title;

                switch (SelectionType)
                {
                    case PickerType.Month:
                        title = GetMonthName(value);
                        break;
                    default:
                        title = value.ToString();
                        break;
                }
                Title = title;
            }
        }

        public int MinYear
        {
            get => (int) GetValue(MinYearProperty);
            set => SetValue(MinYearProperty, value);
        }

        public int MaxYear
        {
            get => (int) GetValue(MaxYearProperty);
            set => SetValue(MaxYearProperty, value);
        }

        public IList EnabledMonths
        {
            get => (IList) GetValue(EnabledMonthsProperty);
            set => SetValue(EnabledMonthsProperty, value);
        }

        private void CustomDatePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndex > -1)
            {
                var value = Items.ElementAt(SelectedIndex);
                SelectedItem = (int) Enum.Parse(typeof(Months), value);
            }
        }

        private static void EnabledMonthsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ym = (YearMonthPicker) bindable;

            ym.D();
        }

        private void D()
        {
            if (Items.Count > 0)
            {
                Items.Clear();
                var months = Enum.GetNames(typeof(Months));
                for (var i = 1; i <= months.Length; i++)
                    if (EnabledMonths.Contains(i))
                    {
                        Items.Add(months[i - 1]);
                        if (i == SelectedItem) SelectedIndex = i - 1;
                    }
                if (SelectedIndex == -1)
                    SelectedIndex = 0;
            }
        }

        private string GetMonthName(int month)
        {
            return Enum.GetName(typeof(Months), (Months) month);
        }

        protected override void OnBindingContextChanged()
        {
            Items.Clear();
            if (SelectionType == PickerType.Year)
            {
                //show only years
                for (var i = MinYear; i <= MaxYear; i++)
                {
                    Items.Add(i.ToString());
                    if (i == SelectedItem)
                        SelectedIndex = i - 1;
                }
            }
            else if (SelectionType == PickerType.Month)
            {
                var months = Enum.GetNames(typeof(Months));
                for (var i = 1; i <= months.Length; i++)
                    if (EnabledMonths.Contains(i))
                    {
                        Items.Add(months[i - 1]);
                        if (i == SelectedItem) SelectedIndex = i - 1;
                    }
            }
        }

        private enum Months
        {
            Январь = 1,
            Февраль,
            Март,
            Апрель,
            Май,
            Июнь,
            Июль,
            Август,
            Сентябрь,
            Октябрь,
            Ноябрь,
            Декабрь
        }
    }
}