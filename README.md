# Service Fabric .NET Libraries

.NET libraries for building and managing [Service Fabric](https://learn.microsoft.com/azure/service-fabric) services.

This repository is a hope of the following NuGet packages:
 - [Microsoft.ServiceFabric.Actors](https://www.nuget.org/packages/Microsoft.ServiceFabric.Actors)
 - [Microsoft.ServiceFabric.Actors.Wcf](https://www.nuget.org/packages/Microsoft.ServiceFabric.Actors.Wcf)
 - [Microsoft.ServiceFabric.AspNetCore.Abstractions](https://www.nuget.org/packages/Microsoft.ServiceFabric.AspNetCore.Abstractions)
 - [Microsoft.ServiceFabric.AspNetCore.Configuration](https://www.nuget.org/packages/Microsoft.ServiceFabric.AspNetCore.Configuration)
 - [Microsoft.ServiceFabric.AspNetCore.HttpSys](https://www.nuget.org/packages/Microsoft.ServiceFabric.AspNetCore.HttpSys)
 - [Microsoft.ServiceFabric.AspNetCore.Kestrel](https://www.nuget.org/packages/Microsoft.ServiceFabric.AspNetCore.Kestrel)
 - [Microsoft.ServiceFabric.Client.Http](https://www.nuget.org/packages/Microsoft.ServiceFabric.Client.Http)
 - [Microsoft.ServiceFabric.Diagnostics.Internal](https://www.nuget.org/packages/Microsoft.ServiceFabric.Diagnostics.Internal)
 - [Microsoft.ServiceFabric.Services](https://www.nuget.org/packages/Microsoft.ServiceFabric.Services)
 - [Microsoft.ServiceFabric.Services.Remoting](https://www.nuget.org/packages/Microsoft.ServiceFabric.Services.Remoting)
 - [Microsoft.ServiceFabric.Services.Wcf](https://www.nuget.org/packages/Microsoft.ServiceFabric.Services.Wcf)

and PowerShell modules:
 - [Microsoft.ServiceFabric.Powershell.Http](https://www.powershellgallery.com/packages/Microsoft.ServiceFabric.Powershell.Http) 

## Getting Started

Follow a [tutorial](https://learn.microsoft.com/azure/service-fabric/service-fabric-quickstart-dotnet) to deploy your
first Service Fabric cluster and .NET service. Learn more about the options for building Service Fabric services in the
[product documentation](https://docs.microsoft.com/azure/service-fabric/service-fabric-choose-framework).


## Releases and Support
Official releases from Microsoft of the NuGet packages in this repo are released directly to NuGet and Web Platform Installer. Get the latest official release [here](http://www.microsoft.com/web/handlers/webpi.ashx?command=getinstallerredirect&appid=MicrosoftAzure-ServiceFabric-VS2015).

**Only officially released NuGet packages from Microsoft are supported for use in production.** If you have a feature or bug fix that you would like to use in your application, please issue a pull request so we can get it into an official release.

## Reporting issues and feedback
Please refer to [Contributing.md](https://github.com/Microsoft/service-fabric/blob/master/CONTRIBUTING.md) at the Service Fabric home repo for details on issue reporting and feedback.

## Contributing code
If you would like to become an active contributor to this project please
follow the instructions provided in [Microsoft Azure Projects Contribution Guidelines](http://azure.github.io/guidelines.html).

For details on contributing to Service Fabric projects, please refer to [Contributing.md](https://github.com/Microsoft/service-fabric/blob/master/CONTRIBUTING.md) at the Service Fabric home repo for details on contributing code.

## How to reflect changes done in Nugets
Nugets from this repo are published via Service Fabric SDK. Once the changes are made in this repo and if there are some changes in nuprojs files, they should reflect in Service Fabric Repo (src\BuildSteps\GenerateNuget\PublicSDK) in respective nuprojs.

## Documentation
Service Fabric has conceptual and reference documentation available at [https://docs.microsoft.com/azure/service-fabric](https://docs.microsoft.com/azure/service-fabric).

These articles will help get you started with Reliable Services and Reliable Actors:

  - [Reliable Services overview](https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-introduction)
  - [Reliable Actors overview](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-actors-introduction)

## Samples
For Service Fabric sample code, check out the [Azure Code Sample gallery](https://azure.microsoft.com/en-us/resources/samples/?service=service-fabric) or go straight to [Azure-Samples on GitHub](https://github.com/Azure-Samples?q=service-fabric).

## License
[MIT](License.txt)

---
*This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.*
 
