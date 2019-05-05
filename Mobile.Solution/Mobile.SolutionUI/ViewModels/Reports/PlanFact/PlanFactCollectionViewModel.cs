using System;
using System.Collections.Generic;
using System.Linq;
using Mobile.Solution.Infrastructure.CustomControls;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public class PlanFactCollectionViewModel : SelectableViewModel
    {
        private Command _commandRefresh;

        public PlanFactCollectionViewModel(IEnumerable<PlanFactReportViewModel> items, string header)
        {
            var itemArray = items.ToArray();
            Items = items.ToList();
            for (int i = 0; i < itemArray.Length; i+=2)
            {
                Items1.Add(itemArray[i]);
                if((i + 1) < itemArray.Length)
                    Items2.Add(itemArray[i + 1]);
            }
            Header = header;
        }

        public List<PlanFactReportViewModel> Items { get; } = new List<PlanFactReportViewModel>();
        public List<PlanFactReportViewModel> Items1 { get; } = new List<PlanFactReportViewModel>();
        public List<PlanFactReportViewModel> Items2 { get; } = new List<PlanFactReportViewModel>();

        public Command CommandRefresh => _commandRefresh ??
                                         (_commandRefresh =
                                             new Command(() => PlanFactTabsViewModel.Instance.Refresh()));
    }
}