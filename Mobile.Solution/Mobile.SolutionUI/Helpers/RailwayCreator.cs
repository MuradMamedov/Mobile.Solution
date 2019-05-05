using Mobile.Solution.Infrastructure.Requests.Reports;

namespace Mobile.Solution.UI.Helpers
{
    public class RailwayCreator : Creator
    {
        public RailwayCreator(ReportDirections direction, params Creator[] excludeTypes)
            : base(direction, excludeTypes)
        {
            Filter = v => SelectedItem == null ||
                          (Direction == ReportDirections.Arrival
                              ? v.ToRailway == SelectedItem
                              : v.FromRailway == SelectedItem);
            RcFilter = v => SelectedItem == null ||
                            (Direction == ReportDirections.Arrival
                                ? v.ToRailway == SelectedItem
                                : v.FromRailway == SelectedItem);
            Group = v => Direction == ReportDirections.Arrival
                ? v.ToRailway
                : v.FromRailway;
            Header = Direction == ReportDirections.Arrival
                ? "Дорога назначения"
                : "Дорога отправления";
        }
    }
}