using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.Infrastructure.Helpers;
using Mobile.Solution.Infrastructure.Requests;
using Mobile.Solution.Infrastructure.Requests.NSI;
using Mobile.Solution.UI.Views;
using Newtonsoft.Json;
using Syncfusion.Data.Extensions;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigation navigation)
        {
            Navigation = navigation;
        }

        public INavigation Navigation { get; }

        public async Task SendPreviousErrors()
        {
            var filePreview = DependencyService.Get<IFilePreview>();
            if (filePreview.Find(ExceptionFileWriter.FileName))
            {
                var errorMessage = filePreview.ReadStringFromFile(ExceptionFileWriter.FileName);
                var appInfo = DependencyService.Get<IAppInfo>();
                await ServerManager.SendError(appInfo.UniqueId, errorMessage);
                filePreview.Delete(ExceptionFileWriter.FileName);
            }
        }

        public async Task<bool> CheckForDictUpdates()
        {
            var updated = false;
            var appInfo = DependencyService.Get<IAppInfo>();
            var filePreview = DependencyService.Get<IFilePreview>();
            var updatedDict = new Dictionary<string, string>();
            foreach (var dict in appInfo.Dictionaries)
            {
                var update = await ServerManager.CheckForDictUpdates(appInfo.UniqueId, dict.Key, dict.Value);
                if (!string.IsNullOrEmpty(update.Item1) && !string.IsNullOrEmpty(update.Item2))
                {
                    updatedDict.Add(dict.Key, update.Item1);
                    filePreview.SaveStringToFile(dict.Key, update.Item2);
                    updated = true;
                }
                while (!filePreview.Find(dict.Key) || string.IsNullOrEmpty(filePreview.ReadStringFromFile(dict.Key)))
                {
                    update = await ServerManager.CheckForDictUpdates(appInfo.UniqueId, dict.Key, "Reload");
                    if (!string.IsNullOrEmpty(update.Item1) && !string.IsNullOrEmpty(update.Item2))
                        filePreview.SaveStringToFile(dict.Key, update.Item2);
                    updated = true;
                }
            }
            appInfo.Dictionaries = updatedDict;
            return updated;
        }

        public async Task<bool> LoadDictionaries()
        {
            bool loaded;
            var appInfo = DependencyService.Get<IAppInfo>();
            var filePreview = DependencyService.Get<IFilePreview>();
            try
            {
                PlanFactTabsViewModel.Instance.CanCreate = false;
                loaded = await Task.Run(() =>
                {
                    foreach (var dict in appInfo.Dictionaries)
                    {
                        if (filePreview.Find(dict.Key))
                        {
                            using (var sr = filePreview.GetFileStream(dict.Key))
                            {
                                using (var reader = new JsonTextReader(sr))
                                {
                                    var serializer = new JsonSerializer();
                                    var prop = typeof(Dictionaries).GetProperties()
                                        .FirstOrDefault(p => p.Name == dict.Key);
                                    var list = serializer.Deserialize(reader, prop.PropertyType);
                                    if (list == null)
                                        return false;

                                    prop.SetValue(Dictionaries.Instance, list);
                                }
                            }
                        }
                    }
                    return true;
                });
            }
            catch
            {
                loaded = false;
            }
            finally
            {
                PlanFactTabsViewModel.Instance.CanCreate = true;
            }
            if (!loaded)
                foreach (var dict in appInfo.Dictionaries)
                    filePreview.Delete(dict.Key);
            return loaded;
        }


        public async Task CheckForUpdates()
        {
#if DEBUG

#else
            var appInfo = DependencyService.Get<IAppInfo>();
            var update = await ServerManager.CheckForUpdates(appInfo.Version);
            if (update.Item1)
            {
                var previewer = DependencyService.Get<IUrlPreview>();
                if (await Dialog.Instance.ConfirmAsync("Доступна новая версия программы", "Обновление", "Обновить",
                    "Отмена"))
                {
                    previewer.OpenUrl(update.Item2);
                    appInfo.HideVersionInfo = false;
                }
            }
#endif
        }

        public async Task CheckRegistration()
        {
#if DEBUG

#else
            var appInfo = DependencyService.Get<IAppInfo>();
            if (!appInfo.IsRegistered)
                await Navigation.PushAsync(new RegistrationPhoneView(new RegistrationViewModel()));
#endif
        }
    }
}