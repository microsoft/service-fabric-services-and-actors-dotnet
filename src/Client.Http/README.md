# Microsoft.ServiceFabric.Client.Http

An HTTP client for connecting to [Service Fabric](https://learn.microsoft.com/azure/service-fabric) clusters and managing
applications, services, partitions, nodes, and other cluster resources.

## Key Types

- `ServiceFabricClientBuilder` — builder for configuring cluster endpoints, security, and retry settings.
- `IServiceFabricClient` — the main client interface, exposing API groups for applications, services, nodes, cluster operations,
  and more.

## Usage

Connect to a local development cluster:

```csharp
IServiceFabricClient client = await new ServiceFabricClientBuilder()
    .UseEndpoints(new Uri("http://localhost:19080"))
    .BuildAsync();

PagedData<NodeInfo> nodes = await client.Nodes.GetNodeInfoListAsync();
```

Connect to a secured cluster:

```csharp
IServiceFabricClient client = await new ServiceFabricClientBuilder()
    .UseEndpoints(new Uri("https://mycluster.eastus.cloudapp.azure.com:19080"))
    .UseX509Security(cancellation => Task.FromResult<SecuritySettings>(
        new X509SecuritySettings(clientCertificate, remoteSecuritySettings)))
    .BuildAsync();
```

## Documentation

- [Service Fabric REST API reference](https://learn.microsoft.com/rest/api/servicefabric)
- [Connect to a secure cluster](https://learn.microsoft.com/azure/service-fabric/service-fabric-connect-to-secure-cluster)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
