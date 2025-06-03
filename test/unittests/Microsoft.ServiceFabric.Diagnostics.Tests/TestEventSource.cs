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
    [EventSource(Name = "TestEventSource")]
    internal class TestEventSource : ServiceFabricEventSource
    {
    }
}
