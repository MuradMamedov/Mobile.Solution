using System;

namespace Mobile.Solution.Infrastructure.Requests.Reports.Contracts
{
    public class ParkProfitLaden
    {
        public DateTime DateAccountByDeparture { get; set; }

        public decimal ProfitWithVat { get; set; }

        public byte TaxationType { get; set; }

        public int CarNumber { get; set; }
    }
}