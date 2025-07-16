using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal interface IDiagnosticsSource
    {
        DateTime OnRequestResponseBegin();
        void OnRequestResponseEnd(DateTime startTime);
        DateTime OnCreateTransportMessageSerializationBegin();
        void OnCreateTransportMessageSerializationEnd(DateTime startTime);
        DateTime OnRemotingRequestDeserializationBegin();
        void OnRemotingRequestDeserializationEnd(DateTime startTime);
    }
}
