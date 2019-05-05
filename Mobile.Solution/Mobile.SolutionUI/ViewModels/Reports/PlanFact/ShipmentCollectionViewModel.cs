using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Helpers;
using Mobile.Solution.Infrastructure.Requests.Reports;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;
using Mobile.Solution.UI.Helpers;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public enum DataSetType
    {
        Plans,
        Facts,
        PlanFacts
    }

    public class ShipmentCollectionViewModel : SelectableViewModel
    {
        private DiagramTypes _chartType;
		private DateTime _selectedDate;

		public ShipmentCollectionViewModel(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems,
            List<RegCard> regCards, DataSetType type, Creator creator, PlanFactParameters parameters,
            PeriodSection section, string header, DiagramTypes chartType, DateTime? selectedDate = null)
        {
			_selectedDate = selectedDate.HasValue ? selectedDate.Value : _selectedDate;
			_chartType = chartType;
            SetType = type;
            Header = header;
            Creator = creator;
            Section = section;
            Parameters = parameters;
            PlanItems = planItems;
            FactItems = factItems;
            RegCards = regCards;
            InitializeDetails();
        }

        public DataSetType SetType { get; }

        private void InitializeDetails()
        {
            try
            {
                IsBusy = true;

                if (!IsMonth)
                {
                    SelectedShipment =
                        new ShipmentViewModel(Creator, Parameters, PeriodSection.Arbitrary, SetType, _chartType);
                    Tabs = new List<SelectableViewModel>
                    {
                        SelectedShipment
                    };
                }
                else
                {
                    Tabs = new List<SelectableViewModel>
                    {
                        new ShipmentViewModel(Creator, Parameters, PeriodSection.Previous, SetType, _chartType),
                        new ShipmentViewModel(Creator, Parameters, PeriodSection.CurrentDay, SetType, _chartType),
                        new ShipmentViewModel(Creator, Parameters, PeriodSection.NextDay, SetType, _chartType),
                        new ShipmentViewModel(Creator, Parameters, PeriodSection.CurrentPeriod, SetType, _chartType),
                        new ShipmentViewModel(Creator, Parameters, PeriodSection.FirstTriad, SetType, _chartType),
                        new ShipmentViewModel(Creator, Parameters, PeriodSection.SecondTriad, SetType, _chartType),
                        new ShipmentViewModel(Creator, Parameters, PeriodSection.ThirdTriad, SetType, _chartType),
                        new ShipmentViewModel(Creator, Parameters, PeriodSection.Month, SetType, _chartType),
						new ShipmentViewModel(Creator, Parameters, PeriodSection.ArbitraryDate, SetType, _chartType, _selectedDate),
						new ShipmentViewModel(Creator, Parameters, PeriodSection.Arbitrary, SetType, _chartType)
                    };
                }

                foreach (ShipmentViewModel tab in Tabs)
                    tab.SetData(PlanItems, FactItems, RegCards, Parameters);
                PeriodSections = new ObservableEnum<PeriodSection>(
                    Section,
                    async value =>
                    {
                        if (value == PeriodSection.ArbitraryDate)
                        {
                            if (Section != PeriodSection.ArbitraryDate)
                            {
                                SelectedDate = Parameters.PeriodType == PeriodTypes.AnyPeriod ? Parameters.DateFrom : Parameters.Date;
                                var minDate = Parameters.PeriodType == PeriodTypes.AnyPeriod ? Parameters.DateFrom : Parameters.Date;
                                var maxDate = Parameters.PeriodType == PeriodTypes.AnyPeriod ? Parameters.DateTo : Parameters.Date.AddMonths(1).AddDays(-1);
                                SelectedDate = await Dialog.Instance.PromptDateAsync(_selectedDate, minDate, maxDate);
                            }
							SelectedShipment = Tabs.FirstOrDefault(s => (s as ShipmentViewModel).Section == value);
                            (SelectedShipment as ShipmentViewModel).InitDateDiagram(_selectedDate);
						}
                        else
                        {
                            DateLabelIsVisible = false;
                            if (!IsMonth) return;
                            SelectedShipment = Tabs.FirstOrDefault(s => (s as ShipmentViewModel).Section == value);
                        }
                        Section = value;
                    }, ResourceContainer.ResourceManager);
                PeriodSections.DeleteItem(PeriodSection.Arbitrary);
				if (Parameters.Date.Month != DateTime.Now.Month)
                {
                    PeriodSections.DeleteItem(PeriodSection.Previous);
                    PeriodSections.DeleteItem(PeriodSection.CurrentDay);
                    PeriodSections.DeleteItem(PeriodSection.NextDay);
                    PeriodSections.DeleteItem(PeriodSection.CurrentPeriod);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task RetrieveData()
        {
            try
            {
                IsBusy = true;
                PlanItems = await PlanFactReport.Instance.InitPlanRequest(Parameters);
                FactItems = await PlanFactReport.Instance.InitFactRequest(Parameters);
                RegCards = await RegCardReport.Instance.InitRequest(Parameters);

                Tabs.Remove(SelectedShipment);
                SelectedShipment =
                    new ShipmentViewModel(Creator, Parameters, (SelectedShipment as ShipmentViewModel).Section, SetType, _chartType);
                Tabs.Add(SelectedShipment);
                foreach (ShipmentViewModel tab in Tabs)
                    tab.SetData(PlanItems, FactItems, RegCards, Parameters);
            }
            catch (Exception ex)
            {
                Dialog.Instance.Alert(ex.Message, ResourceContainer.ResourceManager.GetString("DialogErrorTitle"));
            }
            finally
            {
                IsBusy = false;
                CommandRefresh.ChangeCanExecute();
            }
        }

        #region Properties

        /// <summary>
        ///     Агрегированные параметры для обновления дочерних око
        /// </summary>
        public PlanFactParameters Parameters { get; }

        public List<CargoPlanFact> PlanItems { get; private set; }

        public List<CargoPlanFact> FactItems { get; private set; }

        public List<RegCard> RegCards { get; private set; }

        public PeriodSection Section { get; internal set; }

        public ObservableEnum<PeriodSection> PeriodSections { get; private set; }

        public Creator Creator { get; }

        public SelectableViewModel SelectedShipment
        {
            get => _selectedShipment;

            private set { Set(() => SelectedShipment, ref _selectedShipment, value); }
        }

        SelectableViewModel _selectedShipment;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                DateLabelText = _selectedDate.ToString("dd-MM-yyyy");
                DateLabelIsVisible = true;
            }
        }

        public bool IsMonth => Parameters.PeriodType == PeriodTypes.Month;

        private bool _dateLabelIsVisible;
		public bool DateLabelIsVisible 
        {
			get => _dateLabelIsVisible;
			private set { Set(() => DateLabelIsVisible, ref _dateLabelIsVisible, value); }
        }

		private string _dateLabelText;
		public string DateLabelText 
        {
            get => _dateLabelText;
			private set { Set(() => DateLabelText, ref _dateLabelText, value); }
        }

        #endregion Properties

        #region Commands

        public Command CommandRefresh
        {
            get
            {
                return _commandRefresh ?? (_commandRefresh = new Command(async () => await RetrieveData(),
                           () => !IsBusy));
            }
        }

        private Command _commandRefresh;

        #endregion
    }
}