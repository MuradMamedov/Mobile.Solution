using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mobile.Solution.Infrastructure.CustomControls;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
	public abstract class ReportTabsViewModel : SelectableViewModel
	{
		public List<ReportCollectionViewModel> Tabs
		{
			get => _tabs;

			private set { Set(() => Tabs, ref _tabs, value); }
		}

		private List<ReportCollectionViewModel> _tabs;

		public ObservableCollection<ReportViewModel> Items { get; set; } =
			new ObservableCollection<ReportViewModel>();

        #region Commands

        public abstract Command CommandCreate { get; }

        public abstract Command CommandRefresh { get; }
									
		#endregion
	}
}
