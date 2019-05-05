using System.IO;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.DownloadManager;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Application = Mobile.Solution.UI.Application;

namespace Mobile.Solution.Droid
{
    [Activity(Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            InitDownloadManager();

            Forms.Init(this, bundle);

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
                return Path.Combine(ApplicationContext.GetExternalFilesDir(Environment.DirectoryDownloads).AbsolutePath,
                    fileName);
            };
        }

        public override void OnBackPressed()
        {
            var np = Xamarin.Forms.Application.Current.MainPage as NavigationPage;
            if (np != null)
            {
                var md = np.CurrentPage as MasterDetailPage;
                if (md != null && !md.IsPresented &&
                    (
                        !(md.Detail is NavigationPage) ||
                        ((NavigationPage) md.Detail).Navigation.NavigationStack.Count == 1 &&
                        ((NavigationPage) md.Detail).Navigation.ModalStack.Count == 0
                    ))
                    MoveTaskToBack(true);
                else
                    base.OnBackPressed();
            }
        }
    }
}