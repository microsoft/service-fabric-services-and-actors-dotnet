using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Xunit;

// This attribute is used to run tests in the same assembly in sequence.
// It is necessary for ActorRemotingProviderAttributeTest suit to run properly,
// because ActorRemotingProviderAtrribut has static state and cannot be teste in parallel.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
