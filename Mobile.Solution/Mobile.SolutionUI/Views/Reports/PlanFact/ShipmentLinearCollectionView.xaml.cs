using System;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.UI.ViewModels;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Mobile.Solution.UI.Views
{
    public partial class ShipmentLinearCollectionView
    {
        public ShipmentLinearCollectionView()
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
                BindingContext = parent.BindingContext,
                DisplayMemberPath = "Value"
            };
            picker.SetBinding(PopupEnumPicker<DiagramTypes>.ItemsSourceProperty, "DiagramType.ValuesList");
            picker.SetBinding(PopupEnumPicker<DiagramTypes>.SelectedItemProperty, "DiagramType.SelectedValue");
            await PopupNavigation.PushAsync(picker);
        }
    }
}