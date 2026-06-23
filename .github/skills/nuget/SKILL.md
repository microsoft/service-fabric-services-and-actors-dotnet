---
name: nuget
description: |
  Upgrade, add, remove, or review NuGet package references and dependencies in this repo. Use when changing `version.json`,
  `Directory.Packages.props`, `<PackageReference/>` elements in `*.csproj` files, or diagnosing dependency conflicts.
argument-hint: "(package name | project | review)"
---

# NuGet Package Guidelines

Definition of terms:
- _Library_ is a project in this repo. Most libraries build NuGet packages.
- _Test_ is a project in this repo. Most test projects test a single library.
- _Dependency_ is a NuGet package consumed by this repo.
- _TFM_ is a `TargetFramework` moniker like `net8.0`, `net462`, etc.

## Organization

- **Use [Central Package Management](https://learn.microsoft.com/nuget/consume-packages/Central-Package-Management)**.
  All libraries must be tested with the same dependencies so that they can all be loaded by the same consumer.
- **Set `CentralPackageTransitivePinningEnabled` to `true`**.
  Transitive dependencies pinned in the `Directory.Packages.props` don't have to be referenced in every project separately.
- **Make `Directory.Build.props` place all build outputs in the root of the repository** to detect transitive dependency
  conflicts as double-writes in the MSBuild structured .binlog and allow this skill to find project assets.
  ```xml
  <BaseOutputPath>$(MSBuildThisFileDirectory)bin\</BaseOutputPath>
  <BaseIntermediateOutputPath>$(MSBuildThisFileDirectory)obj\$(MSBuildProjectName)</BaseIntermediateOutputPath>
  ```
- **Split the library and test dependencies in separate `Directory.Packages.props`**.
  Prevent accidental leakage of test-only dependencies into the libraries through the central pinning.
  - Place dependencies shared by _both_ libraries and tests in the repo root.
  - Place library-only dependencies like `Nerdbank.GitVersioning` in the `src` folder.
  - Place test-only dependencies like `xunit.v3` in the `test` folder.
  - Make the library- and test-specific files `<Import Project="..\Directory.Packages.props" />` with shared dependencies.
- **Keep `<PackageVersion/>` and `<PackageReference/>` items sorted**.
  Avoid discrepancies introduced in parallel and reduce merge conflicts.
- **Add end-of-line comments for non-trivial packages and versions**.
  Comments are end-of-line to simplify sorting.
  - Keep lines short:
    - Use acronyms for package names: `x.v.a` instead of `xunit.v3.assert`.
      Most package names already appear in the `.props` file or can be easily determined by `Get-Packages`.
    - Combine packages with the same dependency version: `(M, M.B.AI)->4.5.4` instead of `M->4.5.4; M.B.AI->4.5.4`.
    - Use wildcards for package families: `M.*->4.5.4` instead of `(M.B.AI, M.E.DI)->4.5.4`.
  - Explain versions held below latest to maintain transitive SemVer contract or minimize the dependency graph.
    `<PackageVersion Include="Microsoft.Diagnostics.Tracing.TraceEvent" Version="3.1.12" /> <!-- 3.2.2 breaks SemVer: S.C.I 8.0.0->9.0.8 -->`
  - Explain transitive pins.
    - Required to resolve conflicts.
      `<PackageVersion Include="System.Collections.Immutable" Version="8.0.0" /> <!-- x.v.a->6.0.0; M.D.T.TE->8.0.0 -->`
    - Required to avoid vulnerabilities.
      `<PackageVersion Include="System.Text.Json" Version="8.0.5" /> <!-- M.E.L.C->8.0.4 vulnerable: CVE-2024-43485 -->`
- **Add explicit `$(TargetFramework)` conditions to every target-specific dependency**.
  Make the target-specific intent explicit, easier to understand and help detect discrepancies as build errors.
  - Make both `<PackageVersion/>` and `<PackageReference/>` items conditional.
  - Add `Condition="'$(TargetFramework)' == '{TFM}'"` for packages used by a single target.
  - Add `Condition="$(TargetFramework.StartsWith('net4'))"` for packages used by multiple .NET Framework targets, such as
    when a `net462` library is tested by a `net472` test.
  - Place `Condition` attribute after `Include` to allow re-sorting the package items quickly.
    E.g. <PackageReference Include="Microsoft.Bcl.Memory" Condition="'$(TargetFramework)' == '{TFM}'"/>
  - When multiple items need the same condition, move them to an `<ItemGroup Condition="..."/>`.
  - Don't create single-item <ItemGroup Condition="..."/>, add `Condition="..."` to the package items instead

## Diagnostics

- **Use NuGet MCP tools to plan version changes**.
  If unavailable, use `dotnet list package --vulnerable` and `dotnet list package --outdated`.

- **Run `dotnet nuget why` to understand dependency chains and target frameworks**.
  Note that it prints the dependency versions _resolved_ for projects and not the versions _requested_ by the packages.
  ```powershell
  dotnet nuget why 'System.Collections.Immutable' # All projects, all frameworks
  dotnet nuget why test/Actors 'System.Collections.Immutable' # Limit to project
  dotnet nuget why test/Actors 'System.Collections.Immutable' -f net472 # Limit to framework
  ```

- **Use `Get-Packages` from `.github/skills/nuget/scripts/NuGetHelpers.ps1` to understand requested versions**.
  It uses `obj/{ProjectName}/project.assets.json` files to emit one row per parent -> dependency edge with both
  the constraint string (`RequestedVersion`) and the version NuGet picked (`ResolvedVersion`).
  ```powershell
  dotnet restore # Generates project.assets.json files
  . ./.github/skills/nuget/scripts/NuGetHelpers.ps1
  Get-Packages | Where-Object Package -eq 'System.Collections.Immutable'          # Limit to package
  Get-Packages | Where-Object Framework -eq 'net472'                              # Limit to framework
  Get-Packages | Where-Object Project -eq 'Microsoft.ServiceFabric.Actors.Tests'  # Limit to project
  Get-Packages | Format-Table -AutoSize  # For human analysis
  ```

- **Use `Get-PackageConflicts` from `.github/skills/nuget/scripts/NuGetHelpers.ps1` to find dependency conflicts**.
  ```powershell
  Get-PackageConflicts | Select-Object Package, RequestedVersion, Framework -Unique # All conflicts
  ```
  Alternatively, open `msbuild.binlog` (produced by `dotnet build -bl`) in the _MSBuild Structured Log Viewer_
  and inspect the synthesized `DoubleWrites` node; it lists `.dll` file paths with conflicting package versions.

## Rules

- **Check for upgrades whenever the library `version.json` changes**.
  Every release should use latest (highest-quality, most secure and performant) dependencies subject to the rules below.
- **Choose direct dependency versions with least transitive conflicts**
  E.g. with existing dependency `D1` -> `T/1.0.0` adding `D2/2.0.0` -> `T/2.0.0` introduces conflict between `T/1.0.0`
  and `T/2.0.0` and requires a transitive pin. If `D2/1.0.0` -> `T/1.0.0` the older `D2` version should be referenced instead.
  - For each direct dependency that introduces transitive conflict, check its older versions, including older major versions
    and find the latest not-vulnerable and not-deprecated version that doesn't.
  - Prefer downgrading direct dependencies to avoid transitive conflicts over resolving them with pinning.
- **Minimize the combined dependency graph for consumers**.
  Libraries cannot be tested with every possible combination of current and future dependencies that may be used by consumers.
  Minimizing the dependency graph is particularly important for .NET Framework libraries, where failures caused by version
  mismatch of strongly-named assemblies are more common than the actual breaking changes.
  - _For dependencies aligned with .NET, build library with matching `TargetFrameworks` and target-specific dependency versions_.
    Example: `Microsoft.Extensions.*` package family shipped versions `8.*` with .NET 8 and `10.*` with .NET 10. Library
    referencing them with the `net8.0` TFM should also have the `net10.0` TFM and reference target-specific versions.
- **Upgrade direct dependencies to their latest stable versions within [SemVer](https://semver.org/) constraints**.
  Libraries should maintain the SemVer contract transitively. Some packages break the SemVer contract. For example,
  `Microsoft.Diagnostics.Tracing.TraceEvent -> System.Collections.Immutable` reference `3.1.12 -> 8.0.0` jumped to `3.2.2 -> 9.0.8`.
  - Upgrade to latest _minor_, SemVer-compatible dependency versions before shipping every _minor_ new version of the libraries.
  - Upgrade to latest _major_ dependency versions before shipping every _major_ new version of the libraries.
- **Upgrade vulnerable and deprecated dependencies to the recommended version**.
  Vulnerable and deprecated dependencies may be reported as build errors and must be upgraded irrespective of the library
  release cycle.
  - _Do_ maintain the SemVer contract while upgrading vulnerable dependencies.
    Most vulnerabilities are addressed by patch versions, which can be referenced at any time.
  - If upgrading library dependencies is not possible under SemVer rules, pin the recommended dependency version for tests.
    Suppose a library with dependency on `Foo` vulnerable version `1.0.0` with recommended upgrade to version `2.0.0`.
    If a new major version of the library cannot be released immediately, `Foo` should be pinned to version `2.0.0` for
    tests only, while the library will continue shipping with dependency on `1.0.0`.
- **Resolve transitive dependency conflicts**
  - _Choose direct dependency versions with least transitive conflicts_ as described above.
  - _Pin remaining conflicting transitive dependencies to the higher of the conflicting versions, which may not be the latest_.
  - _Don't upgrade transitive pins unless they are vulnerable_.
- **Remove unnecessary <PackageVersion/> and <PackageReference/> items**.
  - Projects should have the minimum number of `<PackageReference/>` items required to build.
  - `Directory.Packages.props` files should have the minimum number of `<PackageVersion/>` items required to meet these rules.
- **Revalidate the combined dependency graph after adding, removing or changing any <PackageVersion/> or <PackageReference/>**.
  _Use `Get-PackageConflicts` from `.github/skills/nuget/scripts/NuGetHelpers.ps1` to find dependency conflicts_ as described
  above. Any new package or version in any project can change the combined dependency graph for the repo and break these rules.
