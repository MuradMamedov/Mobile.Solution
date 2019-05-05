using System;
using Windows.System;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.UWP.Dependencies;
using Xamarin.Forms;

[assembly: Dependency(typeof(UrlPreview))]

namespace Mobile.Solution.UWP.Dependencies
{
    public class UrlPreview : IUrlPreview
    {
        public async void OpenUrl(string url)
        {
            await Launcher.LaunchUriAsync(new Uri(url));
        }
    }
}