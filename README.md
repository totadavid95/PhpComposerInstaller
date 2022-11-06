# Automated PHP, Xdebug, Composer installer

**The latest release can be downloaded by clicking the button:**

[![Download latest](https://img.shields.io/badge/Download%20Installer-32A852?style=for-the-badge&logoColor=white&logo=DocuSign)](https://github.com/totadavid95/PhpComposerInstaller/releases/latest/download/PhpComposerInstaller.zip)

[Kattints ide a MAGYAR nyelvű dokumentációért! (Click here for HUNGARIAN documentation.)](https://github.com/totadavid95/PhpComposerInstaller/blob/master/README_hu.md)

- Always **extract** the downloaded *.zip* file, then run the extracted `PhpComposerInstall.exe` executable. **DO NOT EXECUTE DIRECTLY FROM THE ARCHIVE!**
- The installer requires [.NET 4.7.2](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472) to run. (This requirement is fulfilled on Windows 10 and 11 systems by default.)

This documentation always applies to the latest release only, previous versions can be viewed in the GitHub history.

Table of contents:
- [Automated PHP, Xdebug, Composer installer](#automated-php-xdebug-composer-installer)
  - [Project description](#project-description)
  - [Downloading earlier versions](#downloading-earlier-versions)
  - [How the installer works](#how-the-installer-works)
  - [Command line switches](#command-line-switches)
  - [Motivation](#motivation)
  - [Frequent issues](#frequent-issues)
    - [Bug report](#bug-report)
  - [Manual installation (on Windows)](#manual-installation-on-windows)
    - [Installing PHP](#installing-php)
    - [Installing Visual C++ Redistributable](#installing-visual-c-redistributable)
    - [Installing Xdebug for PHP](#installing-xdebug-for-php)
    - [Install Composer](#install-composer)
    - [Adding the installation to the Path environment variable](#adding-the-installation-to-the-path-environment-variable)
  - [Testing the installed tools](#testing-the-installed-tools)
  - [Manual installation (on Linux)](#manual-installation-on-linux)
  - [Help and support](#help-and-support)

## Project description
This installer was primarily created for the students of *Server-side web programming* course taught also by me at *Eötvös Loránd Univerity* in Hungary as a utility that provides a straightforward way to install and set up PHP, Xdebug and Composer under Windows.

## Downloading earlier versions
All previous versions of the installer can be downloaded on the [Releases](https://github.com/totadavid95/PhpComposerInstaller/releases) page. The `PhpComposerIntaller.zip` file contains the executable while the other two files in a given release contain the source code.

## How the installer works
The installer downloads the latest releases of the aforementioned software, then installs and configures them. 

If an installation has already been completed, the existing installation is removed first, and a clean installation of the latest version is performed. This mechanism can also be used to fix a broken installation or to perform an update.

The installer does not require administrative priviledges since it only installs for the logged in Windows user.

## Command line switches
The utility can optionally be called with the command line switches described below, for example `PhpComposerInstall.exe --no-cleanup`.

Available switches:
- `--uninstall`
  - perform an uninstall without any re-installation
- `--no-cleanup`
  - do not remove the downloaded / temporary files

## Motivation
In order to be able to work and practice properly in classes, a local development environment must be set up on the university lab's computers (and in many cases also on the student's own machines). Setting up this environment so that it is simple enough and still works as intended, even on lab computers (which do not have neither PHP or Composer installed by default), is not an obvious task - and even if it was, it would still take away valuable time from the class unnecessarily. Therefore we have created this installation system which only needs to be executed once and the current user can keep using the required tools on that computer in every class.

## Frequent issues
If the installer or any of the installed software doesn't work properly, the culprit could be one of the following:
- Some antivirus software (e.g. typically Avast) mistakenly classify one of the installation steps (usually Composer) as malware (also known as a false positive alarm). If you use such software, try suspending its protection and running the installer again. The installer is not "viral", the source code can be found here on GitHub, the build is also done here, using Actions. The whole process is transparent and secure.
- It is possible that some kind of "fluctuation" occurred on your network and the installer not connect to one of the servers from which the programs are downloaded, or the connection was interrupted in the meantime, e.g. during download. It's worth trying again, possibly even at a later time.
- Hibákhoz vezethet, tipikusan pl. a Tinker esetében, ha a Windows **felhasználónévben** ékezet van, hiszen ennélfogva minden útvonal ékezetes, ami az adott felhasználóhoz tartozik. Mivel a telepítő a felhasználó szintjén végzi a telepítést, ezért a telepített PHP és Composer szintén érintett. Ennek egy lehetséges megoldására [készítettem egy videót](https://www.youtube.com/watch?v=_zL3Pxnidj0).
- Using non-English characters in your Windows **username** may lead to errors in some cases, typically when using Tinker. Since the installer performs the installation at user level, the installed PHP and Composer can also be affected. There is already [a video on fixing this issue](https://www.youtube.com/watch?v=_zL3Pxnidj0) (in Hungarian).

### Bug report
If your issue is still unresolved after reading the above, see the [Help and support](#help-and-support) section.

## Manual installation (on Windows)
The installer performs the following steps programmatically.

### Installing PHP
- The official PHP webpage contains the currently supported versions of PHP: [https://windows.php.net/download](https://windows.php.net/download) 
- Download the __x86 / x64 Non Thread Safe__ version of your choise (preferably the latest one). Use the x86 version on 32-bit operating systems and the x64 version for a 64-bit OS.
- Extract the downloaded *.zip* archive.
- PHP requires a configuration file names __php.ini__ which is not included in the archive by default. Create a duplicate of the included __php.ini-development__ (which serves as an example configuration) and rename it __php.ini__.
- Open __php.ini__ and perform the following changes:
  - Find the following row:  `;extension_dir = "ext"`, and remove the `;` (comment sign) from the beginning. This will make PHP look for its extensions in its own directory. (Since PHP was installed locally, it would not look for the extensions in the right place by default.)
  - Next we will need to enable some extensions which will be required to run Composer, Laravel, etc properly later. Uncomment the following lines, thereby enabling the given extensions:
    ```ini
    ;extension=curl
    ;extension=fileinfo
    ;extension=mbstring
    ;extension=openssl
    ;extension=pdo_mysql
    ;extension=pdo_sqlite
    ```
- Save the __php.ini__ file when you're done.
- Copy the contents of the `php` folder to:
  `%LOCALAPPDATA%\Programs\php` (create the `php` folder in `Programs` if it does not exist yet).

Hint: copy `%LOCALAPPDATA%\Programs` to the address bar of your file explorer.

### Installing Visual C++ Redistributable

The [installation requirements](https://www.php.net/manual/en/install.windows.requirements.php) specify the Visual C++ Redistributable is needed to run PHP on Windows systems. Ezért, ha a PHP az első futtatáskor "dll hibát" ír, telepítsd az OS architektúrádnak (32 vagy 64 bit) megfelelő Visual C++ runtime library-t:
If PHP gives a "dll error" on first execution, download the appropriate (32 or 64 bit depending on your OS architechure) version of the Visual C++ runtime:
- [vc_redist.x86.exe](https://aka.ms/vs/16/release/vc_redist.x86.exe) (32 bit)
- [vc_redist.x64.exe](https://aka.ms/vs/16/release/vc_redist.x64.exe) (64 bit)

### Installing Xdebug for PHP
- Select the Xdebug version corresponing your installed PHP version from [https://xdebug.org/download](https://xdebug.org/download) from the __Windows binaries__ section. (tartsd szem előtt, hogy x86 (32 bit) / x64 (64 bit) kell-e, a TS / NTS a Thread Safe-t / Non-Thread Safe-t jelenti a végén, az aktuális PHP verziódat és az architektúrát pedig a `php -v` parancs adja meg).
- Download Xdebug as a file named __php_xdebug.dll__.
- Copy the downloaded __php_xdebug.dll__ file to the __ext__ directory of your PHP installation.
- Open __php.ini__. The ini file follows this structure:
  ```ini
  [section_name]
  property_name = property_value
  ```
  - First enable Xdebug as a Zend extension under the `[PHP]` section of the file:
    ```ini
    [PHP]
    ; Here you will find lots of settings and comment, but just 
    ; insert this at the botton of the section before the next [section] begins:
    zend_extension = xdebug
    ``` 
  - Then configure the settings related to Xdebug by inserting a new `[xdebug]` section at the very bottom of your __php.ini__ file:
    ```ini
    [xdebug]
    xdebug.cli_color = 1
    xdebug.client_host = localhost
    xdebug.client_port = 9003
    ```
    - You may find a detailed description of all Xdebug settings in the [official documentation](https://xdebug.org/docs/all_settings).
- Save the __php.ini__ file.
- If a PHP process was running on the machine bound to this PHP configurating, then the process needs to be restarted in order to load the new PHP configuration.

### Install Composer
- Download the latest Composer v2 release be clicking here: [https://getcomposer.org/download/latest-2.x/composer.phar](https://getcomposer.org/download/latest-2.x/composer.phar)
- Copy the downloaded `composer.phar` file to: `%LOCALAPPDATA%\Programs\composer` (create the `composer` folder in `Programs` if it does not exist yet).
- Since this is a PHP archive (PHAR - **PH**p **AR**chive), Windows is not able to execute it directly (like it executes php.exe). We need to create a command file which will make the OS execute this PHAR in the PHP interpreter when opened. This will make the `composer` command available in the command line, so calling Composer will basically execute this `bat` file. Create a `composer.bat` file the __composer__ directory which contains the following one line:
  ```bat
  @php "%~dp0composer.phar" %*
  ```
- This will make the `composer.bat` call PHP to run Composer (which is a PHP archive) forwarding the parameters as well (hence %).

### Adding the installation to the Path environment variable
At this point the installation is almost done, but currently you can only call PHP and Composer by knowing their absolute path. Typing just `php` or `composer` to the command line will give an error since these commands to not exist.

To make `php` and `composer` simply executable from any open command line, we need to add the two folders created inside `%localappdata/Programs` to the Path environment variable. 

This is most easily done by typing "Edit environment variables for your account" into the Search bar. Alternatively you can press Win+R and execute `rundll32 sysdm.cpl,EditEnvironmentVariables`.

It is important to note that you only need to work in the upper part of the dialog, since those are the environment variable of the __current user__, and you are not able to modify the systemwide properies on the lab machines anyway.

**You'll need to exit the previously opened command prompt for the changes to take effect.** The freshly opened command lines will use the modified Path value so you'll be able to use this commands after you've done the setup.

## Testing the installed tools
After successfully completing the operations described above, the `php` and `composer` commands should become available in the command line (you will need to re-open the previously opened ones), and you can try them like this:
`php -v` and `composer -V`

In both cases you should be able to see the version number of the installed product, and the PHP version command should also display some version information about Xdebug.

You may get some more details about your PHP installation by copying the following code into a PHP file (e.g *text.php*):
```php
<?php
    // display all information about the PHP installation
    phpinfo();

    // display all information about the Xdebug installation
    xdebug_info();
?>
```

Now open a command line in the directory of the created `test.php` file (or use `cd` to get there and) and execute:  
`php -S localhost:3000 test.php`

You may now view [http://localhost:3000/](http://localhost:3000/) in a browser.

This will display a through report on PHP's data and settings, for example you can see the version number, list of enabled extensions, if Xdebug is present, etc. Don't forget to close the command line. This will also stop the development server, of course.

There two tests can alse be executed using the tools included in the `InstallTest` folder of the installer.

## Manual installation (on Linux)
Our automatic installer only works under Windows systems, but this guide may help you perform the same on Linux operating systems. The process is based on [this tutorial](https://tecadmin.net/how-to-install-php-8-on-ubuntu-20-04/) and has been tested on Xubuntu 20.04 under VirtualBox.

1. Update the package repository:
   
   ```shell
   sudo apt update
   ```
2. Install a package which helps manage PPAs:
   
   ```shell
   sudo apt install software-properties-common -y 
   ```
3. Add a new [PPA package repository](https://launchpad.net/~ondrej/+archive/ubuntu/php) which manages PHP version:
   
   ```shell
   sudo add-apt-repository ppa:ondrej/php 
   ```
4. Now the system can find how to install PHP, so we can start by installing the PHP interpreter itself. The version number must be entered, but it is also possible to deviate from the version number shown here as an example (at the time of writing the description, 8.1 is the latest), it must be used consistently, and the same must be entered afterwards everywhere.
  
   ```shell
   sudo apt install php8.1
   ```
5. Install the needed extensions for PHP 8.1. These are basically the following, but you can install additional ones if you want:
   
   ```shell
   sudo apt install php8.1-cli php8.1-xml php8.1-curl php8.1-fileinfo php8.1-mbstring php8.1-sqlite3 php8.1-xdebug
   ```
6. Check if PHP works using this command:
   
   ```shell
   php -v
   ```
   - As a result, you should see an output similar to the one below (naturally not with these exact version numbers):
     ```text
     PHP 8.1.2 (cli) (built: Jan 24 2022 10:42:33) (NTS)
     Copyright (c) The PHP Group
     Zend Engine v4.1.2, Copyright (c) Zend Technologies
         with Zend OPcache v8.1.2, Copyright (c), by Zend Technologies
         with Xdebug v3.1.2, Copyright (c) 2002-2021, by Derick Rethans
     ```

7. Install Composer. At this point PHP CLI is available, so you can use the solution provided by the [Composer homepage](https://getcomposer.org/doc/faqs/how-to-install-composer-programmatically.md) to execute this installer script:

    ```shell
    #!/bin/sh

    EXPECTED_CHECKSUM="$(php -r 'copy("https://composer.github.io/installer.sig", "php://stdout");')"
    php -r "copy('https://getcomposer.org/installer', 'composer-setup.php');"
    ACTUAL_CHECKSUM="$(php -r "echo hash_file('sha384', 'composer-setup.php');")"

    if [ "$EXPECTED_CHECKSUM" != "$ACTUAL_CHECKSUM" ]
    then
      >&2 echo 'ERROR: Invalid installer checksum'
      rm composer-setup.php
      exit 1
    fi

    php composer-setup.php --quiet
    RESULT=$?
    rm composer-setup.php
    exit $RESULT
    ```
    - Save the script (for example as `composer.sh`), set the execute priviledge and execute it.
8. The previous step has created the `composer.phar` file which is not available everywhere yet. To make Composer globally available on your machine, copy this newly created file to the `bin` folder:
   
   ```shell
   sudo mv composer.phar /usr/local/bin/composer
   ```
9. Check your Composer installation using the following command:

   ```shell
   composer -V
   ```
## Help and support
If you have any questions about the installer, feel free to ask me and I'll gladly help (totadavid95@inf.elte.hu or Teams chat). If you have a GitHub account, you can also open an issue.