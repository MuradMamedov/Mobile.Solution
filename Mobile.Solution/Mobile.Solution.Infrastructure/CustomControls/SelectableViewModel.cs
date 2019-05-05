using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public class SelectableViewModel : ViewModelBase, ISelectable
    {
        private static readonly string IsBusyTextLoadingData = "Загрузка данных...";
        private string _header;

        private bool _isBusy;
        private string _isBusyText = IsBusyTextLoadingData;
        private bool _isEnabled = true;
        private bool _isSelected;
        private List<SelectableViewModel> _tabs = new List<SelectableViewModel>();
        private SelectableViewModel _selectedTab;

        public string Header
        {
            get { return _header; }
            set { Set(() => Header, ref _header, value); }
        }

        public Page TargetPage { get; set; }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    RaisePropertyChanged(() => IsBusy);
                }
            }
        }

        public string IsBusyText
        {
            get { return _isBusyText; }
            set
            {
                if (_isBusyText != value)
                {
                    _isBusyText = value;
                    RaisePropertyChanged(() => IsBusyText);
                }
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set { Set(() => IsEnabled, ref _isEnabled, value); }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set { Set(() => IsSelected, ref _isSelected, value); }
        }

        public SelectableViewModel SelectedTab
        {
            get
            {
                return _selectedTab;
            }

            set
            {
                Set(() => SelectedTab, ref _selectedTab, value);
            }
        }

        public List<SelectableViewModel> Tabs
        {
            get => _tabs;
            protected set
            {
                Set(() => Tabs, ref _tabs, value);
            }
        }
    }
}