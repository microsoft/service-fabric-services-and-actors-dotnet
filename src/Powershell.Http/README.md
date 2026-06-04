# Microsoft.ServiceFabric.Powershell.Http

PowerShell module for managing [Service Fabric](https://learn.microsoft.com/azure/service-fabric) clusters over HTTP. Provides
cmdlets for deploying and managing applications, services, nodes, and other cluster resources.

## Key Cmdlets

- `Connect-SFCluster` — connects to a Service Fabric cluster endpoint.
- `Get-SFApplication` / `New-SFApplication` / `Remove-SFApplication` — manage applications.
- `Get-SFService` / `New-SFService` / `Remove-SFService` — manage services.
- `Get-SFNode` — query cluster nodes.
- `Copy-SFApplicationPackage` — upload application packages to the image store.

## Usage

```powershell
# Connect to a local development cluster
Connect-SFCluster -ConnectionEndpoint http://localhost:19080

# List all applications
Get-SFApplication

# Deploy an application
Copy-SFApplicationPackage -ApplicationPackagePath .\MyAppPkg
Register-SFApplicationType -ImageStorePath -ApplicationTypeBuildPath MyAppPkg
New-SFApplication -Name fabric:/MyApp -TypeName MyApp -TypeVersion 1.0.0
```

## Documentation

- [Manage applications using PowerShell](https://learn.microsoft.com/azure/service-fabric/service-fabric-deploy-remove-applications)
- [Connect to a secure cluster](https://learn.microsoft.com/azure/service-fabric/service-fabric-connect-to-secure-cluster)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
