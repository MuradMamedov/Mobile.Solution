using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.Dependencies
{
    public struct Report
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public DateTime DateCreated { get; set; }
    }

    public interface IFilePreview
    {
        void OpenFileNotification(string fileName, string title, string text, string mimeType);

        void SaveStringToFile(string fileName, string str);

        string ReadStringFromFile(string fileName);

        StreamReader GetFileStream(string fileName);

        bool Find(string fileName);

        void Delete(string fileName);

        List<Report> GetReports(string ext);

        void SaveImage(ContentView view);
    }
}