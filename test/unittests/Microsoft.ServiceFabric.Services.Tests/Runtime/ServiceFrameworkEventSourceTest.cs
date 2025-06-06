using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.InteropServices;
using Inspector;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.ServiceFabric.Services.Runtime
{
    public abstract class ServiceFrameworkEventSourceTest
    {
#if NET // Remove #if once on net472+ where IsOSPlatform is available
        public sealed class Class : ServiceFrameworkEventSourceTest
        {
            [Fact]
            public void UsesRuntimeInformationIsOSPlatformToDetectLinux()
            {
                Func<OSPlatform, bool> expected = typeof(RuntimeInformation).Method<Func<OSPlatform, bool>>(nameof(RuntimeInformation.IsOSPlatform));
                Func<OSPlatform, bool> actual = typeof(ServiceFrameworkEventSource).Field<Func<OSPlatform, bool>>();
                Assert.Equal(expected, actual);
            }
        }

        public sealed class Constructor : ServiceFrameworkEventSourceTest, IDisposable
        {
            readonly Func<OSPlatform, bool> isOsPlatform = Mock.Of<Func<OSPlatform, bool>>();

            public Constructor()
            {
                // Enable mocking of OSPlatform detection
                typeof(ServiceFrameworkEventSource).Field<Func<OSPlatform, bool>>().Set(isOsPlatform);

                // Dispose Writer singleton to allow event enablement to work on instances created by the tests
                var writer = typeof(ServiceFrameworkEventSource).Property<ServiceFrameworkEventSource>();
                writer.Value.Dispose();
            }

            public void Dispose()
            {
                // Restore OSPlatform detection
                typeof(ServiceFrameworkEventSource).Field<Func<OSPlatform, bool>>().Set(RuntimeInformation.IsOSPlatform);

                // Restore Writer singleton
                typeof(ServiceFrameworkEventSource).Property<ServiceFrameworkEventSource>().Set(new ServiceFrameworkEventSource());
            }

            [Fact]
            public void EnablesUnstructuredEventPublishingOnLinux()
            {
                Mock.Get(isOsPlatform).Setup(_ => _.Invoke(OSPlatform.Linux)).Returns(true);

                using var sut = new ServiceFrameworkEventSource();

                Assert.True(sut.IsEnabled(EventLevel.Informational, EventKeywords.None)); // None = no filtering
                EventListener listener = sut.Field("m_Dispatchers").Value.Field<EventListener>();
                Assert.IsType<UnstructuredTracePublisher>(listener);
            }

            [Fact]
            public void DoesntEnableUnstructuredEventPublishingOnWindows()
            {
                Mock.Get(isOsPlatform).Setup(_ => _.Invoke(OSPlatform.Linux)).Returns(false);

                using var sut = new ServiceFrameworkEventSource();

                Assert.False(sut.IsEnabled());
            }
        }
#endif
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
                using var sut = new ServiceFrameworkEventSource();

                string manifest = EventSource.GenerateManifest(sut.GetType(), sut.GetType().Assembly.Location);

                string manifestFile = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(sut.GetType().Assembly.Location), sut.Name), "man");
                File.WriteAllText(manifestFile, manifest);
                output.WriteLine("To register generated manifest for ETL tools, run");
                output.WriteLine($"sudo wevtutil install-manifest {manifestFile}");
            }
        }
    }
}
