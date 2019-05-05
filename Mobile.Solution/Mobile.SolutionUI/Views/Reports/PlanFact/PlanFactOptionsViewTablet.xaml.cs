using System;
using Rg.Plugins.Popup.Services;

namespace Mobile.Solution.UI.Views
{
    public partial class PlanFactOptionsViewTablet
    {
        public PlanFactOptionsViewTablet(object viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}