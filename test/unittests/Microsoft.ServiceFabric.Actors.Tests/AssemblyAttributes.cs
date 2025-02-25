using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Xunit;

// This sets FabricTransportActorRemotingProvider as the default remoting provider for the whole Microsoft.ServiceFabric.Actors.Test project.
// This is neccessary as not setting a remoting provider explicitly will raise an exception.
[assembly: FabricTransportActorRemotingProvider(RemotingClientVersion=RemotingClientVersion.V2_1, RemotingListenerVersion=RemotingListenerVersion.V2_1)]

// This attribute is used to run tests in the same assembly in sequence.
// It is necessary for ActorRemotingProviderAttributeTest suit to run properly,
// because ActorRemotingProviderAtrribut has static state and cannot be teste in parallel.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
