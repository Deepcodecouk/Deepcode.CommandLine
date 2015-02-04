$global:msBuildPath = "C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
$global:nugetPath = ".\.nuget\nuget.exe"

########################################################################################################
# Deployment and project file handling.
########################################################################################################

function global:Solution-File
{
    Param(
        [parameter(Mandatory=$true)] [string] $solutionName
        )

    return Resolve-Path "..\src\$solutionName.sln" | Convert-Path
}

##########################
# Deployment-File
#   Obtains an absolute file reference to a file within the
#   temporary ./deployments folder
function global:Deployment-File
{
    Param(
        [parameter(Mandatory=$true)] [string] $fileName
    )

    return Resolve-Path ".\deployments\$fileName" | Convert-Path
}

##########################
# Deployment-Folder
#   Obtains an absolute path reference to a folder within
#   the temporary ./deployments folder. If the folder does
#   not exist, it is created....
function global:Deployment-Folder
{
    Param(
        [parameter(Mandatory=$true)] [string] $folderName
    )

    $folder = ".\deployments\$folderName"
    if( (Test-Path $folder) -eq $false)
    {
        New-Item $folder -ItemType Directory | Out-Null
    }

    return Resolve-Path $folder | Convert-Path
}

function global:Trash-Deployment-Folder
{
    $folder = ".\deployments"

    if( (Test-Path $folder) -eq $true)
    {
        Remove-Item $folder -Recurse
    }
}

########################################################################################################
# NUGET
########################################################################################################

##########################
# Package-Nuget
#    Creates the nuget package specified
#
function global:Package-Nuget
{
    Param(
        [parameter(Mandatory=$true)] [string] $nuspec,
        [parameter(Mandatory=$true)] [string] $output,
        [parameter(Mandatory=$true)] [string] $artefacts,
        [parameter(Mandatory=$true)] [string] $version       
    )

    & $global:nugetPath pack $nuspec -OutputDirectory $output -BasePath $artefacts -Version $version
}

########################################################################################################
# MSBUILD 
########################################################################################################

##########################
# Build-Project
#    Builds the project specified with build outputs going into
#    the output folder specified. If the build fails, the script
#    will be terminated.
function global:Build-Project
{
    Param(
        [parameter(Mandatory=$true)] [string] $project,
        [parameter(Mandatory=$true)] [string] $output,
        [string]$configuration = "Debug",
        [bool]$debugSymbols = $false,
        [string]$debugType = "None"

    )

    & $global:msBuildPath /m $project /p:OutputPath=$output /p:Configuration=$configuration /p:DebugSymbols=$debugSymbols /p:DebugType=$debugType

    if( $LASTEXITCODE -ne 0 )
    {
        Write-Host "Error: Script terminated as Build-Project exited with error code $LASTEXITCODE" -ForegroundColor Red
        Exit
    }
}
