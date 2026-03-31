# Code

## Clone
```
git clone https://github.com/microsoft/service-fabric-dotnet.git
cd ./service-fabric-dotnet
```

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

You can run `dotnet` commands in the root of the repo for all projects in the `code.slnx`. You can also run `dotnet`
in a directory containing a specific project you're interested in. For example, `cd test/Services.Remoting` if you want
to build and test only the `Microsoft.ServiceFabric.Services.Remoting.Tests.csproj` and its dependencies.

## Restore

Restore local tools and packages.
```
dotnet tool restore
dotnet restore
```

## Build

Speed up build by skipping restore and building specific projects you're modifying, typically a test project.
```
dotnet build --no-restore
```

## Test
```
dotnet test -c Release -f net10.0
```
The Remoting tests have known failures in the `Debug` configuration, so we use the `--configuration` parameter to run
`Release` tests for all projects. This parameter can be omitted to run `Debug` tests for a specific project.

On Windows, strong name verification must be disabled to avoid `net472` test failures.
Run `init.cmd` or `eng\SkipStrongName.ps1` if you encounter them.

## Pack
```
dotnet pack
```
NuGet packages and PowerShell modules are produced in the [out/packages](./out/packages) directory.

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

## External Contributors
- Find an existing or submit a new [issue](https://github.com/microsoft/service-fabric-dotnet/issues) to discuss your idea with us first.
- Sign the [Microsoft Contributor License Agreement](https://cla.microsoft.com/).
- [Fork](https://docs.github.com/articles/fork-a-repo) this repository and implement your proposal in the fork.

## All Contributors
- Create a [draft pull request](https://docs.github.com/articles/creating-a-pull-request).
- Make sure the validation build completes successfully.
- Link the pull request from the work item/issue you've created.
- Publish the pull request and address the Copilot review feedback.
- Tag the person you were discussing your proposal to review the PR.
