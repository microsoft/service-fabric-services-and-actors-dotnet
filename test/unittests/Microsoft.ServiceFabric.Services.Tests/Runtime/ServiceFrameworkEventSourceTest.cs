using System.Diagnostics.Tracing;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.ServiceFabric.Services.Runtime
{
    public abstract class ServiceFrameworkEventSourceTest
    {
        readonly ServiceFrameworkEventSource sut = new ServiceFrameworkEventSource();

        public sealed class Guid : ServiceFrameworkEventSourceTest
        {
            [Fact]
            public void RemainsUnchangedForBackwardCompatibilityWithCollectionTools()
            {
                Assert.Equal(new System.Guid("13c2a97d-71da-5ab5-47cb-1497aec602e1"), new ServiceFrameworkEventSource().Guid);
            }
        }

        public sealed class Manifest : ServiceFrameworkEventSourceTest
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
