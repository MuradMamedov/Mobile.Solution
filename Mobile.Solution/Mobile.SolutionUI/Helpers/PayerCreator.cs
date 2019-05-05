using Mobile.Solution.Infrastructure.Requests.Reports;

namespace Mobile.Solution.UI.Helpers
{
    public class PayerCreator : Creator
    {
        public PayerCreator(ReportDirections direction, params Creator[] excludeTypes)
            : base(direction, excludeTypes)
        {
            Filter = v => SelectedItem == null || v.Payer == SelectedItem;
            RcFilter = v => SelectedItem == null || v.Payer == SelectedItem;

            Group = v => v.Payer;
            Header = "Плательщики";
        }
    }
}