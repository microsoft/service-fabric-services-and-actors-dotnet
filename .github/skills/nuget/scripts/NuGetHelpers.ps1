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
            [hashtable] $resolved = @{}
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
    Get-Packages `
        | Group-Object Framework, Package `
        | Where-Object { ($_.Group.ResolvedVersion | Sort-Object -Unique).Count -gt 1 } `
        | ForEach-Object { $_.Group } `
        | Select-Object Framework, Package, RequestedVersion, RequestedBy, Project `
        | Sort-Object Framework, Package, RequestedVersion, RequestedBy, Project -Unique
}
