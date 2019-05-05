using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Mobile.Solution.Infrastructure.Requests.NSI
{
    public static class RegCodes
    {
        public static Tuple<string, string, string> GetValue(string key)
        {
            key = "C" + key;
            var type = typeof(RegCodes);
            {
                var resource = type.Namespace + ".RegCodes.xml";
                using (var stream = type.GetTypeInfo().Assembly.GetManifestResourceStream(resource))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var doc = XDocument.Parse(reader.ReadToEnd());
                        var val = doc.Element("codes")?.Element(key)?.Value.Split('@');
                        return new Tuple<string, string, string>(val[0], val[1], val[2]);
                    }
                }
            }
        }
    }
}