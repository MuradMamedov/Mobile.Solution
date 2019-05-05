using Mobile.Solution.Infrastructure.Requests.Reports;

namespace Mobile.Solution.UI.Helpers
{
    public class RegCardCreator : Creator
    {
        public RegCardCreator(ReportDirections direction, params Creator[] excludeTypes)
            : base(direction, excludeTypes)
        {
            Header = "Причины по УК";
        }
    }
}