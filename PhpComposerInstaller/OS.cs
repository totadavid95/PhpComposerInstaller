using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PhpComposerInstaller {
    internal class OS {
        /// <summary>
        /// Finds the locations of the given program by calling the where.exe utility.
        /// </summary>
        public static List<string> FindProgramLocations(string name) {
            List<string> locations = new List<string>();

            var proc = new Process {
                StartInfo = new ProcessStartInfo {
                    // TODO: Remove static C:\Windows\ path.
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

        /// <summary>
        /// Adds the given path to the user's Path environment variable (if it's not already there).
        /// </summary>
        public static bool AddToCurrentUserPath(string path) {
            var currentPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

            // If we get null for the current Path variable, or the path to add is already in the Path,
            // then there is no need to continue.
            if (currentPath == null || currentPath.ToLower().Contains(path.ToLower())) {
                return false;
            }

            string updatedPath;
            string currentPathTrimmed = currentPath.Trim();

            // Do not add extra separator characters if the current Path is empty or already ends with a separator.
            if (currentPathTrimmed == "" || currentPathTrimmed.EndsWith(Path.PathSeparator.ToString())) {
                updatedPath = currentPathTrimmed + path;
            } else {
                updatedPath = currentPathTrimmed + Path.PathSeparator.ToString() + path;
            }

            Environment.SetEnvironmentVariable("Path", updatedPath, EnvironmentVariableTarget.User);
            return true;
        }

        /// <summary>
        /// Removes the given path from the user's Path environment variable (if it's there).
        /// </summary>
        public static bool RemoveFromCurrentUserPath(string pathToRemove) {
            var currentPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

            // If we get null for the current Path variable, or there is no match for the path
            // to remove, then there is no need to continue.
            if (
                currentPath == null ||
                currentPath.IndexOf(pathToRemove, StringComparison.CurrentCultureIgnoreCase) == -1
            ) {
                return false;
            }

            // We need to split the Path variable into its parts (or paths), using the standard
            // separator character (;), and then we need to filter out the paths that match the
            // path to remove, ignoring case sensitivity.
            var currentPathSplitted = currentPath.Split(Path.PathSeparator);

            var currentPathFiltered = currentPathSplitted.Where(
                path => !path.Equals(pathToRemove, StringComparison.CurrentCultureIgnoreCase)
            );

            var updatedPath = string.Join(Path.PathSeparator.ToString(), currentPathFiltered);

            Environment.SetEnvironmentVariable("Path", updatedPath, EnvironmentVariableTarget.User);
            return true;
        }

        /// <summary>
        /// Deletes the given directory and all its contents.
        /// </summary>
        public static void DeleteDirectory(string path) {
            foreach (string directory in Directory.GetDirectories(path)) {
                DeleteDirectory(directory);
            }

            try {
                Directory.Delete(path, true);
            } catch (IOException) {
                Directory.Delete(path, true);
            } catch (UnauthorizedAccessException) {
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// Copies the contents of the source directory to the destination directory.
        /// Source: https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        /// </summary>
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
