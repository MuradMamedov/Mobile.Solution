using System;
using System.Collections.Generic;
using Foundation;
using Mobile.Solution.iOS.Dependencies;
using Mobile.Solution.Infrastructure.Dependencies;
using Security;
using UIKit;
using Xamarin.Forms;
using System.Linq;

[assembly: Dependency(typeof(AppInfo))]

namespace Mobile.Solution.iOS.Dependencies
{
    public class AppInfo : IAppInfo
    {
        public string Version
        {
            get
            {
                var ver = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
                return ver.ToString();
            }
        }

        public string UniqueId
        {
            get
            {
                string res;
                try
                {
                    res = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
                    if (string.IsNullOrEmpty(res))
                    {
                        var query = new SecRecord(SecKind.GenericPassword)
                        {
                            Service = NSBundle.MainBundle.BundleIdentifier,
                            Account = "UniqueID"
                        };

                        var uniqueId = SecKeyChain.QueryAsData(query);
                        if (uniqueId == null)
                        {
                            query.ValueData = NSData.FromString(Guid.NewGuid().ToString());
                            var err = SecKeyChain.Add(query);
                            if (err != SecStatusCode.Success && err != SecStatusCode.DuplicateItem)
                                res = "Cannot store Unique ID";
                            else
                                res = query.ValueData.ToString();
                        }
                        else
                        {
                            res = uniqueId.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    res = ex.Message;
                }
                return $"iOS_{res}_{PhoneNumber}";
            }
        }

        public bool IsRegistered
        {
            get
            {
                try
                {
                    var query = new SecRecord(SecKind.GenericPassword)
                    {
                        Service = NSBundle.MainBundle.BundleIdentifier,
                        Account = "Registered"
                    };

                    var isRegistered = SecKeyChain.QueryAsData(query);
                    return isRegistered != null;
                }
                catch
                {
                    // ignored
                }
                return false;
            }

            set
            {
                try
                {
                    var query = new SecRecord(SecKind.GenericPassword)
                    {
                        Service = NSBundle.MainBundle.BundleIdentifier,
                        Account = "Registered"
                    };
                    if (value)
                    {
                        var isRegistered = SecKeyChain.QueryAsData(query);
                        if (isRegistered == null)
                        {
                            query.ValueData = NSData.FromString(Guid.NewGuid().ToString());
                            SecKeyChain.Add(query);
                        }
                    }
                    else
                    {
                        SecKeyChain.Remove(query);
                    }
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
                string res;
                try
                {
                    var query = new SecRecord(SecKind.GenericPassword)
                    {
                        Service = NSBundle.MainBundle.BundleIdentifier,
                        Account = "PhoneNumber"
                    };

                    var phoneNumber = SecKeyChain.QueryAsData(query);

                    res = phoneNumber?.ToString();
                }
                catch (Exception ex)
                {
                    res = ex.Message;
                }
                return res;
            }

            set
            {
                try
                {
                    var query = new SecRecord(SecKind.GenericPassword)
                    {
                        Service = NSBundle.MainBundle.BundleIdentifier,
                        Account = "PhoneNumber"
                    };

                    var phoneNumber = SecKeyChain.QueryAsData(query);
                    if (phoneNumber == null)
                    {
                        query.ValueData = NSData.FromString(value);
                        SecKeyChain.Add(query);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        public Dictionary<string, string> Dictionaries
        {
            get
            {
                var res = new Dictionary<string, string>();
                foreach (var dict in typeof(Infrastructure.Requests.NSI.Dictionaries).GetProperties().Where(prop => prop.PropertyType.GenericTypeArguments.Count() > 0).Select(prop => prop.Name))
                {
                    var query = new SecRecord(SecKind.GenericPassword)
                    {
                        Service = NSBundle.MainBundle.BundleIdentifier,
                        Account = dict
                    };

                    var guid = SecKeyChain.QueryAsData(query);

                    res.Add(dict, guid?.ToString());
                }
                return res;
            }
            set
            {
                foreach (var v in value)
                {
                    var query = new SecRecord(SecKind.GenericPassword)
                    {
                        Service = NSBundle.MainBundle.BundleIdentifier,
                        Account = v.Key
                    };

                    var guid = SecKeyChain.QueryAsData(query);
                    if (guid == null)
                    {
                        query.ValueData = NSData.FromString(v.Value);
                        SecKeyChain.Add(query);
                    }
                }
            }
        }

        public bool HideVersionInfo
        {
            get
            {
                try
                {
                    var query = new SecRecord(SecKind.GenericPassword)
                    {
                        Service = NSBundle.MainBundle.BundleIdentifier,
                        Account = "HideVersionInfo"
                    };

                    var hideVersionInfo = SecKeyChain.QueryAsData(query);
                    return hideVersionInfo != null;
                }
                catch
                {
                    // ignored
                }
                return false;
            }

            set
            {
                try
                {
                    var query = new SecRecord(SecKind.GenericPassword)
                    {
                        Service = NSBundle.MainBundle.BundleIdentifier,
                        Account = "HideVersionInfo"
                    };
                    if (value)
                    {
                        var hideVersionInfo = SecKeyChain.QueryAsData(query);
                        if (hideVersionInfo == null)
                        {
                            query.ValueData = NSData.FromString(Guid.NewGuid().ToString());
                            SecKeyChain.Add(query);
                        }
                    }
                    else
                    {
                        SecKeyChain.Remove(query);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}