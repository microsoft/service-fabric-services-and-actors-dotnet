// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics.Tracing;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing
{
    /// <summary>
    /// A low-level implementation interface used by the descendants of the <see cref="ServiceFabricEventSource"/>
    /// to emit idiomatic Service Fabric traces with well-known payload properties type and id. These properties are
    /// recognized by the Trace Viewer and the Asgard Kusto Logs ingestion.
    /// </summary>
    /// <remarks>
    /// <para> This interface is not well-suited for instrumenting business logic. Use the high-level <see cref="ITrace"/>
    /// interface for that purpose instead. </para>
    /// <para> Event methods implementing this interface must be defined with an <see cref="EventAttribute"/> with the
    /// well-known IDs: <see cref="ServiceFabricEventSource.InfoTextEventId"/>, <see cref="ServiceFabricEventSource.WarningTextEventId"/>,
    /// <see cref="ServiceFabricEventSource.ErrorTextEventId"/>, the <see cref="ServiceFabricEventSource.TextEventFormat"/> message template,
    /// and matching <see cref="EventLevel"/> values. </para>
    /// </remarks>
    interface ITextEventSource
    {
        void InfoText(string id, string type, string message);
        void WarningText(string id, string type, string message);
        void ErrorText(string id, string type, string message);
    }
}
