using System;
using System.Linq;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Helpers;
using Mobile.Solution.Infrastructure.Requests.NSI.Contracts;
using Mobile.Solution.Infrastructure.Requests.Reports;
using Mobile.Solution.UI.Helpers;
using Mobile.Solution.UI.ViewModels;
using Syncfusion.SfChart.XForms;

namespace Mobile.Solution.UI.Views
{
    public partial class ShipmentHistogramView
    {
        public ShipmentHistogramView()
        {
            InitializeComponent();
        }

        private async void SfChart_SelectionChanging(object sender, ChartSelectionChangingEventArgs e)
        {
            try
            {
                var viewModel = BindingContext as ShipmentViewModel;
                var item = (sender as SfChart).BindingContext as ShipmentElementViewModel;
                await ShowDetails(e, viewModel, item);
            }
            catch (Exception ex)
            {
                Dialog.Instance.Alert(ex.Message, ResourceContainer.ResourceManager.GetString("DialogErrorTitle"));
            }
        }

        private async Task ShowDetails(ChartSelectionEventArgs e, ShipmentViewModel viewModel,
            ShipmentElementViewModel item)
        {
            var selectedItem = item.Requirements.ElementAt(e.SelectedDataPointIndex > -1
                ? e.SelectedDataPointIndex
                : e.PreviousSelectedIndex);
            var xValue = selectedItem.FullValue;

            if (viewModel.Creator.Tabs.Count > 1)
            {
                InfoOrgPassport recip = null;
                InfoOrgPassport sender = null;
                InfoOrgPassport payer = null;
                InfoOrgPassport owner = null;
                InfoSumFreight freight = null;
                InfoUniqueStation toStation = null;
                InfoUniqueStation fromStation = null;

                if (viewModel.Creator is ContractorCreator)
                {
                    switch (viewModel.Creator.Direction)
                    {
                        case ReportDirections.Arrival:
                            recip = new InfoOrgPassport {Id = selectedItem.ExampleElement.ParamRecipID};
                            break;
                        case ReportDirections.Departure:
                            sender = new InfoOrgPassport {Id = selectedItem.ExampleElement.ParamSenderID};
                            break;
                    }
                }

                if (viewModel.Creator is PayerCreator)
                {
                    payer = new InfoOrgPassport {Id = selectedItem.ExampleElement.PayerCode};
                }

                if (viewModel.Creator is OwnerCreator)
                {
                    owner = new InfoOrgPassport {Id = selectedItem.ExampleElement.ParamCarOwnerId};
                }

                if (viewModel.Creator is OwnerCreator)
                {
                    freight = new InfoSumFreight {Number = selectedItem.ExampleElement.ParamFreightGroup};
                }

                if (viewModel.Creator is StationCreator)
                {
                    switch (viewModel.Creator.Direction)
                    {
                        case ReportDirections.Arrival:
                            toStation = new InfoUniqueStation {StCode = selectedItem.ExampleElement.ToStationCode};
                            break;
                        case ReportDirections.Departure:
                            fromStation = new InfoUniqueStation {StCode = selectedItem.ExampleElement.FromStationCode};
                            break;
                    }
                }

                var creator = viewModel.Creator.Clone();
                creator.SelectedItem = xValue;

                var tabsViewModel = new ShipmentTabsViewModel(
                    viewModel.PlanItems,
                    viewModel.FactItems,
                    viewModel.RegCards,
                    creator,
                    new PlanFactParameters(viewModel.Parameters)
                    {
                        Recip = recip ?? viewModel.Parameters.Recip,
                        Sender = sender ?? viewModel.Parameters.Sender,
                        Payer = payer ?? viewModel.Parameters.Payer,
                        Owner = owner ?? viewModel.Parameters.Owner,
                        FreightGroup = freight ?? viewModel.Parameters.FreightGroup,
                        ToStation = toStation ?? viewModel.Parameters.ToStation,
                        FromStation = fromStation ?? viewModel.Parameters.FromStation
                    },
                    viewModel.Section,
                    viewModel.SelectedDate);
                await Application.Navigation.PushAsync(new ShipmentTabsView(tabsViewModel));
                await tabsViewModel.InitializeTabs();
            }
        }
    }
}