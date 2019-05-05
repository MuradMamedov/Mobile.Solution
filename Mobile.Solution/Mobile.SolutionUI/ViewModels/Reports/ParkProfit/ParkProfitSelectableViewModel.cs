using System;
using System.Collections.Generic;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Requests.NSI.Contracts;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;

namespace Mobile.Solution.UI.ViewModels
{
    public class ParkProfitSelectableViewModel : SelectableViewModel
    {
        private List<InfoCarInAgreement> _cars;

        private DateTime _dateFrom;

        private DateTime _dateTo;

        private List<ParkProfitEmpty> _emptyItems;

        private List<ParkProfitLaden> _ladenItems;

        public List<ParkProfitLaden> LadenItems
        {
            get => _ladenItems;
            private set { Set(() => LadenItems, ref _ladenItems, value); }
        }

        public List<ParkProfitEmpty> EmptyItems
        {
            get => _emptyItems;
            private set { Set(() => EmptyItems, ref _emptyItems, value); }
        }

        public List<InfoCarInAgreement> Cars
        {
            get => _cars;
            private set { Set(() => Cars, ref _cars, value); }
        }

        public DateTime DateFrom
        {
            get => _dateFrom;
            private set { Set(() => DateFrom, ref _dateFrom, value); }
        }

        public DateTime DateTo
        {
            get => _dateTo;
            private set { Set(() => DateTo, ref _dateTo, value); }
        }


        internal virtual void SetData(List<ParkProfitLaden> ladenItems, List<ParkProfitEmpty> emptyItems,
            List<InfoCarInAgreement> cars, DateTime dateFrom, DateTime dateTo)
        {
            DateFrom = dateFrom;
            DateTo = dateTo;
            LadenItems = new List<ParkProfitLaden>(ladenItems);
            EmptyItems = new List<ParkProfitEmpty>(emptyItems);
            Cars = new List<InfoCarInAgreement>(cars);
        }
    }
}