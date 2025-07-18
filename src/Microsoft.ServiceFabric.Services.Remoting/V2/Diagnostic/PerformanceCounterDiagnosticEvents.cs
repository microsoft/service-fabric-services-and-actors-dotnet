using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class PerformanceCounterDiagnosticEvents : IDiagnosticEvents
    {
        public DateTime OnCreateTransportMessageBegin()
        {
            throw new NotImplementedException();
        }

        public void OnCreateTransportMessageEnd(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public DateTime OnRemotingRequestBegin()
        {
            throw new NotImplementedException();
        }

        public void OnRemotingRequestEnd(DateTime startTime)
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
