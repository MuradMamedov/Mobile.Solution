using Mobile.Solution.Infrastructure.Requests.Reports;

namespace Mobile.Solution.UI.Helpers
{
    public class ContractorCreator : Creator
    {
        public ContractorCreator(ReportDirections direction, params Creator[] excludeTypes)
            : base(direction, excludeTypes)
        {
            Filter = v => SelectedItem == null ||
                          (Direction == ReportDirections.Arrival
                              ? v.Recip == SelectedItem
                              : v.Sender == SelectedItem);
            RcFilter = v => SelectedItem == null ||
                            (Direction == ReportDirections.Arrival
                                ? v.Recip == SelectedItem
                                : v.Sender == SelectedItem);

            Group = v => Direction == ReportDirections.Arrival
                ? v.Recip
                : v.Sender;
            Header = Direction == ReportDirections.Arrival
                ? "Грузополучатели"
                : "Грузоотправители";
        }
    }
}