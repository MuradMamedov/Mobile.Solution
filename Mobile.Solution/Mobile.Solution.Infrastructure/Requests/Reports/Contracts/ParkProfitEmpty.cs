using System;

namespace Mobile.Solution.Infrastructure.Requests.Reports.Contracts
{
    public class ParkProfitEmpty
    {
        public long InvoiceId { get; set; }

        public DateTime DateReady { get; set; }

        public decimal Cost { get; set; }
    }
}