# Contributing

## Clone
```
git clone https://github.com/microsoft/service-fabric-dotnet.git
cd ./service-fabric-dotnet
```

## Project Structure

| Directory             | Purpose                                                |
|-----------------------|--------------------------------------------------------|
| `src/`                | Product projects (libraries shipped as NuGet packages) |
| `src/Constants`       | Compile-time constants used by other projects          |
| `test/`               | Test projects (xUnit, one per src project)             |
| `test/TestFramework/` | Shared test utilities                                  |
| `properties/`         | Shared MSBuild props, signing key                      |
| `refs/`               | Reference assemblies without NuGet packages            |

Product projects target `net8.0;net462` with the exception of `Client.Http` and `PowerShell.Http` which still target
`netstandard2.0`. Test projects target `net10.0;net9.0;net8.0;net472`.

The `src/` and `test/` directories contain sub-directories named based on the projects within them. For example, `src/Actors`
contains `Microsoft.ServiceFabric.Actors.csproj` and `test/Actors` contains `Microsoft.ServiceFabric.Actors.Tests.csproj`.
We omit the `Microsoft.ServiceFabric` prefix and the `Tests` suffix from directory names.

## Install pre-requisites

### Windows
```
sudo ./init.cmd
```

### Ubuntu
```
chmod +x ./init.sh
sudo ./init.sh
```

## dotnet

- Run the `dotnet` commands in the root of the repo for all projects in the `code.slnx`.
- Run `dotnet` in a directory containing a specific project you're interested in to reduce time and output.
  For example, `cd test/Services.Remoting` if you want to build and test only the `Microsoft.ServiceFabric.Services.Remoting.Tests.csproj`
  and its dependencies.

## Restore

Restore local tools and packages.
```
dotnet tool restore
dotnet restore
```

## Build
```
dotnet build -graph --no-restore
```
- `-graph` eliminates the `MSB3026` warnings from multiple projects trying to copy the same file to the shared build output.
  Note that it prevents MSBuild from using environment variables as properties and you may have to specify them explicitly
  instead, i.e. `-c Release` or `-p PublicRelease=true`.
- `--no-restore` saves a few seconds for every build


## Test
```
dotnet test -c Release -f net10.0
```
- `-c Release` avoids known failures in the `Debug` configuration of some test projects.
- `-f net10.0` or `-f net472` reduces test time by limiting them to a specific .NET framework.
- You must run tests on all frameworks, without the `-f` switch, to verify all code changes.
- On Windows, the `net472` tests will fail unless strong name verification of Service Fabric assemblies was disabled by
  `init.cmd` or `eng\SkipStrongName.ps1`.

You can use `-f net472` or `-f net10.0` to speed up verification during development. Run tests on all
target frameworks before submitting a pull request.

## Pack
```
dotnet pack
```
NuGet packages and PowerShell modules are produced in the [out/packages](./out/packages) directory.

## Build Conventions

- **Central package management**: All package versions defined in `Directory.Packages.props`
- **C# version**: `latestMajor` (set in `Directory.Build.props`)
- **Assembly signing**: Delay-signed with `properties/Key.snk`

## Integrate

You can build and test both Runtime and SDK packages locally.

### Build the Runtime First

```powershell
cd C:\Src
git clone https://msazure@dev.azure.com/msazure/One/_git/WindowsFabric
cd C:\Src\WindowsFabric
.\init.ps1 # Restore
sfbuild -DevBuild $false # Pack
```

### Build SDK packages with local Runtime packages

Open a separate, non-CoreXT shell.
```powershell
cd C:\Src\service-fabric-dotnet
.\eng\ReferenceLocalRuntimePackages.ps1
dotnet restore
dotnet pack -c Debug
```

### Build Runtime packages with local SDK packages

Use the same CoreXT shell you built the Runtime initially.
```powershell
cd C:\Src\WindowsFabric
..\service-fabric-dotnet\eng\ReferenceLocalSdkPackages.ps1
.\init.ps1 # Restore
sfbuild -DevBuild $false # Pack
```

### Note

You must repeat the `Reference` and `Restore` steps above when switching between the SDK and the Runtime builds to make
sure you're building with latest package versions and assemblies.
- Packages must be removed from the cache because version numbers may not change for every local build.
- Package references must be updated because local versions may change based on branch names and commit history.
- The CoreXT shell changes the NuGet package cache location for any projects built from it.
- Building the SDK projects within the CoreXT shell is not supported and may not work.

# Pull Requests

## Service Fabric Engineers
- Find an existing or submit a new [work item](https://dev.azure.com/msazure/One/_backlogs/backlog/Service%20Fabric%20Programming%20Model/Backlog%20items)
  to discuss your idea with us first.
- Join the [service-fabric-write](https://repos.opensource.microsoft.com/orgs/microsoft/teams/service-fabric-write) team.
- Create a branch called `user/{youraccount}/{youproposal}` in your local clone.
- Ask Copilot `/review with multiple models` of your changes.

## External Contributors
- Find an existing or submit a new [issue](https://github.com/microsoft/service-fabric-dotnet/issues) to discuss your idea with us first.
- Sign the [Microsoft Contributor License Agreement](https://cla.microsoft.com/).
- [Fork](https://docs.github.com/articles/fork-a-repo) this repository and implement your proposal in the fork.

## All Contributors
- Create a [draft pull request](https://docs.github.com/articles/creating-a-pull-request).
- Follow the [git commit guidelines](https://cbea.ms/git-commit) to author the PR title and description.
  - Write for the person reading history of the _target_ branch in the future.
  - Describe _what_ it changes and _why_, not _how_ it was developed or which prior PRs it addresses.
  - Title should use the imperative mood and be limited to 72 characters.
  - Don't include textual tags `[MyFeature]` in the title.
- Make sure the validation build completes successfully.
- Link the pull request from the work item/issue you've created.
- Publish the pull request and address the Copilot review feedback.
- Resolve review comment threads after addressing the feedback.
- Tag the person you were discussing your proposal to review the PR.

## Maintainers
- Ask Copilot `/review PR #N with multiple models and post feedback to GitHub`.
- Re-open comment threads if the feedback was not addressed adequately.
- Merge the pull request after approving it.
