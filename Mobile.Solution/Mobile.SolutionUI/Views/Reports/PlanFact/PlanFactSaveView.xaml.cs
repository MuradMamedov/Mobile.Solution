using System;
using Rg.Plugins.Popup.Services;

namespace Mobile.Solution.UI.Views
{
    public partial class PlanFactSaveView
    {
        public PlanFactSaveView(object viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }

        private async void ClosePopup(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }
    }
}