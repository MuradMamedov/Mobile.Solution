using System;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.UI.ViewModels;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Mobile.Solution.UI.Views
{
    public partial class ShipmentHistogramCollectionView
    {
        public ShipmentHistogramCollectionView()
        {
            InitializeComponent();
        }

        async void Handle_Clicked(object sender, EventArgs e)
        {
            var parent = Parent;
            while (!(parent is ShipmentDiagramsView))
            {
                parent = parent.Parent;
            }
            var picker = new PopupEnumPicker<DiagramTypes>
            {
                BindingContext = (parent.BindingContext as ShipmentDiagramsViewModel).DiagramType,
                DisplayMemberPath = "Value"
            };
            picker.SetBinding(PopupEnumPicker<DiagramTypes>.ItemsSourceProperty, "ValuesList");
            picker.SetBinding(PopupEnumPicker<DiagramTypes>.SelectedItemProperty, "SelectedValue");
            await PopupNavigation.PushAsync(picker);
        }
    }
}