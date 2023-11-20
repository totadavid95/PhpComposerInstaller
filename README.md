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
  - [Command line switches](#command-line-switches)
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
  - [Contributing](#contributing)
  - [Releasing a new version](#releasing-a-new-version)

## Project description
This installer was primarily created for the students of *Server-side web programming* course taught also by me at *Eötvös Loránd Univerity* in Hungary as a utility that provides a straightforward way to install and set up PHP, Xdebug and Composer under Windows.

The installer does not require administrative privileges since it only installs for the logged in Windows user, therefore it can be executed on the lab computers. This saves us a lot of wasted time and unnecessary headaches.

The installer downloads the latest releases of the aforementioned software, then installs and configures them. If an installation has already been completed, the existing installation is removed first, and a clean installation of the latest version is performed. This mechanism can also be used to fix a broken installation or to perform an update.

The installer does not require administrative privileges since it only installs for the logged in Windows user.

## Command line switches
The utility can optionally be called with the command line switches described below, for example `PhpComposerInstall.exe --no-cleanup`.

Available switches:

- `--uninstall` / `-ui`: completely remove the installation that was previously installed by this installer
- `--no-cleanup` / `-nc`: do not remove the downloaded / temporary files after installation
- `--xdebug` / `-xd`: install and configure Xdebug
- `--no-vc-redist` / `--nvc`: do not install VC Redistributable

## Frequent issues
If the installer or any of the installed software doesn't work properly, the culprit could be one of the following:
- Some antivirus software mistakenly classify one of the installation steps (usually Composer) as malware (also known as a false positive alarm). If you use such software, try suspending its protection and running the installer again. The installer is not "viral", the source code can be found here on GitHub.
- It is possible that some kind of "fluctuation" occurred on your network and the download was interrupted. Try running the installer again, possibly at a later time.
- Using non-English characters in your Windows **username** may lead to errors in some cases, typically when using Tinker. There is already [a video on fixing this issue](https://www.youtube.com/watch?v=_zL3Pxnidj0) (in Hungarian).

### Bug report
If your issue is still unresolved after reading the above, see the [Help and support](#help-and-support) section.

## Manual installation (on Windows)
The installer performs the following steps programmatically.

### Installing PHP
- The official PHP webpage contains the currently supported versions of PHP: [https://windows.php.net/download](https://windows.php.net/download) 
- Download the __x86 / x64 Non Thread Safe__ version of your choice (preferably the latest one). Use the x86 version on 32-bit operating systems and the x64 version for a 64-bit OS.
- Extract the downloaded *.zip* archive.
- PHP requires a configuration file named __php.ini__ which is not included in the archive by default. Create a duplicate of the included __php.ini-development__ (which serves as an example configuration) and rename it __php.ini__.
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

The [installation requirements](https://www.php.net/manual/en/install.windows.requirements.php) specify the Visual C++ Redistributable is needed to run PHP on Windows systems. If PHP gives a "dll error" on first execution, download the appropriate (32 or 64 bit depending on your OS architecture) version of the Visual C++ runtime:
- [vc_redist.x86.exe](https://aka.ms/vs/16/release/vc_redist.x86.exe) (32 bit)
- [vc_redist.x64.exe](https://aka.ms/vs/16/release/vc_redist.x64.exe) (64 bit)

### Installing Xdebug for PHP
- Select the Xdebug version corresponding your installed PHP version from [https://xdebug.org/download](https://xdebug.org/download) from the __Windows binaries__ section.
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
- If a PHP process was running on the machine bound to this PHP configuration, then the process needs to be restarted in order to load the new PHP configuration.

### Install Composer
- Download the latest Composer v2 release be clicking here: [https://getcomposer.org/download/latest-2.x/composer.phar](https://getcomposer.org/download/latest-2.x/composer.phar)
- Copy the downloaded `composer.phar` file to: `%LOCALAPPDATA%\Programs\composer` (create the `composer` folder in `Programs` if it does not exist yet).
- Since this is a PHP archive (PHAR - **PH**p **AR**chive), Windows is not able to execute it directly (like it executes php.exe). We need to create a command file which will make the OS execute this PHAR in the PHP interpreter when opened. This will make the `composer` command available in the command line, so calling Composer will basically execute this `bat` file. Create a `composer.bat` file the __composer__ directory which contains the following one line:
  ```bat
  @php "%~dp0composer.phar" %*
  ```
- This will make the `composer.bat` call PHP to run Composer (which is a PHP archive) forwarding the parameters as well (hence %).

### Adding the installation to the Path environment variable
At this point the installation is almost done, but currently you can only call PHP and Composer by knowing their absolute path.

To make `php` and `composer` simply callable from any open command line, we need to add the **two folders** created inside `%localappdata/Programs` to the Path environment variable.

This is most easily done by typing "Edit environment variables for your account" into the Search bar. Alternatively you can press Win+R and execute `rundll32 sysdm.cpl,EditEnvironmentVariables`.

It is important to note that you only need to work in the upper part of the dialog, since those are the environment variable of the __current user__, and you are not able to modify the system-level properties on the lab machines anyway.

**You'll need to close any previously opened command prompts for the changes to take effect.**

## Testing the installed tools
You can test whether your installations were successful by executing `php -v` and `composer -V`. In both cases you should be able to see the version number of the installed product, and the PHP version command should also display some information about Xdebug.

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

You may now view [http://localhost:3000/](http://localhost:3000/) in a browser. This will display a through report about your PHP environment (including version number and the enabled extensions). Don't forget to close the command line. This will also stop the development server, of course.

There two tests can also be executed using the tools included in the `InstallTest` folder of the installer.

## Manual installation (on Linux)
Our automatic installer only works under Windows systems, but this guide may help you perform the same on Linux operating systems. The process is based on [this tutorial](https://tecadmin.net/how-to-install-php-8-on-ubuntu-20-04/) and has been tested under Xubuntu 20.04.

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

## Contributing

Here is a quick guide on how to contribute to this project.

1. Fork the repository on GitHub.
2. Clone your fork from GitHub to your local machine.
3. Download the latest version of Dotnet from [here](https://dotnet.microsoft.com/en-us/download).
4. You can use any IDE you like. I recommend the lightweight [Visual Studio Code](https://code.visualstudio.com/) with the [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp) installed.
5. Make your changes.
6. Create a build by running `dotnet build` in the root directory. This will compile the code to `PhpComposerInstaller/bin/Debug/PhpComposerInstaller.exe`.
7. Test your changes by running the modified installer.
8. If everything works as expected, structure your changes into commits and push them to your fork.
9. Create a pull request on GitHub from your fork to the original repository. Describe your changes and why you think they should be merged.

## Releasing a new version

If you are a maintainer of this project, you can create a new release simply by pushing a new `v*` tag to the repository. GitHub Actions will automatically builds the installer from the source code and creates a new release. The release will contain the installer executable and the test files.
