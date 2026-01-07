using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Tracing.Writer;
using Moq;
using Xunit;
using EventLevel = System.Diagnostics.Tracing.EventLevel;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing
{
    public abstract class ServiceFabricEventSourceTest
    {
#if NET
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

        public partial class Constructor : IDisposable
        {
            readonly Mock<Func<OSPlatform, bool>> isOsPlatform = new Mock<Func<OSPlatform, bool>>();

            public Constructor() =>
                typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(isOsPlatform.Object);

            public void Dispose() =>
                typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(RuntimeInformation.IsOSPlatform);

            [Fact]
            public void EnablesUnstructuredEventPublishingOnLinux()
            {
                isOsPlatform.Setup(_ => _.Invoke(OSPlatform.Linux)).Returns(true);

                using var sut = new TestEventSource();

                Assert.True(sut.IsEnabled(EventLevel.Verbose, EventKeywords.All));
                EventListener listener = sut.Field("m_Dispatchers").Value.Field<EventListener>();
                Assert.IsType<UnstructuredTracePublisher>(listener);
            }

            [Fact]
            public void DoesntEnableUnstructuredEventPublishingOnWindows()
            {
                isOsPlatform.Setup(_ => _.Invoke(OSPlatform.Linux)).Returns(false);

                using var sut = new TestEventSource();

                Assert.False(sut.IsEnabled());
            }
        }

#endif

        public sealed partial class Constructor : ServiceFabricEventSourceTest
        {
            [Theory]
            [InlineData(1, "EventWithIdAndType", "Event with id and type: {0}, {1}, {2}", EventLevel.Informational)]
            [InlineData(2, "EventWithIdOnly", "Event with id only: {0}, {1}", EventLevel.Warning)]
            public void GeneratesEventDescriptorsCorrectly(int eventId, string name, string message, EventLevel eventLevel)
            {
                using var sut = new TestEventSource();

                var eventDescriptors = sut.Field<ReadOnlyDictionary<int, TraceEvent>>().Value;

                TraceEvent traceEvent = eventDescriptors[eventId];
                Assert.Equal(name, traceEvent.EventName);
                Assert.Equal(eventLevel, traceEvent.Level);
                Assert.Equal(message, traceEvent.Message);
            }
        }
    }
}
