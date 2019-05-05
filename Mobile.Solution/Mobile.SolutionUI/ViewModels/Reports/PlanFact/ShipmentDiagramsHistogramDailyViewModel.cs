using System.Collections.Generic;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Requests.Reports;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;
using Mobile.Solution.UI.Helpers;

namespace Mobile.Solution.UI.ViewModels
{
    public class ShipmentDiagramsHistogramDailyViewModel : SelectableViewModel
    {
        public ShipmentDiagramsHistogramDailyViewModel(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems,
            List<RegCard> regCards, Creator creator, PlanFactParameters parameters, PeriodSection section)
        {
            Creator = creator;
            Header = Creator?.Header;
            Section = section;
            Parameters = parameters;
            PlanItems = planItems;
            FactItems = factItems;
            RegCards = regCards;
            InitializeTabs();
        }

        public PlanFactParameters Parameters { get; }

        public List<CargoPlanFact> PlanItems { get; private set; }

        public List<CargoPlanFact> FactItems { get; private set; }

        public List<RegCard> RegCards { get; private set; }

        public Creator Creator { get; }

        public PeriodSection Section { get; internal set; }

        private void InitializeTabs()
        {
            try
            {
                var tabs = new List<SelectableViewModel>
                {
					new ShipmentCollectionViewModel(PlanItems, FactItems, RegCards, DataSetType.PlanFacts, Creator,
						Parameters, Section, "План/факт", DiagramTypes.DailyHistogramChart),
                    new ShipmentCollectionViewModel(PlanItems, FactItems, RegCards, DataSetType.Plans, Creator,
                        Parameters, Section, "План", DiagramTypes.DailyHistogramChart),
                    new ShipmentCollectionViewModel(PlanItems, FactItems, RegCards, DataSetType.Facts, Creator,
                        Parameters, Section, "Факт", DiagramTypes.DailyHistogramChart)
                };
                Tabs = tabs;
            }
            finally
            {
                RaisePropertyChanged(() => Tabs);
            }
        }
    }
}