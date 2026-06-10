# Microsoft.ServiceFabric.Data.Interfaces.V2

Extended Reliable Collections interfaces for [Service Fabric](https://learn.microsoft.com/azure/service-fabric), adding ordinal
string keys, versioned key/value enumeration, advanced dictionary operations, and replicator configuration.

## Key Types

- `OrdinalString` — a `string` wrapper that uses ordinal comparison for `IComparable<T>` and `IEquatable<T>`. Use instead
  of `string` as a Reliable Dictionary key to avoid data corruption and unexpected enumeration results caused by the default
  culture-sensitive string comparison. For `default(OrdinalString)`, `==` and `Equals` treat two defaults as equal and
  member access on the underlying value throws `NullReferenceException` matching `default(string)` semantics.
  `GetHashCode` currently also throws `NullReferenceException`, which violates the `Equals`/`GetHashCode` contract
  and is a known bug.
- `IReliableDictionary3<TKey, TValue>` — **(Beta)** extends `IReliableDictionary2` with versioned key/value enumeration.
- `IReliableDictionary4<TKey, TValue>` — **(Beta)** extends `IReliableDictionary3` with key removal without disk reads.
- `VersionedKeyValuePair<TKey, TValue>` — a key/value pair tagged with a sequence number for change tracking.
- `IReliableStateManager2` — **(Beta)** extends `IReliableStateManagerReplica2` with transaction isolation level configuration.
- `ReliableStateManagerReplicatorSettings2` — additional replicator settings beyond the base configuration.

## Usage

Use `OrdinalString` as the key type for reliable dictionaries:

```csharp
var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<OrdinalString, int>>("counts");

using ITransaction tx = StateManager.CreateTransaction();
await dictionary.AddOrUpdateAsync(tx, "key", 1, (key, value) => value + 1);
await tx.CommitAsync();
```

## Documentation

- [Reliable Collections](https://learn.microsoft.com/azure/service-fabric/service-fabric-reliable-services-reliable-collections)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
