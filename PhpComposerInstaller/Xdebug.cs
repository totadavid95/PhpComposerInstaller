using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace PhpComposerInstaller {
    internal class Xdebug {
        // A megadott PHP főverzióhoz lekéri az Xdebug letöltőlinket és a checksumot. Fontos, hogy
        // a telepítő csak a támogatott PHP verziókat kezeli (amit mindig a PHP hivatalos oldaláról
        // kér le), tehát az Xdebug letöltési oldalán ezek a támogatott verziók jó eséllyel
        // megtalálhatók lesznek
        public static Dictionary<string, string> GetLatestPackage(string phpVersion, string builtWith) {
            var result = new Dictionary<string, string>();

            // Kliens inicializálása, adatok lekérése
            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "C# app");
            string html = client.DownloadString("https://xdebug.org/download/historical");

            // 64 bites csomag - alapértelmezett:
            string pattern = "title\\=[\\\"\\']SHA256\\:\\&nbsp\\;(?<checksum>[a-z0-9]+)[\\\"\\']\\shref=[\\\"\\']\\/files\\/(?<filename>php_xdebug-(?<version>(\\d\\.\\d\\.\\d))-" + phpVersion + "-" + builtWith + "-nts-x86_64.dll)[\\\"\\']";
            // 32 bites csomag:
            if (!Environment.Is64BitOperatingSystem) {
                //pattern = "title\\=[\\\"\\']SHA256\\:\\&nbsp\\;(?<checksum>[a-z0-9]+)[\\\"\\']\\shref=[\\\"\\']\\/files\\/(?<filename>php_xdebug-(?<version>[^-]+)-" + phpVersion + "-" + builtWith + "-nts.dll)[\\\"\\']";
                pattern = pattern.Replace("-x86_64", "");
            }
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = regex.Match(html);
            if (match.Success) {
                /*string[] names = regex.GetGroupNames();
                foreach (var name in names){
                    result.Add(name, match.Groups[name].Value);
                }*/
                result.Add("checksum", match.Groups["checksum"].Value);
                result.Add("version", match.Groups["version"].Value);
                result.Add("downloadlink", "https://xdebug.org/files/" + match.Groups["filename"].Value);
            } else {
                throw new Exception("The latest Xdebug release could not be detected because the regular expression didn't find a match.");
            }
            return result;
        }
    }
}
