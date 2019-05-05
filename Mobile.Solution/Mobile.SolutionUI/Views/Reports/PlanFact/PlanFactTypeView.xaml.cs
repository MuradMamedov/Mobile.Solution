using Mobile.Solution.Infrastructure.CustomControls;

namespace Mobile.Solution.UI.Views
{
    public partial class PlanFactTypeView
    {
        public PlanFactTypeView(SelectableViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}