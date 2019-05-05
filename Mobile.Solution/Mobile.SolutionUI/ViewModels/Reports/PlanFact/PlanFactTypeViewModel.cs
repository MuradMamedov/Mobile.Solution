using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Helpers;
using Mobile.Solution.Infrastructure.Requests.Reports;
using Mobile.Solution.UI.Views;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public class PlanFactTypeViewModel : SelectableViewModel
    {
        private Command _commandAccept;

        public PlanFactTypeViewModel()
        {
            Set();
        }

        public SelectableEnumViewModel<SourceType> Types { get; private set; }

        public Command CommandAccept => _commandAccept ?? (_commandAccept =
                                            new Command(async () => await CommandCreateImplementation()));

        void Set()
        {
            Types = new SelectableEnumViewModel<SourceType>(null, value => CommandAccept.Execute(value),
                ResourceContainer.ResourceManager);
            Types[SourceType.RzdType].IsEnabled = false;
        }

        private async Task CommandCreateImplementation()
        {
            var viewModel = new PlanFactParametersViewModel(null, null, true);
            viewModel.SourceType.SetValue(Types.SelectedValue.Key);
            await Application.Navigation.PushAsync(new PlanFactParametersView(viewModel));
            Types.SetValue(null);
        }
    }
}