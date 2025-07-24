// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class AgregatedDiagnosticEvents : IDiagnosticEvents, IDisposable
    {
        private readonly static UniqueIDiagnosticEventsTypeComparer uniqueIDiagnosticEventsComparer = new UniqueIDiagnosticEventsTypeComparer();
        private readonly HashSet<IDiagnosticEvents> diagnosticsEventSet;

        internal AgregatedDiagnosticEvents(IEnumerable<IDiagnosticEvents> diagnosticEvents)
        {
            if (diagnosticEvents == null || diagnosticEvents.Any(d => d == null)) 
                throw new ArgumentException("Diagnostic events collection cannot be null or contain null elements.", nameof(diagnosticEvents)); 

            this.diagnosticsEventSet = new HashSet<IDiagnosticEvents>(diagnosticEvents ?? throw new ArgumentNullException(nameof(diagnosticEvents)), uniqueIDiagnosticEventsComparer);

            if (diagnosticEvents.Count() != this.diagnosticsEventSet.Count())
                throw new ArgumentException("Duplicate diagnostic events detected.", nameof(diagnosticEvents));
        }

        public void OnRemotingRequestBegin()
        {
            foreach (var ds in diagnosticsEventSet)
            {
                ds.OnRemotingRequestBegin();
            }
        }        
        
        public void OnRemotingRequestEnd(DateTime startTime)
        {
            foreach (var ds in diagnosticsEventSet)
            {
                ds.OnRemotingRequestEnd(startTime);
            }
        }

        public void OnRequestResponseBegin()
        {
            foreach (var ds in diagnosticsEventSet)
            {
                ds.OnRequestResponseBegin();
            }
        }

        public void OnRequestResponseEnd(DateTime startTime)
        {
            foreach (var ds in diagnosticsEventSet)
            {
                ds.OnRequestResponseEnd(startTime);
            }
        }

        public void OnCreateTransportMessageBegin()
        {
            foreach (var ds in diagnosticsEventSet)
            {
                ds.OnCreateTransportMessageBegin();
            }
        }

        public void OnCreateTransportMessageEnd(DateTime startTime)
        {
            foreach (var ds in diagnosticsEventSet)
            {
                ds.OnCreateTransportMessageEnd(startTime);
            }
        }

        public void Dispose()
        {
            foreach (var ds in diagnosticsEventSet)
            {
                if (ds is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        private sealed class UniqueIDiagnosticEventsTypeComparer : IEqualityComparer<IDiagnosticEvents>
        {
            public bool Equals(IDiagnosticEvents x, IDiagnosticEvents y)
            {
                if (x == null && y == null)
                    return true;

                if (x == null || y == null)
                    return false;

                return x.GetType() == y.GetType();
            }

            public int GetHashCode(IDiagnosticEvents obj)
            {
                return obj?.GetType().GetHashCode() ?? 0;
            }
        }
    }
}
