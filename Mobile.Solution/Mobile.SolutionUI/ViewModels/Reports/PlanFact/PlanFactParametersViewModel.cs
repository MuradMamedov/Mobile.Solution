using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.Infrastructure.Helpers;
using Mobile.Solution.Infrastructure.Requests.NSI.Contracts;
using Mobile.Solution.Infrastructure.Requests.Reports;
using Mobile.Solution.UI.Helpers;
using Mobile.Solution.UI.Views;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public class CustomDateTime
    {
        public CustomDateTime(DateTime date)
        {
            Date = date;
        }

        public DateTime Date { get; }

        public override string ToString()
        {
            return $"{Date:MMMM yyyy}";
        }
    }

    public class PlanFactParametersViewModel : SelectableViewModel
    {
        private readonly bool _isNew;
        private ShipmentTabsViewModel _tabsVm;
        private bool _pageCreated;

        public PlanFactParametersViewModel(string name, PlanFactParameters parameters, bool isNew)
        {
            _isNew = isNew;
            _name = name;
            Initialize(parameters);
        }

        private void SetTabsVm()
        {
            var excludes = new List<Creator>();
            if (Parameters.Recip != null)
                excludes.Add(new ContractorCreator(ReportDirections.Arrival));
            if (Parameters.Sender != null)
                excludes.Add(new ContractorCreator(ReportDirections.Departure));
            if (Parameters.Payer != null)
            {
                excludes.Add(new PayerCreator(ReportDirections.Arrival));
                excludes.Add(new PayerCreator(ReportDirections.Departure));
            }
            if (Parameters.Owner != null)
            {
                excludes.Add(new OwnerCreator(ReportDirections.Arrival));
                excludes.Add(new OwnerCreator(ReportDirections.Departure));
            }
            if (Parameters.FreightGroup?.Number > 0)
            {
                excludes.Add(new FreightCreator(ReportDirections.Arrival));
                excludes.Add(new FreightCreator(ReportDirections.Departure));
            }

            if (Parameters.ToStation != null)
            {
                excludes.Add(new RailwayCreator(ReportDirections.Arrival));
                excludes.Add(new StationCreator(ReportDirections.Arrival));
            }
            if (Parameters.FromStation != null)
            {
                excludes.Add(new RailwayCreator(ReportDirections.Departure));
                excludes.Add(new StationCreator(ReportDirections.Departure));
            }

            var mainCreator = new MainCreator(Parameters.ReportDirection.Value, excludes.ToArray());
            var itemsToRemove = mainCreator.Tabs.Where(t => t.GetType() == typeof(StationCreator)).ToList();
            foreach (var item in itemsToRemove)
                mainCreator.Tabs.Remove(item);
            if (_tabsVm == null)
            {
                _tabsVm = new ShipmentTabsViewModel(Name, mainCreator, Parameters, _isNew);
            }
            else
            {
                _tabsVm.Parameters = Parameters;
                _tabsVm.IsNew = true;
                _tabsVm.Creator = mainCreator;
            }
        }

        private void Initialize(PlanFactParameters parameters)
        {
            try
            {
                IsBusy = true;

                SourceType = new SelectableEnumViewModel<SourceType>( Infrastructure.Requests.Reports.SourceType.EtranType, null, ResourceContainer.ResourceManager);

                SelectedDate = Dates.FirstOrDefault(
                    d => d.Date.Date.Equals(DateTime.Now.Date.AddDays(1 - DateTime.Now.Day)));

                ConditionScheme = new ObservableEnum<CargoPlanConditionScheme>(CargoPlanConditionScheme.Gu12Rep, null,
                    ResourceContainer.ResourceManager);

                PlanType = new ObservableEnum<CargoPlanTypeScheme>(CargoPlanTypeScheme.OperativePlan,
                    value => RaisePropertyChanged(() => Header), ResourceContainer.ResourceManager);

                UnitType =
                    new SelectableEnumViewModel<UnitTypes>(UnitTypes.Cars, null, ResourceContainer.ResourceManager);

                ReportDirection = new SelectableEnumViewModel<ReportDirections>(ReportDirections.Departure,
                    value =>
                    {
                        IsDeparture = value == ReportDirections.Departure;
                        IsArrival = value == ReportDirections.Arrival;
                        RaiseValidation();
                    },
                    ResourceContainer.ResourceManager);

                PeriodType = new SelectableEnumViewModel<PeriodTypes>(PeriodTypes.Month,
                    value =>
                    {
                        IsMonth = value == PeriodTypes.Month;
                        IsPeriod = value == PeriodTypes.AnyPeriod;
                        RaiseValidation();
                    },
                    ResourceContainer.ResourceManager);

                Sender = new SearchNsiViewModel<InfoOrgPassport>(ResourceContainer.ResourceManager.GetString("Sender"),
                    callback: value =>
                    {
                        if (value != null)
                        {
                            Recip.SelectedItem = null;
                            ToStation.SelectedItem = null;
                        }
                        RaiseValidation();
                    });

                Payer = new SearchNsiViewModel<InfoOrgPassport>(ResourceContainer.ResourceManager.GetString("Payer"),
                    callback: value => { RaiseValidation(); });

                Owner = new SearchNsiViewModel<InfoOrgPassport>(ResourceContainer.ResourceManager.GetString("Owner"),
                    callback: value => { RaiseValidation(); });

                FromStation = new SearchNsiViewModel<InfoUniqueStation>(
                    ResourceContainer.ResourceManager.GetString("FromStation"),
                    callback: value =>
                    {
                        if (value != null)
                        {
                            Recip.SelectedItem = null;
                            ToStation.SelectedItem = null;
                        }
                        RaiseValidation();
                    });

                Recip = new SearchNsiViewModel<InfoOrgPassport>(ResourceContainer.ResourceManager.GetString("Recip"),
                    callback: value =>
                    {
                        if (value != null)
                        {
                            Sender.SelectedItem = null;
                            FromStation.SelectedItem = null;
                        }
                        RaiseValidation();
                    });

                ToStation = new SearchNsiViewModel<InfoUniqueStation>(
                    ResourceContainer.ResourceManager.GetString("ToStation"),
                    callback: value =>
                    {
                        if (value != null)
                        {
                            Sender.SelectedItem = null;
                            FromStation.SelectedItem = null;
                        }
                        RaiseValidation();
                    });

                FreightGroup = new SearchNsiViewModel<InfoSumFreight>(
                    ResourceContainer.ResourceManager.GetString("FreightGroup"),
                    null, "каменный уголь");
                FreightGroup.Items = FreightGroup.Items.OrderBy(i => i.DisplayName).ToList();
                FreightGroup.Items.Remove(FreightGroup.Items.FirstOrDefault(i => i.Number == 0));
                FreightGroup.Items.Insert(0, new InfoSumFreight {Number = null, Name = "Все"});

                if(parameters != null)
                {
                    SelectedDate = Dates.FirstOrDefault(d => d.Date.Date.Equals(parameters.Date.Date));
                    DateFrom = parameters.DateFrom;
                    DateTo = parameters.DateTo;
                    ConditionScheme.SetValue(parameters.ConditionScheme);
                    PlanType.SetValue(parameters.PlanType);
                    ReportDirection.SetValue(parameters.ReportDirection.Value);
                    PeriodType.SetValue(parameters.PeriodType);
                    SourceType.SetValue(parameters.SourceType);
                    UnitType.SetValue(parameters.UnitType);
                    Sender.SelectedItem = parameters.Sender;
                    Payer.SelectedItem = parameters.Payer;
                    Owner.SelectedItem = parameters.Owner;
                    FromStation.SelectedItem = parameters.FromStation;
                    Recip.SelectedItem = parameters.Recip;
                    ToStation.SelectedItem = parameters.ToStation;
                    FreightGroup.SelectedItem =
                        FreightGroup.Items.FirstOrDefault(item => item.Number == parameters.FreightGroup.Number);
                }

                RaisePropertyChanged("");
                RaiseValidation();
            }
#if DEBUG
#else
            catch
            {
            }
#endif
            finally
            {
                IsBusy = false;
            }
        }

        private void RaiseValidation()
        {
            RaisePropertyChanged(() => IsNotValidated);
            RaisePropertyChanged(() => ValidationText);
            CommandAccept.ChangeCanExecute();
        }

        #region Properties

        #region Parameters

        /// <summary>
        ///     Агрегированные параметры для обновления дочерних око
        /// </summary>
        public PlanFactParameters Parameters => new PlanFactParameters
        {
            Date = SelectedDate.Date,
            DateFrom = DateFrom,
            DateTo = DateTo,
            ConditionScheme = ConditionScheme.SelectedValue.Key,
            PlanType = PlanType.SelectedValue.Key,
            Recip = Recip.SelectedItem,
            Sender = Sender.SelectedItem,
            Payer = Payer.SelectedItem,
            Owner = Owner.SelectedItem,
            FromStation = FromStation.SelectedItem,
            ReportDirection = ReportDirection.SelectedValue.Key,
            PeriodType = PeriodType.SelectedValue.Key,
            SourceType = SourceType.SelectedValue.Key,
            UnitType = UnitType.SelectedValue.Key,
            ToStation = ToStation.SelectedItem,
            FreightGroup = FreightGroup.SelectedItem
        };

        public List<CustomDateTime> Dates
        {
            get
            {
                if (_dates == null)
                {
                    _dates = new List<CustomDateTime>();
                    var startDate = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-4);
                    var endDate = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(2);
                    do
                    {
                        _dates.Add(new CustomDateTime(startDate));
                        startDate = startDate.AddMonths(1);
                    } while (startDate != endDate);
                }
                return _dates;
            }
        }

        private List<CustomDateTime> _dates;

        public CustomDateTime SelectedDate { get; set; }

        public DateTime DateMin { get; } = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-4);

        public DateTime DateMax { get; } = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(2).AddDays(-1);

        public DateTime DateFrom
        {
            get => _dateFrom;
            set { Set(() => DateFrom, ref _dateFrom, value); }
        }


        private DateTime _dateFrom = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day);

        public DateTime DateTo
        {
            get => _dateTo;
            set { Set(() => DateTo, ref _dateTo, value); }
        }

        private DateTime _dateTo = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1);

        public SearchNsiViewModel<InfoOrgPassport> Sender { get; private set; }

        public SearchNsiViewModel<InfoOrgPassport> Payer { get; private set; }

        public SearchNsiViewModel<InfoOrgPassport> Owner { get; private set; }

        public SearchNsiViewModel<InfoUniqueStation> FromStation { get; private set; }

        public SearchNsiViewModel<InfoOrgPassport> Recip { get; private set; }

        public SearchNsiViewModel<InfoUniqueStation> ToStation { get; private set; }

        public SearchNsiViewModel<InfoSumFreight> FreightGroup { get; private set; }

        public ObservableEnum<CargoPlanConditionScheme> ConditionScheme { get; set; }

        public ObservableEnum<CargoPlanTypeScheme> PlanType { get; set; }

        public SelectableEnumViewModel<ReportDirections> ReportDirection { get; set; }

        public SelectableEnumViewModel<PeriodTypes> PeriodType { get; set; }

        public SelectableEnumViewModel<SourceType> SourceType { get; set; }

        public SelectableEnumViewModel<UnitTypes> UnitType { get; set; }

        private bool _isDeparture;

        public bool IsDeparture
        {
            get => _isDeparture;

            private set { Set(() => IsDeparture, ref _isDeparture, value); }
        }

        private bool _isArrival;

        public bool IsArrival
        {
            get => _isArrival;

            private set { Set(() => IsArrival, ref _isArrival, value); }
        }

        private bool _isMonth;

        public bool IsMonth
        {
            get => _isMonth;

            private set { Set(() => IsMonth, ref _isMonth, value); }
        }

        private bool _isPeriod;

        public bool IsPeriod
        {
            get => _isPeriod;

            private set { Set(() => IsPeriod, ref _isPeriod, value); }
        }

        #endregion Parameters

        public string ValidationText
        {
            get
            {
                if (IsDeparture)
                    if (!(Sender?.SelectedItem?.Id > 0 || FromStation?.SelectedItem?.StCode > 0 ||
                          Payer?.SelectedItem?.Id > 0 || Owner?.SelectedItem?.Id > 0))
                        return
                            "Укажите грузоотправителя и/или станцию отправления, и/или плательщика, и/или владельца вагонов";

                if (IsArrival)
                    if (!(Recip?.SelectedItem?.Id > 0 || ToStation?.SelectedItem?.StCode > 0 ||
                          Payer?.SelectedItem?.Id > 0 || Owner?.SelectedItem?.Id > 0))
                        return
                            "Укажите грузоотправителя и/или станцию отправления, и/или плательщика, и/или владельца вагонов";
                return string.Empty;
            }
        }

        public bool IsNotValidated => !(IsDeparture &&
                                        (Sender?.SelectedItem?.Id > 0 || FromStation?.SelectedItem?.StCode > 0) ||
                                        IsArrival &&
                                        (Recip?.SelectedItem?.Id > 0 || ToStation?.SelectedItem?.StCode > 0) ||
                                        Payer?.SelectedItem?.Id > 0 ||
                                        Owner?.SelectedItem?.Id > 0);

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    _name =
                        $"Выполнение плана погрузки {(Parameters.PeriodType == PeriodTypes.Month ? $"{Parameters.Date:MMMM yyyy}" : $"{Parameters.DateFrom:dd MMMM yyyy} - {Parameters.DateTo:dd MMMM yyyy}")}";
                return _name;
            }
            set { Set(() => Name, ref _name, value); }
        }

        private string _name;

        public string AcceptText => "Построить отчет";

        #endregion Properties

        #region Commands

        public Command CommandAccept
        {
            get
            {
                return _commandAccept ?? (_commandAccept = new Command(async () => await CommandAcceptInmplementation(),
                           () => !IsNotValidated));
            }
        }

        private Command _commandAccept;

        internal async Task CommandAcceptInmplementation()
        {
            if (ReportDirection.SelectedValue.Key == ReportDirections.Arrival)
            {
                Sender.SelectedItem = null;
                FromStation.SelectedItem = null;
            }
            else
            {
                Recip.SelectedItem = null;
                ToStation.SelectedItem = null;
            }

            await RetrieveData();
        }

        private async Task RetrieveData()
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                SetTabsVm();
                await Application.RootPage.Navigation.PushAsync(new ShipmentTabsView(_tabsVm));
            }
            else
            {
                SetTabsVm();
                if (!_pageCreated)
                {
                    var paramView = new PlanFactParametersView(this).Content as ScrollView;
                    paramView.BindingContext = this;
                    var masterDetailPage = new ShipmentTabsView(_tabsVm);
                    var tabs = masterDetailPage.Content as Grid;
                    masterDetailPage.Content = null;
                    tabs.BindingContext = _tabsVm;
                    var grid = new Grid
                    {
                        Padding = new Thickness(0, 0, 0, 0),
                        ColumnSpacing= 0,
                        BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition{ Width = new GridLength(320.0)},
                            new ColumnDefinition{ Width = GridLength.Star },
                        }
                    };
                    grid.Children.Add(paramView);
                    grid.Children.Add(tabs, 1, 0);
                    masterDetailPage.Content = grid;
                    await Application.Navigation.PushAsync(masterDetailPage);
                    var page = Application.Navigation.NavigationStack[Application.Navigation.NavigationStack.Count - 2];
                    if (page is PlanFactParametersView)
                        Application.Navigation.RemovePage(page);
                    _pageCreated = true;
                }
            }
            await _tabsVm.RetrieveData();
        }

        private async Task Save()
        {
            var parametersString = JsonConvert.SerializeObject(Parameters);
            var filePreview = DependencyService.Get<IFilePreview>();
            filePreview.SaveStringToFile($"{Name}.{PlanFactParameters.Extension}", parametersString);
            await RetrieveData();
            PlanFactTabsViewModel.Instance.Refresh();
        }

        #endregion
    }
}