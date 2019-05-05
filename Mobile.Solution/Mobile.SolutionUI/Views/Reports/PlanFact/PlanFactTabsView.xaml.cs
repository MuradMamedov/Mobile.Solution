using Mobile.Solution.Infrastructure.CustomControls;

namespace Mobile.Solution.UI.Views
{
    public partial class PlanFactTabsView
    {
        public PlanFactTabsView(SelectableViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}