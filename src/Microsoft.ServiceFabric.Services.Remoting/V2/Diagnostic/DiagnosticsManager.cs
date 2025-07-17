using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class DiagnosticsManager : IDiagnosticsSource
    {
        readonly ITimeProvider timeProvider;
        readonly Guid partitionId;
        readonly long replicaOrInstanceId;
        private List<IDiagnosticsSource> diagnosticsSources;

        internal DiagnosticsManager(ITimeProvider timeProvider, Guid partitionId, long replicaOrInstanceId)
        {
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            this.partitionId = partitionId;
            this.replicaOrInstanceId = replicaOrInstanceId;
            this.diagnosticsSources = new List<IDiagnosticsSource>();
        }

        public DateTime OnRemotingRequestBegin()
        {
            var utcNow = timeProvider.UtcNow;

            diagnosticsSources.ForEach(ds => ds.OnRemotingRequestBegin());
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

        public void RegisterDiagnosticsSource(IDiagnosticsSource diagnosticsSource)
        {
            if (diagnosticsSources.Where(ds => ds.GetType() == diagnosticsSource.GetType()).Any())
            {
                throw new InvalidOperationException($"Diagnostics source {diagnosticsSource.GetType().Name} already registered. Each source can be registered only once.");
            }
            diagnosticsSources.Add(diagnosticsSource);
        }
    }
}
