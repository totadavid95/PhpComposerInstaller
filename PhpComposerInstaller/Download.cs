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
            Console.Write("  * Letöltött fájl ellenőrzése... ");
            if (new Checksum().Check(destination, checksum)) Console.WriteLine("OK.");
            else {
                throw new Exception("Az ellenőrző összeg (sha256 checksum) nem egyezik, valószínűleg sérült a letöltött fájl.");
            }
        }
    }
}
