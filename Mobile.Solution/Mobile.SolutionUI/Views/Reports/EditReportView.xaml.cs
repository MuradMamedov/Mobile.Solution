using System;
using System.Collections.Generic;
using Mobile.Solution.UI.ViewModels;
using Xamarin.Forms;

namespace Mobile.Solution.UI
{
	public partial class EditReportView : ContentPage
	{
		public EditReportView(PlanFactViewModel viewModel)
		{
			InitializeComponent();

			BindingContext = viewModel;
		}
	}
}
