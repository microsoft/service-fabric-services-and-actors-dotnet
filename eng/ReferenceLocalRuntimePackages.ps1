<#
.SYNOPSIS
    Configures the SDK repo to reference locally-built Runtime packages.

.DESCRIPTION
    Updates nuget.config and Directory.Packages.props in the SDK repo so that it references
    locally-built Runtime packages, and clears any cached versions of those packages.

    Run this script after cloning both repos and before building. See CONTRIBUTING.md for details.

.PARAMETER RuntimeRoot
    Path to the Runtime (WindowsFabric) repo root. Default: ..\..\WindowsFabric (sibling directory).

.PARAMETER Release
    Use retail (Release) runtime packages instead of debug.

.EXAMPLE
    .\ReferenceLocalRuntimePackages.ps1
    .\ReferenceLocalRuntimePackages.ps1 -RuntimeRoot D:\WindowsFabric
    .\ReferenceLocalRuntimePackages.ps1 -Release
#>
[CmdletBinding()]
param(
    [string] $RuntimeRoot = (Join-Path $PSScriptRoot '..\..\WindowsFabric'),
    [switch] $Release
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

. (Join-Path $PSScriptRoot 'NuGetHelpers.ps1')

$RuntimeRoot = ValidateDirectoryParameter $RuntimeRoot 'RuntimeRoot'
[string] $SdkRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
[string[]] $RuntimePackages = @(
    'Microsoft.ServiceFabric'
)

function Main {
    [string] $runtimePackagesPath = GetRuntimePackagesPath $RuntimeRoot -Release:$Release
    AddNuGetPackageSource (Join-Path $SdkRoot 'nuget.config') 'Local' $runtimePackagesPath
    UpdateRuntimePackageVersions (Join-Path $SdkRoot 'Directory.Packages.props') $runtimePackagesPath
    ClearCachedPackages $RuntimePackages

    Write-Host ''
    Write-Host 'Done. The SDK repo now references locally-built runtime packages.'
}

function GetRuntimePackageVersion([string] $packagesPath, [string] $packageName) {
    [regex] $pattern = "^$packageName\.(\d+\.\d+\.\d+-.+)$"
    [System.IO.FileInfo[]] $packages = @(Get-ChildItem $packagesPath -Filter "*.nupkg" |
        Where-Object { $_.Name -notmatch '\.symbols\.' } |
        Where-Object { $_.BaseName -match $pattern })
    if ($packages.Count -eq 0) {
        Write-Error "Package '$packageName' not found in '$packagesPath'."
    }
    if ($packages.Count -gt 1) {
        Write-Error (
            "Multiple versions of '$packageName' found in '$packagesPath':`n  " + ($packages.Name -join "`n  ") +
            "`nDo a clean build of the runtime to produce a single version.")
    }
    if ($packages[0].BaseName -match $pattern) {
        [string] $version = $Matches[1]
        Write-Host "  $packageName`: $version"
        return $version
    }
    Write-Error "Could not determine version from '$($packages[0].Name)'."
}

function UpdateRuntimePackageVersions([string] $propsPath, [string] $packagesPath) {
    [string] $content = Get-Content $propsPath -Raw
    [int] $updated = 0
    Write-Host 'Runtime package versions:'
    foreach ($id in $RuntimePackages) {
        [string] $version = GetRuntimePackageVersion $packagesPath $id
        [string] $pattern = "((?:Include|id)=`"$([regex]::Escape($id))`"\s+(?:V|v)ersion=`")[^`"]*(`")"
        if ($content -match $pattern) {
            $content = $content -replace $pattern, "`${1}$version`${2}"
            $updated++
        }
    }
    Set-Content $propsPath $content -NoNewline
    if ($updated -gt 0) {
        Write-Host "Updated $propsPath - set $updated package(s)"
    } else {
        Write-Host "No changes needed in $propsPath"
    }
}

Main
