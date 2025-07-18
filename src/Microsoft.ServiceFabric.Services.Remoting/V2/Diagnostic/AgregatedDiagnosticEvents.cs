using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class AgregatedDiagnosticEvents : IDiagnosticEvents
    {
        private static UniqueIDiagnosticEventsTypeComparer uniqueIDiagnosticEventsComparer = new UniqueIDiagnosticEventsTypeComparer();
        readonly IClock timeProvider;
        private HashSet<IDiagnosticEvents> diagnosticsEventSet;

        internal AgregatedDiagnosticEvents(IClock timeProvider, IEnumerable<IDiagnosticEvents> diagnosticEvents)
        {
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

            if (diagnosticEvents.Any(d => d == null)) 
                throw new ArgumentException("Diagnostic events collection cannot contain null elements.", nameof(diagnosticEvents)); 

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
            throw new NotImplementedException();
        }

        public void OnRequestResponseBegin()
        {
            throw new NotImplementedException();
        }

        public void OnRequestResponseEnd(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public void OnCreateTransportMessageBegin()
        {
            throw new NotImplementedException();
        }

        public void OnCreateTransportMessageEnd(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        private class UniqueIDiagnosticEventsTypeComparer : IEqualityComparer<IDiagnosticEvents>
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
