using Mobile.Solution.Infrastructure.Requests.Reports;

namespace Mobile.Solution.UI.Helpers
{
    public class MainCreator : Creator
    {
        public MainCreator(ReportDirections direction, params Creator[] excludeTypes) : base(direction, excludeTypes)
        {
        }
    }
}