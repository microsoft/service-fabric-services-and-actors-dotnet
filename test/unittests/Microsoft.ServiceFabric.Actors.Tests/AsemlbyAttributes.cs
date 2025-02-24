using Xunit;

// This attribute is used to run the tests in the same assembly in sequence.
// It is necessary for ActorRemotingProviderAttributeTest suit to run properly.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
