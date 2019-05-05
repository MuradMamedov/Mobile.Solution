using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public class CarouselLayout : ScrollView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(CarouselLayout), null,
                propertyChanging: ItemsSourceChanging, propertyChanged: ItemsSourceChanged);

        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(CarouselLayout), 0, BindingMode.TwoWay,
                propertyChanged: SelectedIndexChanged);

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(CarouselLayout), null,
                BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

        public static readonly BindableProperty TemplateSelectorProperty =
            BindableProperty.Create(nameof(TemplateSelector), typeof(TemplateSelector), typeof(CarouselLayout),
                default(TemplateSelector));

        private readonly StackLayout _stack;

        private bool _layingOutChildren;

        private int _selectedIndex;

        public CarouselLayout()
        {
            Orientation = ScrollOrientation.Horizontal;

            _stack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 0
            };

            Content = _stack;
        }

        public IList<View> Children => _stack.Children;

        public IList ItemsSource
        {
            get => (IList) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public TemplateSelector TemplateSelector
        {
            get => (TemplateSelector) GetValue(TemplateSelectorProperty);
            set => SetValue(TemplateSelectorProperty, value);
        }

        public int SelectedIndex
        {
            get => (int) GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
            if (_layingOutChildren) return;

            _layingOutChildren = true;
            foreach (var child in Children) child.WidthRequest = width;
            _layingOutChildren = false;
        }

        public event EventHandler SelectedItemChanged;

        private void UpdateSelectedIndex()
        {
            if (SelectedItem == BindingContext) return;

            SelectedIndex = Children
                .Select(c => c.BindingContext)
                .ToList()
                .IndexOf(SelectedItem);
        }

        private static void ItemsSourceChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            ((CarouselLayout) bindableObject).ItemsSourceChanged();
        }

        private void ItemsSourceChanged()
        {
            _stack.Children.Clear();
            foreach (var item in ItemsSource)
            {
                var view = TemplateSelector.ViewFor(item);
                var bindableObject = (BindableObject) view;
                if (bindableObject != null)
                    bindableObject.BindingContext = item;
                _stack.Children.Add(view);
            }

            if (_selectedIndex >= 0) SelectedIndex = _selectedIndex;
        }

        private static void ItemsSourceChanging(BindableObject bindableObject, object oldValue, object newValue)
        {
            ((CarouselLayout) bindableObject).ItemsSourceChanging();
        }

        private void ItemsSourceChanging()
        {
            if (ItemsSource == null) return;
            _selectedIndex = ItemsSource.IndexOf(SelectedItem);
        }

        private static void SelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((CarouselLayout) bindable).UpdateSelectedItem();
        }

        private void UpdateSelectedItem()
        {
            SelectedItem = SelectedIndex > -1 && SelectedIndex < Children.Count
                ? Children[SelectedIndex].BindingContext
                : null;
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var carouselLayout = (CarouselLayout) bindable;
            carouselLayout.UpdateSelectedIndex();
            carouselLayout.SelectedItemChanged?.Invoke(carouselLayout, EventArgs.Empty);
        }
    }
}