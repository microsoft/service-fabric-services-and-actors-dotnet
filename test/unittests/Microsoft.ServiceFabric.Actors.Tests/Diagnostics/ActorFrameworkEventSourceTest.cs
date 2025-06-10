using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.InteropServices;
using Inspector;
using Microsoft.ServiceFabric.Services;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Microsoft.ServiceFabric.Diagnostics.Tracing;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    public abstract class ActorFrameworkEventSourceTest
    {
#if NET // Remove #if once on net472+ where IsOSPlatform is available
        public sealed class Class : ActorFrameworkEventSourceTest
        {
            [Fact]
            public void UsesRuntimeInformationIsOSPlatformToDetectLinux()
            {
                Func<OSPlatform, bool> expected = typeof(RuntimeInformation).Method<Func<OSPlatform, bool>>(nameof(RuntimeInformation.IsOSPlatform));
                Func<OSPlatform, bool> actual = typeof(ActorFrameworkEventSource).Field<Func<OSPlatform, bool>>();
                Assert.Equal(expected, actual);
            }
        }

        public sealed class Constructor : ActorFrameworkEventSourceTest, IDisposable
        {
            readonly Func<OSPlatform, bool> isOsPlatform = Mock.Of<Func<OSPlatform, bool>>();

            public Constructor()
            {
                // Enable mocking of OSPlatform detection
                typeof(ActorFrameworkEventSource).Field<Func<OSPlatform, bool>>().Set(isOsPlatform);

                // Dispose Writer singleton to allow event enablement to work on instances created by the tests
                var writer = typeof(ActorFrameworkEventSource).Property<ActorFrameworkEventSource>();
                writer.Value.Dispose();
            }

            public void Dispose()
            {
                // Restore OSPlatform detection
                typeof(ActorFrameworkEventSource).Field<Func<OSPlatform, bool>>().Set(RuntimeInformation.IsOSPlatform);

                // Restore Writer singleton
                typeof(ActorFrameworkEventSource).Property<ActorFrameworkEventSource>().Set(new ActorFrameworkEventSource());
            }

            [Fact]
            public void EnablesUnstructuredEventPublishingOnLinux()
            {
                Mock.Get(isOsPlatform).Setup(_ => _.Invoke(OSPlatform.Linux)).Returns(true);

                using var sut = new ActorFrameworkEventSource();

                Assert.True(sut.IsEnabled(EventLevel.Informational, EventKeywords.None)); // None = no filtering
                EventListener listener = sut.Field("m_Dispatchers").Value.Field<EventListener>();
                Assert.IsType<UnstructuredTracePublisher>(listener);
            }

            [Fact]
            public void DoesntEnableUnstructuredEventPublishingOnWindows()
            {
                Mock.Get(isOsPlatform).Setup(_ => _.Invoke(OSPlatform.Linux)).Returns(false);

                using var sut = new ActorFrameworkEventSource();

                Assert.False(sut.IsEnabled());
            }
        }
#endif

        public sealed class Guid : ActorFrameworkEventSourceTest
        {
            [Fact]
            public void RemainsUnchangedForBackwardCompatibilityWithCollectionTools()
            {
                Assert.Equal(new System.Guid("0e1ec353-9f02-55d7-fbb8-f3857458acbd"), new ActorFrameworkEventSource().Guid);
            }
        }

        public sealed class Manifest : ActorFrameworkEventSourceTest
        {
            readonly ITestOutputHelper output;

            public Manifest(ITestOutputHelper output) => this.output = output;

            [Fact]
            public void CanBeSavedForRegistrationWithExternalTools()
            {
                using var sut = new ActorFrameworkEventSource();

                string manifest = EventSource.GenerateManifest(sut.GetType(), sut.GetType().Assembly.Location);

                string manifestFile = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(sut.GetType().Assembly.Location), sut.Name), "man");
                File.WriteAllText(manifestFile, manifest);
                output.WriteLine("To register generated manifest for ETL tools, run");
                output.WriteLine($"sudo wevtutil install-manifest {manifestFile}");
            }
        }
    }
}
