using Mobile.Solution.UI.ViewModels;
using Xamarin.Forms;

namespace Mobile.Solution.UI.Views
{
    public partial class VersionInfoPage
    {
        public VersionInfoPage()
        {
            BindingContext = new VersionInfoViewModel();
            InitializeComponent();

        }
    }
}
