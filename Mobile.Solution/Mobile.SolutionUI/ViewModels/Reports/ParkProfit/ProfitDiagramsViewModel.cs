using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Requests.NSI.Contracts;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;
using Syncfusion.SfChart.XForms;
using Xamarin.Forms;

namespace Mobile.Solution.UI.ViewModels
{
    public class ProfitDiagramsRevenueNetWoViewModel : SelectableViewModel
    {
        private ObservableCollection<ChartDataPoint> _regression = new ObservableCollection<ChartDataPoint>();

        private ObservableCollection<ChartDataPoint> _revenueNetWoEmpty = new ObservableCollection<ChartDataPoint>();

        public ObservableCollection<ChartDataPoint> RevenueNetWoEmpty
        {
            get => _revenueNetWoEmpty;
            set { Set(() => RevenueNetWoEmpty, ref _revenueNetWoEmpty, value); }
        }

        public ObservableCollection<ChartDataPoint> Regression
        {
            get => _regression;
            set { Set(() => Regression, ref _regression, value); }
        }

        public ImageSource ImageSource => ImageSource.FromResource("Mobile.Solution.UI.Images.example1.png");
    }

    public class ProfitDiagramsEmptyInLoadShare : SelectableViewModel
    {
        private ObservableCollection<ChartDataPoint> _emptyInLoadShare = new ObservableCollection<ChartDataPoint>();

        public ObservableCollection<ChartDataPoint> EmptyInLoadShare
        {
            get => _emptyInLoadShare;
            set { Set(() => EmptyInLoadShare, ref _emptyInLoadShare, value); }
        }

        public ImageSource ImageSource => ImageSource.FromResource("Mobile.Solution.UI.Images.example2.png");
    }

    public class ProfitDiagramsInternalExternalViewModel : SelectableViewModel
    {
        private ObservableCollection<ChartDataPoint> _externalCarriages = new ObservableCollection<ChartDataPoint>();

        private ObservableCollection<ChartDataPoint> _internalCarriages = new ObservableCollection<ChartDataPoint>();

        public ObservableCollection<ChartDataPoint> InternalCarriages
        {
            get => _internalCarriages;
            set { Set(() => InternalCarriages, ref _internalCarriages, value); }
        }

        public ObservableCollection<ChartDataPoint> ExternalCarriages
        {
            get => _externalCarriages;
            set { Set(() => ExternalCarriages, ref _externalCarriages, value); }
        }

        public ImageSource ImageSource => ImageSource.FromResource("Mobile.Solution.UI.Images.example3.png");
    }

    public class ProfitDiagramsViewModel : ParkProfitSelectableViewModel
    {
        private readonly ProfitDiagramsEmptyInLoadShare _emptyInLoadShare;
        private readonly ProfitDiagramsInternalExternalViewModel _internalExternalViewModel;

        private readonly ProfitDiagramsRevenueNetWoViewModel _revenueNetWo;

        private List<SelectableViewModel> _items = new List<SelectableViewModel>();

        public ProfitDiagramsViewModel()
        {
            _revenueNetWo = new ProfitDiagramsRevenueNetWoViewModel {Header = "Пример текста 1"};
            _emptyInLoadShare = new ProfitDiagramsEmptyInLoadShare {Header = "Пример текста 2"};
            _internalExternalViewModel = new ProfitDiagramsInternalExternalViewModel {Header = "Пример текста 3"};
            Header = "Доходность";

            Items.Add(_revenueNetWo);
            Items.Add(_emptyInLoadShare);
            Items.Add(_internalExternalViewModel);
        }

        public List<SelectableViewModel> Items
        {
            get => _items;
            set { Set(() => Items, ref _items, value); }
        }

        internal override void SetData(List<ParkProfitLaden> ladenItems, List<ParkProfitEmpty> emptyItems,
            List<InfoCarInAgreement> cars, DateTime dateFrom, DateTime dateTo)
        {
            base.SetData(ladenItems, emptyItems, cars, dateFrom, dateTo);
            _revenueNetWo.RevenueNetWoEmpty.Clear();
            _revenueNetWo.Regression.Clear();
            _emptyInLoadShare.EmptyInLoadShare.Clear();
            _internalExternalViewModel.InternalCarriages.Clear();
            _internalExternalViewModel.ExternalCarriages.Clear();

            var xVals = new List<double>();
            var yVals = new List<double>();

            if (LadenItems.Count > 0)
            {
                var carCount = Cars.Count;
                if (carCount > 0)
                {
                    var currentDate = DateFrom;
                    while (currentDate <= DateTo)
                    {
                        var internCarriage =
                            LadenItems.Where(
                                    li =>
                                        li.DateAccountByDeparture.Date.Equals(currentDate.Date) &&
                                        li.TaxationType == 1)
                                .Sum(li => li.ProfitWithVat);
                        var extrnCarriage =
                            LadenItems.Where(
                                    li =>
                                        li.DateAccountByDeparture.Date.Equals(currentDate.Date) &&
                                        li.TaxationType != 1)
                                .Sum(li => li.ProfitWithVat);
                        var emptyCarriage =
                            EmptyItems.Where(ei => ei.DateReady.Date.Equals(currentDate.Date)).Sum(li => li.Cost);

                        var revenue = (internCarriage + extrnCarriage * (decimal) 1.18 - emptyCarriage) / carCount;
                        _revenueNetWo.RevenueNetWoEmpty.Add(new ChartDataPoint(currentDate, (int) revenue));

                        var share = internCarriage + extrnCarriage * (decimal) 1.18 > 0
                            ? emptyCarriage / (internCarriage + extrnCarriage * (decimal) 1.18)
                            : 0;
                        _emptyInLoadShare.EmptyInLoadShare.Add(new ChartDataPoint(currentDate, (int) share));

                        _internalExternalViewModel.InternalCarriages.Add(new ChartDataPoint(currentDate,
                            (double) internCarriage));

                        _internalExternalViewModel.ExternalCarriages.Add(new ChartDataPoint(currentDate,
                            (double) extrnCarriage));

                        yVals.Add((double) revenue);
                        xVals.Add((currentDate - DateFrom).TotalDays);
                        currentDate = currentDate.AddDays(1);
                    }
                    double slope;
                    double rsquared;
                    double yintercept;
                    LinearRegression(xVals.ToArray(), yVals.ToArray(), 0, xVals.Count, out rsquared, out yintercept,
                        out slope);
                    currentDate = DateFrom;
                    while (currentDate <= DateTo)
                    {
                        _revenueNetWo.Regression.Add(new ChartDataPoint(currentDate,
                            (currentDate - DateFrom).TotalDays * slope + yintercept));
                        currentDate = currentDate.AddDays(1);
                    }
                }
            }
        }

        //http://mathworld.wolfram.com/LeastSquaresFitting.html
        private void LinearRegression(double[] xVals, double[] yVals,
            int inclusiveStart, int exclusiveEnd,
            out double rsquared, out double yintercept,
            out double slope)
        {
            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double ssX;
            double sumCodeviates = 0;
            double sCo;
            double count = exclusiveEnd - inclusiveStart;

            for (var ctr = inclusiveStart; ctr < exclusiveEnd; ctr++)
            {
                var x = xVals[ctr];
                var y = yVals[ctr];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }
            ssX = sumOfXSq - sumOfX * sumOfX / count;
            var rNumerator = count * sumCodeviates - sumOfX * sumOfY;
            var rDenom = (count * sumOfXSq - sumOfX * sumOfX)
                         * (count * sumOfYSq - sumOfY * sumOfY);
            sCo = sumCodeviates - sumOfX * sumOfY / count;

            var meanX = sumOfX / count;
            var meanY = sumOfY / count;
            var dblR = rNumerator / Math.Sqrt(rDenom);
            rsquared = dblR * dblR;
            yintercept = meanY - sCo / ssX * meanX;
            slope = sCo / ssX;
        }
    }
}