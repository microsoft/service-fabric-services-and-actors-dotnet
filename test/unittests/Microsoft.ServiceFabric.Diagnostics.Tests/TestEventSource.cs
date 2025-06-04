using System.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Diagnostics.Tracing.Util;

namespace Microsoft.ServiceFabric.Diagnostics.Tests
{      
    [EventSource(Name = "TestEventSource")]
    internal class TestEventSource : ServiceFabricEventSource
    {
        [Event(1, Message = "Event with id and type: {0}, {1}, {2}", Level = EventLevel.Informational)]
        public void EventWithIdAndType(string id, string type, string message)
        {
            this.WriteEvent(1, id, type, message);
        }

        [Event(2, Message = "Event with id and type: {0}, {1}, {2}", Level = EventLevel.Informational)]
        public void EventWithIdAndTypeReveresedOrder(string id, string message, string type)
        {
            this.WriteEvent(2, id, message, type);
        }

        [Event(3, Message = "Event with id only: {0}, {1}", Level = EventLevel.Warning)]
        public void EventWithIdOnly(string id, string message)
        {
            this.WriteEvent(3, id, message);
        }

        public TestEventSource(IPlatformInformation platformInformation)
            : base("TestPackage", platformInformation)
        {
        }
    }
}
