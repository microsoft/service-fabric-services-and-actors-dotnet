using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Diagnostics.Tracing.Util;
using Microsoft.ServiceFabric.Diagnostics.Tracing.Writer;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Tests
{    public class TestServiceFabricEventSource
    {
#if DotNetCoreClr
        public IPlatformInformation GetPlatformInformation(OSPlatform platform)
        {
            var mockPlatform = new Mock<IPlatformInformation>();
            mockPlatform.Setup(x => x.IsLinuxPlatform()).Returns(platform==OSPlatform.Linux);

            return mockPlatform.Object;
        }

        [Fact]
        public void VariantWriteViaNative_OnNonLinux_ThrowsPlatformNotSupportedException()
        {
            var eventSource = new TestEventSource(GetPlatformInformation(OSPlatform.Windows));
            int eventId = 1;
            int argCount = 3;
            Variant v0 = "test";
            Variant v1 = 42;
            Variant v2 = true;

            Assert.Throws<PlatformNotSupportedException>(() => eventSource.VariantWriteViaNative(eventId, argCount, v0, v1, v2));
        }        
        
        [Theory]
        [InlineData(1, true, 1)]
        [InlineData(2, true, 2)]
        [InlineData(3, true, -1)]
        public void Constructor_OnLinux_GeneratesEventDescriptorsCorrectly(int eventId, bool expectedHasId, int expectedTypeFieldIndex)
        {
            var eventSource = new TestEventSource(GetPlatformInformation(OSPlatform.Linux));

            var eventDescriptorsField = typeof(ServiceFabricEventSource).GetField("eventDescriptors", BindingFlags.NonPublic | BindingFlags.Instance);
            var eventDescriptors = (ReadOnlyDictionary<int, TraceEvent>)eventDescriptorsField.GetValue(eventSource);

            Assert.True(eventDescriptors.ContainsKey(eventId));
            var traceEvent = eventDescriptors[eventId];
            Assert.Equal(expectedHasId, traceEvent.hasId);
            Assert.Equal(expectedTypeFieldIndex, traceEvent.typeFieldIndex);
        }
#else
        [Fact]
        public void VariantWriteViaNative_NotAvailableOnNetFramework()
        {
            var eventSourceType = typeof(ServiceFabricEventSource);

            var method = eventSourceType.GetMethod("VariantWriteViaNative", BindingFlags.Public | BindingFlags.Instance);

            Assert.Null(method);
        }
#endif
    }
}
