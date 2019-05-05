using System.Collections;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public partial class CarouselView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(CarouselView), default(IList),
                BindingMode.TwoWay, null, ItemsSourceChanged);

        public static readonly BindableProperty LabelsHeightProperty =
            BindableProperty.Create(nameof(LabelsHeight), typeof(double), typeof(CarouselView), default(double),
                BindingMode.OneWay, null, LabelsHeightChanged);

        public static readonly BindableProperty IsLabelsVisibleProperty =
            BindableProperty.Create(nameof(IsLabelsVisible), typeof(bool), typeof(CarouselView), true,
                BindingMode.OneWay, null, IsLabelsVisibleChanged);

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(CarouselView),
                default(DataTemplate), BindingMode.TwoWay, null, ItemTemplateChanged);

        public static readonly BindableProperty TemplateSelectorProperty =
            BindableProperty.Create(nameof(TemplateSelector), typeof(TemplateSelector), typeof(CarouselView),
                default(TemplateSelector), BindingMode.Default, null, TemplateSelectorChanged);

        public CarouselView()
        {
            InitializeComponent();
            Items.SelectedItemChanged += (sender, args) => { Content.SelectedItem = Items.SelectedItem; };
            Content.SelectedItemChanged += (sender, args) => { Items.SelectedItem = Content.SelectedItem; };
        }

        public double LabelsHeight
        {
            get => (double) GetValue(LabelsHeightProperty);
            set => SetValue(LabelsHeightProperty, value);
        }

        public bool IsLabelsVisible
        {
            get => (bool) GetValue(IsLabelsVisibleProperty);
            set => SetValue(IsLabelsVisibleProperty, value);
        }

        public IList ItemsSource
        {
            get => (IList) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
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

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (CarouselView) bindable;
            view.SetItemsSource(newValue as IList);
        }

        private void SetItemsSource(IList itemsSource)
        {
            Items.ItemsSource = itemsSource;
            Content.ItemsSource = itemsSource;
        }

        private static void LabelsHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (CarouselView) bindable;
            view.SetLabelsHeight((double) newValue);
        }

        private void SetLabelsHeight(double height)
        {
            LabelsRow.Height = height;
        }


        private static void IsLabelsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (CarouselView) bindable;
            view.SetIsLabelsVisible((bool) newValue);
        }

        private void SetIsLabelsVisible(bool isVisible)
        {
            LabelsRow.Height = isVisible ? LabelsHeight : GridLength.Auto;
            Items.IsVisible = isVisible;
        }

        private static void ItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (CarouselView) bindable;
            view.SetItemTemplate(newValue as DataTemplate);
        }

        private void SetItemTemplate(DataTemplate itemTemplate)
        {
            Items.ItemTemplate = itemTemplate;
        }

        private static void TemplateSelectorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (CarouselView) bindable;
            view.SetTemplateSelector(newValue as TemplateSelector);
        }

        private void SetTemplateSelector(TemplateSelector templateSelector)
        {
            Content.TemplateSelector = templateSelector;
        }
    }
}