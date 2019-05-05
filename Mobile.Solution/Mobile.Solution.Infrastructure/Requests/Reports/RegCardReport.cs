using System.Collections.Generic;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.Configuration;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.Requests.Reports
{
    public sealed class RegCardReport : Request<RegCard>
    {
        private static RegCardReport _instance;

        // Protected

        // Private

        private RegCardReport()
        {
        }

        // Public
        public static RegCardReport Instance => _instance ?? (_instance = new RegCardReport());

        public async Task<List<RegCard>> InitRequest(PlanFactParameters parameters)
        {
            return await FetchRequest(GenerateUrl(Config.RestApiAddressRegCard, parameters));
        }

        private string GenerateUrl(string format, PlanFactParameters parameters)
        {
            var appInfo = DependencyService.Get<IAppInfo>();
            if (parameters.PeriodType == PeriodTypes.AnyPeriod)
                return string.Format(format,
                    appInfo.UniqueId,
                    $"{parameters.DateFrom:dd-MM-yyyy}",
                    $"{parameters.DateTo:dd-MM-yyyy}",
                    parameters.Recip?.Id,
                    parameters.Sender?.Id,
                    parameters.Payer?.Id,
                    parameters.Owner?.Id,
                    parameters.FromStation?.StCode,
                    parameters.ToStation?.StCode,
                    parameters.FreightGroup?.Number);
            return string.Format(format,
                appInfo.UniqueId,
                $"{parameters.Date:dd-MM-yyyy}",
                $"{parameters.Date.AddMonths(1).AddDays(-1):dd-MM-yyyy}",
                parameters.Recip?.Id,
                parameters.Sender?.Id,
                parameters.Payer?.Id,
                parameters.Owner?.Id,
                parameters.FromStation?.StCode,
                parameters.ToStation?.StCode,
                parameters.FreightGroup?.Number);
        }
    }
}