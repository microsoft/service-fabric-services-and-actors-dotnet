using System;
using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class AgregatedDiagnosticEvents : IDiagnosticEvents
    {
        readonly ITimeProvider timeProvider;
        private List<IDiagnosticEvents> diagnosticEvents = new List<IDiagnosticEvents>();

        internal AgregatedDiagnosticEvents(ITimeProvider timeProvider)
        {
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public DateTime OnRemotingRequestBegin()
        {
            var utcNow = timeProvider.UtcNow;

            diagnosticEvents.ForEach(ds => ds.OnRemotingRequestBegin());
            return utcNow;
        }

        public void OnRemotingRequestEnd(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public DateTime OnCreateTransportMessageBegin()
        {
            throw new NotImplementedException();
        }

        public void OnCreateTransportMessageEnd(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public DateTime OnRequestResponseBegin()
        {
            throw new NotImplementedException();
        }

        public void OnRequestResponseEnd(DateTime startTime)
        {
            throw new NotImplementedException();
        }
    }
}
