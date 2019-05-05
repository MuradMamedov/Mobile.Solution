using System.Collections.Generic;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.Configuration;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;

namespace Mobile.Solution.Infrastructure.Requests.Reports
{
    public class ParkProfitLadenReport : Request<ParkProfitLaden>
    {
        private static ParkProfitLadenReport _instance;

        // Protected

        // Private

        private ParkProfitLadenReport()
        {
        }

        // Public
        public static ParkProfitLadenReport Instance => _instance ?? (_instance = new ParkProfitLadenReport());

        public virtual async Task<List<ParkProfitLaden>> InitRequest(string parameters)
        {
            return await FetchRequest($@"{Config.RestApiAddressString}report/GetParkProfitLaden?{parameters}");
        }
    }
}