# Microsoft.ServiceFabric.AspNetCore.HttpSys

An HTTP.sys-based communication listener for ASP.NET Core applications on [Service Fabric](https://learn.microsoft.com/azure/service-fabric).

HTTP.sys supports Windows authentication and port sharing, making it suitable for scenarios where services share a host
port.

> **Note:** HTTP.sys is only available on Windows.

## Key Types

- `HttpSysCommunicationListener` — an `ICommunicationListener` that starts an HTTP.sys server with dynamic port binding
  from the service manifest.

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
                new HttpSysCommunicationListener(context, "ServiceEndpoint", (url, listener) =>
                    Host.CreateDefaultBuilder()
                        .ConfigureWebHostDefaults(webBuilder => webBuilder.UseHttpSys().UseStartup<Startup>().UseUrls(url))
                        .Build()))
        ];
    }
}
```

## Documentation

- [ASP.NET Core in Service Fabric](https://learn.microsoft.com/azure/service-fabric/service-fabric-reliable-services-communication-aspnetcore)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
