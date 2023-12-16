using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace PhpComposerInstaller {
    /// <summary>
    /// Xdebug utility class.
    /// </summary>
    internal class Xdebug {
        /// <summary>
        /// Gets the latest Xdebug package for the given PHP version.
        /// </summary>
        public static Dictionary<string, string> GetLatestPackage(string phpVersion, string builtWith) {
            var result = new Dictionary<string, string>();

            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "C# app");
            string html = client.DownloadString("https://xdebug.org/download/historical");

            // 64 bit package - default
            string pattern = "title\\=[\\\"\\']SHA256\\:\\&nbsp\\;(?<checksum>[a-z0-9]+)[\\\"\\']\\shref=[\\\"\\']\\/files\\/(?<filename>php_xdebug-(?<version>(\\d\\.\\d\\.\\d))-" + phpVersion + "-" + builtWith + "-nts-x86_64.dll)[\\\"\\']";

            // 32 bit package - if the OS is 32 bit
            if (!Environment.Is64BitOperatingSystem) {
                pattern = pattern.Replace("-x86_64", "");
            }

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = regex.Match(html);

            if (match.Success) {
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
