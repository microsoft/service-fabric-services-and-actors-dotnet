#if DotNetCoreClr

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
        }
    }
}
#endif
