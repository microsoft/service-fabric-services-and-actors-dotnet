using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal interface IDiagnosticEvents
    {
        void OnRequestResponseBegin();
        void OnRequestResponseEnd(DateTime startTime);
        void OnCreateTransportMessageBegin();
        void OnCreateTransportMessageEnd(DateTime startTime);
        void OnRemotingRequestBegin();
        void OnRemotingRequestEnd(DateTime startTime);
    }
}
