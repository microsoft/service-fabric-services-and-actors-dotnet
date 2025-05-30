using System.Diagnostics.Tracing;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    public abstract class ActorFrameworkEventSourceTest
    {
        readonly ActorFrameworkEventSource sut = new ActorFrameworkEventSource();

        public sealed class Guid : ActorFrameworkEventSourceTest
        {
            [Fact]
            public void RemainsUnchangedForBackwardCompatibilityWithCollectionTools()
            {
                Assert.Equal(new System.Guid("0e1ec353-9f02-55d7-fbb8-f3857458acbd"), sut.Guid);
            }
        }

        public sealed class Manifest : ActorFrameworkEventSourceTest
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
                output.WriteLine($"sudo wevtutil install-manifest {manifestFile}");
            }
        }
    }
}
