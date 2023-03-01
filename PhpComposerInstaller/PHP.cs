using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using TinyJson;

namespace PhpComposerInstaller
{
    internal class PHP
    {
        /// <summary>
        /// Gets the PHP version by location.
        /// </summary>
        public static string GetPhpVersionByLocation(string location)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = location,
                    Arguments = "-v",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            Regex regex = new Regex("PHP\\s((\\d\\.?)+)", RegexOptions.IgnoreCase);

            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine()?.TrimEnd(Environment.NewLine.ToCharArray());
                if (line != null)
                {
                    Match match = regex.Matches(line).OfType<Match>().LastOrDefault();
                    if (match != null && match.Success)
                    {
                        return match.Groups[1].Captures[0].Value;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Kills the running PHP processes by location.
        /// </summary>
        public static bool KillRunningPhpProcessesByLocation(string location)
        {
            bool result = false;
            var processes = Process.GetProcessesByName("php");

            foreach (var process in processes)
            {
                if (process.MainModule != null)
                {
                    string processPath = process.MainModule.FileName;
                    if (processPath == location)
                    {
                        Console.WriteLine("    * Killing process: " + processPath);
                        process.Kill();
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the default PHP installation location (which is installed by this tool).
        /// </summary>
        public static string GetDefaultPhpInstallationLocation()
        {
            return Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\php\php.exe";
        }

        /// <summary>
        /// Gets currently supported PHP releases from the PHP website, and also provides the necessary
        /// information for installation and configuration.
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> GetPhpReleases()
        {
            var result = new Dictionary<string, Dictionary<string, string>>();

            // Init WebClient and download the releases.json file from the PHP website
            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "C# app");
            var releasesString = client.DownloadString("https://windows.php.net/downloads/releases/releases.json");

            // Parse the JSON file, and get the necessary information
            var releases = (Dictionary<string, object>)releasesString.FromJson<object>();

            foreach (string version in releases.Keys)
            {
                var release = (Dictionary<string, object>)releases[version];

                foreach (string property in release.Keys)
                {
                    // Get the NTS package for the current architecture
                    if (
                        property.StartsWith("nts-") && (
                            Environment.Is64BitOperatingSystem && property.EndsWith("-x64") ||
                            !Environment.Is64BitOperatingSystem && property.EndsWith("-x86")
                        )
                    )
                    {
                        var package = (Dictionary<string, object>)release[property];
                        var zip = (Dictionary<string, object>)package["zip"];

                        result.Add(version, new Dictionary<string, string>() {
                            // Subversion (eg. the main version is PHP 7.4, the subversion is 7.4.27)
                            { "subversion", (string)release["version"] },

                            // Which compiler was used to build the release (vc15, vs16, etc.)
                            { "builtwith", property.Split('-')[1].ToLower() },

                            // Download link for the release
                            { "downloadlink", "https://windows.php.net/downloads/releases/" + (string)zip["path"] },
                            
                            // SHA256 checksum for the release
                            { "checksum", (string)zip["sha256"] },
                        });

                        break;
                    }
                }
            }

            return result;
        }

    }
}
