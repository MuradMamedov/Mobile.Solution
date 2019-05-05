using System.IO;
using Windows.Storage;
using Mobile.Solution.UI;
using Plugin.DownloadManager;
using Syncfusion.ListView.XForms.UWP;
using Syncfusion.SfChart.XForms.UWP;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Mobile.Solution.UWP
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            SfListViewRenderer.Init();
            new SfChartRenderer();

            InitDownloadManager();

            var application = new Application();
            Infrastructure.CustomControls.Dialog.Init(UI.Application.RootPage);
            LoadApplication(application);
            application.Initialize();
        }


        private void InitDownloadManager()
        {
            CrossDownloadManager.Current.PathNameForDownloadedFile = file =>
            {
                var fileName = file.Headers.ContainsKey("name") ? file.Headers["name"] : Path.GetRandomFileName();
                return Path.Combine(ApplicationData.Current.LocalFolder.Path, fileName);
            };
        }
    }
}