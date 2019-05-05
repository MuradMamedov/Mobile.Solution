using System.Collections.Generic;

namespace Mobile.Solution.Infrastructure.Dependencies
{
    public interface IAppInfo
    {
        Dictionary<string, string> Dictionaries { get; set; }

        string Version { get; }

        string UniqueId { get; }

        bool IsRegistered { get; set; }

        bool HideVersionInfo { get; set; }

        string PhoneNumber { get; set; }
    }
}