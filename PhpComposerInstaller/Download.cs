using System;
using System.Threading;

namespace PhpComposerInstaller {
    internal class Download {
        public static void DownloadFile(string label, Uri address, string destination) {
            var waitHandle = new ManualResetEvent(initialState: true);
            Console.Write("  * " + label);
            var downloadApi = new Downloader(waitHandle);
            downloadApi.DownloadFile(address, destination);
            waitHandle.Reset();
            waitHandle.WaitOne();
        }

        public static void DownloadAndCheckFile(string label, Uri address, string checksum, string destination) {
            DownloadFile(label, address, destination);
            Console.Write("  * Checking downloaded file... ");
            if (new Checksum().Check(destination, checksum)) Console.WriteLine("OK.");
            else {
                throw new Exception("SHA256 checksum does not match, the file may be corrupted, please try again.");
            }
        }
    }
}
