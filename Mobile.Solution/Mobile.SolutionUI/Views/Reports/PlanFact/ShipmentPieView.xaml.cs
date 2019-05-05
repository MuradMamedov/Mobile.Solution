using System;
using System.Linq;

namespace Mobile.Solution.UI.Views
{
    public partial class ShipmentPieView
    {
        public ShipmentPieView()
        {
            InitializeComponent();
        }

        void Handle_Tapped(object sender, EventArgs e)
        {
            Legend.IsVisible = !Legend.IsVisible;
            ExpandImg.Rotation = Legend.IsVisible ? 0 : 180;
        }
    }
}