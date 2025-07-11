// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class DiagnosticsEventManager
    {
        internal delegate void OnDiagnosticEvent();

        internal delegate void OnDiagnosticEvent<T>(T eventData);

        internal OnDiagnosticEvent<DateTime> OnRequestStart { get; set; }

        internal OnDiagnosticEvent<DateTime> OnRequestEnd { get; set; }

        internal OnDiagnosticEvent<DateTime> OnCreateTransportMessage { get; set; }

        internal OnDiagnosticEvent<DateTime> OnCreateRemotingMessage { get; set; }

        // internal void RemotingRequestStart(DateTime startTime)
        // {
        //     var processingTime = DateTime.UtcNow - startTime;
        //     var callbacks = this.OnActorRequestProcessingFinish;
        //     if (callbacks != null)
        //     {
        //         callbacks(processingTime);
        //     }
        // }
    }
}
