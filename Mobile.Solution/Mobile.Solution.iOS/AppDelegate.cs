using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using Mobile.Solution.iOS.Dependencies;
using Plugin.DownloadManager;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfChart.XForms.iOS.Renderers;
using Syncfusion.SfCalendar.XForms.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Mobile.Solution.Infrastructure.Helpers;
using Syncfusion.SfPdfViewer.XForms.iOS;

namespace Mobile.Solution.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            Forms.Init();

            new SfChartRenderer();
            new SfCalendarRenderer();
            new SfPdfDocumentViewRenderer();
            SfListViewRenderer.Init();

            var application = new UI.Application();
            Infrastructure.CustomControls.Dialog.Init(UI.Application.RootPage);
            LoadApplication(application);
            application.Initialize();

            InitDownloadManager();

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
                );

                app.RegisterUserNotificationSettings(notificationSettings);
            }

            if (options != null)
                if (options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
                {
                    var localNotification =
                        options[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
                    if (localNotification != null)
                    {
                        var okayAlertController = UIAlertController.Create(localNotification.AlertAction,
                            localNotification.AlertBody, UIAlertControllerStyle.Alert);
                        okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                        var window = UIApplication.SharedApplication.KeyWindow;
                        window.RootViewController.PresentViewController(okayAlertController, true, null);

                        // reset our badge
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                    }
                }

            return base.FinishedLaunching(app, options);
        }

        public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier,
            Action completionHandler)
        {
            CrossDownloadManager.BackgroundSessionCompletionHandler = completionHandler;
        }

        private void InitDownloadManager()
        {
            CrossDownloadManager.Current.PathNameForDownloadedFile = file =>
            {
                var fileName = file.Headers.ContainsKey("name") ? file.Headers["name"] : Path.GetRandomFileName();
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
            };
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            var window = UIApplication.SharedApplication.KeyWindow;

            var okayAlertController = UIAlertController.Create(notification.AlertAction,
                notification.AlertBody, UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default,
                action => FilePreview.DeleteFile(notification.AlertBody)));
            okayAlertController.AddAction(UIAlertAction.Create("Открыть", UIAlertActionStyle.Default,
                action => FilePreview.FileOpenHandler(notification.AlertBody)));

            window.RootViewController.PresentViewController(okayAlertController, true, null);

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }


        private static async void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            await newExc.ToLogUnhandledException();
        }

        private static async void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            await newExc.ToLogUnhandledException();
        }
    }
}