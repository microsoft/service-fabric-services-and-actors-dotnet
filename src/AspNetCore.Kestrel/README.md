# Microsoft.ServiceFabric.AspNetCore.Kestrel

A Kestrel-based communication listener for ASP.NET Core applications on [Service Fabric](https://learn.microsoft.com/azure/service-fabric).

Kestrel is the recommended web server for Service Fabric services.

## Key Types

- `KestrelCommunicationListener` — an `ICommunicationListener` that starts a Kestrel server with dynamic port binding from
  the service manifest.

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
                        .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>().UseUrls(url))
                        .Build()))
        ];
    }
}
```

## Documentation

- [ASP.NET Core in Service Fabric](https://learn.microsoft.com/azure/service-fabric/service-fabric-reliable-services-communication-aspnetcore)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
