using System;
using System.Linq;

namespace Mobile.Solution.UI.Views
{
    public partial class RegCardsView
    {
        public RegCardsView()
        {
            InitializeComponent();
        }

        void Handle_Tapped(object sender, EventArgs e)
        {
            Legend.IsVisible = !Legend.IsVisible;

            ExpandImg.IsVisible = !Legend.IsVisible;
            RespLabel.IsVisible = Legend.IsVisible;
        }

        void Handle_LayoutChanged(object sender, EventArgs e)
        {
            Legend.HeightRequest = Legend.IsVisible ? Legend.Children.Sum(s => s.Height) : 0;
        }
    }
}