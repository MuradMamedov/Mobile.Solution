using System;
using System.Collections.Generic;
using System.Linq;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Requests.NSI;
using Mobile.Solution.Infrastructure.Requests.Reports;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;
using Mobile.Solution.UI.Helpers;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public class RegCardChartDataPoint
    {
        public RegCardChartDataPoint(string v1, double v2, double v3)
        {
            Label = v1;
            Value = v2;
            Percent = v3;
        }

        public string Label { get; }
        public double Value { get; }
        public double Percent { get; }
    }

    public class RegCardReason
    {
        private int _code;
        private Tuple<string, string, string> _tuple;

        public RegCardReason(int code, Tuple<string, string, string> fields)
        {
            _code = code;
            _tuple = fields;
            Descr = fields.Item1;
            Note = "Примечание: " + fields.Item2;
            Resp = fields.Item3;
        }

        public int Code
        {
            get { return _code; }
        }

        public string Descr { get; private set; }
        public string Note { get; private set; }
        public string Resp { get; private set; }
    }

    public class RegCardViewModel : SelectableViewModel
    {
        public RegCardViewModel(Creator creator, PlanFactParameters parameters, PeriodSection section,
            Command commandRefresh)
        {
            Creator = creator;
            Header = creator?.Header;
            Section = section;
            Parameters = parameters;
            CommandRefresh = commandRefresh;
        }

        public List<CargoPlanFact> PlanItems { get; private set; }

        public List<CargoPlanFact> FactItems { get; private set; }

        public List<RegCard> RegCards { get; private set; }

        public Creator Creator { get; }

        public string Title { get; private set; }

        public PlanFactParameters Parameters { get; private set; }

        public PeriodSection Section { get; private set; }

        public List<RegCardChartDataPoint> Items { get; private set; }

        public List<RegCardReason> CodesDescriptions { get; private set; }

        public double MaximumValue
        {
            get
            {
                if (Items?.Count > 0)
                    return Math.Max(10, (int) (Items.Max(p => p.Value) * 1.2));
                return 10;
            }
        }

        public Command CommandRefresh { get; }

        private void InitItems(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems,
            List<RegCard> regCards)
        {
            PlanItems = planItems.Where(v => Creator.Filter(v)).ToList();
            FactItems = factItems.Where(v => Creator.Filter(v)).ToList();
            RegCards = regCards.Where(Creator.RcFilter).ToList();
        }

        public void SetData(List<CargoPlanFact> plans, List<CargoPlanFact> facts, List<RegCard> regCards,
            PlanFactParameters parameters)
        {
            InitItems(plans, facts, regCards);
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

        private void InitDiagram(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems, List<RegCard> regCards)
        {
            IEnumerable<IGrouping<int, RegCard>> group = new List<IGrouping<int, RegCard>>();

            double plansCount = planItems.Sum(p => p.CarsCount) - factItems.Sum(p => p.CarsCount);
            group = regCards.GroupBy(p => p.ReasonCode);
            var itms = group.Select(g => new RegCardReason(g.Key, RegCodes.GetValue(g.Key.ToString())));
            CodesDescriptions = new List<RegCardReason>(itms);
            if (plansCount > 0)
            {
                var items = new List<RegCardChartDataPoint>();
                var withoutReasons = plansCount;
                foreach (
                    var d in group.OrderBy(g => g.Key))
                {
                    var value = d.Sum(f => f.CarCount);
                    withoutReasons -= value;
                    if (value > 0)
                        items.Add(new RegCardChartDataPoint($"{d.Key} код", value, Math.Round(value / plansCount, 2)));
                }
                if (withoutReasons > 0)
                {
                    items.Insert(0,
                        new RegCardChartDataPoint("0 код", withoutReasons, Math.Round(withoutReasons / plansCount, 2)));
                    CodesDescriptions.Insert(0, new RegCardReason(0, RegCodes.GetValue("0")));
                }
                CodesDescriptions = CodesDescriptions.OrderBy(cd => cd.Code).ToList();
                Items = items.OrderByDescending(i => i.Value).ToList();

                var route = string.Empty;
                var cparent = Creator.Parent;
                while (cparent != null)
                {
                    if (!string.IsNullOrEmpty(cparent.Header) && !string.IsNullOrEmpty(cparent.SelectedItem))
                    {
                        route += $"{cparent.Header}: {cparent.SelectedItem}";
                        cparent = cparent.Parent;
                        if (!string.IsNullOrEmpty(cparent?.Header) && !string.IsNullOrEmpty(cparent?.SelectedItem))
                            route += ", ";
                    }
                    else
                        cparent = cparent.Parent;
                }
                if (!string.IsNullOrEmpty(route))
                    route += ". ";
                Title =
                    $"{route}Всего - {plansCount}, по причинам - {plansCount - withoutReasons}, нет причины - {withoutReasons}";
            }
        }
    }
}