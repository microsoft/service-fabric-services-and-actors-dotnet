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

[string] $RuntimeRoot = ValidateDirectoryParameter $RuntimeRoot 'RuntimeRoot'
[string] $SdkRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
[string[]] $RuntimePackages = @(
    'Microsoft.ServiceFabric'
    'Microsoft.ServiceFabric.Data'
    'Microsoft.ServiceFabric.Data.Extensions'
    'Microsoft.ServiceFabric.Data.Interfaces'
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
    [System.IO.FileInfo] $pkg = Get-ChildItem $packagesPath -Filter "$packageName.*.nupkg" |
        Where-Object { $_.Name -notmatch '\.symbols\.' } |
        Select-Object -First 1
    if (-not $pkg) {
        Write-Error "Package '$packageName' not found in '$packagesPath'."
    }
    if ($pkg.BaseName -match '^(.+?)\.(\d+\.\d+\.\d+-.+)$') {
        [string] $version = $Matches[2]
        Write-Host "  $packageName`: $version"
        return $version
    }
    Write-Error "Could not determine version from '$($pkg.Name)'."
}

function UpdateRuntimePackageVersions([string] $propsPath, [string] $packagesPath) {
    [string] $content = Get-Content $propsPath -Raw
    [int] $updated = 0
    Write-Host 'Runtime package versions:'
    foreach ([string] $id in $RuntimePackages) {
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
