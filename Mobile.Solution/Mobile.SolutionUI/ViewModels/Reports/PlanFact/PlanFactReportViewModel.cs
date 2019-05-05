using System.IO;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.Infrastructure.Requests.Reports;
using Mobile.Solution.UI.Views;
using Newtonsoft.Json;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Pages;

namespace Mobile.Solution.UI.ViewModels
{
    public class PlanFactReportViewModel
    {
        private readonly string _name;

        private readonly PlanFactParameters _parameters;

        private readonly PlanFactTabsViewModel _parent;

        public PlanFactReportViewModel(PlanFactTabsViewModel parent, Report report)
        {
            _parent = parent;
            _parameters = JsonConvert.DeserializeObject<PlanFactParameters>(report.Content);
            _name = report.Name;
            Report = report;
        }

        public string Name => Path.GetFileNameWithoutExtension(_name);

        public string PropertiesStr =>
            $"{(_parameters.SourceType == SourceType.EtranType ? "Погрузка" : "Погрузка по Сети РЖД")} ({Report.DateCreated:dd MMMM yyyy})";

        public Report Report { get; }

        #region Commands

        public Command CommandAccept => _commandAccept ?? (_commandAccept =
                                            new Command(async () => await CommandAcceptImplementation()));

        private Command _commandAccept;

        private async Task CommandAcceptImplementation()
        {
            await new PlanFactParametersViewModel(Name, _parameters, false).CommandAcceptInmplementation();
        }

        public Command CommandEdit => _commandEdit ??
                                      (_commandEdit = new Command(async () => await CommandEditImplementation()));

        private Command _commandEdit;

        private async Task CommandEditImplementation()
        {
            var viewModel = new PlanFactParametersViewModel(Name, _parameters, false);
            await Application.Navigation.PushAsync(new PlanFactParametersView(viewModel));
        }

        public Command CommandDelete => _commandDelete ?? (_commandDelete =
                                            new Command(async () => await CommandDeleteImplementation()));

        private Command _commandDelete;

        private async Task CommandDeleteImplementation()
        {
            if (await Dialog.Instance.ConfirmAsync("Удалить отчет?", okText: "Удалить", cancelText: "Отмена"))
            {
                var filePreview = DependencyService.Get<IFilePreview>();
                filePreview.Delete(_name);
                _parent.Refresh();

            }
        }

        public Command CommandShowOptions => _commandShowOptions ?? (_commandShowOptions =
                                           new Command(async () => await CommandShowOptionsImplementation()));

        private Command _commandShowOptions;

        private async Task CommandShowOptionsImplementation()
        {
            if (Device.Idiom == TargetIdiom.Phone)
                await PopupNavigation.PushAsync(new PopupPage { Content = new PlanFactOptionsView(this) });
        }

        #endregion
    }
}