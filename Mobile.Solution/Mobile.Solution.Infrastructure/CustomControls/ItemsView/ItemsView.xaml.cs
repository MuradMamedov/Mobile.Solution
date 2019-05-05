using System;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public partial class ItemsView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(ICollection), typeof(ItemsView), default(ICollection),
                BindingMode.TwoWay, null, ItemsSourceChanged);

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(ItemsView), default(object),
                BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(ItemsView),
                default(DataTemplate));

        private readonly ICommand _selectedCommand;

        public ItemsView()
        {
            InitializeComponent();
            _selectedCommand = new Command<object>(item => SelectedItem = item);
        }

        public ICollection ItemsSource
        {
            get => (ICollection) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate) GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public event EventHandler SelectedItemChanged;

        public Element GetElementAt(int index)
        {
            if(StackLayout.Children.Count > index)
                return StackLayout.Children[index];
            return null;
        }

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var itemsLayout = (ItemsView) bindable;
            itemsLayout.SetItems();
        }

        private void SetItems()
        {
            StackLayout.Children.Clear();

            if (ItemsSource == null)
                return;

            foreach (var item in ItemsSource)
                StackLayout.Children.Add(GetItemView(item));

            SelectedItem = ItemsSource.Cast<object>().FirstOrDefault();
        }

        private View GetItemView(object item)
        {
            var content = ItemTemplate.CreateContent();
            var view = content as View;
            if (view != null)
            {
                view.BindingContext = item;

                view.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = _selectedCommand,
                    CommandParameter = item
                });
				var tapGR = new TapGestureRecognizer();
				tapGR.Tapped += (s, e) => { SelectedItem = item; };
				view.GestureRecognizers.Add(tapGR);
                return view;
            }
            return null;
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var itemsView = (ItemsView) bindable;

            if (newValue == oldValue)
                return;

            var items = itemsView.ItemsSource?.OfType<ISelectable>();

            if(items != null)
                foreach (var item in items)
                    item.IsSelected = item == newValue;

            itemsView.SelectedItemChanged?.Invoke(itemsView, EventArgs.Empty);
        }
    }
}