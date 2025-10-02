// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class AggregatedDiagnosticEvents : IDiagnosticEvents
    {
        readonly IEnumerable<IDiagnosticEvents> diagnosticEvents;

        internal AggregatedDiagnosticEvents(IEnumerable<IDiagnosticEvents> diagnosticEvents)
        {
            if (diagnosticEvents == null || diagnosticEvents.Any(d => d == null))
                throw new ArgumentException("Diagnostic events collection cannot be null or contain null elements.", nameof(diagnosticEvents));

            this.diagnosticEvents = diagnosticEvents;
        }

        public void OnRemotingRequestBegin()
        {
            foreach (IDiagnosticEvents ds in diagnosticEvents)
            {
                ds.OnRemotingRequestBegin();
            }
        }

        public void OnRemotingRequestEnd(DateTime startTime)
        {
            foreach (IDiagnosticEvents ds in diagnosticEvents)
            {
                ds.OnRemotingRequestEnd(startTime);
            }
        }

        public void OnRequestResponseBegin()
        {
            foreach (IDiagnosticEvents ds in diagnosticEvents)
            {
                ds.OnRequestResponseBegin();
            }
        }

        public void OnRequestResponseEnd(DateTime startTime)
        {
            foreach (IDiagnosticEvents ds in diagnosticEvents)
            {
                ds.OnRequestResponseEnd(startTime);
            }
        }

        public void OnCreateTransportMessageBegin()
        {
            foreach (IDiagnosticEvents ds in diagnosticEvents)
            {
                ds.OnCreateTransportMessageBegin();
            }
        }

        public void OnCreateTransportMessageEnd(DateTime startTime)
        {
            foreach (IDiagnosticEvents ds in diagnosticEvents)
            {
                ds.OnCreateTransportMessageEnd(startTime);
            }
        }
    }
}
