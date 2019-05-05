using System.Threading.Tasks;
using Mobile.Solution.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Mobile.Solution.UI
{
    public partial class Application
    {
        public Application()
        {
            InitializeComponent();

            RootPage = new MainPage();

            MainPage = RootPage;
        }

        public static MainPage RootPage { get; private set; }

        public static INavigation Navigation => RootPage.Navigation;

        public async Task Initialize()
        {
            await RootPage.Initialize();
        }

        protected override async void OnResume()
        {
            await RootPage.CheckForUpdates();
            await RootPage.CheckForDictUpdates();
        }
    }
}