namespace Mobile.Solution.Infrastructure.Requests.NSI.Contracts
{
    public class InfoUniqueStation : INsiItem
    {
        public bool ReplyFromEtran { get; set; }

        public int StCode { get; set; }

        public int StCode6 { get; set; }

        public short StDpRwId { get; set; }

        public short RwCode { get; set; }

        public int? DpId { get; set; }

        public int StCnId { get; set; }

        public string StName12Char { get; set; }

        public string StName { get; set; }

        public bool FreightSign { get; set; }

        public string DisplayCode => $"Код ({StCode})";

        public string RequestParameter => "InfoUniqueStation";

        public string DisplayName => $"{StName} ({StCode})";

        public override string ToString()
        {
            return DisplayName ?? "";
        }
    }
}