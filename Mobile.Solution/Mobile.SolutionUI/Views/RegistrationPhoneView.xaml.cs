using Mobile.Solution.UI.ViewModels;
using Xamarin.Forms;

namespace Mobile.Solution.UI.Views
{
    public partial class RegistrationPhoneView
    {
        public RegistrationPhoneView(RegistrationViewModel viewModel)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = viewModel;
        }
    }
}