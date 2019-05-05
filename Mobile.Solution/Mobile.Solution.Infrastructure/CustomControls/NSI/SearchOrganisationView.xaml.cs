using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public partial class SearchOrganisationView
    {
        public SearchOrganisationView()
        {
            InitializeComponent();
        }

        async void Handle_Focused(object sender, FocusEventArgs e)
        {
            await Navigation.PushAsync(new SearchOrganisationPage(BindingContext));
        }
    }
}