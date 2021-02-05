using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace szerveroldali_webprog
{
    public class FetchApi
    {
        private string _phpReleases;
        private string _phpRel;
        private string _phpVer;

        public FetchApi() {
            GetPhpReleases();
            GetLatestPhpRelease();
            GetLatestPhpVersion();
        }
        
        public void GetPhpReleases() {
            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "C# app");
            this._phpReleases = client.DownloadString("https://windows.php.net/downloads/releases/releases.json");
        }

        public string GetLatestPhpRelease() {
            Regex regex = new Regex("\"([\\d.]+)\":\\s{", RegexOptions.IgnoreCase);
            Match match = regex.Matches(this._phpReleases).OfType<Match>().LastOrDefault();
            if (match != null && match.Success) {
                this._phpRel = match.Groups[1].Captures[0].Value;
                return this._phpRel;
            }
            throw new Exception("Couldn't find the latest PHP release.");
        }
        
        public string GetLatestPhpVersion() {
            Regex regex = new Regex("\"version\": \"([^\"]+)\"", RegexOptions.IgnoreCase);
            Match match = regex.Matches(this._phpReleases).OfType<Match>().LastOrDefault();
            if (match != null && match.Success) {
                this._phpVer = match.Groups[1].Captures[0].Value;
                return this._phpVer;
            }
            throw new Exception("Couldn't find the latest PHP version.");
        }

        public Uri GetLatestPhpVersionDownloadLink() {
            string pattern = "\"(php-" + this.GetLatestPhpVersion() + "-nts-Win32-v.\\d\\d-x64.zip)\"";
            if (!Environment.Is64BitOperatingSystem) pattern = "\"(php-" + this.GetLatestPhpVersion() + "-nts-Win32-v.\\d\\d-x86.zip)\"";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = regex.Matches(this._phpReleases).OfType<Match>().LastOrDefault();
            if (match != null && match.Success) {
                return new Uri("https://windows.php.net/downloads/releases/" + match.Groups[1].Captures[0].Value);
            }
            throw new Exception("Couldn't find download link for the latest PHP version.");
        }

        public Uri GetLatestXDebugDownloadLink() {
            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "C# app");
            
            string download = client.DownloadString("https://xdebug.org/download");
            string pattern = "(php_xdebug-[^-]+-" + this._phpRel + "-v.\\d\\d-nts-x86_64.dll)";
            if (!Environment.Is64BitOperatingSystem) pattern = "(php_xdebug-[^-]+-" + this._phpRel + "-v.\\d\\d-nts.dll)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = regex.Matches(download).OfType<Match>().LastOrDefault();
            if (match != null && match.Success) {
                return new Uri("https://xdebug.org/files/" + match.Groups[1].Captures[0].Value);
            }
            throw new Exception("Couldn't find download link for the latest XDebug version.");
        }
    }
}