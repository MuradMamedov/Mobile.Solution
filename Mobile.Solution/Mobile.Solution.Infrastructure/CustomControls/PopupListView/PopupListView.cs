using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public class PopupListView<T> : ContentPage
        where T : IPopupListViewItem
    {
        public PopupListView(PopupListViewModel<T> viewModel)
        {
            viewModel.Navigation = Navigation;
            BindingContext = viewModel;
            var listView = new ListView
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    var checkBox = new Switch();
                    checkBox.SetBinding(Switch.IsToggledProperty, new Binding("IsChecked", BindingMode.TwoWay));

                    var label = new Label();
                    label.SetBinding(Label.TextProperty, new Binding("DisplayName"));
                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Spacing = 0,
                            Children =
                            {
                                checkBox,
                                label
                            }
                        }
                    };
                })
            };
            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("Items"));
            listView.ItemSelected += (s, e) =>
            {
                var selectedItem = (T) e.SelectedItem;
                selectedItem.IsChecked = !selectedItem.IsChecked;
            };

            var applyBtn = new Button
            {
                Text = "Принять"
            };
            applyBtn.SetBinding(Button.CommandProperty, new Binding("CommandApply"));

            Content = new StackLayout
            {
                Children =
                {
                    listView,
                    applyBtn
                }
            };
        }
    }
}