using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public class PopupListViewModel<T> : ViewModelBase
        where T : IPopupListViewItem
    {
        public PopupListViewModel(IEnumerable<T> items)
        {
            Items = new List<T>(items);
        }

        #region Properties

        public List<T> Items
        {
            get => _items;
            set { Set(() => Items, ref _items, value); }
        }

        private List<T> _items;

        public T SelectedItem
        {
            get => _selectedItem;
            set { Set(() => SelectedItem, ref _selectedItem, value); }
        }

        private T _selectedItem;

        public INavigation Navigation { get; set; }

        #endregion Properties

        #region Commands

        public Command CommandApply

        {
            get { return _commandApply ?? (_commandApply = new Command(async () => { await ApplyCommandImplementation(); })); }
        }

        private Command _commandApply;

        private async Task ApplyCommandImplementation()
        {
            await Navigation.PopModalAsync();
        }

        #endregion Commands
    }
}