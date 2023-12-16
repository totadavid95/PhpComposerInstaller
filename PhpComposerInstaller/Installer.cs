using System;
using System.IO;

namespace PhpComposerInstaller {
    /// <summary>
    /// Installer utility class.
    /// </summary>
    internal class Installer {
        private static readonly string LocalAppDataFolder = Environment.GetEnvironmentVariable("LocalAppData");
        private static readonly string ProgramsFolder = Path.Combine(LocalAppDataFolder, "Programs");

        private static readonly string PhpInstallationPath = Path.Combine(ProgramsFolder, "php");
        private static readonly string ComposerInstallationPath = Path.Combine(ProgramsFolder, "composer");

        /// <summary>
        /// Copies the downloaded files to the local app data folder and removes the previous installation if it exists.
        /// </summary>
        /// <param name="sourceFolder">The source folder containing the downloaded files.</param>
        /// <param name="installationPath">The installation path for the program.</param>
        /// <param name="programName">The name of the program being installed.</param>
        public static void CopyToAndRemoveIfExists(string sourceFolder, string installationPath, string programName) {
            // Create the Programs folder if it does not exist.
            if (!Directory.Exists(ProgramsFolder)) {
                Directory.CreateDirectory(ProgramsFolder);
            }

            // Remove the previous installation if it exists.
            if (Directory.Exists(installationPath)) {
                OS.DeleteDirectory(installationPath);
                Console.WriteLine($"  * The previously installed {programName} has been successfully uninstalled.");
            }

            // Copy the downloaded files to the local app data folder.
            OS.DirectoryCopy(Path.Combine(Constants.TempDirectory, sourceFolder), installationPath, true);
            Console.WriteLine($"  * {programName} has been successfully installed.");
        }

        /// <summary>
        /// Removes the program files from the local app data folder if they exist.
        /// </summary>
        /// <param name="installationPath">The installation path for the program.</param>
        /// <param name="programName">The name of the program being uninstalled.</param>
        public static void RemoveFromLocalAppData(string installationPath, string programName) {
            if (Directory.Exists(installationPath)) {
                OS.DeleteDirectory(installationPath);
                Console.WriteLine($"  * {programName} program files have been successfully removed.");
            } else {
                Console.WriteLine($"  * {programName} program files not found, maybe {programName} is not installed.");
            }
        }

        /// <summary>
        /// Adds the program to the PATH environment variable if it is not already added.
        /// </summary>
        /// <param name="installationPath">The installation path for the program.</param>
        /// <param name="programName">The name of the program being added to the PATH.</param>
        public static void AddToPathIfNecessary(string installationPath, string programName) {
            if (OS.AddToCurrentUserPath(installationPath)) {
                Console.WriteLine($"  * {programName} was successfully added to the PATH environment variable.");
            } else {
                Console.WriteLine($"  * {programName} is already added to the PATH environment variable.");
            }
        }

        /// <summary>
        /// Removes the program from the PATH environment variable if it is added.
        /// </summary>
        /// <param name="installationPath">The installation path for the program.</param>
        /// <param name="programName">The name of the program being removed from the PATH.</param>
        public static void RemoveFromPathIfNecessary(string installationPath, string programName) {
            if (OS.RemoveFromCurrentUserPath(installationPath)) {
                Console.WriteLine($"  * {programName} was successfully removed from the PATH environment variable.");
            } else {
                Console.WriteLine($"  * {programName} is not included in the PATH environment variable.");
            }
        }

        /// <summary>
        /// Copies the downloaded PHP files to the local app data folder and removes the previous installation if it
        /// exists.
        /// </summary>
        public static void CopyPhpToLocalAppData() {
            CopyToAndRemoveIfExists("php", PhpInstallationPath, "PHP");
        }

        /// <summary>
        /// Copies the downloaded Composer files to the local app data folder and removes the previous installation if
        /// it exists.
        /// </summary>
        public static void CopyComposerToLocalAppData() {
            CopyToAndRemoveIfExists("composer", ComposerInstallationPath, "Composer");
        }

        /// <summary>
        /// Removes the PHP files from the local app data folder, if they exist.
        /// </summary>
        public static void RemovePhpFromLocalAppData() {
            RemoveFromLocalAppData(PhpInstallationPath, "PHP");
        }

        /// <summary>
        /// Removes the Composer files from the local app data folder, if they exist.
        /// </summary>
        public static void RemoveComposerFromLocalAppData() {
            RemoveFromLocalAppData(ComposerInstallationPath, "Composer");
        }

        /// <summary>
        /// Adds PHP to the PATH environment variable if it is not already added.
        /// </summary>
        public static void AddPhpToPathIfNecessary() {
            AddToPathIfNecessary(PhpInstallationPath, "PHP");
        }

        /// <summary>
        /// Adds Composer to the PATH environment variable if it is not already added.
        /// </summary>
        public static void AddComposerToPathIfNecessary() {
            AddToPathIfNecessary(ComposerInstallationPath, "Composer");
        }

        /// <summary>
        /// Removes PHP from the PATH environment variable if it is added.
        /// </summary>
        public static void RemovePhpFromPathIfNecessary() {
            RemoveFromPathIfNecessary(PhpInstallationPath, "PHP");
        }

        /// <summary>
        /// Removes Composer from the PATH environment variable if it is added.
        /// </summary>
        public static void RemoveComposerFromPathIfNecessary() {
            RemoveFromPathIfNecessary(ComposerInstallationPath, "Composer");
        }
    }
}
