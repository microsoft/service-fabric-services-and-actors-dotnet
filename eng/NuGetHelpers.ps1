function ValidateDirectoryParameter([string] $path, [string] $parameterName) {
    [string] $fullPath = [System.IO.Path]::GetFullPath($path)
    if (-not (Test-Path -Path $fullPath -PathType Container)) {
        Write-Error "Specify -$parameterName explicitly because the default directory '$fullPath' doesn't exist or is not a directory."
    }
    return $fullPath
}

function GetRuntimePackagesPath([string] $runtimeRoot, [switch] $release) {
    [string] $flavor = if ($release) { 'retail' } else { 'debug' }
    [string] $path = Join-Path $runtimeRoot "out\packages\PublicSDK\$flavor-amd64"

    if (-not (Test-Path $path)) {
        Write-Error "Runtime packages not found at '$path'. Build the runtime first: cd `"$runtimeRoot`" && .\init.ps1 && sfbuild."
    }
    return $path
}

function ClearCachedPackages([string[]] $packageNames) {
    [string] $globalPackagesPath = (& dotnet nuget locals global-packages --list) -replace '^.*:\s*', ''
    foreach ($id in $packageNames) {
        [string] $pattern = Join-Path $globalPackagesPath "$($id.ToLowerInvariant())*"
        foreach ($cachedPath in (Get-ChildItem $pattern -Directory -ErrorAction SilentlyContinue)) {
            Remove-Item $cachedPath.FullName -Recurse -Force
            Write-Host "Cleared cached package: $($cachedPath.FullName)"
        }
    }
}

function AddNuGetPackageSource([string] $configPath, [string] $key, [string] $packagesPath) {
    [string] $content = Get-Content $configPath -Raw
    [string] $entry = "    <add key=`"$key`" value=`"$packagesPath`" />"
    if ($content -match "<add\s+key=`"$key`"") {
        $content = $content -replace "<add\s+key=`"$key`"\s+value=`"[^`"]*`"\s*/>", "<add key=`"$key`" value=`"$packagesPath`" />"
    } else {
        $content = $content -replace '(\s*</packageSources>)', "`n$entry`$1"
    }
    Set-Content $configPath $content -NoNewline
    Write-Host "Updated $configPath"
}

function Get-Packages {
    # Emits one row per (project, framework, parent -> dependency) edge found in obj\<Project>\project.assets.json.
    # RID-qualified targets are skipped because they list the same managed packages as the RID-less target.
    [string] $pathPattern = 'obj\*\project.assets.json'
    if (-not (Test-Path $pathPattern)) {
        Write-Error "No project.assets.json files match '$pathPattern'. Run 'dotnet restore' first."
        return
    }
    Get-ChildItem $pathPattern | ForEach-Object {
        [string] $proj = ($_.FullName -replace '.*\\obj\\(.+)\\project\.assets\.json', '$1')
        [hashtable] $assets = Get-Content $_ -Raw | ConvertFrom-Json -AsHashtable
        foreach ($target in $assets.targets.Keys) {
            if ($target -match '/') { continue }
            [string] $tfm = $target `
                -replace '\.NETFramework,Version=v(\d+)\.(\d+)\.?(\d*)', 'net$1$2$3' `
                -replace '\.NETStandard,Version=v(\d+\.\d+)', 'netstandard$1'
            [hashtable] $packages = $assets.targets[$target]
            # Map "<packageId>" -> resolved "<version>" within this (project, framework) target
            $resolved = @{}
            foreach ($id in $packages.Keys) {
                if ($packages[$id].type -ne 'package') { continue }
                [string] $name, [string] $version = $id -split '/', 2
                $resolved[$name] = $version
            }
            foreach ($parentId in $packages.Keys) {
                [hashtable] $deps = $packages[$parentId].dependencies
                if (-not $deps) { continue }
                foreach ($depId in $deps.Keys) {
                    if (-not $resolved.ContainsKey($depId)) { continue }
                    [PSCustomObject] @{
                        Project = $proj
                        Framework = $tfm
                        Package = $depId
                        ResolvedVersion = $resolved[$depId]
                        RequestedVersion = $deps[$depId]
                        RequestedBy = $parentId
                    }
                }
            }
        }
    }
}

function Get-PackageConflicts {
    # Detect packages resolved to multiple versions within the same target framework, and emit one
    # row per requester. Such conflicts cause double writes in the shared output directory
    # bin\<Configuration>\<TargetFramework>\.
    # See .github/instructions/nuget.instructions.md for why this repo uses a shared output directory.
    Get-Packages `
        | Group-Object Framework, Package `
        | Where-Object { ($_.Group.ResolvedVersion | Sort-Object -Unique).Count -gt 1 } `
        | ForEach-Object { $_.Group } `
        | Select-Object Framework, Package, RequestedVersion, RequestedBy, Project `
        | Sort-Object Framework, Package, RequestedVersion, RequestedBy, Project -Unique
}
