# Microsoft.ServiceFabric.AspNetCore.Configuration

An ASP.NET Core configuration provider that reads settings from [Service Fabric](https://learn.microsoft.com/azure/service-fabric)
configuration packages and automatically reloads when packages are updated.

## Key APIs

- `AddServiceFabricConfiguration()` — extension method on `IConfigurationBuilder` that reads Service Fabric XML configuration
  packages into the ASP.NET Core `IConfiguration` system.

## Usage

```csharp
class MyService : StatelessService
{
    public MyService(StatelessServiceContext context) : base(context) { }

    protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
    {
        return
        [
            new ServiceInstanceListener(context =>
                new KestrelCommunicationListener(context, "ServiceEndpoint", (url, listener) =>
                    Host.CreateDefaultBuilder()
                        .ConfigureAppConfiguration(config => config.AddServiceFabricConfiguration(context.CodePackageActivationContext))
                        .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>().UseUrls(url))
                        .Build()))
        ];
    }
}
```

## Documentation

- [ASP.NET Core in Service Fabric](https://learn.microsoft.com/azure/service-fabric/service-fabric-reliable-services-communication-aspnetcore)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
