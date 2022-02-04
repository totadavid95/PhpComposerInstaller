using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PhpComposerInstaller {
    internal class OS {
        // A where.exe-t hívja meg (beépített Windows szolgáltatás), hogy megkeresse egy parancs
        // előfordulási helyeit a gépen, amennyiben azok regisztrálva vannak a környezeti változókban.
        public static List<string> FindProgramLocations(string name) {
            List<string> locations = new List<string>();
            var proc = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "C:\\Windows\\System32\\where.exe",
                    Arguments = name,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream) {
                string line = proc.StandardOutput.ReadLine()?.TrimEnd(Environment.NewLine.ToCharArray());
                locations.Add(line);
            }
            return locations;
        }

        // Hozzáad egy útvonalat a user szintű Path környezeti változóhoz, ha az még nincs hozzáadva.
        public static bool AddToCurrentUserPath(string path) {
            var currentPathVariable = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

            // Ha a jelenlegi Path változóra null-t kapunk, vagy az útvonal már benne van, akkor
            // be is fejezhetjük a metódust.
            if (currentPathVariable == null || currentPathVariable.ToLower().Contains(path.ToLower())) return false;

            var updatedPathVariable = currentPathVariable + Path.PathSeparator.ToString() + path;
            Environment.SetEnvironmentVariable("Path", updatedPathVariable, EnvironmentVariableTarget.User);
            return true;
        }

        // Eltávolít egy útvonalat a user szintű Path környezeti változóból, ha az hozzá van adva.
        public static bool RemoveFromCurrentUserPath(string pathToRemove) {
            var currentPathVariable = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

            // Ha a jelenlegi Path változóra null-t kapunk, vagy abszolút nincs találat az
            // eltávolítani kívánt útvonalra, akkor felesleges is tovább vizsgálódni.
            if (
                currentPathVariable == null ||
                currentPathVariable.IndexOf(pathToRemove, StringComparison.CurrentCultureIgnoreCase) == -1
            ) {
                return false;
            }

            // Részekre (vagyis az egyes útvonalakra) kell bontani a Path változót a "hivatalos"
            // elválasztó karakter (;) mentén, majd az újraegyesítésből ki kell hagyni azokat az
            // útvonalakat, amelyek kis- és nagybetű érzékenység nélkül egyeznek az eltávolítani
            // kívánt útvonallal.
            var currentPathSplitted = currentPathVariable.Split(Path.PathSeparator);
            var currentPathFiltered = currentPathSplitted.Where(
                path => !path.Equals(pathToRemove, StringComparison.CurrentCultureIgnoreCase)
            );
            var updatedPathVariable = string.Join(
                Path.PathSeparator.ToString(),
                currentPathFiltered
            );
            Environment.SetEnvironmentVariable("Path", updatedPathVariable, EnvironmentVariableTarget.User);
            return true;
        }

        // Különféle könyvtárműveletek
        public static void DeleteDirectory(string path) {
            foreach (string directory in Directory.GetDirectories(path)) {
                DeleteDirectory(directory);
            }
            try {
                Directory.Delete(path, true);
            }
            catch (IOException) {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException) {
                Directory.Delete(path, true);
            }
        }

        // https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists) {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files) {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs) {
                foreach (DirectoryInfo subdir in dirs) {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
