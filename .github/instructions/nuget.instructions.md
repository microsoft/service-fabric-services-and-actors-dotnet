---
description: "Use when upgrading, adding, removing, or reviewing NuGet package references and dependencies."
applyTo: "**/Directory.Packages.props,**/*.csproj,version.json"
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
- **Split the library and test dependencies in separate `Directory.Packages.props`**.
  Prevent accidental leakage of test-only dependencies into the libraries through the central pinning.
  - Place dependencies shared by _both_ libraries and tests in the repo root.
  - Place library-only dependencies like `Nerdbank.GitVersioning` in the `src` folder.
  - Place test-only dependencies like `Microsoft.NET.Test.Sdk` in the `test` folder.
  - Make the library- and test-specific files `<Import Project="..\Directory.Packages.props" />` with shared dependencies.
- **Keep `<PackageVersion/>` and `<PackageReference/>` items sorted**.
  Avoid discrepancies introduced in parallel and reduce merge conflicts.
- **Add end-of-line comments for non-trivial packages and versions**.
  Comments are end-of-line to simplify sorting.
  - Keep lines short:
    - Use acronyms for package names: `x.v.a` instead of `xunit.v3.assert`.
      Most package names already appear in the `.props` file or can be easily determined by `Get-TransitivePackage`.
    - Combine packages with the same dependency version: `(M, M.B.AI)->4.5.4` instead of `M->4.5.4; M.B.AI->4.5.4`.
    - Use wildcards for package families: `M.*->4.5.4` instead of `(M.B.AI, M.E.DI)->4.5.4`.
  - Explain versions held below latest to maintain SemVer contract for transitive dependencies.
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
  - When multiple items need the same condition, move them to an `<ItemGroup Condition="..."/>`.

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

- **Use `Get-TransitivePackage` from `eng/NuGetHelpers.ps1` to understand requested versions**.
  It uses `obj/{ProjectName}/project.assets.json` files to print dependency versions _requested_ by the packages.
  ```powershell
  dotnet restore # Generates project.assets.json files
  . ./eng/NuGetHelpers.ps1
  Get-TransitivePackage 'System.Collections.Immutable' # All projects, all frameworks
  Get-TransitivePackage 'System.Collections.Immutable' -framework 'net472' # Limit to framework
  Get-TransitivePackage 'System.Collections.Immutable' -project 'Microsoft.ServiceFabric.Actors.Tests' # Limit to project
  Get-TransitivePackage 'System.Collections.Immutable' | Group-Object RequestedVersion # For further manipulation
  Get-TransitivePackage 'System.Collections.Immutable' | Format-Table -GroupBy RequestedVersion # For human analysis
  ```

- **Run `dotnet build -bl` and examine `DoubleWrites` to find dependency conflicts**.
  Projects in this repo use shared build output and conflicting dependencies can be quickly detected as double writes.
  - Open the `msbuild.binlog` in the _MSBuild Structured Log Viewer_, and search for `DoubleWrites`.
  - If present, the `DoubleWrites` section will show `.dll` file paths with package names and conflicting versions.

## Rules

- **Check for upgrades whenever the library `version.json` changes**.
  Libraries should use the highest-quality, most secure and performant dependencies available at the time of release.
- **Upgrade direct dependencies to their latest stable versions within [SemVer](https://semver.org/) constraints**.
  Libraries should maintain the SemVer contract transitively. Some packages break the SemVer contract. For example, 
  `Microsoft.Diagnostics.Tracing.TraceEvent -> System.Collections.Immutable` reference `3.1.12 -> 8.0.0` jumped to `3.2.2 -> 9.0.8`.
  - Upgrade to latest _minor_, SemVer-compatible dependency versions before shipping every _minor_ new version of the libraries.
  - Upgrade to latest _major_ dependency versions before shipping every _major_ new version of the libraries.
- **Minimize the combined dependency graph for consumers**.
  Libraries cannot be tested with every possible combination of current and future dependencies that may be used by consumers.
  Minimizing the dependency graph is particularly important for .NET Framework libraries, where failures caused by version 
  mismatch of strongly-named assemblies are more common than the actual breaking changes.
  - Pin conflicting transitive dependencies to the _higher_ of the conflicting versions, which may not be the _latest_.
  - Don't upgrade transitive pins unless they are vulnerable.
  - For dependencies aligned with .NET, build library with matching `TargetFrameworks` and target-specific dependency versions.
    Example: `Microsoft.Extensions.*` package family shipped versions `8.*` with .NET 8 and `10.*` with .NET 10. Library
    referencing them with the `net8.0` TFM should also have the `net10.0` TFM and reference target-specific versions.
- **Upgrade vulnerable and deprecated dependencies to the recommended version**.
  Vulnerable and deprecated dependencies may be reported as build errors and must be upgraded irrespective of the library
  release cycle.
  - _Do_ maintain the SemVer contract while upgrading vulnerable dependencies.
    Most vulnerabilities are addressed by patch versions, which can be referenced at any time.
  - If upgrading library dependencies is not possible under SemVer rules, pin the recommended dependency version for tests.
    Suppose a library with dependency on `Foo` vulnerable version `1.0.0` with recommended upgrade to version `2.0.0`.
    If a new major version of the library cannot be released immediately, `Foo` should be pinned to version `2.0.0` for
    tests only, while the library will continue shipping with dependency on `1.0.0`.
- **Remove unnecessary <PackageVersion/> and <PackageReference/> items**.
  - Projects should have the minimum number of `<PackageReference/>` items required to build.
  - `Directory.Packages.props` files should have the minimum number of `<PackageVersion/>` items required to meet these rules.
- **Re-evaluate every package on any change**.
  Any new package or any new version can change the dependency graph in a way that breaks these rules.
