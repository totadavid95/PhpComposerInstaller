﻿using System;
using System.ComponentModel;
using System.Net;
using System.Threading;

namespace PhpComposerInstaller {
    public class Downloader {
        private volatile bool _completed;
        private volatile ProgressBar _progressBar;
        private EventWaitHandle _waitHandle;

        public Downloader(EventWaitHandle waitHandle) {
            _waitHandle = waitHandle;
        }

        public void DownloadFile(Uri address, string location) {
            _completed = false;
            _progressBar = new ProgressBar();

            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "C# app");
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
            client.DownloadFileAsync(address, location);
        }

        public bool DownloadCompleted { get { return _completed; } }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e) {
            _progressBar.Report((double)e.ProgressPercentage / 100);
        }

        private void Completed(object sender, AsyncCompletedEventArgs e) {
            _progressBar.Dispose();
            if (e.Cancelled) Console.WriteLine("Cancelled.");
            else Console.WriteLine("Downloaded.");
            _completed = true;
            _waitHandle.Set();
        }
    }
}