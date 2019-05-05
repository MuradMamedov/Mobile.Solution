using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.Configuration;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.Requests
{
    public abstract class Request<T>
    {
        public static string UrlEncode(string value)
        {
            var reservedCharacters = "!*'();:@&=+$,/?%#[]";

            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var @char in value)
            {
                if (reservedCharacters.IndexOf(@char) == -1)
                    sb.Append(@char);
                else
                    sb.AppendFormat("%{0:X2}", (int) @char);
            }
            return sb.ToString();
        }

        public static async Task<bool> TestConnection(string uniqueId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var uri = new Uri(string.Format(Config.RestStatusAddressString, uniqueId));
                    var response = await httpClient.GetAsync(uri);
                    return response?.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                // ignored
            }
            return false;
        }

        internal static async Task SendError(string uniqueId, string errorMessage)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("UniqueId", uniqueId),
                    new KeyValuePair<string, string>("Content", errorMessage)
                });
                await httpClient.PostAsync(new Uri(string.Format(Config.RestErrorAddressString)), content);
            }
        }

        public static async Task<bool> RegisterPhone(string uniqueId, long phoneNumber)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var uri =
                        new Uri(
                            $"{Config.RestApiAddressString}register/RegisterPhone?uniqueId={uniqueId}&phoneNumber={phoneNumber}");
                    var response = await httpClient.GetAsync(uri);
                    return response?.StatusCode == HttpStatusCode.Accepted;
                }
            }
            catch
            {
                // ignored
            }
            return false;
        }

        internal static async Task<Tuple<string, string>> CheckForDictUpdates(string uniqueId, string nsi, string guid)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var uri =
                        new Uri(string.Format(Config.RestApiAddressDict, uniqueId, nsi, guid));
                    var response = await httpClient.GetAsync(uri);
                    if (response.StatusCode == HttpStatusCode.NotModified)
                        return new Tuple<string, string>(null, null);
                    return new Tuple<string, string>(response.Headers.GetValues("guid").FirstOrDefault(),
                        await response.Content.ReadAsStringAsync());
                }
            }
            catch
            {
                // ignored
            }
            return new Tuple<string, string>(null, null);
        }

        internal static async Task<bool> CheckCode(string uniqueId, int code)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var uri =
                        new Uri($"{Config.RestApiAddressString}register/CheckCode?uniqueId={uniqueId}&code={code}");
                    var response = await httpClient.GetAsync(uri);
                    return response?.StatusCode == HttpStatusCode.Accepted;
                }
            }
            catch
            {
                // ignored
            }
            return false;
        }

        public static async Task<Tuple<bool, string>> CheckForUpdates(string version)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage response = null;
                    if (Device.RuntimePlatform == Device.iOS)
                        response =
                            await httpClient.GetAsync(new Uri($"{Config.RestUpdateAddressString}version={version}"));
                    if (Device.RuntimePlatform == Device.Android)
                        response = await httpClient.GetAsync(
                            new Uri($"{Config.RestUpdateAddressString}version={version}&os=android"));
                    if (Device.RuntimePlatform == Device.Windows || Device.RuntimePlatform == Device.WinPhone)
                        response = await httpClient.GetAsync(
                            new Uri($"{Config.RestUpdateAddressString}version={version}&os=uwp"));
                    return new Tuple<bool, string>(response?.StatusCode == HttpStatusCode.OK,
                        await response.Content.ReadAsStringAsync());
                }
            }
            catch
            {
                // ignored
            }
            return new Tuple<bool, string>(false, "");
        }

        protected virtual async Task<List<T>> FetchRequest(string url)
        {
            try
            {
                using (var httpClient = new HttpClient())
                using (
                    var response = await httpClient.GetAsync(new Uri(url)))
                {
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        var report = JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());
                        return report;
                    }
                }
            }
            catch
            {
                // ignored
            }
            return new List<T>();
        }
    }
}