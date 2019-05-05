using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Mobile.Solution.Droid.Dependencies;
using Mobile.Solution.Infrastructure.Dependencies;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Application = Android.App.Application;
using Environment = System.Environment;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;

[assembly: Dependency(typeof(FilePreview))]

namespace Mobile.Solution.Droid.Dependencies
{
    public class FilePreview : IFilePreview
    {
        private static string GetFilePath(string fileName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
        }

        #region IFilePreview

        public void OpenFileNotification(string fileName, string title, string text, string mimeType)
        {
            var intent = new Intent();
            intent.SetAction(Intent.ActionView);
            intent.SetDataAndType(
                Uri.Parse(
                    $"file://{Path.Combine(Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads).AbsolutePath, fileName)}"),
                mimeType);

            const int pendingIntentId = 0;
            var pendingIntent =
                PendingIntent.GetActivity(Application.Context, pendingIntentId, intent, PendingIntentFlags.OneShot);


            var builder = new Notification.Builder(Application.Context).SetContentTitle(title)
                .SetContentText(text)
                .SetContentIntent(pendingIntent)
                .SetAutoCancel(true)
                .SetPriority((int) NotificationPriority.High)
                .SetSmallIcon(Resource.Drawable.excel_icon)
                .SetCategory(Notification.CategoryProgress);
            var notification = builder.Build();

            var notificationManager =
                Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

            const int notificationId = 0;
            notificationManager?.Notify(notificationId, notification);
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
            using (var streamReader = new StreamReader(path))
            {
                return streamReader.ReadToEnd();
            }
        }

        public StreamReader GetFileStream(string fileName)
        {
            var path = GetFilePath(fileName);
            return new StreamReader(path);
        }

        public bool Find(string fileName)
        {
            var path = GetFilePath(fileName);
            return File.Exists(path);
        }

        public void Delete(string fileName)
        {
            File.Delete(GetFilePath(fileName));
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
            var viewGroup = renderer.ViewGroup;
            var drawinCacheEnabled = viewGroup.DrawingCacheEnabled;
            viewGroup.DrawingCacheEnabled = true;
            var bitmap = viewGroup.GetDrawingCache(true);
            var filePath = Path.Combine(Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures).AbsolutePath, "Снимок.png");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            }
            viewGroup.DrawingCacheEnabled = drawinCacheEnabled;
            var intent = new Intent();
            intent.SetAction(Intent.ActionView);
            intent.SetDataAndType(Uri.Parse($"file://{filePath}"), "image/png");
            intent.AddFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }

        #endregion
    }
}