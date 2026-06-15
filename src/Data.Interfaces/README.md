# Microsoft.ServiceFabric.Data.Interfaces

Core abstractions for [Service Fabric](https://learn.microsoft.com/azure/service-fabric) Reliable Collections — transactional,
replicated, persistent data structures available to stateful services.

## Key Types

- `IReliableStateManager` — manages all `IReliableState` instances for a service replica.
- `IReliableDictionary<TKey, TValue>` — a replicated, transactional, persistent key/value store.
- `IReliableQueue<T>` — a replicated, transactional, persistent FIFO queue.
- `IReliableConcurrentQueue<T>` — a replicated, persistent queue with best-effort FIFO ordering.
- `ITransaction` — a unit of work with ACID properties for reading and writing reliable state.
- `IStateSerializer<T>` — custom serializer for values stored in Reliable Collections.

## Usage

Access Reliable Collections from a stateful service:

```csharp
class MyService : StatefulService
{
    public MyService(StatefulServiceContext context) : base(context) { }

    protected override async Task RunAsync(CancellationToken cancellationToken)
    {
        var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("counts");

        using ITransaction tx = StateManager.CreateTransaction();
        await dictionary.AddOrUpdateAsync(tx, "key", 1, (key, value) => value + 1);
        await tx.CommitAsync();
    }
}
```

## Documentation

- [Reliable Collections](https://learn.microsoft.com/azure/service-fabric/service-fabric-reliable-services-reliable-collections)
- [Working with Reliable Collections](https://learn.microsoft.com/azure/service-fabric/service-fabric-work-with-reliable-collections)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
