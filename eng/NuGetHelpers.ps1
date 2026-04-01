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
    foreach ([string] $id in $packageNames) {
        [string] $pattern = Join-Path $globalPackagesPath "$($id.ToLowerInvariant())*"
        foreach ([System.IO.DirectoryInfo] $cachedPath in (Get-ChildItem $pattern -Directory -ErrorAction SilentlyContinue)) {
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
