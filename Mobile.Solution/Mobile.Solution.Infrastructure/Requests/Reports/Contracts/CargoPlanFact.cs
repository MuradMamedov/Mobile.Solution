using System;

namespace Mobile.Solution.Infrastructure.Requests.Reports.Contracts
{
    public class CargoPlanFact
    {
        public DateTime Date { get; set; }

        public int CarsCount { get; set; }

        public int Weight { get; set; }

        public int FromStationCode { get; set; }

        public string FromStation { get; set; }

        public int ToStationCode { get; set; }

        public string ToStation { get; set; }

        public string Sender { get; set; }

        public string Recip { get; set; }

        public string Payer { get; set; }

        public int PayerCode { get; set; }

        public string FromRailway { get; set; }

        public string ToRailway { get; set; }

        public string ClaimNumber { get; set; }

        public byte ClaimVersion { get; set; }

        public long StateId { get; set; }

        public int ParamRecipID { get; set; }

        public int ParamSenderID { get; set; }

        public short? ParamFreightGroup { get; set; }

        public short ParamFromRwCode { get; set; }

        public short ParamToRwCode { get; set; }

        public int? ParamCarOwnerId { get; set; }

        public string CarOwner { get; set; }

        public string FreightGroup { get; set; }
    }
}