using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

/// <summary>
/// Composer utility class.
/// </summary>
namespace PhpComposerInstaller {
    internal class Composer {
        /// <summary>
        /// Returns the download link for the latest stable Composer 2.x version.
        /// This link is same for all versions, so we can hardcode it.
        /// </summary>
        public static string GetDownloadLinkForLatestVersion() {
            return "https://getcomposer.org/download/latest-2.x/composer.phar";
        }

        /// <summary>
        /// Returns the checksum for the latest stable Composer 2 version.
        /// This link is same for all versions, so we can hardcode it.
        /// </summary>
        public static string GetChecksumForLatestVersion() {
            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "C# app");

            return client.DownloadString("https://getcomposer.org/download/latest-2.x/composer.phar.sha256");
        }

        /// <summary>
        /// Returns the default Composer installation location (which is installed by this tool).
        /// </summary>
        public static string GetDefaultComposerInstallationLocation() {
            return Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\composer\composer.bat";
        }

        /// <summary>
        /// Returns the Composer version by the given location.
        /// </summary>
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

        /// <summary>
        /// Generates the composer.bat file in the given location.
        /// </summary>
        public static void GenerateComposerBatchFile() {
            File.WriteAllText("PhpComposerInstallerDownloads/composer/composer.bat", "@php \"%~dp0composer.phar\" %*\n");
        }
    }
}
