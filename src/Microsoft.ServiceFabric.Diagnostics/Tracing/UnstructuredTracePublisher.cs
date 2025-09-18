#if !NETFRAMEWORK

using System.Diagnostics.Tracing;
using System.Fabric.Common.Tracing;
using System.Globalization;
using System.Linq;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing
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
            bool isTextEvent = IsTextEvent(written);

            string task = written.EventSource.Name;
            string id = isTextEvent ? (string)written.Payload[0] : string.Empty;
            string @event = isTextEvent ? (string)written.Payload[1] : written.EventName;
            var level = (ushort)written.Level;
            var text = string.Format(CultureInfo.InvariantCulture, written.Message, written.Payload.ToArray());

            publish(task, @event, id, level, text);
        }

        bool IsTextEvent(EventWrittenEventArgs written) =>
            written.EventId >= ServiceFabricEventSource.InfoTextEventId &&
            written.EventId <= ServiceFabricEventSource.ErrorTextEventId &&
            written.EventSource is ITextEventSource;
    }
}

#endif
