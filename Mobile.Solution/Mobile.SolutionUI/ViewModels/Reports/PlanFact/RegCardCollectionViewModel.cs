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
    public class RegCardCollectionViewModel : SelectableViewModel
    {
        public RegCardCollectionViewModel(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems,
            List<RegCard> regCards, Creator creator, PlanFactParameters parameters, PeriodSection? section)
        {
            Creator = creator;
            Header = Creator?.Header;
            Section = section;
            Parameters = parameters;
            PlanItems = planItems;
            FactItems = factItems;
            RegCards = regCards;
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                IsBusy = true;

                if (!IsMonth)
                {
                    SelectedShipment = new RegCardViewModel(Creator, Parameters, PeriodSection.Arbitrary,
                        CommandRefresh);
                    Tabs = new List<SelectableViewModel>
                    {
                        SelectedShipment
                    };
                }
                else
                {
                    Tabs = new List<SelectableViewModel>
                    {
                        new RegCardViewModel(Creator, Parameters, PeriodSection.Previous, CommandRefresh),
                        new RegCardViewModel(Creator, Parameters, PeriodSection.CurrentDay, CommandRefresh),
                        new RegCardViewModel(Creator, Parameters, PeriodSection.NextDay, CommandRefresh),
                        new RegCardViewModel(Creator, Parameters, PeriodSection.CurrentPeriod, CommandRefresh),
                        new RegCardViewModel(Creator, Parameters, PeriodSection.Month, CommandRefresh),
						new RegCardViewModel(Creator, Parameters, PeriodSection.ArbitraryDate, CommandRefresh),
						new RegCardViewModel(Creator, Parameters, PeriodSection.Arbitrary, CommandRefresh)
                    };
                }

                foreach (RegCardViewModel tab in Tabs)
                    tab.SetData(PlanItems, FactItems, RegCards, Parameters);

                PeriodSections = new ObservableEnum<PeriodSection>(
                    Section ?? PeriodSection.CurrentPeriod,
                    value =>
                    {
                        if (!IsMonth) return;
                        SelectedShipment = Tabs.FirstOrDefault(s => (s as RegCardViewModel).Section == value) as RegCardViewModel;
                    }, ResourceContainer.ResourceManager);
                PeriodSections.DeleteItem(PeriodSection.NextDay);
                PeriodSections.DeleteItem(PeriodSection.FirstTriad);
                PeriodSections.DeleteItem(PeriodSection.SecondTriad);
                PeriodSections.DeleteItem(PeriodSection.ThirdTriad);
				PeriodSections.DeleteItem(PeriodSection.ArbitraryDate);
				PeriodSections.DeleteItem(PeriodSection.Arbitrary);
                if (Parameters.Date.Month != DateTime.Now.Month)
                {
                    PeriodSections.DeleteItem(PeriodSection.Previous);
                    PeriodSections.DeleteItem(PeriodSection.CurrentDay);
                    PeriodSections.DeleteItem(PeriodSection.NextDay);
                    PeriodSections.DeleteItem(PeriodSection.CurrentPeriod);
                }
                else
                    PeriodSections.DeleteItem(PeriodSection.Month);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #region Properties

        public PlanFactParameters Parameters { get; }

        public List<CargoPlanFact> PlanItems { get; private set; }

        public List<CargoPlanFact> FactItems { get; private set; }

        public List<RegCard> RegCards { get; private set; }

        public PeriodSection? Section { get; internal set; }

        public ObservableEnum<PeriodSection> PeriodSections { get; private set; }

        public Creator Creator { get; }

        public RegCardViewModel SelectedShipment
        {
            get => _selectedShipment;

            private set { Set(() => SelectedShipment, ref _selectedShipment, value); }
        }

        RegCardViewModel _selectedShipment;

        public bool IsMonth => Parameters.PeriodType == PeriodTypes.Month;

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

        private async Task RetrieveData()
        {
            try
            {
                IsBusy = true;
                PlanItems = await PlanFactReport.Instance.InitPlanRequest(Parameters);
                FactItems = await PlanFactReport.Instance.InitFactRequest(Parameters);
                RegCards = await RegCardReport.Instance.InitRequest(Parameters);

                Tabs.Remove(SelectedShipment);
                SelectedShipment =
                    new RegCardViewModel(Creator, Parameters, SelectedShipment.Section, CommandRefresh);
                Tabs.Add(SelectedShipment);
                foreach (RegCardViewModel tab in Tabs)
                    tab.SetData(PlanItems, FactItems, RegCards, Parameters);
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

        #endregion
    }
}