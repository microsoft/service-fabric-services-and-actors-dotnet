# Microsoft.ServiceFabric.Actors.Wcf

Provides WCF-based remoting for [Service Fabric](https://learn.microsoft.com/azure/service-fabric) Reliable Actors, replacing
the default Fabric transport with WCF communication.

> **Note:** This package targets .NET Framework only.

## Key Types

- `WcfActorRemotingProviderAttribute` — assembly-level attribute that configures actor remoting to use WCF transport.

## Usage

Apply the attribute at the assembly level to enable WCF remoting:

```csharp
[assembly: WcfActorRemotingProvider]
```

## Documentation

- [Reliable Actors](https://learn.microsoft.com/azure/service-fabric/service-fabric-reliable-actors-introduction)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
