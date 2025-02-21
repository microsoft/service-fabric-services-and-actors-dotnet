using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;

// This sets FabricTransportActorRemotingProvider as the default remoting provider for the whole Microsoft.ServiceFabric.Actors.Test project.
// This is neccessary as not setting a remoting provider explicitly will raise an exception.

[assembly: FabricTransportActorRemotingProvider(RemotingClientVersion=RemotingClientVersion.V2_1, RemotingListenerVersion=RemotingListenerVersion.V2_1)]
