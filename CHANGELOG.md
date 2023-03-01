# PHP & Composer Installer Changelog

All notable changes to this project will be documented in this file. The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [v1.2.4] - 2023-03-01

### Added
- Add "Contributing" section in README.
- Add "Releasing new versions" section in README.
- Enable `mysqli` extension when installing PHP.

### Changed
- Code refactoring (remove Hungarian comments, add documentation comments, etc.).
- Update license.
- Changelog now uses [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) format.

### Fixed
- Enable `zip` extension when installing PHP. This is required for Composer to work properly with PHP 8.2 and above. Reported by Erik Tűri.
- Rewrite Release CI, because the previous one used deprecated practices.
- Fix nits in README.

## [v1.2.3] - 2022-09-08

### Changed
- Version selector now starts with the latest PHP.
- New download button in README.
- Installing Xdebug and VC Redist is now optional.
  - You can use the following switches for Xdebug/VC Redist installation: `--with-xdebug`, `--with-vc-redist`
  
### Fixed
- Xdebug download link detection fix.

## [v1.2.2] - 2022-07-02

### Changed
- Translate installer to English.

## [v1.2.1] - 2022-04-02

### Changed
- Improve testing tools.
- Improve descriptions.

## [v1.2.0] - 2022-04-02

### Added
- PHP version selector. (Based on PHP's official website.)
- Detect incompatible softwares.
- Uninstall switch: `--uninstall`
- Enabling CURL in PHP config to improve Composer performance.
- Automatic Visual C++ Redistributable installation.
- Checksum-based verification after every file download.
- Testing tools (InstallTest folder).

### Fixed
- Xdebug config fix.

### Changed
- More detailed console output.
- Improve CI.
- Update README.
