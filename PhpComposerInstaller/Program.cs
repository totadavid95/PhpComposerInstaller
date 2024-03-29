// PHP, Xdebug, Composer installer
// Created by: David Tota
// Contact: totadavid95@inf.elte.hu
// GitHub: https://github.com/totadavid95/PhpComposerInstaller

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace PhpComposerInstaller {
    /// <summary>
    /// Installer program root class.
    /// </summary>
    internal class Program {
        private static Dictionary<string, Dictionary<string, string>> phpReleases;
        private static string selectedPhpRelease;
        private static string defaultPhpLocation = PHP.GetDefaultPhpInstallationLocation();
        private static string xdebugVersion;
        private static string defaultComposerLocation = Composer.GetDefaultComposerInstallationLocation();
        private static bool phpAlreadyInstalled = false;
        private static bool composerAlreadyInstalled = false;

        /// <summary>
        /// The dictionary of command-line options with their default values and descriptions.
        /// </summary>
        private static readonly Dictionary<string, Option> options = new Dictionary<string, Option>()
        {
            { "uninstall",  new Option(value: false,    description: "Uninstall PHP and Composer from the local AppData directory") },
            { "cleanup",    new Option(value: true,     description: "Delete the temporary files after installation") },
            { "composer",   new Option(value: true,     description: "Install Composer") },
            { "xdebug",     new Option(value: false,    description: "Install Xdebug") },
            { "vc-redist",  new Option(value: true,     description: "Install Visual C++ Redistributable") }
        };

        /// <summary>
        /// The option handler instance.
        /// </summary>
        private static OptionHandler optionHandler;

        /// <summary>
        /// Uninstalls previously installed PHP and Composer, if they are installed with this tool.
        private static void HandleUninstall() {
            Console.WriteLine("Phase 1: Uninstalling");
            Console.WriteLine("---------");

            Installer.RemovePhpFromLocalAppData();
            Installer.RemovePhpFromPathIfNecessary();

            Installer.RemoveComposerFromLocalAppData();
            Installer.RemoveComposerFromPathIfNecessary();
        }

        /// <summary>
        /// Checks if PHP and Composer are already installed, and if they are, what version they are.
        /// </summary>
        private static void HandlePriorCheck() {
            // Detecting architecture
            Console.WriteLine("  * Detected architecture: " + (Environment.Is64BitOperatingSystem ? "64 bit" : "32 bit"));

            // Searching for PHP and Composer
            var phpLocations = OS.FindProgramLocations("php");
            var composerLocations = OS.FindProgramLocations("composer");

            // Check typical installation locations
            if (phpLocations.Contains(defaultPhpLocation) || File.Exists(defaultPhpLocation)) {
                Console.WriteLine("  * Currently installed PHP version: " + PHP.GetPhpVersionByLocation(defaultPhpLocation));
                phpAlreadyInstalled = true;
            }
            if (composerLocations.Contains(defaultComposerLocation) || File.Exists(defaultComposerLocation)) {
                Console.WriteLine("  * Currently installed Composer version: " + Composer.GetComposerVersionByLocation(defaultComposerLocation));
                composerAlreadyInstalled = true;
            }

            // Searching for other PHP and Composer installations, which are not installed by this installer.
            // These installations are only matters if they are in the PATH environment variable, because they
            // may cause conflicts with our installation.
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
            } else {
                Console.WriteLine("  * No problems found.");
            }
        }

        /// <summary>
        /// Handles the PHP release selection.
        /// </summary>
        private static void HandlePhpReleaseSelection() {
            Console.Write("  * Fetching currently supported PHP releases from the official php.net site... ");
            phpReleases = PHP.GetPhpReleases();
            Console.WriteLine("OK.");

            Console.WriteLine("  * Currently, these supported PHP editions are available. Please choose which one");
            Console.WriteLine("    you want to install!");
            if (phpAlreadyInstalled || composerAlreadyInstalled) {
                Console.WriteLine("    ATTENTION! Immediately after the selection, the installer overwrites/updates");
                Console.WriteLine("    the currently installed PHP/Composer in the %LOCALAPPDATA%/Programs directory!");
            }

            var phpVersions = phpReleases.Keys.ToArray();
            Array.Reverse(phpVersions);

            selectedPhpRelease = Selector.ShowSelectorMenu(phpVersions);
            Console.WriteLine("  * Selected: " + selectedPhpRelease);
        }

        /// <summary>
        /// Handles the creation of the temporary directory.
        /// </summary>
        private static void HandleTempDirectory() {
            if (Directory.Exists(Constants.TempDirectory)) {
                OS.DeleteDirectory(Constants.TempDirectory);
                Console.WriteLine($"  * Temp directory ({Constants.TempDirectory}) deleted.");
            }

            Directory.CreateDirectory(Constants.TempDirectory);

            string composerDir = Path.Combine(Constants.TempDirectory, "composer");
            Directory.CreateDirectory(composerDir);
            Console.WriteLine($"  * Temp directory ({composerDir}) created.");
        }

        /// <summary>
        /// Handles the downloading of the PHP and Xdebug binaries.
        /// </summary>
        private static void HandleDownloads() {
            Download.DownloadAndCheckFile(
                "Downloading PHP " + selectedPhpRelease + "... ",
                new Uri(phpReleases[selectedPhpRelease]["downloadlink"]),
                phpReleases[selectedPhpRelease]["checksum"],
                Path.Combine(Constants.TempDirectory, "php.zip")
            );

            if (optionHandler.IsOptionEnabled("xdebug")) {
                var xdebug = Xdebug.GetLatestPackage(selectedPhpRelease, phpReleases[selectedPhpRelease]["builtwith"]);
                xdebugVersion = xdebug["version"];
                Download.DownloadAndCheckFile(
                    "Downloading Xdebug for PHP " + selectedPhpRelease + " " + phpReleases[selectedPhpRelease]["builtwith"].ToUpper() + " NTS... ",
                    new Uri(xdebug["downloadlink"]),
                    xdebug["checksum"],
                    Path.Combine(Constants.TempDirectory, "xdebug.dll")
                );

                Download.DownloadFile(
                    "Downloading Xdebug " + xdebug["version"] + " source code... ",
                    new Uri("https://github.com/xdebug/xdebug/archive/refs/tags/" + xdebug["version"] + ".zip"),
                    Path.Combine(Constants.TempDirectory, "xdebug_src.zip")
                );
            }

            if (optionHandler.IsOptionEnabled("vc-redist")) {
                // This is required for PHP, see https://www.php.net/manual/en/install.windows.requirements.php
                Download.DownloadFile(
                    "Downloading Visual C++ Redistributable " + (Environment.Is64BitOperatingSystem ? "x64" : "x86") + " installer... ",
                    new Uri(
                        Environment.Is64BitOperatingSystem
                            ? "https://aka.ms/vs/16/release/vc_redist.x64.exe"
                            : "https://aka.ms/vs/16/release/vc_redist.x86.exe"
                    ),
                    Path.Combine(Constants.TempDirectory, "vc_redist.exe")
                );
            }

            if (optionHandler.IsOptionEnabled("composer")) {
                Download.DownloadAndCheckFile(
                    "Downloading latest Composer 2.x... ",
                    new Uri(Composer.GetDownloadLinkForLatestVersion()),
                    Composer.GetChecksumForLatestVersion(),
                    Path.Combine(Constants.TempDirectory, "composer.phar")
                );
            }
        }

        /// <summary>
        /// Handles the configuration of the PHP and Xdebug binaries.
        /// </summary>
        private static void HandleConfiguration() {
            // PHP
            Console.Write("  * Extracting PHP program files... ");
            ZipFile.ExtractToDirectory(
                Path.Combine(Constants.TempDirectory, "php.zip"),
                Path.Combine(Constants.TempDirectory, "php")
            );
            Console.WriteLine("OK.");

            if (optionHandler.IsOptionEnabled("xdebug")) {
                Console.Write("  * Extracting Xdebug configuration... ");
                ZipFile.ExtractToDirectory(
                    Path.Combine(Constants.TempDirectory, "xdebug_src.zip"),
                    Path.Combine(Constants.TempDirectory, "xdebug_src")
                );
                File.Copy(
                    Path.Combine(Constants.TempDirectory, "xdebug_src", $"xdebug-{xdebugVersion}", "xdebug.ini"),
                    Path.Combine(Constants.TempDirectory, "xdebug.ini")
                );
                Console.WriteLine("OK.");

                Console.Write("  * Copy xdebug.dll to php/ext directory... ");
                File.Copy(
                    Path.Combine(Constants.TempDirectory, "xdebug.dll"),
                    Path.Combine(Constants.TempDirectory, "php", "ext", "php_xdebug.dll")
                );
                Console.WriteLine("OK.");
            }

            Console.Write("  * Create and configure php.ini... ");
            File.Copy(
                Path.Combine(Constants.TempDirectory, "php", "php.ini-development"),
                Path.Combine(Constants.TempDirectory, "php", "php.ini"),
                true
            );

            string phpIniContent = File.ReadAllText(Path.Combine(Constants.TempDirectory, "php", "php.ini"));
            phpIniContent = phpIniContent.Replace(";extension_dir = \"ext\"", "extension_dir = \"ext\"");
            phpIniContent = phpIniContent.Replace(";extension=curl", "extension=curl");
            phpIniContent = phpIniContent.Replace(";extension=fileinfo", "extension=fileinfo");
            phpIniContent = phpIniContent.Replace(";extension=mbstring", "extension=mbstring");
            phpIniContent = phpIniContent.Replace(";extension=openssl", "extension=openssl");
            phpIniContent = phpIniContent.Replace(";extension=pdo_mysql", "extension=pdo_mysql");
            phpIniContent = phpIniContent.Replace(";extension=pdo_sqlite", "extension=pdo_sqlite");

            // Some students uses MySQL instead of the recommended database manager.
            phpIniContent = phpIniContent.Replace(";extension=mysqli", "extension=mysqli");

            // Fix for PHP 8.2 - previously, the zip extension was enabled by default,
            // but from PHP 8.2 we need to enable it manually.
            phpIniContent = phpIniContent.Replace(";extension=zip", "extension=zip");

            if (optionHandler.IsOptionEnabled("xdebug")) {
                phpIniContent = phpIniContent.Replace(";extension=xsl", ";extension=xsl\nzend_extension=xdebug");

                // See https://xdebug.org/docs/all_settings
                string xdebugDefaultConfig = File.ReadAllText(Path.Combine(Constants.TempDirectory, "xdebug.ini"));
                xdebugDefaultConfig = xdebugDefaultConfig.Replace(" — do not modify by hand", "");
                xdebugDefaultConfig = xdebugDefaultConfig.Replace(";xdebug.cli_color = 0", "xdebug.cli_color = 1");
                xdebugDefaultConfig = xdebugDefaultConfig.Replace(";xdebug.client_host = localhost", "xdebug.client_host = localhost");
                xdebugDefaultConfig = xdebugDefaultConfig.Replace(";xdebug.client_port = 9003", "xdebug.client_port = 9003");
                phpIniContent += "\n[xdebug]\n" + xdebugDefaultConfig;
            }

            File.WriteAllText(Path.Combine(Constants.TempDirectory, "php", "php.ini"), phpIniContent);
            Console.WriteLine("OK.");

            // Composer
            if (optionHandler.IsOptionEnabled("composer")) {
                Console.Write("  * Copy composer.phar to the composer directory... ");
                File.Copy(
                    Path.Combine(Constants.TempDirectory, "composer.phar"),
                    Path.Combine(Constants.TempDirectory, "composer", "composer.phar")
                );
                Console.WriteLine("OK.");

                Console.Write("  * Create runnable .bat file for Composer... ");
                Composer.GenerateComposerBatchFile();
                Console.WriteLine("OK.");
            }
        }

        /// <summary>
        /// Handles the installation of the PHP and Composer binaries. If the user
        /// has enabled --with-vc-redist, the Visual C++ Redistributable will be
        /// installed as well.
        /// </summary>
        private static void HandleInstallation() {
            Console.WriteLine("  * Stop PHP processes that conflicts with this installer... ");
            PHP.KillRunningPhpProcessesByLocation(defaultPhpLocation);

            if (optionHandler.IsOptionEnabled("vc-redist")) {
                Console.Write("  * Run Visual C++ Redistributable Installer... ");
                var proc = new Process {
                    StartInfo = new ProcessStartInfo {
                        FileName = Path.Combine(Constants.TempDirectory, "vc_redist.exe"),
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

            Installer.CopyPhpToLocalAppData();
            Installer.AddPhpToPathIfNecessary();

            if (optionHandler.IsOptionEnabled("composer")) {
                Installer.CopyComposerToLocalAppData();
                Installer.AddComposerToPathIfNecessary();
            }
        }

        /// <summary>
        /// Cleans up the temporary directory if the user didn't specify the --no-cleanup argument.
        /// </summary>
        private static void HandleCleanup() {
            if (optionHandler.IsOptionEnabled("cleanup")) {
                if (Directory.Exists(Constants.TempDirectory)) {
                    OS.DeleteDirectory(Constants.TempDirectory);
                    Console.WriteLine($"  * Temp directory ({Constants.TempDirectory}) deleted.");
                }
            } else {
                Console.WriteLine("  * Skipped.");
            }
        }

        /// <summary>
        /// Handles the installation process.
        /// </summary>
        private static void HandleInstall() {
            Console.WriteLine("Phase 1: Diagnostics");
            Console.WriteLine("---------");
            HandlePriorCheck();
            Console.WriteLine(" ");

            Console.WriteLine("Phase 2: PHP release selection");
            Console.WriteLine("---------");
            HandlePhpReleaseSelection();
            Console.WriteLine(" ");

            Console.WriteLine("Phase 3: Download tools from the Internet");
            Console.WriteLine("---------");
            HandleTempDirectory();
            HandleDownloads();
            Console.WriteLine(" ");

            Console.WriteLine("Phase 4: Configure downloaded tools");
            Console.WriteLine("---------");
            HandleConfiguration();
            Console.WriteLine(" ");

            Console.WriteLine("Phase 5: Installing programs");
            Console.WriteLine("---------");
            HandleInstallation();
            Console.WriteLine(" ");

            Console.WriteLine("Phase 6: Delete temporary files");
            Console.WriteLine("---------");
            HandleCleanup();
            Console.WriteLine(" ");

            Console.WriteLine("Phase 7: Test your installation manually!");
            Console.WriteLine("---------");
            Console.WriteLine("  1.) Open the \"version.bat\" file in the InstallTest directory, then make sure that");
            Console.WriteLine("      the version number of PHP and Composer is displayed on the console.");
            Console.WriteLine(" ");
            Console.WriteLine("  2.) Open the \"phpinfo.bat\" file in the InstallTest directory, which will start a PHP server");
            Console.WriteLine("      and then open it in MS Edge. If this works well, the installation is probably successful.");
            Console.WriteLine("      You can also visit this page if you want to check any PHP settings.");
            Console.WriteLine(" ");
        }

        /// <summary>
        /// Main method of the program.
        /// </summary>
        static void Main(string[] args) {
            try {
                // Do not allow to run the installer multiple times at the same time.
                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1) {
                    throw new Exception("Another instance of the installer is already running.");
                }

                // Initialize the option handler.
                optionHandler = new OptionHandler(options);
                optionHandler.HandleArgs(args);

                // If the user specified the --uninstall argument, we should uninstall the programs.
                // Otherwise, we should install them.
                if (optionHandler.IsOptionEnabled("uninstall")) {
                    HandleUninstall();
                } else {
                    HandleInstall();
                }
            } catch (Exception exception) {
                // If any exception occurs, we want to display the error message and then exit the program.
                Console.WriteLine("Installation tool encountered an error:");
                Console.WriteLine(exception.Message);
            } finally {
                // TODO: Do not wait for keypress if the program is started from the command line.
                // This is only necessary if the program is started from the file explorer.
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
