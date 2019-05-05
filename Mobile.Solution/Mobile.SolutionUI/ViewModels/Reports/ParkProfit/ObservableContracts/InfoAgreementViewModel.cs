using GalaSoft.MvvmLight;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Requests.NSI.Contracts;

namespace Mobile.Solution.UI.ViewModels
{
    public class InfoAgreementViewModel : ObservableObject, IPopupListViewItem
    {
        public InfoAgreementViewModel(InfoAgreement model)
        {
            Model = model;
        }

        #region Properties

        public InfoAgreement Model { get; }

        public bool IsChecked
        {
            get => _isChecked;

            set { Set(() => IsChecked, ref _isChecked, value); }
        }

        private bool _isChecked;

        public string DisplayName => Model.DisplayName;

        public int Id => Model.Id;

        public string Name => Model.Name;

        public string Comment => Model.Comment;

        #endregion Properties
    }
}