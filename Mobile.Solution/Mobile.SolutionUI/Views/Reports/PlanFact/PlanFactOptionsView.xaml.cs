using System;
using Rg.Plugins.Popup.Services;

namespace Mobile.Solution.UI.Views
{
    public partial class PlanFactOptionsView
    {
        public PlanFactOptionsView() {}

        public PlanFactOptionsView(object viewModel)
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