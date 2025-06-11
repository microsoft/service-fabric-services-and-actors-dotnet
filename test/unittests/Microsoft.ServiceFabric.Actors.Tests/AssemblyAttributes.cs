using Xunit;

// Run tests sequentially to avoid race conditions in tests manipulating static fields
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
