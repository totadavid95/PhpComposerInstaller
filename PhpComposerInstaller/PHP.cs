using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using TinyJson;

namespace PhpComposerInstaller {
    internal class PHP {
        // Abszolút útvonal megadásával meghív egy php.exe-t, amelynek lekéri a verziószámát
        // a -v kapcsoló segítségével
        public static string GetPhpVersionByLocation(string location) {
            var proc = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = location,
                    Arguments = "-v",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            Regex regex = new Regex("PHP\\s((\\d\\.?)+)", RegexOptions.IgnoreCase);
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

        // Kilövi az éppen folyamatban lévő PHP process-eket, ha azokat egy megadott útvonalon
        // lévő php.exe futtatja (a location-be bele kell írni a "php.exe"-t is)
        public static bool KillRunningPhpProcessesByLocation(string location) {
            bool result = false;
            var processes = Process.GetProcessesByName("php");
            foreach (var process in processes) {
                if (process.MainModule != null) {
                    string processPath = process.MainModule.FileName;
                    if (processPath == location) {
                        Console.WriteLine("    * Folyamat leállítása: " + processPath);
                        process.Kill();
                        result = true;
                    }
                }
            }
            return result;
        }

        // Megadja az alapértelmezett telepítési útvonalon lévő php-t (amit ez a telepítő rak fel)
        public static string GetDefaultPhpInstallationLocation() {
            return Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\php\php.exe";
        }

        // Lekéri a PHP weboldaláról a jelenleg támogatott kiadások listáját, illetve megadja a telepítésükhöz,
        // konfigurálásukhoz szükséges információkat is.
        public static Dictionary<string, Dictionary<string, string>> GetPhpReleases() {
            var result = new Dictionary<string, Dictionary<string, string>>();

            // Kliens inicializálása, adatok lekérése
            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "C# app");
            var releasesString = client.DownloadString("https://windows.php.net/downloads/releases/releases.json");

            // JSON parse-olása, feldolgozása
            var releases = (Dictionary<string, object>)releasesString.FromJson<object>();
            foreach (string version in releases.Keys) {
                var release = (Dictionary<string, object>)releases[version];
                foreach (string property in release.Keys) {
                    // Az architektúrának (32 vagy 64 bit) megfelelő NTS csomag megkeresése
                    if (
                        property.StartsWith("nts-") && (
                            Environment.Is64BitOperatingSystem && property.EndsWith("-x64") ||
                            !Environment.Is64BitOperatingSystem && property.EndsWith("-x86")
                        )
                    ) {
                        var package = (Dictionary<string, object>)release[property];
                        var zip = (Dictionary<string, object>)package["zip"];
                        result.Add(version, new Dictionary<string, string>() {
                            // Alverzió (pl. a főverzió PHP 7.4, az alverzió 7.4.27)
                            { "subversion", (string)release["version"] },
                            // Mivel lett buildelve az adott kiadás (vc15, vs16, stb)
                            { "builtwith", property.Split('-')[1].ToLower() },
                            // Az architektúrának megfelelő letöltőlink
                            { "downloadlink", "https://windows.php.net/downloads/releases/" + (string)zip["path"] },
                            // Checksum a letöltött csomag ellenőrzéséhez
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
