#if !NETFRAMEWORK

using System.Diagnostics.Tracing;
using System.Fabric.Common.Tracing;
using System.Globalization;
using System.Linq;

namespace Microsoft.ServiceFabric.Services
{
    /// <summary>
    /// Signature of the <see cref="TraceViaNative.WriteUnstructured" /> method used by the tests
    /// to isolate the <see cref="UnstructuredTracePublisher" /> implementation.
    /// </summary>
    delegate void WriteUnstructured(string task, string @event, string id, ushort level, string text);

    /// <summary>
    /// Publishes <see cref="EventSource"/> events to the Service Fabric tracing pipeline used on Linux.
    /// </summary>
    sealed class UnstructuredTracePublisher : EventListener
    {
        readonly WriteUnstructured publish = TraceViaNative.WriteUnstructured;

        protected override void OnEventWritten(EventWrittenEventArgs written)
        {
            string task = written.EventSource.Name;
            string @event = written.EventName;
            string id = written.EventId.ToString();
            var level = (ushort)written.Level;
            var text = string.Format(CultureInfo.InvariantCulture, written.Message, written.Payload.ToArray());
            publish(task, @event, id, level, text);
        }
    }
}

#endif
