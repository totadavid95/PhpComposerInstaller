using System;
using System.ComponentModel;
using System.Net;
using System.Threading;

namespace PhpComposerInstaller {
    /// <summary>
    /// Download utility class, integrating file downloader and progress bar.
    /// </summary>
    public class Downloader {
        private volatile bool completed;
        private volatile ProgressBar progressBar;
        private readonly EventWaitHandle waitHandle;

        public Downloader(EventWaitHandle waitHandle) {
            this.waitHandle = waitHandle;
        }

        /// <summary>
        /// Downloads the file from the given address to the given local destination,
        /// and shows a progress bar in the console while downloading.
        /// </summary>
        public void DownloadFile(Uri address, string location) {
            completed = false;
            InitializeProgressBar();

            using (WebClient client = new WebClient()) {
                client.Headers.Add(HttpRequestHeader.UserAgent, "C# app");
                client.DownloadFileCompleted += Completed;
                client.DownloadProgressChanged += DownloadProgress;
                client.DownloadFileAsync(address, location);
            }
        }

        /// <summary>
        /// Returns true if the download is completed.
        /// </summary>
        public bool DownloadCompleted => completed;

        /// <summary>
        /// Updates the progress bar.
        /// </summary>
        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e) {
            progressBar.Report(e.ProgressPercentage / 100.0);
        }

        /// <summary>
        /// Disposes the progress bar and sets the wait handle.
        /// </summary>
        private void Completed(object sender, AsyncCompletedEventArgs e) {
            DisposeProgressBar();

            if (e.Cancelled) {
                Console.WriteLine("Cancelled.");
            } else {
                Console.WriteLine("Downloaded.");
            }

            completed = true;
            waitHandle.Set();
        }

        /// <summary>
        /// Initializes the progress bar.
        /// </summary>
        private void InitializeProgressBar() {
            progressBar = new ProgressBar();
        }

        /// <summary>
        /// Disposes the progress bar.
        /// </summary>
        private void DisposeProgressBar() {
            progressBar.Dispose();
        }
    }
}
