using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class DiagnosticsManager: IDiagnosticsSource
    {
        private ITimeProvider timeProvider;
        private Guid partitionId;
        private long replicaOrInstanceId;
        
        internal DiagnosticsManager(ITimeProvider timeProvider, Guid partitionId, long replicaOrInstanceId)
        {
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            this.partitionId = partitionId;
            this.replicaOrInstanceId = replicaOrInstanceId;
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
