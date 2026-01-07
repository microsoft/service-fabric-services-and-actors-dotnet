// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing
{
    sealed class Trace : ITrace, IEquatable<Trace>
    {
        readonly string type;
        readonly string id;
        readonly ITextEventSource events;

        internal Trace(Type type, ServiceContext context, ITextEventSource events)
        {
            this.type = (type ?? throw new ArgumentNullException(nameof(type))).Name;
            id = TraceId(context ?? throw new ArgumentNullException(nameof(context)));
            this.events = events ?? throw new ArgumentNullException(nameof(events));
        }

        void ITrace.Error(string message) =>
            events.ErrorText(id, type, message);

        void ITrace.Info(string message) =>
            events.InfoText(id, type, message);

        void ITrace.Warning(string message) =>
            events.WarningText(id, type, message);

        string TraceId(ServiceContext context) =>
            $"{context.PartitionId:B}:{context.ReplicaOrInstanceId}";

        bool IEquatable<Trace>.Equals(Trace other) =>
            other != null && type == other.type && id == other.id && events == other.events;
    }
}
