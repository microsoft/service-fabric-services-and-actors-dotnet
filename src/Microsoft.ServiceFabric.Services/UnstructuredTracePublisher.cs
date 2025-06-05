// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
#if !NETFRAMEWORK

namespace Microsoft.ServiceFabric.Services
{
    using System.Diagnostics.Tracing;
    using System.Fabric.Common.Tracing;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Signature of the <see cref="TraceViaNative.WriteUnstructured" /> method used by the tests
    /// to isolate the <see cref="UnstructuredTracePublisher" /> implementation.
    /// </summary>
    internal delegate void WriteUnstructured(string task, string @event, string id, ushort level, string text);

    /// <summary>
    /// Publishes <see cref="EventSource"/> events to the Service Fabric tracing pipeline used on Linux.
    /// </summary>
    internal sealed class UnstructuredTracePublisher : EventListener
    {
        private readonly WriteUnstructured publish = TraceViaNative.WriteUnstructured;

        protected override void OnEventWritten(EventWrittenEventArgs written)
        {
            string task = written.EventSource.Name;
            string @event = written.EventName;
            string id = written.EventId.ToString();
            var level = (ushort)written.Level;
            var text = string.Format(CultureInfo.InvariantCulture, written.Message, written.Payload.ToArray());
            this.publish(task, @event, id, level, text);
        }
    }
}

#endif
