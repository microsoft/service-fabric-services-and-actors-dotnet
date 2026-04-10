# Microsoft.ServiceFabric.Actors

A framework for building Reliable Actors - lightweight, single-threaded objects with managed state and lifecycle running
on [Service Fabric](https://learn.microsoft.com/azure/service-fabric).

## Key Types

- `Actor` — base class for actor implementations; provides state management, timers, and reminders.
- `IActor` — marker interface for actor service contracts.
- `ActorService` — the service that hosts and manages actor instances.
- `ActorId` — unique identity for actors (supports `long`, `Guid`, and `string` keys).
- `IRemindable` — implement this interface to receive durable timer-based reminders.
- `ActorProxy` — creates typed proxies to invoke actor methods remotely.

## Usage

Define an actor interface:

```csharp
public interface IMyActor : IActor
{
    Task<int> GetCountAsync();
    Task IncrementAsync();
}
```

Implement the actor:

```csharp
class MyActor : Actor, IMyActor
{
    public MyActor(ActorService actorService, ActorId actorId) : base(actorService, actorId) { }

    public async Task<int> GetCountAsync() =>
        await StateManager.GetOrAddStateAsync("count", 0);

    public async Task IncrementAsync()
    {
        int count = await StateManager.GetOrAddStateAsync("count", 0);
        await StateManager.SetStateAsync("count", count + 1);
    }
}
```

Call the actor from a client:

```csharp
var actor = ActorProxy.Create<IMyActor>(new ActorId(1), new Uri("fabric:/MyApp/MyActorService"));
await actor.IncrementAsync();
int count = await actor.GetCountAsync();
```

## Documentation

- [Reliable Actors](https://learn.microsoft.com/azure/service-fabric/service-fabric-reliable-actors-introduction)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
