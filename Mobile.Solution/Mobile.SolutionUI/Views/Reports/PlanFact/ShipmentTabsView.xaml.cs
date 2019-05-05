using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.CustomControls;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.UI.Helpers;
using Mobile.Solution.UI.ViewModels;
using Rg.Plugins.Popup.Services;
using Syncfusion.Data.Extensions;
using Xamarin.Forms;

namespace Mobile.Solution.UI.Views
{
	public class BreadCrubmViewModel : SelectableViewModel
	{
        public Page Page { get; private set; }
        public bool IsImageVisible { get; set; } = true;
		public BreadCrubmViewModel(Page page)
		{
			Header = page.Title;
            Page = page;
		}
	}

    public partial class ShipmentTabsView
    {
        private readonly ShipmentTabsViewModel _viewModel;

        public ShipmentTabsView(ShipmentTabsViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = _viewModel = viewModel;

            if (_viewModel.Creator.GetType() != typeof(MainCreator))
            {
                var existingPages = Application.Navigation.NavigationStack.ToArray();
                var bcVm = new List<BreadCrubmViewModel>();
                for (int i = 0; i < existingPages.Length; i++)
                {
                    if(existingPages[i].GetType() == typeof(ShipmentTabsView))
                        bcVm.Add(new BreadCrubmViewModel(existingPages[i]));
                }
                bcVm.Add(new BreadCrubmViewModel(this));
				bcVm.First().IsImageVisible = false;
				BreadCrumbs.ItemsSource = bcVm;
                BreadCrumbs.SelectedItem = bcVm.Last();
                BreadCrumbs.IsVisible = bcVm.Count > 0;
                BreadCrumbs.SelectedItemChanged += async (s, e) =>
                {
                    existingPages = Application.Navigation.NavigationStack.ToArray();
                    for (int i = existingPages.Length - 1; i > -1; i--)
                    {
                        if ((BreadCrumbs.SelectedItem as BreadCrubmViewModel).Page == existingPages[i])
                            break;
                        await Application.Navigation.PopAsync(false);
                    }
                };
				SizeChanged +=(s,e)=>
                    BreadCrumbs.ScrollToAsync(BreadCrumbs.GetElementAt(BreadCrumbs.ItemsSource.Count - 1), ScrollToPosition.End, true);
            }
		}

        private void Handle_Capture(object sender, EventArgs e)
        {
            try
            {
                var filePreview = DependencyService.Get<IFilePreview>();
                var scrollView = (TabbedView.GetCurrentView().Content as ContentView)?.Content as ScrollView;
                if (scrollView?.Content is ContentView)
                {
                    filePreview.SaveImage(scrollView.Content as ContentView);
                }
                else
                {
                    var grid = scrollView?.Content;
                    ContentView view = null;
                    if (grid != null)
                    {
                        var children = grid.GetType()
                            .GetProperties()
                            .FirstOrDefault(p => p.Name == "Children")
                            ?.GetValue(grid) as IEnumerable;
                        if (children != null)
                            foreach (var gridChild in children)
                            {
                                view = gridChild as ContentView;
                                if (view != null)
                                    break;
                            }
                        if (view != null)
                            filePreview.SaveImage(view);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        private async void Handle_SaveClicked(object sender, EventArgs e)
        {
            var page = new PlanFactSaveView(BindingContext);
            await PopupNavigation.PushAsync(page);
        }

        private async void Return_Clicked(object sender, EventArgs e)
        {
            try
            {
                var existingPages = Application.Navigation.NavigationStack.ToList();
                if (_viewModel.Creator.GetType() == typeof(MainCreator))
                {
                    await CheckSave();

                    for (var i = existingPages.Count - 1; i > 0; i--)
                        await Application.Navigation.PopAsync(false);
                }
                else
                {
                    for (var i = existingPages.Count - 1; i > 1; i--)
                    {
                        await Application.Navigation.PopAsync(false);
                        var page = Application.Navigation.NavigationStack[i - 1];
                        if ((page.BindingContext as ShipmentTabsViewModel)?.Creator.GetType() == typeof(MainCreator))
                            return;
                    }
                }
            }
            catch
            {
                //ignored
            }
        }

        public async Task CheckSave()
        {
            if (_viewModel.IsNew)
                if (await Dialog.Instance.ConfirmAsync("Сохранить?", okText: "Ок", cancelText: "Отмена"))
		            await PopupNavigation.PushAsync(new PlanFactSaveView(BindingContext));
		}
    }
}