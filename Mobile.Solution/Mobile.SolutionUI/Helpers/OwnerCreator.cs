using Mobile.Solution.Infrastructure.Requests.Reports;

namespace Mobile.Solution.UI.Helpers
{
    public class OwnerCreator : Creator
    {
        public OwnerCreator(ReportDirections direction, params Creator[] excludeTypes)
            : base(direction, excludeTypes)
        {
            Filter = v => SelectedItem == null || v.CarOwner == SelectedItem;
            RcFilter = v => SelectedItem == null || v.CarOwner == SelectedItem;

            Group = v => v.CarOwner;
            Header = "Владельцы вагонов";
        }
    }
}