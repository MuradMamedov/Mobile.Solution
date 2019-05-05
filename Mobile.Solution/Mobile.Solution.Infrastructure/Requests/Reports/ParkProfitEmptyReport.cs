using System.Collections.Generic;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.Configuration;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;

namespace Mobile.Solution.Infrastructure.Requests.Reports
{
    public class ParkProfitEmptyReport : Request<ParkProfitEmpty>
    {
        private static ParkProfitEmptyReport _instance;

        // Protected

        // Private

        private ParkProfitEmptyReport()
        {
        }

        // Public
        public static ParkProfitEmptyReport Instance => _instance ?? (_instance = new ParkProfitEmptyReport());

        public virtual async Task<List<ParkProfitEmpty>> InitRequest(string parameters)
        {
            return await FetchRequest($@"{Config.RestApiAddressString}report/GetParkProfitEmpty?{parameters}");
        }
    }
}