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

## Build
```
dotnet build
```

## Test
```
dotnet test -c Release -f net10.0
```
The Remoting tests have known failures in the `Debug` configuration, so we use the `--configuration` parameter to run
`Release` tests for all projects. This parameter can be omitted to run `Debug` tests for a specific project.

On Linux, we specify the `--framework` parameter explicitly to avoid `net472` tests failures.
This parameter can be omitted on Windows.

On Windows, strong name verification must be disabled to avoid `net472` test failures.
Run `init.cmd` or `SkipStrongName.ps1` if you encounter them.

## Pack
```
dotnet pack
```
NuGet packages and PowerShell modules are produced in the [out/packages](./out/packages) directory.

# Understand

Agent- and human-readable instructions are available in the [.github](.github/) folder.

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
- Create a [draft pull request](https://docs.github.com/articles/creating-a-pull-request) and make sure the validation
  build completes successfully.
- Link the pull request from the work item/issue you've created.
- Publish the pull request and tag the person you were discussing it to review it.
