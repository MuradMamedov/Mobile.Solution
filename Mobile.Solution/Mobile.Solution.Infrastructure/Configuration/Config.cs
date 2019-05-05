using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Mobile.Solution.Infrastructure.Configuration
{
    public static class Config
    {
        public static string RestApiAddressPlan => GetValue("rest-api-plan");

        public static string RestApiAddressFact => GetValue("rest-api-fact");

        public static string RestApiAddressRegCard => GetValue("rest-api-regCard");

        public static string RestApiAddressPlanFactFile => GetValue("rest-api-planFactFile");

        public static string RestApiAddressPlanRzd => GetValue("rest-api-plan-rzd");

        public static string RestApiAddressFactRzd => GetValue("rest-api-fact-rzd");

        public static string RestApiAddressRegCardRzd => GetValue("rest-api-regCard-rzd");

        public static string RestApiAddressPlanFactFileRzd => GetValue("rest-api-planFactFile-rzd");

        public static string RestApiAddressString => GetValue("rest-api-address");

        public static string RestStatusAddressString => GetValue("rest-status-address");

        public static string RestErrorAddressString => GetValue("rest-error-address");

        public static string RestUpdateAddressString => GetValue("rest-update-address");

        public static string RestApiAddressDict => GetValue("rest-dict-update-address");

        public static string GetValue(string key)
        {
            var type = typeof(Config);
            {
                var resource = type.Namespace + ".config.xml";
                using (var stream = type.GetTypeInfo().Assembly.GetManifestResourceStream(resource))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var doc = XDocument.Parse(reader.ReadToEnd());
                        return doc.Element("config")?.Element(key)?.Value;
                    }
                }
            }
        }
    }
}