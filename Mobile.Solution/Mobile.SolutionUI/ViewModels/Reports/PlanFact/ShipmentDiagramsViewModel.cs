using System;
using System.Collections.Generic;
using System.Linq;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Helpers;
using Mobile.Solution.Infrastructure.Requests.Reports;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;
using Mobile.Solution.UI.Helpers;

namespace Mobile.Solution.UI.ViewModels
{
    public enum DiagramTypes
    {
		HistogramChart,
		DailyHistogramChart,
        PieChart,
    }

    public class ShipmentDiagramsViewModel : SelectableViewModel
    {
        ShipmentTabsViewModel _parent;
        private DateTime _selectedDate;

		public ShipmentDiagramsViewModel(ShipmentTabsViewModel parent, List<CargoPlanFact> planItems, List<CargoPlanFact> factItems,
            List<RegCard> regCards, Creator creator, PlanFactParameters parameters, PeriodSection section, DateTime? selectedDate)
        {
			_selectedDate = selectedDate.HasValue ? selectedDate.Value : _selectedDate;
			_parent = parent;
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

        public SelectableViewModel SelectedDiagram { get; private set; }

        public ObservableEnum<DiagramTypes> DiagramType { get; private set; }

        private void InitializeTabs()
        {
            try
            {
                var tabs = new List<SelectableViewModel>();
                tabs.Add(new ShipmentDiagramsHistogramViewModel(PlanItems, FactItems, RegCards, Creator, Parameters,
                    Section, _selectedDate));
				tabs.Add(new ShipmentDiagramsHistogramDailyViewModel(PlanItems, FactItems, RegCards, Creator, Parameters,
			        Section));
                tabs.Add(new ShipmentDiagramsLinearViewModel(PlanItems, FactItems, RegCards, Creator, Parameters,
                    Section));
                tabs.Add(new ShipmentDiagramsPieViewModel(PlanItems, FactItems, RegCards, Creator, Parameters,
                    Section));
                Tabs = tabs;

                DiagramType = new ObservableEnum<DiagramTypes>(DiagramTypes.HistogramChart,
                    value =>
                    {
                        switch (value)
                        {
                            case DiagramTypes.HistogramChart:
                                SelectedDiagram =
                                    Tabs.FirstOrDefault(t => t.GetType() == typeof(ShipmentDiagramsHistogramViewModel));
                                break;
							case DiagramTypes.DailyHistogramChart:
								SelectedDiagram =
									Tabs.FirstOrDefault(t => t.GetType() == typeof(ShipmentDiagramsHistogramDailyViewModel));
								break;
                            //case DiagramTypes.LinearChart:
                                //SelectedDiagram =
                                //    Tabs.FirstOrDefault(t => t.GetType() == typeof(ShipmentDiagramsLinearViewModel));
                                //break;
                            case DiagramTypes.PieChart:
                                SelectedDiagram =
                                    Tabs.FirstOrDefault(t => t.GetType() == typeof(ShipmentDiagramsPieViewModel));
                                break;
                            default:
                                SelectedDiagram = null;
                                break;
                        }
                        RaisePropertyChanged(() => SelectedDiagram);
                        foreach(var tab in _parent.Tabs)
                        {
                            if(tab is ShipmentDiagramsViewModel && tab != this)
                                (tab as ShipmentDiagramsViewModel).DiagramType.SetValue(value);
                        }
                    }, ResourceContainer.ResourceManager);
            }
            finally
            {
                RaisePropertyChanged(() => Tabs);
            }
        }
    }
}