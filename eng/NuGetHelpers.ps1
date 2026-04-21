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

function Get-TransitivePackage([Parameter(Mandatory)] [string] $package, [string] $project, [string] $framework) {
    [string] $projectPattern = $project ? $project : '*'
    [string] $pathPattern = "obj\$projectPattern\project.assets.json"
    if (-not (Test-Path $pathPattern)) {
        Write-Error "No project.assets.json files match '$pathPattern'. Run 'dotnet restore' first."
        return
    }
    Get-ChildItem $pathPattern | ForEach-Object {
        [string] $projectName = ($_.FullName -replace '.*\\obj\\(.+)\\project\.assets\.json', '$1')
        [hashtable] $assets = Get-Content $_ -Raw | ConvertFrom-Json -AsHashtable
        foreach ($target in $assets.targets.Keys) {
            [string] $tfm = $target `
                -replace '\.NETFramework,Version=v(\d+)\.(\d+)\.?(\d*)', 'net$1$2$3' `
                -replace '\.NETStandard,Version=v(\d+\.\d+)', 'netstandard$1'
            if ($framework -and $tfm -ne $framework) { continue }
            [hashtable] $packages = $assets.targets[$target]
            foreach ($parentId in $packages.Keys) {
                [hashtable] $parentData = $packages[$parentId]
                [hashtable] $dependencies = $parentData.dependencies
                if ($dependencies -and $dependencies.ContainsKey($package)) {
                    [PSCustomObject] @{
                        Project = $projectName
                        Framework = $tfm
                        RequestedVersion = $dependencies[$package]
                        RequestedBy = $parentId
                    }
                }
            }
        }
    } | Sort-Object Project, Framework, RequestedBy -Unique
}
