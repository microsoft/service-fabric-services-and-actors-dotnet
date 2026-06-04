<#
.SYNOPSIS
    Configures the Runtime repo to reference locally-built SDK packages.

.DESCRIPTION
    Updates NuGet.config, corext.config, and Directory.Packages.props in the Runtime repo so that
    it references locally-built SDK packages. Also adds a NuGet source for locally-built Runtime
    packages because the SDK packages depend on them.

    Run this script from a CoreXT shell. See CONTRIBUTING.md for details.

.PARAMETER Release
    Use retail (Release) runtime packages instead of debug.

.EXAMPLE
    .\ReferenceLocalSdkPackages.ps1
    .\ReferenceLocalSdkPackages.ps1 -Release
#>
[CmdletBinding()]
param(
    [switch] $Release
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

. (Join-Path $PSScriptRoot 'NuGetHelpers.ps1')

if (-not $env:ROOT) {
    Write-Error 'Run this script from a CoreXT shell.'
}

[string] $RuntimeRoot = $env:ROOT
[string] $SdkRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
[string[]] $SdkPackages = @(
    'Microsoft.ServiceFabric.Actors'
    'Microsoft.ServiceFabric.Actors.Wcf'
    'Microsoft.ServiceFabric.AspNetCore.Abstractions'
    'Microsoft.ServiceFabric.AspNetCore.Configuration'
    'Microsoft.ServiceFabric.AspNetCore.HttpSys'
    'Microsoft.ServiceFabric.AspNetCore.Kestrel'
    'Microsoft.ServiceFabric.Client.Http'
    'Microsoft.ServiceFabric.Data'
    'Microsoft.ServiceFabric.Data.Interfaces'
    'Microsoft.ServiceFabric.Data.Interfaces.V2'
    'Microsoft.ServiceFabric.Diagnostics.Internal'
    'Microsoft.ServiceFabric.FabricTransport.Internal'
    'Microsoft.ServiceFabric.Powershell.Http'
    'Microsoft.ServiceFabric.Services'
    'Microsoft.ServiceFabric.Services.Remoting'
    'Microsoft.ServiceFabric.Services.Wcf'
)

function Main {
    [string] $sdkPackagesPath = Join-Path $SdkRoot 'out\packages'
    [string] $runtimePackagesPath = GetRuntimePackagesPath $RuntimeRoot -Release:$Release
    [string] $sdkVersion = GetSdkPackageVersion

    AddCorextPackageSource (Join-Path $RuntimeRoot '.corext\corext.config') $sdkPackagesPath
    AddNuGetPackageSource (Join-Path $RuntimeRoot 'NuGet.config') 'LocalSdk' $sdkPackagesPath
    AddNuGetPackageSource (Join-Path $RuntimeRoot 'NuGet.config') 'LocalRuntime' $runtimePackagesPath
    UpdateSdkPackageVersions (Join-Path $RuntimeRoot '.corext\corext.config') $sdkVersion
    UpdateSdkPackageVersions (Join-Path $RuntimeRoot 'src\Directory.Packages.props') $sdkVersion
    ClearCachedPackages $SdkPackages

    Write-Host ''
    Write-Host 'Done. The Runtime repo now references locally-built SDK packages.'
    Write-Host 'Please run init.ps1 before building it again.'
}

function GetSdkPackageVersion {
    Push-Location $SdkRoot
    try {
        [string] $version = & dotnet nbgv get-version -v NuGetPackageVersion
        if ($LASTEXITCODE -ne 0) {
            Write-Error 'nbgv failed to determine package version. Restore it with: dotnet tool restore'
        }
        Write-Host "SDK package version: $version"
        return $version
    } finally {
        Pop-Location
    }
}

function AddCorextPackageSource([string] $configPath, [string] $packagesPath) {
    [string] $content = Get-Content $configPath -Raw
    [string] $entry = "    <repo name=`"Local`" uri=`"$packagesPath`" />"
    if ($content -match '<repo\s+name="Local"') {
        $content = $content -replace '<repo\s+name="Local"\s+uri="[^"]*"\s*/>', "<repo name=`"Local`" uri=`"$packagesPath`" />"
    } else {
        $content = $content -replace '(\s*</repositories>)', "`n$entry`$1"
    }
    Set-Content $configPath $content -NoNewline
    Write-Host "Updated $configPath"
}

function UpdateSdkPackageVersions([string] $propsPath, [string] $version) {
    [string] $content = Get-Content $propsPath -Raw
    [int] $updated = 0
    foreach ($id in $SdkPackages) {
        # Match both Directory.Packages.props (Include=, Version=) and corext.config (id=, version=) syntax
        [string] $pattern = "((?:Include|id)=`"$([regex]::Escape($id))`"\s+(?:V|v)ersion=`")[^`"]*(`")"
        if ($content -match $pattern) {
            $content = $content -replace $pattern, "`${1}$version`${2}"
            $updated++
        }
    }
    Set-Content $propsPath $content -NoNewline
    if ($updated -gt 0) {
        Write-Host "Updated $propsPath - set $updated package(s) to version $version"
    } else {
        Write-Host "No changes needed in $propsPath"
    }
}

Main
