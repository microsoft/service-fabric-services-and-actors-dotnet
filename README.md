# Service Fabric .NET Libraries

.NET libraries for building and managing [Service Fabric](https://learn.microsoft.com/azure/service-fabric) services.

NuGet packages:
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

PowerShell modules:
 - [Microsoft.ServiceFabric.Powershell.Http](https://www.powershellgallery.com/packages/Microsoft.ServiceFabric.Powershell.Http) 

## Getting Started

- [Setup your development environment](https://learn.microsoft.com/azure/service-fabric/service-fabric-get-started).
- [Deploy your first Service Fabric Cluster and .NET Service](https://learn.microsoft.com/azure/service-fabric/service-fabric-quickstart-dotnet).
- [Learn more](https://docs.microsoft.com/azure/service-fabric/service-fabric-choose-framework).
- Find samples [in the docs](https://learn.microsoft.com/samples/browse/?products=azure-service-fabric) or
  [on GitHub](https://github.com/orgs/Azure-Samples/repositories?q=service-fabric&type=all).

## Support

Supported versions of the NuGet packages and PowerShell modules in this repo are available from https://NuGet.org and https://PowerShellGallery.com.

### We don't support custom-built or pre-release versions in production workloads.

Supported versions of the operating systems as well as the Service Fabric and .NET runtimes are listed
[here](https://learn.microsoft.com/azure/service-fabric/service-fabric-versions).
Detailed notes and download links for past releases are available in the 
[microsoft/service-fabric](https://github.com/microsoft/service-fabric/tree/master/release_notes) repo.

For for general questions on using Service Fabric to build and run applications, please use
[Stack Overflow](http://stackoverflow.com/questions/tagged/azure-service-fabric) with tag `azure-service-fabric`.

For live-site problems, check out our [TSGs](https://github.com/Azure/Service-Fabric-Troubleshooting-Guides)
or [submit an Azure support request](https://portal.azure.com/#blade/Microsoft_Azure_Support/HelpAndSupportBlade/newsupportrequest)
to get help from our 24x7 support team. You can also request Azure support to get professional help with general Service
Fabric deployment and development questions.

## Feedback

Please report any security-related issues [privately](./SECURITY.md). Otherwise, we would love to get bug reports and
feature requests for the Service Fabric _.NET libraries_ [here](https://github.com/microsoft/service-fabric-dotnet/issues).
Please submit issues for the larger Service Fabric _runtime_ in the
[microsoft/service-fabric](https://github.com/microsoft/service-fabric/issues) repo.
