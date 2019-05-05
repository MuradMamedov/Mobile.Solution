using Mobile.Solution.Infrastructure.Requests.Reports;

namespace Mobile.Solution.UI.Helpers
{
    public class StationCreator : Creator
    {
        public StationCreator(ReportDirections direction, params Creator[] excludeTypes)
            : base(direction, excludeTypes)
        {
            Filter = v => SelectedItem == null ||
                          (Direction == ReportDirections.Arrival
                              ? v.ToStation == SelectedItem
                              : v.FromStation == SelectedItem);
            RcFilter = v => SelectedItem == null ||
                            (Direction == ReportDirections.Arrival
                                ? v.ToStation == SelectedItem
                                : v.FromStation == SelectedItem);

            Group = v => Direction == ReportDirections.Arrival
                ? v.ToStation
                : v.FromStation;
            Header = Direction == ReportDirections.Arrival
                ? "Ст. назн."
                : "Ст. отпр.";
        }
    }
}