using System.Diagnostics.Tracing;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.ServiceFabric.Services
{
    public abstract class ServiceEventSourceTest
    {
        readonly ServiceEventSource sut = ServiceEventSource.Instance;

        public sealed class Guid : ServiceEventSourceTest
        {
            [Fact]
            public void RemainsUnchangedForBackwardCompatibilityWithCollectionTools()
            {
                Assert.Equal(new System.Guid("27b7a543-7280-5c2a-b053-f2f798e2cbb7"), sut.Guid);
            }
        }

        public sealed class Manifest : ServiceEventSourceTest
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
