using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.Infrastructure.Requests;
using Mobile.Solution.UI.Views;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public class RegistrationViewModel : SelectableViewModel
    {
        private string _code = "";
        private Command _commandGet;

        private Command _commandSend;

        private string _number = "";


        public string Number
        {
            get => _number;

            set
            {
                _number = value;
                CommandGet.ChangeCanExecute();
                RaisePropertyChanged(() => IsValidated);
            }
        }

        public bool IsValidated => !IsBusy && Number.Length == 15;

        public bool IsCodeValidated => !IsBusy && GetNumber(Code) > 99999;

        public string Code
        {
            get => _code;

            set
            {
                _code = value;
                CommandSend.ChangeCanExecute();
                RaisePropertyChanged(() => IsCodeValidated);
            }
        }

        public Command CommandGet
        {
            get
            {
                return _commandGet ?? (_commandGet = new Command(async p => await CommandGetImplementation(),
                           arg => IsValidated));
            }
        }

        public Command CommandSend
        {
            get
            {
                return _commandSend ?? (_commandSend = new Command(async p => await CommandSendImplementation(),
                           arg => IsCodeValidated));
            }
        }

        private async Task CommandGetImplementation()
        {
            IsBusy = true;
            try
            {
                var appInfo = DependencyService.Get<IAppInfo>();
                if (!await ServerManager.RegisterPhone(appInfo.UniqueId, GetNumber(Number)))
                {
                    Dialog.Instance.Alert("Ошибка при попытке регистрации.");
                }
                else
                {
                    await Application.Navigation.PushAsync(new RegistrationCodeView(this));
                    Code = null;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CommandSendImplementation()
        {
            IsBusy = true;
            try
            {
                var appInfo = DependencyService.Get<IAppInfo>();
                if (!await ServerManager.CheckCode(appInfo.UniqueId, (int) GetNumber(Code)))
                {
                    Dialog.Instance.Alert("Ошибка при попытке регистрации.");
                }
                else
                {
                    appInfo.IsRegistered = true;
                    appInfo.PhoneNumber = Number;
                    Application.Navigation.RemovePage(Application.Navigation.NavigationStack[Application.Navigation.NavigationStack.Count - 2]);
                    await Application.Navigation.PopAsync(false);
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                IsBusy = false;
            }
        }


        public long GetNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;
            long number = 0;
            long multiply = 1;
            for (var i = input.Length - 1; i >= 0; i--)
                if (char.IsDigit(input[i]))
                {
                    number += (input[i] - '0') * multiply;
                    multiply *= 10;
                }
            return number;
        }
    }
}