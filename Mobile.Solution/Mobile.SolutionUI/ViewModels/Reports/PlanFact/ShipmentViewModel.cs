using System;
using System.Collections.Generic;
using System.Linq;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Requests.Reports;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;
using Mobile.Solution.UI.Helpers;
using Syncfusion.SfChart.XForms;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public class ShipmentViewModel : SelectableViewModel
    {
        private static string[] _indexcolors =
        {
            "#FFFF00", "#1CE6FF", "#FF34FF", "#FF4A46", "#008941", "#006FA6", "#A30059",
            "#FFDBE5", "#7A4900", "#0000A6", "#63FFAC", "#B79762", "#004D43", "#8FB0FF", "#997D87",
            "#5A0007", "#809693", "#FEFFE6", "#1B4400", "#4FC601", "#3B5DFF", "#4A3B53", "#FF2F80",
            "#61615A", "#BA0900", "#6B7900", "#00C2A0", "#FFAA92", "#FF90C9", "#B903AA", "#D16100",
            "#DDEFFF", "#000035", "#7B4F4B", "#A1C299", "#300018", "#0AA6D8", "#013349", "#00846F",
            "#372101", "#FFB500", "#C2FFED", "#A079BF", "#CC0744", "#C0B9B2", "#C2FF99", "#001E09",
            "#00489C", "#6F0062", "#0CBD66", "#EEC3FF", "#456D75", "#B77B68", "#7A87A1", "#788D66",
            "#885578", "#FAD09F", "#FF8A9A", "#D157A0", "#BEC459", "#456648", "#0086ED", "#886F4C",

            "#34362D", "#B4A8BD", "#00A6AA", "#452C2C", "#636375", "#A3C8C9", "#FF913F", "#938A81",
            "#575329", "#00FECF", "#B05B6F", "#8CD0FF", "#3B9700", "#04F757", "#C8A1A1", "#1E6E00",
            "#7900D7", "#A77500", "#6367A9", "#A05837", "#6B002C", "#772600", "#D790FF", "#9B9700",
            "#549E79", "#FFF69F", "#201625", "#72418F", "#BC23FF", "#99ADC0", "#3A2465", "#922329",
            "#5B4534", "#FDE8DC", "#404E55", "#0089A3", "#CB7E98", "#A4E804", "#324E72", "#6A3A4C",
            "#83AB58", "#001C1E", "#D1F7CE", "#004B28", "#C8D0F6", "#A3A489", "#806C66", "#222800",
            "#BF5650", "#E83000", "#66796D", "#DA007C", "#FF1A59", "#8ADBB4", "#1E0200", "#5B4E51",
            "#C895C5", "#320033", "#FF6832", "#66E1D3", "#CFCDAC", "#D0AC94", "#7ED379", "#012C58",

            "#7A7BFF", "#D68E01", "#353339", "#78AFA1", "#FEB2C6", "#75797C", "#837393", "#943A4D",
            "#B5F4FF", "#D2DCD5", "#9556BD", "#6A714A", "#001325", "#02525F", "#0AA3F7", "#E98176",
            "#DBD5DD", "#5EBCD1", "#3D4F44", "#7E6405", "#02684E", "#962B75", "#8D8546", "#9695C5",
            "#E773CE", "#D86A78", "#3E89BE", "#CA834E", "#518A87", "#5B113C", "#55813B", "#E704C4",
            "#00005F", "#A97399", "#4B8160", "#59738A", "#FF5DA7", "#F7C9BF", "#643127", "#513A01",
            "#6B94AA", "#51A058", "#A45B02", "#1D1702", "#E20027", "#E7AB63", "#4C6001", "#9C6966",
            "#64547B", "#97979E", "#006A66", "#391406", "#F4D749", "#0045D2", "#006C31", "#DDB6D0",
            "#7C6571", "#9FB2A4", "#00D891", "#15A08A", "#BC65E9", "#FFFFFE", "#C6DC99", "#203B3C",

            "#671190", "#6B3A64", "#F5E1FF", "#FFA0F2", "#CCAA35", "#374527", "#8BB400", "#797868",
            "#C6005A", "#3B000A", "#C86240", "#29607C", "#402334", "#7D5A44", "#CCB87C", "#B88183",
            "#AA5199", "#B5D6C3", "#A38469", "#9F94F0", "#A74571", "#B894A6", "#71BB8C", "#00B433",
            "#789EC9", "#6D80BA", "#953F00", "#5EFF03", "#E4FFFC", "#1BE177", "#BCB1E5", "#76912F",
            "#003109", "#0060CD", "#D20096", "#895563", "#29201D", "#5B3213", "#A76F42", "#89412E",
            "#1A3A2A", "#494B5A", "#A88C85", "#F4ABAA", "#A3F3AB", "#00C6C8", "#EA8B66", "#958A9F",
            "#BDC9D2", "#9FA064", "#BE4700", "#658188", "#83A485", "#453C23", "#47675D", "#3A3F00",
            "#061203", "#DFFB71", "#868E7E", "#98D058", "#6C8F7D", "#D7BFC2", "#3C3E6E", "#D83D66",

            "#2F5D9B", "#6C5E46", "#D25B88", "#5B656C", "#00B57F", "#545C46", "#866097", "#365D25",
            "#252F99", "#00CCFF", "#674E60", "#FC009C", "#92896B", "#000000"
        };

        public static List<Color> _brushes { get; }

        static ShipmentViewModel()
        {
            _brushes = new List<Color>();
            foreach (var color in _indexcolors)
            {
                _brushes.Add(Color.FromHex(color));
            }
        }

        private DataSetType _setType;

        private DiagramTypes _chartType;

        public ShipmentViewModel(Creator creator, PlanFactParameters parameters, PeriodSection section,
            DataSetType type, DiagramTypes chartType, DateTime? selectedDate = null)
        {
            SelectedDate = selectedDate.HasValue ? selectedDate.Value : SelectedDate;
            _chartType = chartType;
            _setType = type;
            Creator = creator;
            Section = section;
            Parameters = parameters;
        }

        #region Properties

        public List<Color> Brushes { get { return _brushes; } }

		public DateTime SelectedDate { get; private set; }

		public bool PlansVisible => _setType != DataSetType.Facts;

        public bool FactsVisible => _setType != DataSetType.Plans;

        public int GroupSize
        {
            get
            {
                switch (Device.Idiom)
                {
                    case TargetIdiom.Desktop:
                        return int.MaxValue;
                    case TargetIdiom.Phone:
                                        return 6;
                    case TargetIdiom.Tablet:
                                        return 12;
                    default: return 6;
                }
            }
        }
        public double Koeff => 4.0;
        public double MaxKoeff => 1.8;

        public List<CargoPlanFact> PlanItems { get; protected set; }

        public List<CargoPlanFact> FactItems { get; protected set; }

        public List<RegCard> RegCards { get; protected set; }

        public Creator Creator { get; }

        public PlanFactParameters Parameters { get; set; }

        public PeriodSection Section { get; private set; }

        public string Title { get; set; }

        public string AxisTitle => Creator.Header;

        public double LabelRotationAngle => Creator is ContractorCreator || Creator is PayerCreator ||
                                            Creator is OwnerCreator || Creator is FreightCreator
            ? 270
            : 0;

        public bool IsLabelsVisible => Items?.Count > 1;

        public List<ShipmentElementViewModel> Items { get; private set; } =
            new List<ShipmentElementViewModel>();

        public List<ShipmentPieChartDataPoint> PieChartItems { get; private set; } =
            new List<ShipmentPieChartDataPoint>();

        public List<ChartDataPoint> Facts { get; } =
            new List<ChartDataPoint>();

        public List<ChartDataPoint> Plans { get; } =
            new List<ChartDataPoint>();

        public List<ShipmentChartDataPoint> Requirements { get; } =
            new List<ShipmentChartDataPoint>();

        public List<DeviationChartDataPoint> Shortages { get; } =
            new List<DeviationChartDataPoint>();

        public List<DeviationChartDataPoint> OverFulfillments { get; } =
            new List<DeviationChartDataPoint>();

        #endregion

        public void SetData(List<CargoPlanFact> plans, List<CargoPlanFact> facts, List<RegCard> regCards,
            PlanFactParameters parameters)
        {
            FilterData(plans, facts, regCards);
            Parameters = parameters;

            switch (Section)
            {
                case PeriodSection.Month:
                    InitMonthDiagram();
                    break;
                case PeriodSection.Previous:
                    InitPreviousDiagram();
                    break;
                case PeriodSection.CurrentDay:
                    InitCurrentDayDiagram();
                    break;
                case PeriodSection.NextDay:
                    InitNextDayDiagram();
                    break;
                case PeriodSection.CurrentPeriod:
                    InitCurrentPeriodDiagram();
                    break;
                case PeriodSection.FirstTriad:
                case PeriodSection.SecondTriad:
                case PeriodSection.ThirdTriad:
                    InitTriadDiagram();
                    break;
				case PeriodSection.ArbitraryDate:
					break;
				case PeriodSection.Arbitrary:
                    InitDiagram(PlanItems, FactItems, RegCards);
                    break;
            }
        }

        #region Functions

        private void FilterData(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems,
            List<RegCard> regCards)
        {
            PlanItems = planItems.Where(v => Creator.Filter(v)).ToList();
            FactItems = factItems.Where(v => Creator.Filter(v)).ToList();
            RegCards = regCards.Where(Creator.RcFilter).ToList();
        }

        private ShipmentElementViewModel CreateItem(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems,
            List<RegCard> regCards)
        {
            var item = new ShipmentElementViewModel(planItems, factItems, regCards)
            {
                AxisTitle = AxisTitle,
                LabelRotationAngle = LabelRotationAngle,
                Title = Title
            };
            Items.Add(item);
            return item;
        }

        private void InitDiagram(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems, List<RegCard> regCards)
        {
			Items.Clear();
			PieChartItems.Clear();
			Facts.Clear();
			Plans.Clear();
			Requirements.Clear();
			Shortages.Clear();
			OverFulfillments.Clear();
            FillDataCollections(planItems, factItems);
            switch (_chartType)
            {
                case DiagramTypes.DailyHistogramChart:
                    InitHistogramDailyDiagram(planItems, factItems, regCards);
                    break;
                case DiagramTypes.HistogramChart:
                    InitHistogramDiagram(planItems, factItems, regCards);
                    break;
                //case DiagramTypes.LinearChart:
                    //InitLinearDiagram(planItems, factItems);
                    //break;
                case DiagramTypes.PieChart:
                    break;
            }
        }

        private void FillDataCollections(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems)
        {
            IEnumerable<IGrouping<string, CargoPlanFact>> planGroup;
            IEnumerable<IGrouping<string, CargoPlanFact>> factGroup;
            var keys =
                new Dictionary<string, Tuple<List<CargoPlanFact>, List<CargoPlanFact>>>();

            switch (_chartType)
            {
                case DiagramTypes.DailyHistogramChart:
                    planGroup = planItems.Where(v => Creator.Filter(v)).GroupBy(p => p.Date.ToString("dd.MM.yyyy"));
                    factGroup = factItems.Where(v => Creator.Filter(v)).GroupBy(p => p.Date.ToString("dd.MM.yyyy"));
                    break;
                default:
                    planGroup = planItems.Where(v => Creator.Filter(v)).GroupBy(p => Creator.Group(p));
                    factGroup = factItems.Where(v => Creator.Filter(v)).GroupBy(p => Creator.Group(p));
                    break;
            }

            foreach (var plans in planGroup)
            {
                keys.Add(plans.Key,
                    new Tuple<List<CargoPlanFact>, List<CargoPlanFact>>(plans.ToList(),
                        factGroup.FirstOrDefault(f => f.Key == plans.Key)?.ToList() ?? new List<CargoPlanFact>()));
            }
            foreach (var facts in factGroup)
            {
                if (!keys.ContainsKey(facts.Key))
                {
                    keys.Add(facts.Key,
                        new Tuple<List<CargoPlanFact>, List<CargoPlanFact>>(new List<CargoPlanFact>(),
                            facts.ToList() ?? new List<CargoPlanFact>()));
                }
            }
            var plansSum = planItems.Sum(p => Parameters.UnitType == UnitTypes.Cars ? p.CarsCount : (double)p.Weight);
            var factsSum = factItems.Sum(p => Parameters.UnitType == UnitTypes.Cars ? p.CarsCount : (double)p.Weight);
            if (Parameters.UnitType == UnitTypes.Weights)
            {
                plansSum = Math.Round(plansSum / 1000.0, 2);
                factsSum = Math.Round(factsSum / 1000000.0, 2);
                if (Math.Abs(plansSum - factsSum) < 0.1)
                    factsSum = plansSum;
            }
            var pieChartItems = new List<Tuple<ShipmentPieChartDataPoint, ShipmentPieChartDataPoint>>();
            foreach (
                var key in keys)
            {
                var planValue = key.Value.Item1.Sum(p => Parameters.UnitType == UnitTypes.Cars
                    ? p.CarsCount
                    : (double)p.Weight);
                var factValue = key.Value.Item2.Sum(p => Parameters.UnitType == UnitTypes.Cars
                    ? p.CarsCount
                    : (double)p.Weight);
                if (Parameters.UnitType == UnitTypes.Weights)
                {
                    planValue = Math.Round(planValue / 1000.0, 2);
                    factValue = Math.Round(factValue / 1000000.0, 2);
                    if (Math.Abs(planValue - factValue) < 0.1)
                        factValue = planValue;
                }

                if (planValue == 0 && factValue == 0)
                    continue;

                var exampleElement = key.Value.Item1.FirstOrDefault() ?? key.Value.Item2.FirstOrDefault();

                var category = key.Key.Length > 23 ? key.Key.Substring(0, 20) + "..." : key.Key;

                pieChartItems.Add(new Tuple<ShipmentPieChartDataPoint, ShipmentPieChartDataPoint>(
                                    new ShipmentPieChartDataPoint(key.Key, planValue, plansSum),
                                    new ShipmentPieChartDataPoint(key.Key, factValue, factsSum)));
                switch (_setType)
                {
                    case DataSetType.Plans:
                        factValue = 0;
                        break;
                    case DataSetType.Facts:
                        planValue = 0;
                        break;
                }

                Requirements.Add(new ShipmentChartDataPoint(category, key.Key, factValue > planValue ? planValue : factValue, exampleElement));
                OverFulfillments.Add(
                    new DeviationChartDataPoint(category, key.Key, factValue > planValue ? Math.Round(factValue - planValue, 2) : 0,
                        factValue > planValue ? planValue : (double?)null, factValue < planValue));
                Shortages.Add(new DeviationChartDataPoint(category, key.Key, factValue > planValue ? 0 : Math.Round(planValue - factValue, 2),
                                                          factValue > planValue ? (double?)null : planValue,
                                                          factValue < planValue));
            }

            var colorCounter = 0;
            PieChartItems = new List<ShipmentPieChartDataPoint>();
            foreach (var item in pieChartItems.OrderByDescending(i => i.Item1.YValue).ToList())
            {
                item.Item1.Color = item.Item2.Color = Color.FromHex(_indexcolors[colorCounter % _indexcolors.Length]);
                PieChartItems.Add(PlansVisible ? item.Item1 : item.Item2);
                colorCounter++;
            }

            SetTitle();
        }

        private void InitHistogramDiagram(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems, List<RegCard> regCards)
        {
            Items = new List<ShipmentElementViewModel>();

            var counter = 0;
            var currentItem = CreateItem(planItems, factItems, regCards);

            var orderedFacts = Requirements.OrderByDescending(
                f => f.YValue + OverFulfillments.FirstOrDefault(of => of.Category == f.FullValue).YValue +
                     Shortages.FirstOrDefault(of => of.Category == f.FullValue).YValue);

            var ft = orderedFacts.FirstOrDefault();
            var ovf = OverFulfillments.FirstOrDefault(of => of.Category == ft.FullValue);
            var sh = Shortages.FirstOrDefault(s => s.Category == ft.FullValue);
            var maximumValue = (ft?.YValue ?? 0) + (ovf?.YValue ?? 0) + (sh?.YValue ?? 0);
            currentItem.MaximumValue = MaxKoeff * maximumValue;

            foreach (var fact in orderedFacts)
            {
                if (counter == GroupSize && orderedFacts.Count() - Items.Count * GroupSize > 1)
                {
                    currentItem = CreateItem(planItems, factItems, regCards);
                    currentItem.MaximumValue = MaxKoeff * maximumValue;
                    counter = 0;
                }

                var overFull = OverFulfillments.FirstOrDefault(of => of.Category == fact.FullValue);

                var shortage = Shortages.FirstOrDefault(of => of.Category == fact.FullValue);

                var koefVal = Math.Round(maximumValue / Koeff, 2);
                if (fact.YValue > 0)
                    fact.YValue = maximumValue / Koeff > fact.YValue ? koefVal : fact.YValue;
                else if (overFull.YValue > 0)
                    overFull.YValue = maximumValue / Koeff > overFull.YValue ? koefVal : overFull.YValue;
                else if (shortage.YValue > 0)
                    shortage.YValue = maximumValue / Koeff > shortage.YValue ? koefVal : shortage.YValue;

                currentItem.Requirements.Add(fact);

                currentItem.OverFulfillments.Add(overFull);

                currentItem.Shortages.Add(shortage);

                counter++;
            }
        }

        private void InitLinearDiagram(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems)
        {
            var startDate = Parameters.PeriodType == PeriodTypes.Month ? Parameters.Date : Parameters.DateFrom;
            var endDate = Parameters.PeriodType == PeriodTypes.Month
                ? Parameters.Date.AddMonths(1).AddDays(-1)
                : Parameters.DateTo;
            do
            {
                var planValue = planItems.Where(f => f.Date.Date == startDate)
                    .Sum(p => Parameters.UnitType == UnitTypes.Cars ? p.CarsCount : (double)p.Weight);
                var factValue = factItems.Where(f => f.Date.Date == startDate)
                    .Sum(p => Parameters.UnitType == UnitTypes.Cars ? p.CarsCount : (double)p.Weight);
                if (Parameters.UnitType == UnitTypes.Weights)
                {
                    planValue = Math.Round(planValue / 1000.0, 2);
                    factValue = Math.Round(factValue / 1000000.0, 2);
                    if (Math.Abs(planValue - factValue) < 0.1)
                        factValue = planValue;
                }
                if (factValue > 0)
                    Facts.Add(new ChartDataPoint(startDate.ToString("dd.MM.yyyy"), factValue));
                if (planValue > 0)
                    Plans.Add(new ChartDataPoint(startDate.ToString("dd.MM.yyyy"), planValue));

                startDate = startDate.AddDays(1);
            } while (startDate != endDate);
        }

        private void InitHistogramDailyDiagram(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems, List<RegCard> regCards)
        {
            Items = new List<ShipmentElementViewModel>();

            var counter = 0;
            var currentItem = CreateItem(planItems, factItems, regCards);

			var orderedFacts = Requirements.OrderByDescending(
                            	 f => f.YValue + OverFulfillments.FirstOrDefault(of => of.Category == f.FullValue).YValue +
                            		  Shortages.FirstOrDefault(of => of.Category == f.FullValue).YValue);

            var ft = orderedFacts.FirstOrDefault();
            var ovf = OverFulfillments.FirstOrDefault(of => of.Category == ft.FullValue);
            var sh = Shortages.FirstOrDefault(s => s.Category == ft.FullValue);
            var maximumValue = (ft?.YValue ?? 0) + (ovf?.YValue ?? 0) + (sh?.YValue ?? 0);
            currentItem.MaximumValue = MaxKoeff * maximumValue;

			foreach (var fact in orderedFacts.OrderBy(d => d.FullValue))
            {
                if (counter == GroupSize && orderedFacts.Count() - Items.Count * GroupSize > 1)
                {
                    currentItem = CreateItem(planItems, factItems, regCards);
                    currentItem.MaximumValue = MaxKoeff * maximumValue;
                    counter = 0;
                }

                var overFull = OverFulfillments.FirstOrDefault(of => of.Category == fact.FullValue);

                var shortage = Shortages.FirstOrDefault(of => of.Category == fact.FullValue);

                var koefVal = Math.Round(maximumValue / Koeff, 2);
                if (fact.YValue > 0)
                    fact.YValue = maximumValue / Koeff > fact.YValue ? koefVal : fact.YValue;
                else if (overFull.YValue > 0)
                    overFull.YValue = maximumValue / Koeff > overFull.YValue ? koefVal : overFull.YValue;
                else if (shortage.YValue > 0)
                    shortage.YValue = maximumValue / Koeff > shortage.YValue ? koefVal : shortage.YValue;

                currentItem.Requirements.Add(fact);

                currentItem.OverFulfillments.Add(overFull);

                currentItem.Shortages.Add(shortage);

                counter++;
            }
        }

        private void SetTitle()
        {
            if (Creator?.Parent?.GetType() == typeof(MainCreator))
            {
                var str = string.Empty;
                if (Parameters?.Recip != null)
                    str += $" в адрес {Parameters?.Recip?.DisplayName}";
                else if (Parameters?.Sender != null)
                    str += $" {Parameters?.Sender?.DisplayName}";
                if (Parameters?.ToStation != null)
                    str += $" - {Parameters?.ToStation.DisplayName}";
                else if (Parameters?.FromStation != null)
                    str += $" - {Parameters?.FromStation.DisplayName}";
                else if (Parameters?.Payer != null)
                    str += $", плательщик - {Parameters?.Payer?.DisplayName}";
                else if (Parameters?.Owner != null)
                    str += $", владелец - {Parameters?.Owner?.DisplayName}";
                switch (Section)
                {
                    case PeriodSection.Month:
                        Title =
                            $"Отклонение в целом за месяц{str} ({Parameters?.Date:MMMM yyyy})";
                        break;
                    case PeriodSection.Previous:
                        Title =
                            $"Отклонение за прошедщие жд сутки{str}  ({(DateTime.Now.Hour >= 18 ? DateTime.Now.Date : DateTime.Now.Date.AddDays(-1)):dd MMMM yyyy})";
                        break;
                    case PeriodSection.CurrentDay:
                        Title =
                            $"Отклонение за текущие жд сутки{str}  ({(DateTime.Now.Hour >= 18 ? DateTime.Now.Date.AddDays(1) : DateTime.Now.Date):dd MMMM yyyy})";
                        break;
                    case PeriodSection.NextDay:
                        Title =
                            $"Отклонение за следующие жд сутки{str}  ({(DateTime.Now.Hour >= 18 ? DateTime.Now.Date.AddDays(2) : DateTime.Now.Date.AddDays(1)):dd MMMM yyyy})";
                        break;
                    case PeriodSection.CurrentPeriod:
                        Title =
                            $"Отклонение по прошедщие жд сутки{str}  ({Parameters?.Date:dd MMMM yyyy}{(DateTime.Now.Day == 1 ? "" : $"- {(DateTime.Now.Hour >= 18 ? DateTime.Now.Date : DateTime.Now.Date.AddDays(-1)):dd MMMM yyyy}")})";
                        break;

                    case PeriodSection.FirstTriad:
                    case PeriodSection.SecondTriad:
                    case PeriodSection.ThirdTriad:
                        Title =
                            $"Отклонение в целом за {(int)Section} декаду {str} ({Parameters?.Date:MMMM yyyy})";
                        break;
					case PeriodSection.ArbitraryDate:
						Title =
                            $"Отклонение в целом за сутки{str}";
						break;
					case PeriodSection.Arbitrary:
                        Title =
                            $"Отклонение за период {str} ({Parameters?.DateFrom:dd MMMM yyyy} - {Parameters?.DateTo:dd MMMM yyyy})";

                        break;
                }
            }
            else
            {
                Title = string.Empty;
                var cparent = Creator.Parent;
                while (cparent != null)
                {
                    if (!string.IsNullOrEmpty(cparent.Header) && !string.IsNullOrEmpty(cparent.SelectedItem))
                    {
                        Title += $"{cparent.Header}: {cparent.SelectedItem}";
                        cparent = cparent.Parent;
                        if (!string.IsNullOrEmpty(cparent?.Header) && !string.IsNullOrEmpty(cparent?.SelectedItem))
                            Title += ", ";
                    }
                    else
                        cparent = cparent.Parent;
                }
            }
            switch (_setType)
            {
                case DataSetType.PlanFacts:
                    Title +=
                        $": план - {Requirements.Sum(f => f.YValue) + Shortages.Sum(o => o.YValue)}, факт - {Requirements.Sum(f => f.YValue) + OverFulfillments.Sum(f => f.YValue)}";
                    break;
                case DataSetType.Plans:
                    Title +=
                        $": план - {Requirements.Sum(f => f.YValue) + Shortages.Sum(o => o.YValue)}";
                    break;
                case DataSetType.Facts:
                    Title +=
                        $": факт - {Requirements.Sum(f => f.YValue) + OverFulfillments.Sum(f => f.YValue)}";
                    break;
            }
            Title += Parameters.UnitType == UnitTypes.Cars ? " (вагоны)" : " (тыс. тонн)";
        }

        private void InitPreviousDiagram()
        {
            var date = DateTime.Now.Hour >= 18
                ? DateTime.Now.Date
                : DateTime.Now.Date.AddDays(-1);
            var planItems = PlanItems.Where(p => p.Date == date).ToList();
            var factItems = FactItems.Where(f => f.Date == date).ToList();
            var regCards = RegCards.Where(r => r.Date == date).ToList();

            InitDiagram(planItems, factItems, regCards);
        }

        private void InitCurrentPeriodDiagram()
        {
            var date = DateTime.Now.Hour >= 18
                ? DateTime.Now.Date
                : DateTime.Now.Date.AddDays(-1);

            var planItems = PlanItems.Where(p => p.Date <= date).ToList();
            var factItems = FactItems.Where(f => f.Date <= date).ToList();
            var regCards = RegCards.Where(r => r.Date <= date).ToList();

            InitDiagram(planItems, factItems, regCards);
        }

        private void InitMonthDiagram()
        {
            var date = DateTime.Now.Hour >= 18
                ? DateTime.Now.Date
                : DateTime.Now.Date.AddDays(-1);

            var planItems = PlanItems;
            var factItems = FactItems.Where(f => f.Date <= date).ToList();
            var regCards = RegCards.Where(r => r.Date <= date).ToList();

            InitDiagram(planItems, factItems, regCards);
        }

        private void InitCurrentDayDiagram()
        {
            var date = DateTime.Now.Hour >= 18
                ? DateTime.Now.Date.AddDays(1)
                : DateTime.Now.Date;

            var planItems = PlanItems.Where(p => p.Date == date).ToList();
            var factItems = FactItems.Where(f => f.Date == date).ToList();
            var regCards = RegCards.Where(r => r.Date == date).ToList();

            InitDiagram(planItems, factItems, regCards);
        }

        private void InitNextDayDiagram()
        {
            var date = DateTime.Now.Hour >= 18
                ? DateTime.Now.Date.AddDays(2)
                : DateTime.Now.Date.AddDays(1);

            var planItems = PlanItems.Where(p => p.Date == date).ToList();
            var factItems = FactItems.Where(f => f.Date == date).ToList();
            var regCards = RegCards.Where(r => r.Date == date).ToList();

            InitDiagram(planItems, factItems, regCards);
        }

        private void InitTriadDiagram()
        {
            DateTime date1;
            DateTime date2;
            switch (Section)
            {
                case PeriodSection.FirstTriad:
                    date1 = Parameters.Date.Date.AddDays(1 - Parameters.Date.Day);
                    date2 = date1.AddDays(9);
                    break;
                case PeriodSection.SecondTriad:
                    date1 = Parameters.Date.Date.AddDays(11 - Parameters.Date.Day);
                    date2 = date1.AddDays(9);
                    break;
                case PeriodSection.ThirdTriad:
                    date1 = Parameters.Date.Date.AddDays(21 - Parameters.Date.Day);
                    date2 = Parameters.Date.Date.AddDays(1 - Parameters.Date.Day).AddMonths(1).AddDays(-1);
                    break;
                default:
                    return;
            }
            var planItems = PlanItems.Where(pi => pi.Date >= date1 && pi.Date <= date2).ToList();
            var factItems = FactItems.Where(fi => fi.Date >= date1 && fi.Date <= date2).ToList();
            var regCards = RegCards.Where(rc => rc.Date >= date1 && rc.Date <= date2).ToList();

            InitDiagram(planItems, factItems, regCards);
        }

		internal void InitDateDiagram(DateTime date)
		{
            SelectedDate = date;
			var planItems = PlanItems.Where(p => p.Date == SelectedDate).ToList();
			var factItems = FactItems.Where(f => f.Date == SelectedDate).ToList();
			var regCards = RegCards.Where(r => r.Date == SelectedDate).ToList();

			SetTitle();
			InitDiagram(planItems, factItems, regCards);
		}

        #endregion
    }

    public class DeviationChartDataPoint : ChartDataPoint
    {
        private readonly bool _isNegative;
        private double? _planValue;


        public DeviationChartDataPoint(IComparable xValue, string category, double yValue, double? planValue,
            bool isNegative = false)
            : base(xValue, yValue)
        {
            Category = category;
            _planValue = planValue;
            _isNegative = isNegative;
            DisplayValue = yValue;
        }

        public string Category { get; set; }

        public double DisplayValue { get; private set; }

        public double? DeviationValue => _planValue > 0
            ? Math.Round(DisplayValue / _planValue.Value * 100, 2)
            : (double?) null;

        public bool IsVisible => DeviationValue > 0 || _planValue == 0;

        public string DeviationPercent
        {
            get
            {
                var str = DeviationValue.HasValue
                    ? DeviationValue > 1
                        ? $"{Math.Round(DeviationValue.Value, 0)} % "
                        : $"{Math.Round(DeviationValue.Value, 2)} % "
                    : null;
                if (_planValue.HasValue && _isNegative)
                    str = "-" + str;
                return str;
            }
        }

        public string Plan
        {
            get
            {
                var str = string.Empty;
                if (_planValue.HasValue)
                    str += $"{_planValue}";
                return str;
            }
        }
    }

    public class ShipmentChartDataPoint : ChartDataPoint
    {
        public ShipmentChartDataPoint(IComparable xValue, string fullValue, double yValue, CargoPlanFact exampleElement)
            : base(xValue, yValue)
        {
            FullValue = fullValue;
            ExampleElement = exampleElement;
            DisplayValue = yValue;
        }

        public string FullValue { get; set; }

        public CargoPlanFact ExampleElement { get; }

        public double DisplayValue { get; private set; }

        public Color Color { get; private set; }
    }

    public class ShipmentPieChartDataPoint : ChartDataPoint
    {
        public ShipmentPieChartDataPoint(IComparable xValue, double yValue, double sumValue)
            : base(xValue, yValue)
        {
            SumValue = sumValue;
        }

        public double? PercentageValue => SumValue > 0
            ? Math.Round(YValue / SumValue * 100, 2)
            : (double?) null;

        public string PercentageValueStr
        {
            get
            {
                var str = PercentageValue.HasValue
                    ? PercentageValue > 1
                        ? $"{Math.Round(PercentageValue.Value, 0)} % "
                        : $"{Math.Round(PercentageValue.Value, 2)} % "
                    : null;
                return str;
            }
        }

        public double SumValue { get; set; }

        public Color Color { get; set; }
    }

    public enum PeriodSection
    {
        Month,
        FirstTriad,
        SecondTriad,
        ThirdTriad,
        Previous,
        CurrentDay,
        NextDay,
        CurrentPeriod,
		ArbitraryDate,
		Arbitrary
    }

    public enum DetailsType
    {
        ByRecip,
        BySender,
        ByFromStation,
        FromStationByToStation,
        ByToStation,
        ToStationByFromStation
    }
}