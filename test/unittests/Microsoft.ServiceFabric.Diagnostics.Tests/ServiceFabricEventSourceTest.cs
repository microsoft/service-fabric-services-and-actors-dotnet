using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Diagnostics.Tracing.Writer;
using Moq;
using Xunit;
using Inspector;
using System.Diagnostics.Tracing;
using System.IO;
using Xunit.Abstractions;
using System.Diagnostics.Eventing.Reader;
using EventLevel = System.Diagnostics.Tracing.EventLevel;


namespace Microsoft.ServiceFabric.Diagnostics.Tests
{
#if DotNetCoreClr
    public abstract class ServiceFabricEventSourceTest
    {
        public sealed class Class : ServiceFabricEventSourceTest
        {
            [Fact]
            public void UsesRuntimeInformationIsOSPlatformToDetectLinux()
            {
                Func<OSPlatform, bool> expected = typeof(RuntimeInformation).Method<Func<OSPlatform, bool>>(nameof(RuntimeInformation.IsOSPlatform));
                Func<OSPlatform, bool> actual = typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>();
                Assert.Equal(expected, actual);
            }
        }


        public sealed class Constructor : ServiceFabricEventSourceTest, IDisposable
        {
            readonly Func<OSPlatform, bool> isOsPlatform = Mock.Of<Func<OSPlatform, bool>>();

            public Constructor()
            {
                // Enable mocking of OSPlatform detection
                typeof(TestEventSource).Field<Func<OSPlatform, bool>>().Set(isOsPlatform);

                // Dispose Writer singleton to allow event enablement to work on instances created by the tests
                var writer = typeof(TestEventSource).Property<TestEventSource>();
                writer.Value.Dispose();
            }

            public void Dispose()
            {
                // Restore OSPlatform detection
                typeof(TestEventSource).Field<Func<OSPlatform, bool>>().Set(RuntimeInformation.IsOSPlatform);

                // Restore Writer singleton
                typeof(TestEventSource).Property<TestEventSource>().Set(new TestEventSource());
            }

            [Fact]
            public void EnablesUnstructuredEventPublishingOnLinux()
            {
                Mock.Get(isOsPlatform).Setup(_ => _.Invoke(OSPlatform.Linux)).Returns(true);

                using var sut = new TestEventSource();

                Assert.True(sut.IsEnabled(EventLevel.Informational, EventKeywords.None)); // None = no filtering
                EventListener listener = sut.Field("m_Dispatchers").Value.Field<EventListener>();
                Assert.IsType<UnstructuredTracePublisher>(listener);
            }

            [Fact]
            public void DoesntEnableUnstructuredEventPublishingOnWindows()
            {
                Mock.Get(isOsPlatform).Setup(_ => _.Invoke(OSPlatform.Linux)).Returns(false);

                using var sut = new TestEventSource();

                Assert.False(sut.IsEnabled());
            }

            [Theory]
            [InlineData(1, "EventWithIdAndType", "Event with id and type: {0}, {1}, {2}", EventLevel.Informational)]
            [InlineData(2, "EventWithIdOnly", "Event with id only: {0}, {1}", EventLevel.Warning)]
            public void DoesGeneratesEventDescriptorsCorrectly(int eventId, string name, string message, EventLevel eventLevel)
            {
                var eventSource = new TestEventSource();

                var eventDescriptorsField = typeof(ServiceFabricEventSource).GetField("eventDescriptors", BindingFlags.NonPublic | BindingFlags.Instance);
                var eventDescriptors = (ReadOnlyDictionary<int, TraceEvent>)eventDescriptorsField.GetValue(eventSource);

                Assert.True(eventDescriptors.ContainsKey(eventId));
                var traceEvent = eventDescriptors[eventId];
                Assert.Equal(name, traceEvent.EventName);
                Assert.Equal(eventLevel, traceEvent.Level);
                Assert.Equal(message, traceEvent.Message);
            }
        }

        public sealed class Manifest : ServiceFabricEventSourceTest
        {
            readonly ITestOutputHelper output;

            public Manifest(ITestOutputHelper output) => this.output = output;

            [Fact]
            public void CanBeSavedForRegistrationWithExternalTools()
            {
                using var sut = new TestEventSource();

                string manifest = EventSource.GenerateManifest(sut.GetType(), sut.GetType().Assembly.Location);

                string manifestFile = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(sut.GetType().Assembly.Location), sut.Name), "man");
                File.WriteAllText(manifestFile, manifest);
                output.WriteLine("To register generated manifest for ETL tools, run");
                output.WriteLine($"sudo wevtutil install-manifest {manifestFile}");
            }
        }
    }
#endif
}
