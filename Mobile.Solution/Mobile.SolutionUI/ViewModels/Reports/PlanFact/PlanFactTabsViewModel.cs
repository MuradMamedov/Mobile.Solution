using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.UI.Views;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public class PlanFactTabsViewModel : SelectableViewModel
    {
        private static readonly Lazy<PlanFactTabsViewModel> _instance =
            new Lazy<PlanFactTabsViewModel>(() => new PlanFactTabsViewModel());

        private bool _canCreate;

        private Command _commandCreate;
        private SelectableViewModel _selectedItem;

        private PlanFactTabsViewModel()
        {
            TargetPage = new PlanFactTabsView(this);
        }

        public static PlanFactTabsViewModel Instance => _instance.Value;

        public bool IsEmpty
        {
            get => Items.Count == 0;
        }

        public bool CanCreate
        {
            get { return _canCreate; }
            set
            {
                if (_canCreate != value)
                {
                    _canCreate = value;
                    CommandCreate.ChangeCanExecute();
                    RaisePropertyChanged(() => CanCreate);
                }
            }
        }

        public ObservableCollection<PlanFactReportViewModel> Items { get; set; } =
            new ObservableCollection<PlanFactReportViewModel>();

        public SelectableViewModel SelectedItem
        {
            get => _selectedItem;
            set { Set(() => SelectedItem, ref _selectedItem, value); }
        }


        public Command CommandCreate => _commandCreate ?? (_commandCreate =
                                            new Command(async arg => await CommandCreateImplementation(),
                                                arg => CanCreate));


        private void GetReports()
        {
            try
            {
                IsBusy = true;
                Items.Clear();
                var filePreview = DependencyService.Get<IFilePreview>();
                var reports = filePreview.GetReports("pfaf");
                foreach (var report in reports.OrderByDescending(r => r.DateCreated))
                    Items.Add(new PlanFactReportViewModel(this, report));
                CreateTabs();
                RaisePropertyChanged(() => IsEmpty);
            }
            catch
            {
                //ignored
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CreateTabs()
        {
            var tabs = new List<SelectableViewModel>();

            tabs.Add(new PlanFactCollectionViewModel(Items, "Все"));

            var group = Items.GroupBy(item => item.Report.DateCreated.Date.AddDays(1 - item.Report.DateCreated.Day));
            foreach (var g in group)
            {
                tabs.Add(new PlanFactCollectionViewModel(g, $"{g.Key.Date:MMMM} '{g.Key.Date:yy}"));
            }
            Tabs = tabs;
            SelectedItem = Tabs.Count > 1 ? Tabs[1] : null;
        }

        public void Refresh()
        {
            GetReports();
        }

        private async Task CommandCreateImplementation()
        {
            var viewModel = new PlanFactTypeViewModel();
            await Application.Navigation.PushAsync(new PlanFactTypeView(viewModel));
        }
    }
}