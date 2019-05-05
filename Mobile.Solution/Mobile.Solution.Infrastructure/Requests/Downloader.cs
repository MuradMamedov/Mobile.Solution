using System;
using System.Collections.Generic;
using System.Diagnostics;
using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;

namespace Mobile.Solution.Infrastructure.Requests
{
    public class Downloader
    {
        private IDownloadFile _file;

        public Downloader()
        {
#if DEBUG
            CrossDownloadManager.Current.CollectionChanged += (sender, e) =>
                Debug.WriteLine(
                    "[DownloadManager] " + e.Action +
                    " -> New items: " + (e.NewItems?.Count ?? 0) +
                    " at " + e.NewStartingIndex +
                    " || Old items: " + (e.OldItems?.Count ?? 0) +
                    " at " + e.OldStartingIndex
                );
#endif
        }

        public IDownloadFile File => _file;

        public IDownloadFile InitializeDownload(string url, Dictionary<string, string> dictionary)
        {
            return _file = CrossDownloadManager.Current.CreateDownloadFile(url, dictionary);
        }

        public void StartDownloading(bool mobileNetworkAllowed)
        {
            CrossDownloadManager.Current.Start(_file, mobileNetworkAllowed);
        }

        public void AbortDownloading()
        {
            CrossDownloadManager.Current.Abort(_file);
        }

        public bool IsDownloading()
        {
            if (_file == null) return false;

            switch (_file.Status)
            {
                case DownloadFileStatus.INITIALIZED:
                case DownloadFileStatus.PAUSED:
                case DownloadFileStatus.PENDING:
                case DownloadFileStatus.RUNNING:
                    return true;

                case DownloadFileStatus.COMPLETED:
                case DownloadFileStatus.CANCELED:
                case DownloadFileStatus.FAILED:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}