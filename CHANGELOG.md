# PHP & Composer Installer Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog][keepachangelog], and this project adheres to [Semantic Versioning][semver].

## [1.2.5] - 2023-12-16

### Added

- Ability to get the installer version with `--version` switch.

### Changed

- Reworked command line option handling.
- Toggleable Composer installation.
- Installing Visual C++ Redistributable is now enabled by default to prevent errors when running PHP.
- Do not allow running the installer when another instance is already running.
- Code refactoring.

### Fixed

- Always waits for key press after the installer is finished.

## [1.2.4] - 2023-03-01

### Added

- Add "Contributing" section in README.
- Add "Releasing new versions" section in README.
- Enable `mysqli` extension when installing PHP.

### Changed

- Code refactoring (remove Hungarian comments, add documentation comments, etc.).
- Update license.
- Changelog now uses [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) format.

### Fixed

- Enable `zip` extension when installing PHP. This is required for Composer to work properly with PHP 8.2 and above.
  Reported by Erik Tűri.
- Rewrite Release CI, because the previous one used deprecated practices.
- Fix nits in README.

## [1.2.3] - 2022-09-08

### Changed

- Version selector now starts with the latest PHP.
- New download button in README.
- Installing Xdebug and VC Redist is now optional.
  - You can use the following switches for Xdebug/VC Redist installation: `--with-xdebug`, `--with-vc-redist`
  
### Fixed

- Xdebug download link detection fix.

## [1.2.2] - 2022-07-02

### Changed

- Translate installer to English.

## [1.2.1] - 2022-04-02

### Changed

- Improve testing tools.
- Improve descriptions.

## [1.2.0] - 2022-04-02

### Added

- PHP version selector. (Based on PHP's official website.)
- Detect incompatible softwares.
- Uninstall switch: `--uninstall`
- Enabling CURL in PHP config to improve Composer performance.
- Automatic Visual C++ Redistributable installation.
- Checksum-based verification after every file download.
- Testing tools (`InstallTest` folder).

### Fixed

- Xdebug config fix.

### Changed

- More detailed console output.
- Improve CI.
- Update README.

[1.2.5]: https://github.com/totadavid95/PhpComposerInstaller/compare/v1.2.4...v1.2.5
[1.2.4]: https://github.com/totadavid95/PhpComposerInstaller/compare/v1.2.3...v1.2.4
[1.2.3]: https://github.com/totadavid95/PhpComposerInstaller/compare/v1.2.2...v1.2.3
[1.2.2]: https://github.com/totadavid95/PhpComposerInstaller/compare/v1.2.1...v1.2.2
[1.2.1]: https://github.com/totadavid95/PhpComposerInstaller/compare/v1.2.0...v1.2.1
[1.2.0]: https://github.com/totadavid95/PhpComposerInstaller/compare/v1.1.0...v1.2.0

[keepachangelog]: https://keepachangelog.com/en/1.0.0/
[semver]: https://semver.org/spec/v2.0.0.html
