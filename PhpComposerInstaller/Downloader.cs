using System;
using System.ComponentModel;
using System.Net;
using System.Threading;

namespace PhpComposerInstaller {
    /// <summary>
    /// Download utility class, which integrates file downloader and progress bar.
    /// </summary>
    public class Downloader {
        private volatile bool _completed;
        private volatile ProgressBar _progressBar;
        private EventWaitHandle _waitHandle;

        public Downloader(EventWaitHandle waitHandle) {
            _waitHandle = waitHandle;
        }

        /// <summary>
        /// Downloads the file from the given address to the given local destination,
        /// and shows a progress bar in the console while downloading.
        /// </summary>
        public void DownloadFile(Uri address, string location) {
            _completed = false;
            _progressBar = new ProgressBar();

            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "C# app");
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
            client.DownloadFileAsync(address, location);
        }

        /// <summary>
        /// Returns true if the download is completed.
        /// </summary>
        public bool DownloadCompleted { get { return _completed; } }

        /// <summary>
        /// Updates the progress bar.
        /// </summary>
        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e) {
            _progressBar.Report((double)e.ProgressPercentage / 100);
        }

        /// <summary>
        /// Disposes the progress bar and sets the wait handle.
        /// </summary>
        private void Completed(object sender, AsyncCompletedEventArgs e) {
            _progressBar.Dispose();

            if (e.Cancelled) {
                Console.WriteLine("Cancelled.");
            } else {
                Console.WriteLine("Downloaded.");
            }

            _completed = true;
            _waitHandle.Set();
        }
    }
}
