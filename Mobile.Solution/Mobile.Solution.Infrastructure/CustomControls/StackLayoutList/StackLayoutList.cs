using System;
using System.Collections;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public class ItemSelectedEventArgs : EventArgs
    {
        public ItemSelectedEventArgs(object selectedItem)
        {
            SelectedItem = selectedItem;
        }

        public object SelectedItem { get; private set; }
    }

    public class StackLayoutList : StackLayout
    {
        public delegate void ItemSelectedHandler(object sender, ItemSelectedEventArgs e);

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource),
            typeof(IList), typeof(StackLayoutList), propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate),
            typeof(DataTemplate), typeof(StackLayoutList), propertyChanged: OnItemTemplateChanged);

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem),
            typeof(object), typeof(StackLayoutList), propertyChanged: OnSelectedItemChanged);

        public IList ItemsSource
        {
            get { return (IList) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate) GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        private static void OnItemTemplateChanged(BindableObject pObj, object pOldVal, object pNewVal)
        {
            var layout = pObj as StackLayoutList;

            if (layout != null && layout.ItemsSource != null)
                layout.BuildLayout();
        }

        private static void OnSelectedItemChanged(BindableObject pObj, object pOldVal, object pNewVal)
        {
            var layout = pObj as StackLayoutList;
            if (layout != null)
            {
                layout.ItemSelected?.Invoke(layout, new ItemSelectedEventArgs(pNewVal));
            }
        }

        private static void OnItemsSourceChanged(BindableObject pObj, object pOldVal, object pNewVal)
        {
            var layout = pObj as StackLayoutList;

            if (layout != null && layout.ItemTemplate != null)
                layout.BuildLayout();
        }

        public event ItemSelectedHandler ItemSelected;

        private void BuildLayout()
        {
            Children.Clear();

            foreach (var item in ItemsSource)
            {
                var view = (View) ItemTemplate.CreateContent();
                view.BindingContext = item;
                var tapGR = new TapGestureRecognizer();
                tapGR.Tapped += (s, e) => { SelectedItem = item; };
                view.GestureRecognizers.Add(tapGR);
                Children.Add(view);
            }
        }
    }
}