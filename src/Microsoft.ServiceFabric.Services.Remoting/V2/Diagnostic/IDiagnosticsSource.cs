using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal interface IDiagnosticsSource
    {
        DateTime OnRequestResponseBegin();
        void OnRequestResponseEnd(DateTime startTime);
        DateTime OnCreateTransportMessageBegin();

        void OnCreateTransportMessageEnd(DateTime startTime);
        DateTime OnCreateRemotingMessageBegin();
        void OnCreateRemotingMessageEnd(DateTime startTime);
    }
}
