using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal interface IDiagnosticEvents
    {
        DateTime OnRequestResponseBegin();
        void OnRequestResponseEnd(DateTime startTime);
        DateTime OnCreateTransportMessageBegin();
        void OnCreateTransportMessageEnd(DateTime startTime);
        DateTime OnRemotingRequestBegin();
        void OnRemotingRequestEnd(DateTime startTime);
    }
}
