using System;
using System.Linq;
using Mobile.Solution.UI.ViewModels;

namespace Mobile.Solution.UI.Views
{
    public partial class PlanFactParametersView
    {
        public PlanFactParametersView(PlanFactParametersViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }

        private async void Return_Clicked(object sender, EventArgs e)
        {
            try
            {
                var existingPages = Application.Navigation.NavigationStack.ToList();
                for (var i = existingPages.Count - 1; i > 0; i--)
                    await Application.Navigation.PopAsync(false);
            }
            catch
            {
                //ignored
            }
        }
    }
}