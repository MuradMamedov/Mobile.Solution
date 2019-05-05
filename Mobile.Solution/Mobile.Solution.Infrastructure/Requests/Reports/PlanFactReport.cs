using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.Configuration;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.Requests.Reports
{
    public sealed class PlanFactReport : Request<CargoPlanFact>
    {
        private static PlanFactReport _instance;

        // Protected

        // Private

        private PlanFactReport()
        {
        }

        // Public
        public static PlanFactReport Instance => _instance ?? (_instance = new PlanFactReport());

        public async Task<List<CargoPlanFact>> InitPlanRequest(PlanFactParameters parameters)
        {
            if (parameters.SourceType == SourceType.RzdType)
                return await FetchRequest(GenerateUrl(Config.RestApiAddressPlanRzd, parameters));
            return await FetchRequest(GenerateUrl(Config.RestApiAddressPlan, parameters));
        }

        public async Task<List<CargoPlanFact>> InitFactRequest(PlanFactParameters parameters)
        {
            if (parameters.SourceType == SourceType.RzdType)
                return await FetchRequest(GenerateUrl(Config.RestApiAddressFactRzd, parameters));
            return await FetchRequest(GenerateUrl(Config.RestApiAddressFact, parameters));
        }

        public async Task<string> GenerateReportFile(PlanFactParameters parameters)
        {
            try
            {
                var url = GenerateUrl(Config.RestApiAddressPlanFactFile, parameters);
                using (var httpClient = new HttpClient {Timeout = new TimeSpan(0, 10, 0)})
                using (
                    var response = await httpClient.GetAsync(new Uri(url)))
                {
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                        return await response.Content.ReadAsStringAsync();
                }
            }
            catch
            {
                // ignored
            }
            return null;
        }

        public Downloader GetFile(string @params, string saveFileName = null)
        {
            var downloader = new Downloader();
            try
            {
                var url = $@"{Config.RestApiAddressString}report/GetFile?{@params}";
                downloader.InitializeDownload(url,
                    new Dictionary<string, string> {{"name", $"{saveFileName ?? @params}.xlsx"}});
            }
            catch
            {
                // ignored
            }
            return downloader;
        }

        private string GenerateUrl(string format, PlanFactParameters parameters)
        {
            var appInfo = DependencyService.Get<IAppInfo>();
            if (parameters.PeriodType == PeriodTypes.AnyPeriod)
                return string.Format(format, appInfo.UniqueId,
                    $"{parameters.DateFrom:dd-MM-yyyy}",
                    $"{parameters.DateTo:dd-MM-yyyy}",
                    (int) parameters.ConditionScheme,
                    (int) parameters.PlanType,
                    parameters.Recip?.Id,
                    parameters.Sender?.Id,
                    parameters.Payer?.Id,
                    parameters.Owner?.Id,
                    parameters.FromStation?.StCode,
                    parameters.FromRwCode,
                    parameters.ToStation?.StCode,
                    parameters.ToRwCode,
                    parameters.FreightGroup?.Number,
                    (int) parameters.UnitType);
            return string.Format(format, appInfo.UniqueId,
                $"{parameters.Date:dd-MM-yyyy}",
                $"{parameters.Date.AddMonths(1).AddDays(-1):dd-MM-yyyy}",
                (int) parameters.ConditionScheme,
                (int) parameters.PlanType,
                parameters.Recip?.Id,
                parameters.Sender?.Id,
                parameters.Payer?.Id,
                parameters.Owner?.Id,
                parameters.FromStation?.StCode,
                parameters.FromRwCode,
                parameters.ToStation?.StCode,
                parameters.ToRwCode,
                parameters.FreightGroup?.Number,
                (int) parameters.UnitType);
        }
    }
}