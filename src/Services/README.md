# Microsoft.ServiceFabric.Services

Core programming model for building [Service Fabric](https://learn.microsoft.com/azure/service-fabric) Reliable Services.

## Key Types

- `StatelessService` — base class for stateless services; override `RunAsync` for background processing and `CreateServiceInstanceListeners`
  to expose endpoints.
- `StatefulService` — base class for stateful services with built-in `IReliableStateManager` for managing replicated state.
- `ICommunicationListener` — the interface all communication listeners implement to open, close, and abort endpoints.

## Usage

Create a stateless service:

```csharp
class MyService : StatelessService
{
    public MyService(StatelessServiceContext context) : base(context) { }

    protected override async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // do work
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
```

Create a stateful service:

```csharp
class MyStatefulService : StatefulService
{
    public MyStatefulService(StatefulServiceContext context) : base(context) { }

    protected override async Task RunAsync(CancellationToken cancellationToken)
    {
        var myDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

        while (!cancellationToken.IsCancellationRequested)
        {
            using ITransaction tx = StateManager.CreateTransaction();
            await myDictionary.AddOrUpdateAsync(tx, "counter", 0, (key, value) => ++value);
            await tx.CommitAsync();
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
```

## Documentation

- [Reliable Services](https://learn.microsoft.com/azure/service-fabric/service-fabric-reliable-services-introduction)
- [Samples](https://learn.microsoft.com/samples/browse/?products=azure-service-fabric)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
