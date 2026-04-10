# Microsoft.ServiceFabric.Services.Remoting

Remoting framework for [Service Fabric](https://learn.microsoft.com/azure/service-fabric) Reliable Services, enabling type-safe
RPC communication between services.

## Key Types

- `IService` — marker interface for remotable service contracts.
- `ServiceProxy` — creates typed proxies to invoke remote service methods.
- `ServiceProxyFactory` — factory for creating service proxies with custom configuration.
- `IServiceRemotingListener` — listener for handling incoming remoting calls.

## Usage

Define a service contract:

```csharp
public interface IMyService : IService
{
    Task<string> GetGreetingAsync(string name);
}
```

Implement the contract in a service:

```csharp
class MyService : StatelessService, IMyService
{
    public MyService(StatelessServiceContext context) : base(context) { }

    public Task<string> GetGreetingAsync(string name) =>
        Task.FromResult($"Hello, {name}!");

    protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners() =>
        this.CreateServiceRemotingInstanceListeners();
}
```

Call the service from a client:

```csharp
var proxy = ServiceProxy.Create<IMyService>(new Uri("fabric:/MyApp/MyService"));
string greeting = await proxy.GetGreetingAsync("World");
```

## Documentation

- [Service remoting overview](https://learn.microsoft.com/azure/service-fabric/service-fabric-reliable-services-communication-remoting)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
