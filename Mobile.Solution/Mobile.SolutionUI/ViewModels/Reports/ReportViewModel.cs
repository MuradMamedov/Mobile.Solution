using System.IO;
using Mobile.Solution.Infrastructure.Dependencies;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public abstract class ReportViewModel
    {
        public ReportViewModel(ReportTabsViewModel parent, Report report, INavigation navigation)
        {
            Navigation = navigation;
            Report = report;
        }

        public Report Report { get; }

        public INavigation Navigation { get; set; }

        #region Commands

        public abstract Command CommandAccept { get; }

        public abstract Command CommandEdit { get; }

        public abstract Command CommandDelete { get; }

        #endregion
    }
}