using Mobile.Solution.Infrastructure.Requests.Reports;

namespace Mobile.Solution.UI.Helpers
{
    public class FreightCreator : Creator
    {
        public FreightCreator(ReportDirections direction, params Creator[] excludeTypes) : base(direction, excludeTypes)
        {
            Filter = v => SelectedItem == null || v.FreightGroup == SelectedItem;
            RcFilter = v => SelectedItem == null || v.FreightGroup == SelectedItem;

            Group = v => v.FreightGroup;
            Header = "Группа груза";
        }
    }
}