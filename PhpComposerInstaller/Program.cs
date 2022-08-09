// ---------------------------------------------------
// PHP, Xdebug, Composer telepítő
// Készítette: Tóta Dávid
// Kapcsolat: totadavid95@inf.elte.hu
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace PhpComposerInstaller {
    internal class Program {
        private static bool uninstall = false;
        private static bool noCleanup = false;
        private static bool withXdebug = false;
        private static bool withVCRedist = false;

        private static Dictionary<string, Dictionary<string, string>> phpReleases;
        private static string selectedPhpRelease;
        private static string defaultPhpLocation = PHP.GetDefaultPhpInstallationLocation();
        private static string xdebugVersion;
        private static string defaultComposerLocation = Composer.GetDefaultComposerInstallationLocation();
        private static bool phpAlreadyInstalled = false;
        private static bool composerAlreadyInstalled = false;

        private static void HandleArgs(string[] args) {
            // Csak uninstall (ha van mit)
            if (args.Contains("--uninstall")) uninstall = true;

            // Hagyja meg a letöltéseket, ideiglenes fájlokat
            if (args.Contains("--no-cleanup")) noCleanup = true;

            if (args.Contains("--with-xdebug")) withXdebug = true;
            if (args.Contains("--with-vc-redist")) withVCRedist = true;
        }

        private static void HandleUninstall() {
            Console.WriteLine("Phase 1: Uninstalling");
            Console.WriteLine("---------");
            Installer.RemoveFromLocalAppData();
            Installer.RemoveFromPathIfNecessary();
        }

        private static void HandlePriorCheck() {
            // 32 vagy 64 bit
            Console.WriteLine("  * Detected architecture: " + (Environment.Is64BitOperatingSystem ? "64 bit" : "32 bit"));

            // Jelenleg telepített verziók vizsgálata
            var phpLocations = OS.FindProgramLocations("php");
            var composerLocations = OS.FindProgramLocations("composer");

            // A telepítő útvonalai
            if (phpLocations.Contains(defaultPhpLocation) || File.Exists(defaultPhpLocation)) {
                Console.WriteLine("  * Currently installed PHP version: " + PHP.GetPhpVersionByLocation(defaultPhpLocation));
                phpAlreadyInstalled = true;
            }
            if (composerLocations.Contains(defaultComposerLocation) || File.Exists(defaultComposerLocation)) {
                Console.WriteLine("  * Currently installed Composer version: " + Composer.GetComposerVersionByLocation(defaultComposerLocation));
                composerAlreadyInstalled = true;
            }

            // Olyan globálisan is elérhető PHP-k és Composer-ek keresése, amiket nem ez a telepítő rakott fel
            // Ezek csak akkor érdekesek, ha benne vannak a Path-ban, mivel ütközést okoznak, és lehet, hogy ezek
            // kerülnek meghívásra, nem a telepített PHP.
            var foreignPhpLocations = OS.FindProgramLocations("php").Where(
                location => !location.Equals(defaultPhpLocation)
            ).ToList();

            var foreignComposerLocations = OS.FindProgramLocations("composer").Where(
                location => !location.Equals(defaultComposerLocation)
            ).ToList();

            if (foreignPhpLocations.Count > 0 || foreignComposerLocations.Count > 0) {
                Console.WriteLine("  * ATTENTION! The installer detected a PHP/Composer installation that is included in the PATH environment");
                Console.WriteLine("    variable, but it was not installed by this installer. This installer also adds the installed PHP to the");
                Console.WriteLine("    PATH, however if there are two PHPs in this environment variable at the same time, it leads to a conflict.");
                Console.WriteLine("    One possible solution to this problem may be to remove the paths listed below from the PATH variable, or");
                Console.WriteLine("    rename the php.exe file in them to php7.4.exe/php8.0.exe or anything other than php.exe.");
                Console.WriteLine(" ");
                if (foreignPhpLocations.Count > 0) {
                    Console.WriteLine("    Detected PHP installs:");
                    foreach (var phpLocation in foreignPhpLocations) {
                        Console.WriteLine("      - " + phpLocation + " (" + PHP.GetPhpVersionByLocation(phpLocation) + ")");
                    }
                    Console.WriteLine(" ");
                }
                if (foreignComposerLocations.Count > 0) {
                    Console.WriteLine("    Detected Composer installs:");
                    foreach (var composerLocation in foreignComposerLocations) {
                        Console.WriteLine("      - " + composerLocation + " (" + Composer.GetComposerVersionByLocation(composerLocation) + ")");
                    }
                    Console.WriteLine(" ");
                }
            }
            else {
                Console.WriteLine("  * No problems found.");
            }
        }

        private static void HandlePhpReleaseSelection() {
            Console.Write("  * Fetching currently supported PHP releases from the official php.net site... ");
            phpReleases = PHP.GetPhpReleases();
            Console.WriteLine("OK.");

            Console.WriteLine("  * Currently, these supported PHP editions are available. Please choose which one");
            Console.WriteLine("    you want to install!");
            if (phpAlreadyInstalled) {
                Console.WriteLine("    ATTENTION! Immediately after the selection, the installer overwrites/updates");
                Console.WriteLine("    the currently installed PHP/Composer in the %LOCALAPPDATA%/Programs directory!");
            }
            selectedPhpRelease = Selector.ShowSelectorMenu(phpReleases.Keys.ToArray());
            Console.WriteLine("  * Selected: " + selectedPhpRelease);
        }

        private static void HandleTempDirectory() {
            if (Directory.Exists("PhpComposerInstallerDownloads")) {
                OS.DeleteDirectory("PhpComposerInstallerDownloads");
                Console.WriteLine("  * Temp directory (PhpComposerInstallerDownloads) deleted.");
            }
            Directory.CreateDirectory("PhpComposerInstallerDownloads");
            Directory.CreateDirectory("PhpComposerInstallerDownloads/composer");
            Console.WriteLine("  * Temp directory (PhpComposerInstallerDownloads) created.");
        }

        private static void HandleDownloads() {
            Download.DownloadAndCheckFile(
                "Downloading PHP " + selectedPhpRelease + "... ",
                new Uri(phpReleases[selectedPhpRelease]["downloadlink"]),
                phpReleases[selectedPhpRelease]["checksum"],
                "PhpComposerInstallerDownloads/php.zip"
            );

            if (withXdebug) {
                var xdebug = Xdebug.GetLatestPackage(selectedPhpRelease, phpReleases[selectedPhpRelease]["builtwith"]);
                xdebugVersion = xdebug["version"];
                Download.DownloadAndCheckFile(
                    "Downloading Xdebug for PHP " + selectedPhpRelease + " " + phpReleases[selectedPhpRelease]["builtwith"].ToUpper() + " NTS... ",
                    new Uri(xdebug["downloadlink"]),
                    xdebug["checksum"],
                    "PhpComposerInstallerDownloads/xdebug.dll"
                );

                Download.DownloadFile(
                    "Downloading Xdebug " + xdebug["version"] + " source code... ",
                    new Uri("https://github.com/xdebug/xdebug/archive/refs/tags/" + xdebug["version"] + ".zip"),
                    "PhpComposerInstallerDownloads/xdebug_src.zip"
                );
            }

            if (withVCRedist) {
                // Ez kell a PHP-hoz, lásd https://www.php.net/manual/en/install.windows.requirements.php
                Download.DownloadFile(
                    "Downloading Visual C++ Redistributable " + (Environment.Is64BitOperatingSystem ? "x64" : "x86") + " installer... ",
                    new Uri(
                        Environment.Is64BitOperatingSystem
                            ? "https://aka.ms/vs/16/release/vc_redist.x64.exe"
                            : "https://aka.ms/vs/16/release/vc_redist.x86.exe"
                    ),
                    "PhpComposerInstallerDownloads/vc_redist.exe"
                );
            }

            Download.DownloadAndCheckFile(
                "Downloading latest Composer 2.x... ",
                new Uri(Composer.GetDownloadLinkForLatestVersion()),
                Composer.GetChecksumForLatestVersion(),
                "PhpComposerInstallerDownloads/composer.phar"
            );
        }

        private static void HandleConfiguration() {
            // PHP
            Console.Write("  * Extracting PHP program files... ");
            ZipFile.ExtractToDirectory("PhpComposerInstallerDownloads/php.zip", "PhpComposerInstallerDownloads/php");
            Console.WriteLine("OK.");

            if (withXdebug) {
                Console.Write("  * Extracting Xdebug configuration... ");
                ZipFile.ExtractToDirectory("PhpComposerInstallerDownloads/xdebug_src.zip", "PhpComposerInstallerDownloads/xdebug_src");
                File.Copy("PhpComposerInstallerDownloads/xdebug_src/xdebug-" + xdebugVersion + "/xdebug.ini", "PhpComposerInstallerDownloads/xdebug.ini");
                Console.WriteLine("OK.");

                Console.Write("  * Copy xdebug.dll to php/ext directory... ");
                File.Copy("PhpComposerInstallerDownloads/xdebug.dll", "PhpComposerInstallerDownloads/php/ext/php_xdebug.dll");
                Console.WriteLine("OK.");
            }

            Console.Write("  * Create and configure php.ini... ");
            File.Copy("PhpComposerInstallerDownloads/php/php.ini-development", "PhpComposerInstallerDownloads/php/php.ini", true);

            string phpIniContent = File.ReadAllText("PhpComposerInstallerDownloads/php/php.ini");
            phpIniContent = phpIniContent.Replace(";extension_dir = \"ext\"", "extension_dir = \"ext\"");
            phpIniContent = phpIniContent.Replace(";extension=curl", "extension=curl");
            phpIniContent = phpIniContent.Replace(";extension=fileinfo", "extension=fileinfo");
            phpIniContent = phpIniContent.Replace(";extension=mbstring", "extension=mbstring");
            phpIniContent = phpIniContent.Replace(";extension=openssl", "extension=openssl");
            phpIniContent = phpIniContent.Replace(";extension=pdo_mysql", "extension=pdo_mysql");
            phpIniContent = phpIniContent.Replace(";extension=pdo_sqlite", "extension=pdo_sqlite");

            if (withXdebug) {
                phpIniContent = phpIniContent.Replace(";extension=xsl", ";extension=xsl\nzend_extension=xdebug");

                // https://xdebug.org/docs/all_settings
                string xdebugDefaultConfig = File.ReadAllText("PhpComposerInstallerDownloads/xdebug.ini");
                xdebugDefaultConfig = xdebugDefaultConfig.Replace(" — do not modify by hand", "");
                xdebugDefaultConfig = xdebugDefaultConfig.Replace(";xdebug.cli_color = 0", "xdebug.cli_color = 1");
                xdebugDefaultConfig = xdebugDefaultConfig.Replace(";xdebug.client_host = localhost", "xdebug.client_host = localhost");
                xdebugDefaultConfig = xdebugDefaultConfig.Replace(";xdebug.client_port = 9003", "xdebug.client_port = 9003");
                phpIniContent += "\n[xdebug]\n" + xdebugDefaultConfig;
            }

            File.WriteAllText("PhpComposerInstallerDownloads/php/php.ini", phpIniContent);
            Console.WriteLine("OK.");

            // Composer
            Console.Write("  * Copy composer.phar to the composer directory... ");
            File.Copy("PhpComposerInstallerDownloads/composer.phar", "PhpComposerInstallerDownloads/composer/composer.phar");
            Console.WriteLine("OK.");

            Console.Write("  * Create runnable .bat file for Composer... ");
            Composer.GenerateComposerBatchFile();
            Console.WriteLine("OK.");
        }

        private static void HandleInstallation() {
            Console.WriteLine("  * Stop PHP processes that conflicts with this installer... ");
            PHP.KillRunningPhpProcessesByLocation(defaultPhpLocation);

            if (withVCRedist) {
                Console.Write("  * Run Visual C++ Redistributable Installer... ");
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "PhpComposerInstallerDownloads/vc_redist.exe",
                        Arguments = "/install /passive /quiet /norestart /log vc_redist.log",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                proc.WaitForExit();
                Console.WriteLine("OK.");
            }

            Installer.CopyToLocalAppData();
            Installer.AddToPathIfNecessary();
        }

        private static void HandleCleanup() {
            if (!noCleanup) {
                if (Directory.Exists("PhpComposerInstallerDownloads")) {
                    OS.DeleteDirectory("PhpComposerInstallerDownloads");
                    Console.WriteLine("  * Temp directory (PhpComposerInstallerDownloads) deleted.");
                }
            } else {
                Console.WriteLine("  * Skipped.");
            }
        }

        static void Main(string[] args) {
            HandleArgs(args);

            try {
                if (uninstall) {
                    HandleUninstall();
                    return;
                }

                // -----------------------------------

                Console.WriteLine("Phase 1: Finding problems");
                Console.WriteLine("---------");
                HandlePriorCheck();
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine("Phase 2: PHP release selection");
                Console.WriteLine("---------");
                HandlePhpReleaseSelection();
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine("Phase 3: Download tools from the Internet");
                Console.WriteLine("---------");
                HandleTempDirectory();
                HandleDownloads();
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine("Phase 4: Configure downloaded tools");
                Console.WriteLine("---------");
                HandleConfiguration();
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine("Phase 5: Installing programs");
                Console.WriteLine("---------");
                HandleInstallation();
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine("Phase 6: Delete temporary files");
                Console.WriteLine("---------");
                HandleCleanup();
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine("Phase 7: Test your installation manually!");
                Console.WriteLine("---------");
                Console.WriteLine("  1.) Open the \"version.bat\" file in the InstallTest directory, then make sure that");
                Console.WriteLine("      the version number of PHP and Composer is displayed on the console.");
                Console.WriteLine(" ");
                Console.WriteLine("  2.) Open the \"phpinfo.bat\" file in the InstallTest directory, which will start a PHP server");
                Console.WriteLine("      and then open it in MS Edge. If this works well, the installation is probably successful.");
                Console.WriteLine("      You can also visit this page if you want to check any PHP settings.");
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine(" Installation is complete. Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex) {
                Console.WriteLine("FAILED.");
                Console.WriteLine(ex.Message);
            }

        }
    }
}