using System;
using Mobile.Solution.UI.ViewModels;

namespace Mobile.Solution.UI.Views
{
    public partial class ParkProfitView
    {
        public ParkProfitView(ParkProfitViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}