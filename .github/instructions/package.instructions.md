---
description: "Use when upgrading, adding, removing, or reviewing package dependencies."
applyTo: "**/Directory.Packages.props,**/*.csproj"
---

# Package dependency management

This repo uses [Central Package Management](https://learn.microsoft.com/nuget/consume-packages/Central-Package-Management).
`CentralPackageTransitivePinningEnabled` injects every `PackageVersion` into every project, even without a transitive
path. A single pin can indirectly resolve conflicts for its own dependencies across all projects. This also means every
pin has repo-wide impact — a pin added for a test-only package is still injected into all product projects. Keep the
number of pins minimal.

Pin `<PackageVersion/>` of transitive dependencies when:
- An implicitly-resolved package version is deprecated or vulnerable.
- Different projects resolve different package versions, causing double writes to the build output.

Rules:
- Add an end-of-line comment for each pinned `<PackageVersion/>` explaining why it was needed.
- A pin is redundant if another pin already forces that version via its own dependency chain.
- Pin at the highest level that eliminates the most double writes.
- Re-evaluate all pins after upgrading direct dependencies.

## net462 facade assemblies

When a `net462` project references a `netstandard2.0` project, NuGet copies ~100 .NET Standard facade assemblies to the
output. Add `net462` as a target to the referenced project to avoid this.

## Investigating dependency conflicts

1. `dotnet build -bl` then open `msbuild.binlog` in MSBuild Structured Log Viewer, search for `DoubleWrites`.
2. Search `obj/{ProjectName}/project.assets.json` for the conflicting package to find which parent packages request
   which versions. Each match is inside a parent package's dependency block.
   ```shell
   grep -r '"System.Memory"' obj/*/project.assets.json # Linux / Git Bash
   Select-String -Path "obj\*\project.assets.json" -Pattern '"System\.Memory"' # PowerShell
   ```
3. `dotnet nuget why <project.csproj> <package> --framework net462` shows one project's dependency path, but does
   **not** show cross-project conflicts. Run per-project and compare manually.
4. After changes, rebuild with `-bl` and verify no `DoubleWrites` remain.

## Upgrading packages

- Upgrade to latest stable unless there is a specific reason not to.
- Keep prerelease packages at their current version unless explicitly asked to upgrade.
- Avoid major dependency upgrades in minor versions of the Service Fabric SDK.
