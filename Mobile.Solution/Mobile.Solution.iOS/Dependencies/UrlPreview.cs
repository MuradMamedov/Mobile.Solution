using Foundation;
using Mobile.Solution.iOS.Dependencies;
using Mobile.Solution.Infrastructure.Dependencies;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(UrlPreview))]

namespace Mobile.Solution.iOS.Dependencies
{
    public class UrlPreview : IUrlPreview
    {
        public void OpenUrl(string url)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
        }
    }
}