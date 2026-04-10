# Microsoft.ServiceFabric.Services.Wcf

WCF-based remoting for [Service Fabric](https://learn.microsoft.com/azure/service-fabric) Reliable Services, replacing the default
Fabric transport with WCF communication.

> **Note:** This package targets .NET Framework only.

## Key Types

- `WcfServiceRemotingProviderAttribute` — assembly-level attribute that configures service remoting to use WCF transport.

## Usage

Apply the attribute at the assembly level to enable WCF remoting:

```csharp
[assembly: WcfServiceRemotingProvider]
```

## Documentation

- [Service remoting](https://learn.microsoft.com/azure/service-fabric/service-fabric-reliable-services-communication-remoting)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
