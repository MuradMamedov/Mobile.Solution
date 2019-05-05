using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.System.Profile;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.Infrastructure.Requests.NSI;
using Syncfusion.Data.Extensions;
using Xamarin.Forms;
using AppInfo = Mobile.Solution.UWP.Dependencies.AppInfo;

[assembly: Dependency(typeof(AppInfo))]

namespace Mobile.Solution.UWP.Dependencies
{
    public class AppInfo : IAppInfo
    {
        public string Version
        {
            get
            {
                var package = Package.Current;
                var packageId = package.Id;
                var version = packageId.Version;

                return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
        }

        public string UniqueId
        {
            get
            {
                string uniqueId;
                try
                {
                    var token = HardwareIdentification.GetPackageSpecificToken(null);
                    var hardwareId = token.Id;
                    var hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
                    var hashed = hasher.HashData(hardwareId);
                    uniqueId = CryptographicBuffer.EncodeToHexString(hashed);
                }
                catch (Exception ex)
                {
                    uniqueId = ex.Message;
                }
                return $"UWP_{uniqueId}_{PhoneNumber}";
            }
        }

        public bool IsRegistered
        {
            get
            {
                try
                {
                    var localSettings =
                        ApplicationData.Current.LocalSettings;

                    return localSettings.Values["Registered"] != null;
                }
                catch
                {
                    //ignored
                }
                return false;
            }
            set
            {
                try
                {
                    var localSettings =
                        ApplicationData.Current.LocalSettings;
                    if (value)
                        localSettings.Values["Registered"] = true;
                    else
                        localSettings.DeleteContainer("Registered");
                }
                catch
                {
                    // ignored
                }
            }
        }

        public string PhoneNumber
        {
            get
            {
                var localSettings =
                    ApplicationData.Current.LocalSettings;
                return localSettings.Values["PhoneNumber"]?.ToString();
            }
            set
            {
                var localSettings =
                    ApplicationData.Current.LocalSettings;
                localSettings.Values["PhoneNumber"] = value;
            }
        }

        public Dictionary<string, string> Dictionaries
        {
            get
            {
                var localSettings =
                    ApplicationData.Current.LocalSettings;
                var res = new Dictionary<string, string>();
                foreach (var dict in typeof(Dictionaries).GetProperties()
                    .Where(prop => prop.PropertyType.GenericTypeArguments.Any())
                    .Select(prop => prop.Name))
                {
                    var guid = localSettings.Values[dict]?.ToString();
                    res.Add(dict, guid);
                }
                return res;
            }
            set
            {
                var localSettings =
                    ApplicationData.Current.LocalSettings;
                foreach (var v in value)
                    localSettings.Values[v.Key] = v.Value;
            }
        }
    }
}