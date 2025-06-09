using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Diagnostics.Tracing.Writer;
using Moq;
using Xunit;
using Inspector;


namespace Microsoft.ServiceFabric.Diagnostics.Tests
{    
    public abstract class ServiceFabricEventSourceTest
    {
#if DotNetCoreClr
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


        public sealed class LinuxSpecificLogic : ServiceFabricEventSourceTest, IDisposable
        {

            readonly Func<OSPlatform, bool> isOsPlatform = Mock.Of<Func<OSPlatform, bool>>();

            public LinuxSpecificLogic()
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

            [Theory]
            [InlineData(1, true, 1)]
            [InlineData(2, true, 2)]
            [InlineData(3, true, -1)]
            public void Constructor_OnLinux_GeneratesEventDescriptorsCorrectly(int eventId, bool expectedHasId, int expectedTypeFieldIndex)
            {
                Mock.Get(isOsPlatform).Setup(_ => _.Invoke(OSPlatform.Linux)).Returns(true);

                var eventSource = new TestEventSource();

                var eventDescriptorsField = typeof(ServiceFabricEventSource).GetField("eventDescriptors", BindingFlags.NonPublic | BindingFlags.Instance);
                var eventDescriptors = (ReadOnlyDictionary<int, TraceEvent>)eventDescriptorsField.GetValue(eventSource);

                Assert.True(eventDescriptors.ContainsKey(eventId));
                var traceEvent = eventDescriptors[eventId];
                Assert.Equal(expectedHasId, traceEvent.hasId);
                Assert.Equal(expectedTypeFieldIndex, traceEvent.typeFieldIndex);
            }

            [Fact]
            public void VariantWriteViaNative_OnNonLinux_ThrowsPlatformNotSupportedException()
            {
                Mock.Get(isOsPlatform).Setup(_ => _.Invoke(OSPlatform.Linux)).Returns(false);

                var eventSource = new TestEventSource();
                int eventId = 1;
                int argCount = 3;
                Variant v0 = "test";
                Variant v1 = 42;
                Variant v2 = true;

                Assert.Throws<PlatformNotSupportedException>(() => eventSource.VariantWriteViaNative(eventId, argCount, v0, v1, v2));
            }
        }

#else
        public sealed class NetFramework : ServiceFabricEventSourceTest
        {
            [Fact]
            public void VariantWriteViaNative_NotAvailableOnNetFramework()
            {
                var eventSourceType = typeof(ServiceFabricEventSource);

                var method = eventSourceType.GetMethod("VariantWriteViaNative", BindingFlags.Public | BindingFlags.Instance);

                Assert.Null(method);
            }
        } 
#endif
    }
}
