using Mobile.Solution.iOS;
using Mobile.Solution.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ReportListViewCell), typeof(ReportListViewCellRenderer))]

namespace Mobile.Solution.iOS
{
	public class ReportListViewCellRenderer : ViewCellRenderer
	{
		public override UIKit.UITableViewCell GetCell(Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);
			cell.SelectionStyle = UIKit.UITableViewCellSelectionStyle.None;
			return cell;
		}
	}
}
