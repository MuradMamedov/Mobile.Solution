using System.Collections.Generic;
using Mobile.Solution.Infrastructure.Requests.NSI.Contracts;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public partial class SearchOrganisationPage
    {
        private SearchNsiViewModel<InfoOrgPassport> _viewModel;

        public SearchOrganisationPage(object context)
        {
            InitializeComponent();
            BindingContext = context;
            _viewModel = context as SearchNsiViewModel<InfoOrgPassport>;
            _viewModel.Navigation = Navigation;
            TabbedView.ItemsSource = new List<SearchNsiViewModel<InfoOrgPassport>>
            {
                new SearchNsiViewModel<InfoOrgPassport>(_viewModel, "Полное наименование", "Name"),
                new SearchNsiViewModel<InfoOrgPassport>(_viewModel, "Краткое наименование", "ShortName"),
                new SearchNsiViewModel<InfoOrgPassport>(_viewModel, "ОКПО", "Okpo")
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var content = ((TabbedView.GetCurrentView().Content as ContentView).Content as Frame).Content as AutoCompleteView;
            content.EntText.Focus();
        }
    }
}