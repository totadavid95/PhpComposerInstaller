// ---------------------------------------------------
// PHP, Xdebug, Composer telepítő
// Készítette: Tóta Dávid
// Kapcsolat: totadavid95@inf.elte.hu
// ---------------------------------------------------

using System;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace szerveroldali_webprog
{
    internal class Program
    {
        private static void Download(string label, Uri address, string dest) {
            Console.Write(label);
            var downloadApi = new DownloadApi();
            downloadApi.DownloadFile(address, dest);
            while (!downloadApi.DownloadCompleted) Thread.Sleep(100);
        }
        
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
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
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
        
        public static bool AddToCurrentUserPath(string path) {
            var currentPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            if (currentPath == null || currentPath.ToLower().Contains(path.ToLower())) {
                return false;
            }
            var updatedPath = currentPath + ";" + path;
            Environment.SetEnvironmentVariable("Path", updatedPath, EnvironmentVariableTarget.User);
            return true;
        }

        public static bool KillRunningPhpProcesses() {
            bool result = false;
            var processes = Process.GetProcessesByName("php");
            foreach (var process in processes) {
                if (process.MainModule != null) {
                    string processPath = process.MainModule.FileName;
                    if (processPath == Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\php\php.exe") {
                        process.Kill();
                        result = true;
                    }
                }
            }
            return result;
        }

        public static void ConfigurePhp() {
            File.Copy("szerveroldali-temp/php-latest/php.ini-development", "szerveroldali-temp/php-latest/php.ini", true);
            string phpIniContent = File.ReadAllText("szerveroldali-temp/php-latest/php.ini");
            
            phpIniContent = phpIniContent.Replace(";extension_dir = \"ext\"", "extension_dir = \"ext\"");
            phpIniContent = phpIniContent.Replace(";extension=fileinfo", "extension=fileinfo");
            phpIniContent = phpIniContent.Replace(";extension=mbstring", "extension=mbstring");
            phpIniContent = phpIniContent.Replace(";extension=openssl", "extension=openssl");
            phpIniContent = phpIniContent.Replace(";extension=pdo_mysql", "extension=pdo_mysql");
            phpIniContent = phpIniContent.Replace(";extension=pdo_sqlite", "extension=pdo_sqlite");
            //phpIniContent += "\n[XDebug]\nxdebug.remote_enable = 1\nxdebug.remote_autostart = 1\nzend_extension=xdebug\n";
            phpIniContent += "\n[xdebug]\nxdebug.client_host = 127.0.0.1\nxdebug.client_port = 9001\n\nxdebug.mode = debug\n";
            
            File.WriteAllText("szerveroldali-temp/php-latest/php.ini", phpIniContent);
        }

        public static void CopyToLocalAppData() {
            string localAppDataFolder = Environment.GetEnvironmentVariable("LocalAppData");
            string programsFolder = localAppDataFolder + @"\Programs";
            if (!Directory.Exists(programsFolder)) {
                Directory.CreateDirectory(programsFolder);
            }
            if (Directory.Exists(programsFolder + @"\php")) {
                DeleteDirectory(programsFolder + @"\php");
                Console.WriteLine("Elözöleg telepített PHP eltávolítva.");
            }
            if (Directory.Exists(programsFolder + @"\composer")) {
                DeleteDirectory(programsFolder + @"\composer");
                Console.WriteLine("Elözöleg telepített Composer eltávolítva.");
            }
            DirectoryCopy("szerveroldali-temp/php-latest", programsFolder + @"\php", true);
            Console.WriteLine("PHP átmásolva.");
            DirectoryCopy("szerveroldali-temp/composer", programsFolder + @"\composer", true);
            Console.WriteLine("Composer átmásolva.");
        }

        public static void AddToPathIfNecessary() {
            // PHP
            if (AddToCurrentUserPath(Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\php")) {
                Console.WriteLine("A PHP hozzáadva a PATH környezeti változóhoz.");
            }
            else {
                Console.WriteLine("A PHP már hozzá van adva a PATH környezeti változóhoz.");
            }
            
            // Composer
            if (AddToCurrentUserPath(Environment.GetEnvironmentVariable("LocalAppData") + @"\Programs\composer")) {
                Console.WriteLine("A Composer hozzáadva a PATH környezeti változóhoz.");
            }
            else {
                Console.WriteLine("A Composer már hozzá van adva a PATH környezeti változóhoz.");
            }   
        }

        public static void UnzipPhp() {
            ZipFile.ExtractToDirectory("szerveroldali-temp/php-latest.zip", "szerveroldali-temp/php-latest");
        }

        public static void GenerateComposerBatchFile() {
            File.WriteAllText("szerveroldali-temp/composer/composer.bat", "@php \"%~dp0composer.phar\" %*\n");
        }
        
        public static void Main(string[] args) {
            Console.WriteLine("A telepítö indul, kérlek várj türelemmel.");

            if (Directory.Exists("szerveroldali-temp"))
            {
                DeleteDirectory("szerveroldali-temp");
                Console.WriteLine("Korábbi temp könyvtár törölve.");
            }
            Directory.CreateDirectory("szerveroldali-temp");
            Directory.CreateDirectory("szerveroldali-temp/composer");
            Console.WriteLine("Aktuális temp könyvtár létrehozva.");

            if (KillRunningPhpProcesses()) {
                Console.WriteLine("Inkompatibilis PHP folyamatok leállítva.");
            }
            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            
            var fetchApi = new FetchApi();

            Download("PHP letöltése... ", fetchApi.GetLatestPhpVersionDownloadLink(), "szerveroldali-temp/php-latest.zip");
            Console.Write("PHP kicsomagolása... ");
            UnzipPhp();
            Console.WriteLine("Kész.");
            Download("XDebug letöltése... ", fetchApi.GetLatestXDebugDownloadLink(), "szerveroldali-temp/php-latest/ext/php_xdebug.dll");
            Download("Composer letöltése... ", new Uri("https://getcomposer.org/composer-stable.phar"), "szerveroldali-temp/composer/composer.phar");
            
            Console.Write("PHP konfigurálása... ");
            ConfigurePhp();
            Console.WriteLine("Kész.");
            
            Console.Write("Composer indítófájl létrehozása... ");
            GenerateComposerBatchFile();
            Console.WriteLine("Kész.");
            
            Console.WriteLine("Programok másolása a telepítés helyére... ");
            CopyToLocalAppData();

            AddToPathIfNecessary();
            
            Console.WriteLine("A telepítö végzett. Nyomj meg egy gombot a kilépéshez...");
            Console.ReadKey();
        }
    }
}