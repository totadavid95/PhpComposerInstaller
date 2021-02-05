@echo off
pushd "%~dp0/PhpComposerInstaller"
if exist Debug rd /s /q Debug
if exist Release rd /s /q Release
"%programfiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" /p:Configuration=Release /p:Platform=AnyCPU
:exit
popd
@echo on