﻿using System;
using System.Collections.Generic;
using Android.OS;
using KeyChain.Net.XamarinAndroid;
using Mobile.Solution.Droid.Dependencies;
using Mobile.Solution.Infrastructure.Dependencies;
using Xamarin.Forms;
using Application = Android.App.Application;
using System.Linq;

[assembly: Dependency(typeof(AppInfo))]

namespace Mobile.Solution.Droid.Dependencies
{
    public class AppInfo : IAppInfo
    {
        public string Version => Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, 0)
            .VersionName;

        public string UniqueId
        {
            get
            {
                string uniqueId;
                try
                {
                    uniqueId = Build.Serial;
                    if (string.IsNullOrEmpty(uniqueId))
                    {
                        var keyChain =
                            new KeyChainHelper(() => Application.Context, "KEYCHNPSWD");
                        uniqueId = keyChain.GetKey("UniqueId");
                        if (string.IsNullOrEmpty(uniqueId))
                        {
                            uniqueId = Guid.NewGuid().ToString();
                            keyChain.SetKey("UniqueId", uniqueId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    uniqueId = ex.Message;
                }
                return $"Droid_{uniqueId}_{PhoneNumber}";
            }
        }

        public bool IsRegistered
        {
            get
            {
                try
                {
                    var keyChain =
                        new KeyChainHelper(() => Application.Context, "KEYCHNPSWD");
                    return keyChain.GetKey("Registered") != null;
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
                    var keyChain =
                        new KeyChainHelper(() => Application.Context, "KEYCHNPSWD");
                    if (value)
                        keyChain.SetKey("Registered", "true");
                    else
                        keyChain.DeleteKey("Registered");
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
                var keyChain = new KeyChainHelper(() => Application.Context, "KEYCHNPSWD");
                return keyChain.GetKey("PhoneNumber");
            }
            set
            {
                var keyChain = new KeyChainHelper(() => Application.Context, "KEYCHNPSWD");
                keyChain.SetKey("PhoneNumber", value);
            }
        }

        public Dictionary<string, string> Dictionaries
        {
            get
            {
                var keyChain = new KeyChainHelper(() => Application.Context, "KEYCHNPSWD");
                var res = new Dictionary<string, string>();
                foreach (var dict in typeof(Infrastructure.Requests.NSI.Dictionaries).GetProperties().Where(prop => prop.PropertyType.GenericTypeArguments.Any()).Select(prop => prop.Name))
                {
                    var guid = keyChain.GetKey(dict);
                    res.Add(dict, guid);
                }
                return res;
            }
            set
            {
                var keyChain = new KeyChainHelper(() => Application.Context, "KEYCHNPSWD");
                foreach (var v in value)
                {
                    keyChain.SetKey(v.Key, v.Value);
                }
            }
        }
    }
}