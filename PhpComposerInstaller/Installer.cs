using System;
using System.IO;

namespace PhpComposerInstaller {
    internal class Installer {
        public static void CopyToLocalAppData() {
            string localAppDataFolder = Environment.GetEnvironmentVariable("LocalAppData");
            string programsFolder = localAppDataFolder + @"\Programs";
            if (!Directory.Exists(programsFolder)) {
                Directory.CreateDirectory(programsFolder);
            }

            // PHP
            if (Directory.Exists(programsFolder + @"\php")) {
                OS.DeleteDirectory(programsFolder + @"\php");
                Console.WriteLine("  * The previously installed PHP has been successfully uninstalled.");
            }

            // Composer
            if (Directory.Exists(programsFolder + @"\composer")) {
                OS.DeleteDirectory(programsFolder + @"\composer");
                Console.WriteLine("  * The previously installed Composer has been successfully uninstalled.");
            }
            OS.DirectoryCopy("PhpComposerInstallerDownloads/php", programsFolder + @"\php", true);
            Console.WriteLine("  * PHP has been successfully installed.");
            OS.DirectoryCopy("PhpComposerInstallerDownloads/composer", programsFolder + @"\composer", true);
            Console.WriteLine("  * Composer has been successfully installed.");
        }

        public static void RemoveFromLocalAppData() {
            string localAppDataFolder = Environment.GetEnvironmentVariable("LocalAppData");
            string programsFolder = localAppDataFolder + @"\Programs";

            // PHP
            if (Directory.Exists(programsFolder + @"\php")) {
                OS.DeleteDirectory(programsFolder + @"\php");
                Console.WriteLine("  * PHP program files have been successfully removed.");
            }
            else {
                Console.WriteLine("  * PHP program files not found, maybe PHP is not installed.");
            }

            // Composer
            if (Directory.Exists(programsFolder + @"\composer")) {
                OS.DeleteDirectory(programsFolder + @"\composer");
                Console.WriteLine("  * Composer program files have been successfully removed.");
            }
            else {
                Console.WriteLine("  * Composer program files not found, maybe PHP is not installed.");
            }
        }

        public static void AddToPathIfNecessary() {
            // PHP
            if (OS.AddToCurrentUserPath(Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\php")) {
                Console.WriteLine("  * PHP was successfully added to the PATH environment variable.");
            } else {
                Console.WriteLine("  * PHP is already added to the PATH environment variable.");
            }

            // Composer
            if (OS.AddToCurrentUserPath(Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\composer")) {
                Console.WriteLine("  * Composer was successfully added to the PATH environment variable.");
            } else {
                Console.WriteLine("  * Composer is already added to the PATH environment variable.");
            }
        }

        public static void RemoveFromPathIfNecessary() {
            // PHP
            if (OS.RemoveFromCurrentUserPath(Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\php")) {
                Console.WriteLine("  * PHP was successfully removed from the PATH environment variable.");
            } else {
                Console.WriteLine("  * PHP is not included in the PATH environment variable.");
            }

            // Composer
            if (OS.RemoveFromCurrentUserPath(Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\composer")) {
                Console.WriteLine("  * Composer was successfully removed from the PATH environment variable.");
            } else {
                Console.WriteLine("  * Composer is not included in the PATH environment variable.");
            }
        }
    }
}
