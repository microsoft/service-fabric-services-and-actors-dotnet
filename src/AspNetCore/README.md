# Microsoft.ServiceFabric.AspNetCore.Abstractions

Base types for running ASP.NET Core Applications on [Service Fabric](https://learn.microsoft.com/azure/service-fabric).

Use when building custom communication listeners. For ready-to-use listeners, install
[Microsoft.ServiceFabric.AspNetCore.Kestrel](https://www.nuget.org/packages/Microsoft.ServiceFabric.AspNetCore.Kestrel)
or [Microsoft.ServiceFabric.AspNetCore.HttpSys](https://www.nuget.org/packages/Microsoft.ServiceFabric.AspNetCore.HttpSys).

## Key Types

- `AspNetCoreCommunicationListener` — abstract base class for building ASP.NET Core communication listeners that integrate
  with Service Fabric endpoint management.
- `ServiceFabricMiddleware` — middleware that rejects requests not intended for the current service replica.
- `UseServiceFabricMiddleware(string urlSuffix)` — extension method on `IApplicationBuilder` for adding the middleware.

## Documentation

- [ASP.NET Core in Service Fabric](https://learn.microsoft.com/azure/service-fabric/service-fabric-reliable-services-communication-aspnetcore)
- [Source code](https://github.com/microsoft/service-fabric-dotnet)
