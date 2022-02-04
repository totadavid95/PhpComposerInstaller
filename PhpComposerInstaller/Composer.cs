using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace PhpComposerInstaller {
    internal class Composer {
        // Megadja a legújabb stabil Composer2 verzió letöltőlinkjét - szerencsére ez egy fix link
        public static string GetDownloadLinkForLatestVersion() {
            return "https://getcomposer.org/download/latest-2.x/composer.phar";
        }

        // Megadja a legújabb stabil Composer2 verzióhoz tartozó ellenőrzőösszeget
        public static string GetChecksumForLatestVersion() {
            // Kliens inicializálása, adatok lekérése
            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "C# app");
            return client.DownloadString("https://getcomposer.org/download/latest-2.x/composer.phar.sha256");
        }

        // Megadja az alapértelmezett telepítési útvonalon lévő Composer-t (amit ez a telepítő rak fel)
        public static string GetDefaultComposerInstallationLocation() {
            return Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\composer\composer.bat";
        }

        // Abszolút útvonal megadásával meghív egy Composert, amelynek lekéri a verziószámát
        // a -V kapcsoló segítségével
        public static string GetComposerVersionByLocation(string location) {
            var proc = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = location,
                    Arguments = "-V",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            Regex regex = new Regex("version\\s((\\d\\.?)+)", RegexOptions.IgnoreCase);
            while (!proc.StandardOutput.EndOfStream) {
                string line = proc.StandardOutput.ReadLine()?.TrimEnd(Environment.NewLine.ToCharArray());
                if (line != null) {
                    Match match = regex.Matches(line).OfType<Match>().LastOrDefault();
                    if (match != null && match.Success) {
                        return match.Groups[1].Captures[0].Value;
                    }
                }
            }
            return null;
        }

        // Batch fájl generálása a futtatáshoz
        public static void GenerateComposerBatchFile() {
            File.WriteAllText("PhpComposerInstallerDownloads/composer/composer.bat", "@php \"%~dp0composer.phar\" %*\n");
        }
    }
}
