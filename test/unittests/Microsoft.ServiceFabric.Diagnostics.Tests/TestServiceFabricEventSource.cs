using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Diagnostics.Tracing.Config;
using Microsoft.ServiceFabric.Diagnostics.Tracing.Writer;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Tests
{
    public class TestServiceFabricEventSource
    {
#if DotNetCoreClr
        [Fact]
        public void VariantWriteViaNative_OnNonLinux_ThrowsPlatformNotSupportedException()
        {
            var eventSource = new TestEventSource();
            int eventId = 1;
            int argCount = 3;
            Variant v0 = "test";
            Variant v1 = 42;
            Variant v2 = true;


            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Assert.Throws<PlatformNotSupportedException>(() => eventSource.VariantWriteViaNative(eventId, argCount, v0, v1, v2));
            }
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
