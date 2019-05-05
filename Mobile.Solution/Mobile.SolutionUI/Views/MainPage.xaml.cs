using System.Threading.Tasks;
using Mobile.Solution.UI.ViewModels;

namespace Mobile.Solution.UI.Views
{
    public partial class MainPage
    {
        public MainPage()
            : base(PlanFactTabsViewModel.Instance.TargetPage)
        {
            InitializeComponent();
        }

        public async Task Initialize()
        {
            var viewModel = new MainPageViewModel(Navigation);

            BindingContext = viewModel;

            await viewModel.CheckForUpdates();

            await viewModel.CheckRegistration();

            await viewModel.CheckForDictUpdates();

            await viewModel.LoadDictionaries();

            await viewModel.SendPreviousErrors();

            PlanFactTabsViewModel.Instance.Refresh();
        }


        public async Task CheckForUpdates()
        {
            await (BindingContext as MainPageViewModel).CheckForUpdates();
        }

        public async Task CheckForDictUpdates()
        {
            if (await (BindingContext as MainPageViewModel).CheckForDictUpdates())
            {
                while (!await (BindingContext as MainPageViewModel).LoadDictionaries())
                {
                }
            }
        }
    }
}