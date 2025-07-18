using Xunit;

// This attribute is used to run tests in the same assembly in sequence.
// It is necessary for ServiceRemotingProviderAttributeTest suit to run properly,
// because ServiceRemotingProviderAttribute has static state and cannot be teste in parallel.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
