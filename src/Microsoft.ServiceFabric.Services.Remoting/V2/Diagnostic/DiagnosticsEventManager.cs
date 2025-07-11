// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class DiagnosticsEventManager
    {
        internal delegate void OnDiagnosticEvent();

        internal delegate void OnDiagnosticEvent<T>(T eventData);

        internal OnDiagnosticEvent OnRequestStart { get; set; }

        internal OnDiagnosticEvent<Stopwatch> OnRequestEnd { get; set; }

        internal OnDiagnosticEvent<Stopwatch> OnCreateTransportMessage { get; set; }

        internal OnDiagnosticEvent<Stopwatch> OnCreateRemotingMessage { get; set; }
    }
}
