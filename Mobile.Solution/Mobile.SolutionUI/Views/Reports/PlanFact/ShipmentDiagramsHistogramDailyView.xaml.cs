using System;
using Mobile.Solution.Infrastructure.Dependencies;
using Xamarin.Forms;
namespace Mobile.Solution.UI.Views
{
    public partial class ShipmentDiagramsHistogramDailyView
    {
        public ShipmentDiagramsHistogramDailyView()
        {
            InitializeComponent();
        }

        void SizeChangedHandler(object sender, EventArgs e)
        {
            var deviceOrientation = DependencyService.Get<IDeviceOrientation>();
            if (deviceOrientation.GetOrientation() == DeviceOrientations.Landscape)
            {
                View.HeightRequest = 800;
            }
            else
                View.HeightRequest = 500;
        }
    }
}