using System;
using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class AgregatedDiagnosticEvents : IDiagnosticEvents
    {
        readonly IClock timeProvider;
        private List<IDiagnosticEvents> diagnosticEvents = new List<IDiagnosticEvents>();

        internal AgregatedDiagnosticEvents(IClock timeProvider)
        {
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public void OnRemotingRequestBegin()
        {
            diagnosticEvents.ForEach(ds => ds.OnRemotingRequestBegin());
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
    }
}
