using System;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.UI.ViewModels;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;

namespace Mobile.Solution.UI.Views
{
    public partial class PlanFactCollectionView
    {
        Point GetAbsolutePosition(View view)
        {
            var x = view.X;
            var y = view.Y;
            var parent = view.Parent as View; while (parent != null)
            {
                x += parent.X + parent.TranslationX;
                y += parent.Y + parent.TranslationY;
                parent = parent.Parent as View;
            }
            return new Point(x, y);
        }

        void Handle_Tapped(object sender, System.EventArgs e)
        {
            if (Device.Idiom != TargetIdiom.Phone)
            {
                var grid = sender as Grid;
                var position = GetAbsolutePosition(grid);
                var popupView = new PlanFactOptionsViewTablet(grid.BindingContext);
                Popup.ShowPopup(popupView, Constraint.Constant(position.X - popupView.WidthRequest + 60), Constraint.Constant(position.Y - 40));
                popupView.GestureRecognizers.Add(new TapGestureRecognizer(s => Popup_Tapped(null, null)));
            }
        }

        void Popup_Tapped(object sender, EventArgs e)
        {
            if(Popup != null)
                Popup.DismissPopup();
        }

        public PlanFactCollectionView()
        {
            InitializeComponent();
            var appInfo = DependencyService.Get<IAppInfo>();
            VersionInfo.IsVisible = !appInfo.HideVersionInfo;
            VersionDescriptionLabel.Text = $"Новое в версии АРМ Аналитика {appInfo.Version}";
            VersionLabel.Text = $"Версия {appInfo.Version}";
            SizeChanged += (sender, e) => Popup_Tapped(null, null);
        }

        private void BindableObject_OnBindingContextChanged(object sender, EventArgs e)
        {
            try
            {
                var planFactCollectionViewModel = BindingContext as PlanFactCollectionViewModel;
                if (planFactCollectionViewModel != null)
                {
                    var sfListView = sender as SfListView;
                    if (sfListView != null)
                        sfListView.HeightRequest =
                            planFactCollectionViewModel.Items.Count *
                            120;
                }
            }
            catch
            {
                // ignored
            }
        }

        async void ShowVersionInfo(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new VersionInfoPage());
            var appInfo = DependencyService.Get<IAppInfo>();
            appInfo.HideVersionInfo = true;
            VersionInfo.IsVisible = !appInfo.HideVersionInfo;
        }
    }
}