using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class PerformanceCounterDiagnosticsSource : IDiagnosticsSource
    {
        public DateTime OnCreateTransportMessageSerializationBegin()
        {
            throw new NotImplementedException();
        }

        public void OnCreateTransportMessageSerializationEnd(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public DateTime OnRemotingRequestDeserializationBegin()
        {
            throw new NotImplementedException();
        }

        public void OnRemotingRequestDeserializationEnd(DateTime startTime)
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
            throw new NotSupportedException("PerformanceCounterDiagnosticsSource does not support registering another diagnostics source. If multiple diagnostics sources are needed, consider using DiagnosticsManager class.");
        }
    }
}
