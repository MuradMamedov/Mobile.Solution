using System;

namespace Mobile.Solution.Infrastructure.Requests.Reports.Contracts
{
    public class RegCard
    {
        public DateTime Date { get; set; }

        public string FromStation { get; set; }

        public string ToStation { get; set; }

        public int CarCount { get; set; }

        public int ReasonCode { get; set; }

        public string Reason { get; set; }

        public string Sender { get; set; }

        public string Recip { get; set; }

        public string Export { get; set; }

        public string ClaimNumber { get; set; }

        public string Payer { get; set; }

        public string CarOwner { get; set; }

        public string FromRailway { get; set; }

        public string ToRailway { get; set; }

        public short? FreightGroupNumber { get; set; }

        public string FreightGroup { get; set; }
    }
}