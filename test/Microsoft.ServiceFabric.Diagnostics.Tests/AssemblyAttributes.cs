using Xunit;

// Run tests sequentially to prevent failures in tests that depend on global state.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
