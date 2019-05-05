using System.Collections.Generic;
using System.Linq;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;

namespace Mobile.Solution.UI.ViewModels
{
    public class ShipmentElementViewModel : SelectableViewModel
    {
        public ShipmentElementViewModel(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems,
            List<RegCard> regCards)
        {
            PlanItems = planItems.ToList();
            FactItems = factItems.ToList();
            RegCards = regCards.ToList();
        }

        public double MaximumValue { get; set; } = 10;

        public double LabelRotationAngle { get; set; }

        public string AxisTitle { get; set; }

        public string Title { get; set; }

        public List<CargoPlanFact> PlanItems { get; }

        public List<CargoPlanFact> FactItems { get; }

        public List<RegCard> RegCards { get; }

        public List<ShipmentChartDataPoint> Requirements { get; } = new List<ShipmentChartDataPoint>();

        public List<DeviationChartDataPoint> Shortages { get; } = new List<DeviationChartDataPoint>();

		public List<DeviationChartDataPoint> OverFulfillments { get; } = new List<DeviationChartDataPoint>(); 

    }
}