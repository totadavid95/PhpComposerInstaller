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
        }

        private static void HandleUninstall() {
            Console.WriteLine("1. fázis: Eltávolítás");
            Console.WriteLine("---------");
            Installer.RemoveFromLocalAppData();
            Installer.RemoveFromPathIfNecessary();
        }

        private static void HandlePriorCheck() {
            // 32 vagy 64 bit
            Console.WriteLine("  * Detektált architektúra: " + (Environment.Is64BitOperatingSystem ? "64 bit" : "32 bit"));

            // Jelenleg telepített verziók vizsgálata
            var phpLocations = OS.FindProgramLocations("php");
            var composerLocations = OS.FindProgramLocations("composer");

            // A telepítő útvonalai
            if (phpLocations.Contains(defaultPhpLocation) || File.Exists(defaultPhpLocation)) {
                Console.WriteLine("  * Jelenleg telepített PHP verzió: " + PHP.GetPhpVersionByLocation(defaultPhpLocation));
                phpAlreadyInstalled = true;
            }
            if (composerLocations.Contains(defaultComposerLocation) || File.Exists(defaultComposerLocation)) {
                Console.WriteLine("  * Jelenleg telepített Composer verzió: " + Composer.GetComposerVersionByLocation(defaultComposerLocation));
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
                Console.WriteLine("  * FIGYELEM! Olyan PHP/Composer telepítéseket érzékeltünk, amelyeket nem ez a telepítő rakott fel,");
                Console.WriteLine("    azonban mivel globális szinten elérhetőek (benne vannak a Path nevű környezeti változóban, ezáltal");
                Console.WriteLine("    úgymond 'bárhonnan' elérhetők), emiatt pedig ütközni fognak a telepítendő verziókkal. A probléma");
                Console.WriteLine("    megoldásához vagy vedd ki őket a Path-ból, vagy nevezd át a php.exe / composer.bat fájlokat valami");
                Console.WriteLine("    másra az alábbi útvonalakon (az alábbi útvonalon lévő telepítések az érintettek):");
                Console.WriteLine(" ");
                if (foreignPhpLocations.Count > 0) {
                    Console.WriteLine("    Detektált PHP telepítések:");
                    foreach (var phpLocation in foreignPhpLocations) {
                        Console.WriteLine("      - " + phpLocation + " (" + PHP.GetPhpVersionByLocation(phpLocation) + ")");
                    }
                    Console.WriteLine(" ");
                }
                if (foreignComposerLocations.Count > 0) {
                    Console.WriteLine("  Detektált Composer  telepítések:");
                    foreach (var composerLocation in foreignComposerLocations) {
                        Console.WriteLine("      - " + composerLocation + " (" + Composer.GetComposerVersionByLocation(composerLocation) + ")");
                    }
                    Console.WriteLine(" ");
                }
            }
            else {
                Console.WriteLine("  * Nem található ütközés");
            }
        }

        private static void HandlePhpReleaseSelection() {
            Console.Write("  * PHP kiadások lekérése a hivatalos php.net oldalról... ");
            phpReleases = PHP.GetPhpReleases();
            Console.WriteLine("OK.");

            Console.WriteLine("  * Jelenleg az alábbi támogatott kiadások érhetők el. Válaszd ki, melyiket szeretnéd telepíteni!");
            if (phpAlreadyInstalled) {
                Console.WriteLine("    FIGYELEM! A telepítés felülírja/frissíti a jelenleg telepített PHP-t, Composer-t!");
            }
            selectedPhpRelease = Selector.ShowSelectorMenu(phpReleases.Keys.ToArray());
            Console.WriteLine("  * Kiválasztva: " + selectedPhpRelease);
        }

        private static void HandleTempDirectory() {
            if (Directory.Exists("PhpComposerInstallerDownloads")) {
                OS.DeleteDirectory("PhpComposerInstallerDownloads");
                Console.WriteLine("  * Korábbi temp könyvtár (PhpComposerInstallerDownloads) törölve.");
            }
            Directory.CreateDirectory("PhpComposerInstallerDownloads");
            Directory.CreateDirectory("PhpComposerInstallerDownloads/composer");
            Console.WriteLine("  * Aktuális temp könyvtár (PhpComposerInstallerDownloads) létrehozva.");
        }

        private static void HandleDownloads() {
            Download.DownloadAndCheckFile(
                "PHP " + selectedPhpRelease + " letöltése... ",
                new Uri(phpReleases[selectedPhpRelease]["downloadlink"]),
                phpReleases[selectedPhpRelease]["checksum"],
                "PhpComposerInstallerDownloads/php.zip"
            );

            var xdebug = Xdebug.GetLatestPackage(selectedPhpRelease, phpReleases[selectedPhpRelease]["builtwith"]);
            xdebugVersion = xdebug["version"];
            Download.DownloadAndCheckFile(
                "Xdebug letöltése PHP " + selectedPhpRelease + " " + phpReleases[selectedPhpRelease]["builtwith"].ToUpper() + " NTS-hez... ",
                new Uri(xdebug["downloadlink"]),
                xdebug["checksum"],
                "PhpComposerInstallerDownloads/xdebug.dll"
            );

            Download.DownloadFile(
                "Xdebug " + xdebug["version"] + " forráskód letöltése... ",
                new Uri("https://github.com/xdebug/xdebug/archive/refs/tags/" + xdebug["version"] + ".zip"),
                "PhpComposerInstallerDownloads/xdebug_src.zip"
            );

            // Ez kell a PHP-hoz, lásd https://www.php.net/manual/en/install.windows.requirements.php
            Download.DownloadFile(
                "Visual C++ Redistributable " + (Environment.Is64BitOperatingSystem ? "x64" : "x86") + " letöltése... ",
                new Uri(
                    Environment.Is64BitOperatingSystem
                        ? "https://aka.ms/vs/16/release/vc_redist.x64.exe"
                        : "https://aka.ms/vs/16/release/vc_redist.x86.exe"
                ),
                "PhpComposerInstallerDownloads/vc_redist.exe"
            );

            Download.DownloadAndCheckFile(
                "A legfrissebb Composer v2 letöltése... ",
                new Uri(Composer.GetDownloadLinkForLatestVersion()),
                Composer.GetChecksumForLatestVersion(),
                "PhpComposerInstallerDownloads/composer.phar"
            );
        }

        private static void HandleConfiguration() {
            // PHP
            Console.Write("  * PHP kicsomagolása... ");
            ZipFile.ExtractToDirectory("PhpComposerInstallerDownloads/php.zip", "PhpComposerInstallerDownloads/php");
            Console.WriteLine("OK.");

            Console.Write("  * Xdebug konfiguráció kinyerése... ");
            ZipFile.ExtractToDirectory("PhpComposerInstallerDownloads/xdebug_src.zip", "PhpComposerInstallerDownloads/xdebug_src");
            File.Copy("PhpComposerInstallerDownloads/xdebug_src/xdebug-" + xdebugVersion + "/xdebug.ini", "PhpComposerInstallerDownloads/xdebug.ini");
            Console.WriteLine("OK.");

            Console.Write("  * xdebug.dll bemásolása a php/ext könyvtárába... ");
            File.Copy("PhpComposerInstallerDownloads/xdebug.dll", "PhpComposerInstallerDownloads/php/ext/php_xdebug.dll");
            Console.WriteLine("OK.");

            Console.Write("  * php.ini létrehozása, beállítása... ");
            File.Copy("PhpComposerInstallerDownloads/php/php.ini-development", "PhpComposerInstallerDownloads/php/php.ini", true);

            string phpIniContent = File.ReadAllText("PhpComposerInstallerDownloads/php/php.ini");
            phpIniContent = phpIniContent.Replace(";extension_dir = \"ext\"", "extension_dir = \"ext\"");
            phpIniContent = phpIniContent.Replace(";extension=curl", "extension=curl");
            phpIniContent = phpIniContent.Replace(";extension=fileinfo", "extension=fileinfo");
            phpIniContent = phpIniContent.Replace(";extension=mbstring", "extension=mbstring");
            phpIniContent = phpIniContent.Replace(";extension=openssl", "extension=openssl");
            phpIniContent = phpIniContent.Replace(";extension=pdo_mysql", "extension=pdo_mysql");
            phpIniContent = phpIniContent.Replace(";extension=pdo_sqlite", "extension=pdo_sqlite");
            phpIniContent = phpIniContent.Replace(";extension=xsl", ";extension=xsl\nzend_extension=xdebug");

            // https://xdebug.org/docs/all_settings
            string xdebugDefaultConfig = File.ReadAllText("PhpComposerInstallerDownloads/xdebug.ini");
            xdebugDefaultConfig = xdebugDefaultConfig.Replace(" — do not modify by hand", "");
            xdebugDefaultConfig = xdebugDefaultConfig.Replace(";xdebug.cli_color = 0", "xdebug.cli_color = 1");
            xdebugDefaultConfig = xdebugDefaultConfig.Replace(";xdebug.client_host = localhost", "xdebug.client_host = localhost");
            xdebugDefaultConfig = xdebugDefaultConfig.Replace(";xdebug.client_port = 9003", "xdebug.client_port = 9003");
            phpIniContent += "\n[xdebug]\n" + xdebugDefaultConfig;

            File.WriteAllText("PhpComposerInstallerDownloads/php/php.ini", phpIniContent);
            Console.WriteLine("OK.");

            // Composer
            Console.Write("  * composer.phar bemásolása a composer könyvtárába... ");
            File.Copy("PhpComposerInstallerDownloads/composer.phar", "PhpComposerInstallerDownloads/composer/composer.phar");
            Console.WriteLine("OK.");

            Console.Write("  * Batch fájl létrehozása a Composerhez... ");
            Composer.GenerateComposerBatchFile();
            Console.WriteLine("OK.");
        }

        private static void HandleInstallation() {
            Console.WriteLine("  * A telepítő által érintett PHP process-ek leállítása (ha vannak)...");
            PHP.KillRunningPhpProcessesByLocation(defaultPhpLocation);

            Console.Write("  * Visual C++ Redistributable telepítő meghívása... ");
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
            Installer.CopyToLocalAppData();
            Installer.AddToPathIfNecessary();
        }

        private static void HandleCleanup() {
            if (!noCleanup) {
                if (Directory.Exists("PhpComposerInstallerDownloads")) {
                    OS.DeleteDirectory("PhpComposerInstallerDownloads");
                    Console.WriteLine("  * Temp könyvtár (PhpComposerInstallerDownloads) törölve.");
                }
            } else {
                Console.WriteLine("  * Kihagyva");
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

                Console.WriteLine("1. fázis: Előzetes ellenőrzés");
                Console.WriteLine("---------");
                HandlePriorCheck();
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine("2. fázis: PHP verzió kiválasztása");
                Console.WriteLine("---------");
                HandlePhpReleaseSelection();
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine("3. fázis: Eszközök letöltése az internetről");
                Console.WriteLine("---------");
                HandleTempDirectory();
                HandleDownloads();
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine("4. fázis: Letöltött állományok konfigurálása");
                Console.WriteLine("---------");
                HandleConfiguration();
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine("5. fázis: Konfigurált programok előkészítése, telepítése");
                Console.WriteLine("---------");
                HandleInstallation();
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine("6. fázis: Ideiglenes fájlok törlése");
                Console.WriteLine("---------");
                HandleCleanup();
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine("7. fázis: Teszteld le a telepítést!");
                Console.WriteLine("---------");
                Console.WriteLine("  1.) Nyisd meg az InstallTest könyvtárban lévő version.bat fájlt, és nézd meg, hogy");
                Console.WriteLine("      kiírja-e a PHP és a Composer verzióját, valamint azt, hogy a PHP-nál szerepel-e");
                Console.WriteLine("      az Xdebug.");
                Console.WriteLine(" ");
                Console.WriteLine("  2.) Nyisd meg az InstallTest könyvtárban lévő phpinfo.bat fájlt, ami elindít egy PHP");
                Console.WriteLine("      szervert, illetve megnyitja azt MS Edge-ben. Ha működik, akkor valószínűleg minden");
                Console.WriteLine("      rendben van, de ezt az oldalt bármilyen konfigurációs kérdés esetén is meg tudod");
                Console.WriteLine("      nyitni.");
                Console.WriteLine(" ");

                // -----------------------------------

                Console.WriteLine(" A telepítö végzett. Nyomj meg egy gombot a kilépéshez...");
                Console.ReadKey();
            }
            catch (Exception ex) {
                Console.WriteLine("HIBA.");
                Console.WriteLine(ex.Message);
            }

        }
    }
}