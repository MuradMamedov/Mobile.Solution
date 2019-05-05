using System;
using System.Collections;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public partial class TabbedView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(TabbedView), default(IList),
                BindingMode.TwoWay, null, ItemsSourceChanged);

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(TabbedView), default(object),
                BindingMode.TwoWay, null, SelectedItemChanged);

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(TabbedView),
                default(DataTemplate), BindingMode.TwoWay, null, ItemTemplateChanged);

        public static readonly BindableProperty TemplateSelectorProperty =
            BindableProperty.Create(nameof(TemplateSelector), typeof(TemplateSelector), typeof(TabbedView),
                default(TemplateSelector), BindingMode.Default, null, TemplateSelectorChanged);

        public static readonly BindableProperty HeaderVisibilityProperty =
            BindableProperty.Create(nameof(HeaderVisibility), typeof(bool), typeof(TabbedView), true,
                BindingMode.OneWay, null, ItemsVisibilityChanged);

        public TabbedView()
        {
            InitializeComponent();
            Items.SelectedItemChanged += HandleSelectedItemChanged;
        }

        public IList ItemsSource
        {
            get => (IList) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public object SelectedItem
        {
            get => (DataTemplate) GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public bool HeaderVisibility
        {
            get => (bool) GetValue(HeaderVisibilityProperty);
            set => SetValue(HeaderVisibilityProperty, value);
        }


        public DataTemplate ItemTemplate
        {
            get => (DataTemplate) GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public TemplateSelector TemplateSelector
        {
            get => (TemplateSelector) GetValue(TemplateSelectorProperty);
            set => SetValue(TemplateSelectorProperty, value);
        }

        public ContentView GetCurrentView()
        {
            return Content;
        }

        private void HandleSelectedItemChanged(object sender, EventArgs e)
        {
            Content.ViewModel = Items.SelectedItem;
        }

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (TabbedView) bindable;
            view.SetItemsSource(newValue as IList);
        }

        private static void SelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (TabbedView) bindable;
            view.SetSelectedItem(newValue);
        }

        private static void ItemsVisibilityChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (TabbedView) bindable;
            view.Items.IsVisible = (bool) newValue;
        }

        private void SetItemsSource(IList itemsSource)
        {
            Items.ItemsSource = itemsSource;
        }

        private void SetSelectedItem(object selectedItem)
        {
            Items.SelectedItem = selectedItem;
        }

        private static void ItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (TabbedView) bindable;
            view.SetItemTemplate(newValue as DataTemplate);
        }

        private void SetItemTemplate(DataTemplate itemTemplate)
        {
            Items.ItemTemplate = itemTemplate;
        }

        private static void TemplateSelectorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (TabbedView) bindable;
            view.SetTemplateSelector(newValue as TemplateSelector);
        }

        private void SetTemplateSelector(TemplateSelector templateSelector)
        {
            Content.TemplateSelector = templateSelector;
        }
    }
}