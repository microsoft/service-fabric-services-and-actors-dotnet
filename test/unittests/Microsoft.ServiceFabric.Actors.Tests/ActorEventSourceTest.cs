using System.Diagnostics.Tracing;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.ServiceFabric.Actors
{
    public abstract class ActorEventSourceTest
    {
        readonly ActorEventSource sut = ActorEventSource.Instance;

        public sealed class Guid : ActorEventSourceTest
        {
            [Fact]
            public void RemainsUnchangedForBackwardCompatibilityWithCollectionTools()
            {
                Assert.Equal(new System.Guid("e2f2656b-985e-5c5b-5ba3-bbe8a851e1d7"), sut.Guid);
            }
        }

        public sealed class Manifest : ActorEventSourceTest
        {
            readonly ITestOutputHelper output;

            public Manifest(ITestOutputHelper output) => this.output = output;

            [Fact]
            public void CanBeSavedForRegistrationWithExternalTools()
            {
                string manifest = EventSource.GenerateManifest(sut.GetType(), sut.GetType().Assembly.Location);
                string manifestFile = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(sut.GetType().Assembly.Location), sut.Name), "man");
                File.WriteAllText(manifestFile, manifest);
                output.WriteLine("To register generated manifest for ETL tools, run");
                output.WriteLine($"sudo wevutil install-manifest {manifestFile}");
            }
        }
    }
}
