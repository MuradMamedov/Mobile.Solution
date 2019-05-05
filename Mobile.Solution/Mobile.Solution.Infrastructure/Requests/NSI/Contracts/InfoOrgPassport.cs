namespace Mobile.Solution.Infrastructure.Requests.NSI.Contracts
{
    public class InfoOrgPassport : INsiItem
    {
        public int? Id { get; set; }

        public long Okpo { get; set; }

        public string Inn { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string CustomName { get; set; }

        public int Kpp { get; set; }

        public string RequestParameter => "InfoOrgPassport";

        public string DisplayName => $"{(!string.IsNullOrEmpty(CustomName) ? CustomName : ShortName)} - {Okpo}";

        public override string ToString()
        {
            return DisplayName;
        }
    }
}