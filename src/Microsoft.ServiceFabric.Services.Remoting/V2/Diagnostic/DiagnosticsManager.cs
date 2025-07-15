using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class DiagnosticsManager: IDiagnosticsSource
    {
        internal DiagnosticsManager(Guid partitionId, long replicaOrInstanceId)
        {
        }

        public DateTime OnCreateRemotingMessageBegin()
        {
            throw new NotImplementedException();
        }

        public void OnCreateRemotingMessageEnd(DateTime startTime)
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
