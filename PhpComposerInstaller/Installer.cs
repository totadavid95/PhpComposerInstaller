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
                Console.WriteLine("  * Elözöleg telepített PHP eltávolítva.");
            }

            // Composer
            if (Directory.Exists(programsFolder + @"\composer")) {
                OS.DeleteDirectory(programsFolder + @"\composer");
                Console.WriteLine("  * Elözöleg telepített Composer eltávolítva.");
            }
            OS.DirectoryCopy("PhpComposerInstallerDownloads/php", programsFolder + @"\php", true);
            Console.WriteLine("  * PHP telepítve.");
            OS.DirectoryCopy("PhpComposerInstallerDownloads/composer", programsFolder + @"\composer", true);
            Console.WriteLine("  * Composer telepítve.");
        }

        public static void RemoveFromLocalAppData() {
            string localAppDataFolder = Environment.GetEnvironmentVariable("LocalAppData");
            string programsFolder = localAppDataFolder + @"\Programs";

            // PHP
            if (Directory.Exists(programsFolder + @"\php")) {
                OS.DeleteDirectory(programsFolder + @"\php");
                Console.WriteLine("  * PHP fájljai eltávolítva.");
            }
            else {
                Console.WriteLine("  * A PHP nincs telepítve.");
            }

            // Composer
            if (Directory.Exists(programsFolder + @"\composer")) {
                OS.DeleteDirectory(programsFolder + @"\composer");
                Console.WriteLine("  * Composer fájljai eltávolítva.");
            }
            else {
                Console.WriteLine("  * A Composer nincs telepítve.");
            }
        }

        public static void AddToPathIfNecessary() {
            // PHP
            if (OS.AddToCurrentUserPath(Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\php")) {
                Console.WriteLine("  * A PHP hozzá lett adva a PATH környezeti változóhoz.");
            } else {
                Console.WriteLine("  * A PHP már hozzá van adva a PATH környezeti változóhoz.");
            }

            // Composer
            if (OS.AddToCurrentUserPath(Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\composer")) {
                Console.WriteLine("  * A Composer hozzá lett adva a PATH környezeti változóhoz.");
            } else {
                Console.WriteLine("  * A Composer már hozzá van adva a PATH környezeti változóhoz.");
            }
        }

        public static void RemoveFromPathIfNecessary() {
            // PHP
            if (OS.RemoveFromCurrentUserPath(Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\php")) {
                Console.WriteLine("  * A PHP el lett távolítva a PATH környezeti változóból.");
            } else {
                Console.WriteLine("  * A PHP nem volt hozzáadva a PATH környezeti változóhoz.");
            }

            // Composer
            if (OS.RemoveFromCurrentUserPath(Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\composer")) {
                Console.WriteLine("  * A Composer el lett távolítva a PATH környezeti változóból.");
            } else {
                Console.WriteLine("  * A Composer nem volt hozzáadva a PATH környezeti változóhoz.");
            }
        }
    }
}
