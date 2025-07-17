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

        public DateTime OnRemotingRequestDeserializationBegin()
        {
            throw new NotImplementedException();
        }

        public void OnRemotingRequestDeserializationEnd(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public DateTime OnCreateTransportMessageSerializationBegin()
        {
            throw new NotImplementedException();
        }

        public void OnCreateTransportMessageSerializationEnd(DateTime startTime)
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

        public void RegisterDiagnosticsSource(IDiagnosticsSource diagnosticsSource)
        {
            throw new NotImplementedException();
        }
    }
}
