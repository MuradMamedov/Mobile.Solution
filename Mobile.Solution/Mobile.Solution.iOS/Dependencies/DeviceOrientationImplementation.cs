using UIKit;
using Mobile.Solution.Infrastructure.Dependencies;
using Xamarin.Forms;
using Mobile.Solution.iOS.Dependencies;

[assembly: Dependency(typeof(DeviceOrientationImplementation))]
namespace Mobile.Solution.iOS.Dependencies
{
    public class DeviceOrientationImplementation : IDeviceOrientation
    {
        public DeviceOrientations GetOrientation()
        {
            var currentOrientation = UIApplication.SharedApplication.StatusBarOrientation;
            bool isPortrait = currentOrientation == UIInterfaceOrientation.Portrait
                || currentOrientation == UIInterfaceOrientation.PortraitUpsideDown;

            return isPortrait ? DeviceOrientations.Portrait : DeviceOrientations.Landscape;
        }
    }
}