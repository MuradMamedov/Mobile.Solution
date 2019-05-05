using System;
using Mobile.Solution.Droid.Dependencies;
using Mobile.Solution.Infrastructure.Dependencies;
using Xamarin.Forms;

[assembly: Dependency(typeof(UrlPreview))]

namespace Mobile.Solution.Droid.Dependencies
{
    public class UrlPreview : IUrlPreview
    {
        public void OpenUrl(string url)
        {
            Device.OpenUri(new Uri(url));
        }
    }
}