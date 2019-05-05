using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.Infrastructure.Helpers;
using Mobile.Solution.Infrastructure.Requests.Reports;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;
using Mobile.Solution.UI.Helpers;
using Newtonsoft.Json;
using Plugin.DownloadManager.Abstractions;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public class ShipmentTabsViewModel : SelectableViewModel
    {
		private DateTime _selectedDate;

		public ShipmentTabsViewModel(List<CargoPlanFact> planItems, List<CargoPlanFact> factItems,
            List<RegCard> regCards, Creator creator, PlanFactParameters parameters,
            PeriodSection section, DateTime? selectedDate = null)
        {
			_selectedDate = selectedDate.HasValue ? selectedDate.Value : _selectedDate;
			PlanItems = planItems;
            FactItems = factItems;
            RegCards = regCards;
            Creator = creator;
            Header = Creator.SelectedItem;
            Parameters = parameters;
            Section = section;
            ReturnIconSource = "return_icon.png";
            CanSave = false;
        }

        public ShipmentTabsViewModel(string name, Creator creator, PlanFactParameters parameters, bool isNew)
        {
            if(DateTime.Now.Day == 1 && DateTime.Now.Hour < 18)
                Section = PeriodSection.Month;
            else
                Section = PeriodSection.CurrentPeriod;
            Parameters = parameters;
            Creator = creator;
            IsNew = isNew;
            _name = name;
            Header = _name;
            ReturnIconSource = "SAVED_icon.png";
            CanSave = true;
        }

        //Переписать функцию.
        public async Task InitializeTabs()
        {
            try
            {
                IsBusy = true;
                DateTime? selectedDate = null;
                var selectedSection = Section;
                var selecteDiagramType = DiagramTypes.HistogramChart;
                if(Tabs.Count > 0)
                {
                    SelectedTab = Tabs.First(t => t.IsSelected);
                    if(SelectedTab is ShipmentDiagramsViewModel)
                    {
                        var selectedDiagramm = ((SelectedTab as ShipmentDiagramsViewModel).SelectedDiagram as SelectableViewModel)
                                               .Tabs.FirstOrDefault(t => t.IsSelected) as ShipmentCollectionViewModel;
                        selectedSection =  selectedDiagramm.PeriodSections.SelectedValue.Key;
                        selecteDiagramType = (SelectedTab as ShipmentDiagramsViewModel).DiagramType.SelectedValue.Key;
                        if (selectedSection == PeriodSection.ArbitraryDate)
                            selectedDate = selectedDiagramm.SelectedDate;
                    }
                    else if (SelectedTab is RegCardCollectionViewModel)
                    {
                        selectedSection = (SelectedTab as RegCardCollectionViewModel).PeriodSections.SelectedValue.Key;
                    }
                }
                await Task.Run(() =>
                {
                    var tabs = new List<SelectableViewModel>();
                    foreach (var tab in Creator.Tabs)
                    {
                        if (tab.GetType() == typeof(RegCardCreator))
                        {
                            if (Parameters.UnitType == UnitTypes.Cars)
                                tabs.Add(new RegCardCollectionViewModel(PlanItems, FactItems, RegCards, tab, Parameters,
                                    tab.Header.Equals(SelectedTab?.Header) ? selectedSection :  Section));
                        }
                        else
                        {
                            var newItem = new ShipmentDiagramsViewModel(this, PlanItems, FactItems, RegCards, tab, Parameters,
                                                tab.Header.Equals(SelectedTab?.Header) ? selectedSection : Section, _selectedDate);
                            newItem.DiagramType.SetValue(selecteDiagramType);
                            tabs.Add(newItem);
                        }
                    }

                    if (SelectedTab != null)
                    {
                        if (SelectedTab is ShipmentDiagramsViewModel)
                        {
                            SelectedTab = tabs.FirstOrDefault(t => t.Header == SelectedTab.Header);
                            if (selectedDate.HasValue)
                            {
                                foreach (var diagram in SelectedTab.Tabs)
                                    foreach (ShipmentCollectionViewModel collection in diagram.Tabs)
                                    {
                                        collection.SelectedDate = selectedDate.Value;
                                        foreach (ShipmentViewModel shipment in collection.Tabs)
                                            shipment.InitDateDiagram(selectedDate.Value);
                                    }
                            }
                        }
                        else
                            SelectedTab = tabs.FirstOrDefault(t => t.Header == SelectedTab.Header);
                    }

                    Tabs = tabs;
                    RaisePropertyChanged(() => Tabs);
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        #region Properties

        public string ReturnIconSource { get; }

        public string Name
        {
            get => _name ?? (_name =
                       $"Выполнение плана погрузки {(Parameters.PeriodType == PeriodTypes.Month ? $"{Parameters.Date:MMMM yyyy}" : $"{Parameters.DateFrom:dd MMMM yyyy} - {Parameters.DateTo:dd MMMM yyyy}")}"
                   );
            set
            {
                var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                var r = new Regex($"[{Regex.Escape(regexSearch)}]");
                value = r.Replace(value, "");
                if (Set(() => Name, ref _name, value))
                {
                    CanSave = _name?.Length > 0;
                    CommandSave.ChangeCanExecute();
                }
                RaisePropertyChanged(() => Name);
            }
        }

        private string _name;

        public bool CanSave { get; set; }

        public bool IsNew { get; set; }

        public bool IsFileEnabled
        {
            get => _isFileEnabled;
            set
            {
                Set(() => IsFileEnabled, ref _isFileEnabled, value);
            }
        }

        private bool _isFileEnabled = true;

        public PlanFactParameters Parameters { get; set; }

        public List<CargoPlanFact> PlanItems { get; private set; }

        public List<CargoPlanFact> FactItems { get; private set; }

        public List<RegCard> RegCards { get; private set; }

        public Creator Creator { get; set; }

        public PeriodSection Section { get; internal set; }

        #endregion

        #region Commands

        public Command CommandRefresh
        {
            get
            {
                return _commandRefresh ?? (_commandRefresh = new Command(async () => await RetrieveData(),
                           () => !IsBusy));
            }
        }

        private Command _commandRefresh;

        public async Task RetrieveData()
        {
            try
            {
                IsBusy = true;
                PlanItems = await PlanFactReport.Instance.InitPlanRequest(Parameters);
                FactItems = await PlanFactReport.Instance.InitFactRequest(Parameters);
                RegCards = await RegCardReport.Instance.InitRequest(Parameters);
                await InitializeTabs();
            }
            catch (Exception ex)
            {
                Dialog.Instance.Alert(ex.Message, ResourceContainer.ResourceManager.GetString("DialogErrorTitle"));
            }
            finally
            {
                IsBusy = false;
                CommandRefresh.ChangeCanExecute();
            }
        }

        public Command CommandGenerateFile
        {
            get
            {
                return _commandGenerateFile ?? (_commandGenerateFile = new Command(async () => await RetrieveFile(),
                           () => IsFileEnabled));
            }
        }

        private Command _commandGenerateFile;

        private async Task RetrieveFile()
        {
            string fileName = null;
            var appInfo = DependencyService.Get<IAppInfo>();
            try
            {
                IsFileEnabled = false;

                fileName =
                    await PlanFactReport.Instance.GenerateReportFile(Parameters);

                var downloader =
                    PlanFactReport.Instance.GetFile(
                        $"uniqueId={appInfo.UniqueId}&fileName={fileName.Replace("\"", "")}", string.Format(
                            $"Выполнение плана погрузки - {Parameters.Date:dd-MM-yyyy}"));

                downloader.File.PropertyChanged += File_PropertyChanged;
                downloader.StartDownloading(true);
            }
            catch
            {
                if (fileName == null)
                    Dialog.Instance.Alert("Произошла ошибка на стороне сервера. Файл не получен.",
                        ResourceContainer.ResourceManager.GetString("DialogErrorTitle"));
                IsFileEnabled = true;
            }
        }

        private void File_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var file = (IDownloadFile) sender;
            if (Device.RuntimePlatform == Device.Windows || Device.RuntimePlatform == Device.WinPhone)
            {
                if ((int) file.TotalBytesExpected == (int) file.TotalBytesWritten)
                {
                    file.PropertyChanged -= File_PropertyChanged;
                    OpenFile(file);
                }
            }
            else if (e.PropertyName == "Status")
            {
                switch (file.Status)
                {
                    case DownloadFileStatus.COMPLETED:
                        OpenFile(file);
                        break;
                    case DownloadFileStatus.FAILED:
                    case DownloadFileStatus.CANCELED:
                        IsFileEnabled = true;
                        break;
                }
            }
        }

        private void OpenFile(IDownloadFile file)
        {
            var filePreview = DependencyService.Get<IFilePreview>();
            if (file.Headers.ContainsKey("name"))
                filePreview.OpenFileNotification(file.Headers["name"], "Загрузка завершена",
                    $"Открыть файл {file.Headers["name"]}",
                    @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            IsFileEnabled = true;
        }

        public Command CommandSave
        {
            get
            {
                return _commandSave ?? (_commandSave = new Command(async () => await CommandSaveImplementation(),
                           () => CanSave));
            }
        }

        private Command _commandSave;

        private async Task CommandSaveImplementation()
        {
            var parametersString = JsonConvert.SerializeObject(Parameters);
            var filePreview = DependencyService.Get<IFilePreview>();
            if (IsNew && filePreview.Find($"{Name}.{PlanFactParameters.Extension}"))
                if (!await Dialog.Instance.ConfirmAsync("Отчет с таким именем уже существует.", okText: "Заменить",
                    cancelText: "Отмена"))
                    return;
            filePreview.SaveStringToFile($"{Name}.{PlanFactParameters.Extension}", parametersString);
            PlanFactTabsViewModel.Instance.Refresh();
            IsNew = false;
        }

        #endregion
    }
}