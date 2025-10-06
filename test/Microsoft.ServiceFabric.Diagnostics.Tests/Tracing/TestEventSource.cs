using System.Diagnostics.Tracing;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing
{      
    [EventSource(Name = "TestEventSource")]
    internal class TestEventSource : ServiceFabricEventSource
    {
        [Event(1, Message = "Event with id and type: {0}, {1}, {2}", Level = EventLevel.Informational)]
        public void EventWithIdAndType(string id, string type, string message)
        {
            this.WriteEvent(1, id, type, message);
        }

        [Event(2, Message = "Event with id only: {0}, {1}", Level = EventLevel.Warning)]
        public void EventWithIdOnly(string id, string message)
        {
            this.WriteEvent(2, id, message);
        }
    }
}
