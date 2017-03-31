# Azure/service-fabric-services-and-actors-dotnet

Reliable Services and Reliable Actors are Service Fabric application frameworks for building highly-scalable distributed cloud applications.

Reliable Services is a light-weight framework for writing services that integrate with the Service Fabric platform and benefit from the full set of platform features. Built on top of Reliable Services, the Reliable Actor framework is an application framework that implements the Virtual Actor model, based on the actor design pattern. More information on Service Fabric programming models can be found in the [Service Fabric documentation](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-choose-framework).

This repo builds the following packages:
 - Microsoft.ServiceFabric.Services
 - Microsoft.ServiceFabric.Services.Remoting
 - Microsoft.ServiceFabric.Services.Wcf
 - Microsoft.ServiceFabric.Actors
 - Microsoft.ServiceFabric.Actors.Wcf

For more Service Fabric open source projects, visit the Service Fabric [home repo](https://github.com/azure/service-fabric).

## Getting Started

### Prerequesites
Each project is a normal C# Visual Studio 2015 project. At minimum, you need [MSBuild 14](https://www.microsoft.com/download/details.aspx?id=48159), [PowerShell](https://msdn.microsoft.com/powershell/mt173057.aspx), and [.NET Framework 4.5.2](https://www.microsoft.com/download/details.aspx?id=42643) to build and generate NuGet packages. 

We recommend installing [Visual Studio 2015 or Visual Studio 2017](https://www.visualstudio.com/vs/) which will set you up with all the .NET build tools and allow you to open the solution files. Community Edition is free and can be used to build everything here.

### Build
To build everything and generate NuGet packages, run the **build.ps1** script. NuGet packages will be dropped in a *drop* directory at the repo root.

Each project can also be built individually directly through Visual Studio or by running the solution file through MSBuild.

## Development
Please refer to [Readme.md](https://github.com/Azure/service-fabric/blob/master/README.md) at the Service Fabric home repo to learn more about our development process.

## Releases and Support
Official releases from Microsoft of the NuGet packages in this repo are released directly to NuGet and Web Platform Installer. Get the latest official release [here](http://www.microsoft.com/web/handlers/webpi.ashx?command=getinstallerredirect&appid=MicrosoftAzure-ServiceFabric-VS2015).

**Only officially released NuGet packages from Microsoft are supported for use in production.** If you have a feature or bug fix that you would like to use in your application, please issue a pull request so we can get it into an official release. 

## Reporting issues and feedback
Please refer to [Contributing.md](https://github.com/Azure/service-fabric/blob/master/CONTRIBUTING.md) at the Service Fabric home repo for details on issue reporting and feedback.

## Contributing code
If you would like to become an active contributor to this project please
follow the instructions provided in [Microsoft Azure Projects Contribution Guidelines](http://azure.github.io/guidelines.html).

For details on contributing to Service Fabric projects, please refer to [Contributing.md](https://github.com/Azure/service-fabric/blob/master/CONTRIBUTING.md) at the Service Fabric home repo for details on contributing code.

## Documentation
Service Fabric has a rich set of conceptual and reference documentation available at [https://docs.microsoft.com/azure/service-fabric](https://docs.microsoft.com/azure/service-fabric). 

These articles will help get you started with Reliable Services and Reliable Actors:
 
  - [Reliable Services overview](https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-introduction)
  - [Reliable Actors overview](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-actors-introduction)

## Samples
For Service Fabric sample code, check out the [Azure Code Sample gallery](https://azure.microsoft.com/en-us/resources/samples/?service=service-fabric) or go straight to [Azure-Samples on GitHub](https://github.com/Azure-Samples?q=service-fabric).

## License
[MIT](License.txt)

---
*This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.*
 
