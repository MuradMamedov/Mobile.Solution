using System;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.Infrastructure.Helpers;
using Mobile.Solution.UI.Views;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public class DownloadsViewModel : SelectableViewModel
    {
        private static readonly Lazy<DownloadsViewModel> _instance =
            new Lazy<DownloadsViewModel>(() => new DownloadsViewModel());

        private DownloadsViewModel()
        {
            Header = ResourceContainer.ResourceManager.GetString("Downloads");

            TargetPage = new DownloadsPage();
            var preview = DependencyService.Get<IFilePreview>();
            preview.GetDownloadedFilesList("*");
            Initialize();
        }

        public static DownloadsViewModel Instance => _instance.Value;

        private void Initialize()
        {
            try
            {
                IsBusy = true;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}