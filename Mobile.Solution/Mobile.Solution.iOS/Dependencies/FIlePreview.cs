using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Foundation;
using Mobile.Solution.iOS.Dependencies;
using Mobile.Solution.Infrastructure.Dependencies;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency(typeof(FilePreview))]

namespace Mobile.Solution.iOS.Dependencies
{
    public class FilePreview : IFilePreview
    {
        internal static void DeleteFile(string fileName)
        {
            File.Delete(GetFilePath(fileName));
        }

        internal static void FileOpenHandler(string fileName)
        {
            try
            {
                var firstController =
                    UIApplication.SharedApplication.KeyWindow.RootViewController.ChildViewControllers.First()
                        .ChildViewControllers.Last()
                        .ChildViewControllers.First();

                var navcontroller = firstController.NavigationController;
                var viewController = navcontroller.ViewControllers.Last();

                var uidic = UIDocumentInteractionController.FromUrl(new NSUrl(GetFilePath(fileName), true));

                uidic.Delegate = new DocInteractionController(viewController);

                uidic.PresentPreview(true);

                uidic.DidEndPreview += (sender, e) => DeleteFile(fileName);
            }
            catch
            {
                // ignored
            }
        }

        private static string GetFilePath(string fileName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
        }

        #region IFilePreview

        public void OpenFileNotification(string fileName, string title, string text, string mimeType)
        {
            var app = (AppDelegate) UIApplication.SharedApplication.Delegate;
            app.InvokeOnMainThread(() =>
            {
                var notification = new UILocalNotification
                {
                    AlertAction = "Файл загружен",
                    AlertBody = fileName,
                    ApplicationIconBadgeNumber = 1
                };

                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            });
        }

        public void SaveStringToFile(string fileName, string str)
        {
            var path = GetFilePath(fileName);
            using (var streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(str);
            }
        }

        public string ReadStringFromFile(string fileName)
        {
            var path = GetFilePath(fileName);
            if(Find(fileName))
	            using (var streamReader = new StreamReader(path))
	            {
	                return streamReader.ReadToEnd();
	            }
            return null;
        }

        public StreamReader GetFileStream(string fileName)
        {
            var path = GetFilePath(fileName);
            return new StreamReader(path);
        }

        public void Delete(string fileName)
        {
            DeleteFile(fileName);
        }

        public bool Find(string fileName)
        {
            var path = GetFilePath(fileName);
            return File.Exists(path);
        }

        public List<Report> GetReports(string type)
        {
            var reports = new List<Report>();

            var files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            foreach (var file in files.Where(f => Path.GetExtension(f).Contains("aflt")))
            {
                using (var f = File.OpenText(file))
                {
                    var v = Path.GetFileNameWithoutExtension(file) + $".{type}";
                    SaveStringToFile(v, f.ReadToEnd());
                    Delete(file);
                }
            }


            files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            foreach (var file in files.Where(f => Path.GetExtension(f).Contains(type)))
                using (var f = File.OpenText(file))
                {
                    reports.Add(new Report
                    {
                        Name = Path.GetFileName(file),
                        Content = f.ReadToEnd(),
                        DateCreated = File.GetCreationTime(file)
                    });
                }
            return reports;
        }

        public void SaveImage(ContentView view)
        {
			var renderer = Platform.GetRenderer(view);
			var nativeView = renderer.NativeView;
            UIGraphics.BeginImageContextWithOptions(new CoreGraphics.CGSize(nativeView.Bounds.Size.Width, nativeView.Bounds.Size.Height), true, 0);
			nativeView.DrawViewHierarchy(nativeView.Bounds, afterScreenUpdates: true);
            UIImage image;
            try
            {
                image = UIGraphics.GetImageFromCurrentImageContext();
                string jpgFilename = GetFilePath("Снимок.png");
                NSData imgData = image.AsPNG();
                NSError err = null;
                if (imgData.Save(jpgFilename, false, out err))
                {
                    FileOpenHandler(jpgFilename);
                }
            }
            finally
            {
                UIGraphics.EndImageContext();
            }
        }

        public void SaveSearchItem<T>(T item)
        {
		}

        #endregion
    }
}