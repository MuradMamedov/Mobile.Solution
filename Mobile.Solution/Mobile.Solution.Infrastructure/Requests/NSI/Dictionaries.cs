using System;
using System.Collections.Generic;
using Mobile.Solution.Infrastructure.Requests.NSI.Contracts;

namespace Mobile.Solution.Infrastructure.Requests.NSI
{
    public class Dictionaries
    {
        private static readonly Lazy<Dictionaries> _instance =
            new Lazy<Dictionaries>(() => new Dictionaries());

        private Dictionaries()
        {
        }

        public static Dictionaries Instance => _instance.Value;

        public List<InfoOrgPassport> InfoOrgPassport { get; set; }

        public List<InfoUniqueStation> FreightStation { get; set; }

        public List<InfoSumFreight> InfoSumFreight { get; set; }
    }
}