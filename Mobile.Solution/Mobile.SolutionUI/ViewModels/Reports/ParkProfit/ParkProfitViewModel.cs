using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.Infrastructure.Helpers;
using Mobile.Solution.Infrastructure.Requests.NSI;
using Mobile.Solution.Infrastructure.Requests.NSI.Contracts;
using Mobile.Solution.Infrastructure.Requests.Reports;
using Mobile.Solution.UI.Views;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public class ParkProfitViewModel : SelectableViewModel
    {
        private static readonly Lazy<ParkProfitViewModel> _instance =
            new Lazy<ParkProfitViewModel>(() => new ParkProfitViewModel());

        private ParkProfitViewModel()
        {
            Header = ResourceContainer.ResourceManager.GetString("ParkProfitReport");

            TargetPage = new ParkProfitView(this);
        }

        public static ParkProfitViewModel Instance => _instance.Value;

        private async Task Initialize()
        {
            try
            {
                IsBusy = true;
                var appInfo = DependencyService.Get<IAppInfo>();

                Tabs = new List<SelectableViewModel> {new ProfitDiagramsViewModel()};

                DateFrom = DateTime.Now.AddDays(1 - DateTime.Now.Day);

                DateTo =
                    DateTime.Now.AddDays(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) -
                                         DateTime.Now.Day);

                var agreements =
                    (await NsiManager.GetNsi<InfoAgreement>(appInfo.UniqueId)).OrderBy(i => i.DisplayName).ToList();

                Parks =
                    new ObservableCollection<InfoAgreementViewModel>(
                        agreements.Select(a => new InfoAgreementViewModel(a)));

                RaisePropertyChanged("");
                CommandAccept.ChangeCanExecute();
            }
            finally
            {
                IsBusy = false;
            }
        }

        #region Properties

        #region Parameters

        public DateTime DateFrom
        {
            get => _dateFrom;
            set { Set(() => DateFrom, ref _dateFrom, value); }
        }

        private DateTime _dateFrom = DateTime.Now.AddMonths(1).AddDays(-1).Date;

        public DateTime DateTo
        {
            get => _dateTo;
            set { Set(() => DateTo, ref _dateTo, value); }
        }

        private DateTime _dateTo = DateTime.Now.AddDays(1 - DateTime.Now.Day).Date;

        public ObservableCollection<InfoAgreementViewModel> Parks { get; set; } =
            new ObservableCollection<InfoAgreementViewModel>();

        public string SelectedParksNames
        {
            get
            {
                return Parks != null
                    ? string.Join("; ", Parks.Where(p => p.IsChecked).Select(p => p.DisplayName))
                    : null;
            }
        }

        #endregion Parameters

        #endregion Properties

        #region Commands

        public Command CommandAccept
        {
            get
            {
                return _commandAccept ?? (_commandAccept = new Command(async () => await RetrieveData(),
                           () => { return Parks?.Count(p => p.IsChecked) > 0; }));
            }
        }

        private Command _commandAccept;

        private async Task RetrieveData()
        {
            try
            {
                IsBusy = true;
                var appInfo = DependencyService.Get<IAppInfo>();

                var ladens = await
                    ParkProfitLadenReport.Instance.InitRequest(
                        $"uniqueId={appInfo.UniqueId}&startDateStr={DateFrom:dd-MM-yyyy}&" +
                        $"endDateStr={DateTo.AddDays(1):dd-MM-yyyy}" +
                        string.Join("",
                            Parks.Where(p => p.IsChecked)
                                .Select(p => "&parks=" + p.Id)));


                var empties = await
                    ParkProfitEmptyReport.Instance.InitRequest(
                        $"uniqueId={appInfo.UniqueId}&startDateStr={DateFrom:dd-MM-yyyy}&" +
                        $"endDateStr={DateTo.AddDays(1):dd-MM-yyyy}" +
                        string.Join("",
                            Parks.Where(p => p.IsChecked)
                                .Select(p => "&parks=" + p.Id)));

                if (empties == null || empties.Count == 0 || ladens == null || !ladens.Any())
                    Dialog.Instance.Alert("Запрос не вернул данных",
                        ResourceContainer.ResourceManager.GetString("DialogAttentionTitle"));

                var cars = new List<InfoCarInAgreement>();
                foreach (var p in Parks.Where(p => p.IsChecked))
                    cars.AddRange(await NsiManager.MatchNsi<InfoCarInAgreement>(appInfo.UniqueId, "null",
                        new Tuple<string, string>("AgreementID", p.Id.ToString())));
            }
            catch (Exception ex)
            {
                Dialog.Instance.Alert(ex.Message, ResourceContainer.ResourceManager.GetString("DialogErrorTitle"));
            }
            finally
            {
                IsBusy = false;
            }
        }

        public Command CommandSelectParks
        {
            get
            {
                return _commandSelectParks ?? (_commandSelectParks = new Command(async () =>
                {
                    var modalPage =
                        new PopupListView<InfoAgreementViewModel>(
                            new PopupListViewModel<InfoAgreementViewModel>(Parks));
                    modalPage.Disappearing += (s, e) =>
                    {
                        RaisePropertyChanged(() => Parks);
                        RaisePropertyChanged(() => SelectedParksNames);
                        CommandAccept.ChangeCanExecute();
                    };
                    await Application.Navigation.PushModalAsync(modalPage);
                }));
            }
        }

        private Command _commandSelectParks;

        #endregion
    }
}