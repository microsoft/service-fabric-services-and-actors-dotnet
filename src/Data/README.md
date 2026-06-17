# Microsoft.ServiceFabric.Data

Default `IReliableStateManagerReplica2` implementation for [Service Fabric](https://learn.microsoft.com/azure/service-fabric)
stateful services, providing transactional access to Reliable Collections.

## Key Types

- `ReliableStateManager` — manages `IReliableState` instances (dictionaries, queues) for a service replica, with support
  for backup, restore, and change notifications.

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
- [Backup and restore](https://learn.microsoft.com/azure/service-fabric/service-fabric-reliable-services-backup-restore)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
