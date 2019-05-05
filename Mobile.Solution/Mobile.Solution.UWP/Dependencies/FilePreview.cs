using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.UWP.Dependencies;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Application = Xamarin.Forms.Application;

[assembly: Dependency(typeof(FilePreview))]

namespace Mobile.Solution.UWP.Dependencies
{
    public class FilePreview : IFilePreview
    {
        private static string GetFilePath(string fileName)
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, fileName);
        }

        #region IFilePreview

        public async void OpenFileNotification(string fileName, string title, string text, string mimeType)
        {
            if (await Application.Current.MainPage.DisplayAlert(title, text, "Открыть", "Отмена"))
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);

                if (file != null)
                    await Launcher.LaunchFileAsync(file);
            }
        }

        public void SaveStringToFile(string fileName, string str)
        {
            var path = GetFilePath(fileName);
            File.WriteAllText(path, str);
        }

        public string ReadStringFromFile(string fileName)
        {
            var path = GetFilePath(fileName);
            return File.ReadAllText(path);
        }

        public StreamReader GetFileStream(string fileName)
        {
            var path = GetFilePath(fileName);
            return new StreamReader(File.OpenRead(path));
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

            var files = Directory.GetFiles(ApplicationData.Current.LocalFolder.Path);
            foreach (var file in files.Where(f => Path.GetExtension(f).Contains("aflt")))
            {
                using (var f = File.OpenText(file))
                {
                    var v = Path.GetFileNameWithoutExtension(file) + $".{type}";
                    SaveStringToFile(v, f.ReadToEnd());
                    Delete(file);
                }
            }


            files = Directory.GetFiles(ApplicationData.Current.LocalFolder.Path);
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

        public async void SaveImage(ContentView view)
        {
            var renderer = Platform.GetRenderer(view);
            var container = renderer.ContainerElement;

            var rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(container);

            var pixelBuffer = await rtb.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();

            var displayInformation = DisplayInformation.GetForCurrentView();

            var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied,
                (uint)rtb.PixelWidth,
                (uint)rtb.PixelHeight,
                displayInformation.RawDpiX,
                displayInformation.RawDpiY,
                pixels);

            await encoder.FlushAsync();
            stream.Seek(0);

            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Снимок.png", CreationCollisionOption.ReplaceExisting);

            using (var fileStream1 = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await RandomAccessStream.CopyAndCloseAsync(stream.GetInputStreamAt(0), fileStream1.GetOutputStreamAt(0));
            }
            await Launcher.LaunchFileAsync(file);
        }

        #endregion
    }
}