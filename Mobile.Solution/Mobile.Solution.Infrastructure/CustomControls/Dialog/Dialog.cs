using System;
using System.Threading;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public class Dialog
    {
        private static Dialog _instance;

        private Dialog()
        {
        }

        public NavigationPage MainPage { get; private set; }

        public static Dialog Instance => _instance;

        public static void Init(NavigationPage mainPage)
        {
            _instance = new Dialog {MainPage = mainPage};
        }

        public async Task<bool> ConfirmAsync(string message, string title = null, string okText = "Ок",
            string cancelText = "Отмена")
        {
            return await MainPage.DisplayAlert(title, message, okText, cancelText);
        }

        public async void Alert(string message, string title = null, string cancel = "Ок")
        {
            await MainPage.DisplayAlert(title, message, "Ок");
        }

		public async Task<DateTime> PromptDateAsync(DateTime? selectedDate = null, DateTime? minDate = null, DateTime? maxDate = null, string okText = "Ок")
		{
			var token = new CancellationTokenSource();

            var page = new PopupDatePicker(selectedDate, minDate, maxDate, okText);
            await PopupNavigation.PushAsync(page);
            page.Disappearing += (sender, e) => 
            {
                token.Cancel();
            };
            await Task.Run(async () =>
            {
                for (;!token.IsCancellationRequested;)
                    await Task.Delay(200).ConfigureAwait(false);
            }, token.Token).ConfigureAwait(false);
            return page.SelectedDate;
		}
    }
}