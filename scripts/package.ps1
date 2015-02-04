$ErrorActionPreference = "Stop"
.\lib\common-outputs.ps1
.\lib\common.ps1

$version="0.0.1"

# This script will...
#    1. Destroy the ./deployments folder
#    2. Build solution
#    3. Package nuget packages

####################
# Confirm version updates
Write-Section "CONFIRM VERSIONS"
Write-Host "Confirm you have updated the version numbers for publishing in package.ps1"
Write-Host "  Deepcode.CommandLine: ", $version
Write-Host ""
Write-Host "Type YES to continue"
$response = Read-Host
if( $response -ne "YES" )
{
    Write-Error "Aborted - please update versions first"
    return;
}

####################
# Kill output folder
Trash-Deployment-Folder

####################
# Deployment folders
$buildPath = Deployment-Folder build
$packagePath = Deployment-Folder nuget

####################
# Build project
Write-Section "BUILD: Deepcode.CommandLine"
$target = Solution-File "Deepcode.CommandLine"
Build-Project $target $buildPath

####################
# Create nugets
Write-Section "PACKAGE: Deepcode.CommandLine"
$basePath = Deployment-Folder build
Package-Nuget .\nuspecs\Deepcode.CommandLine.nuspec $packagePath $basePath $version


