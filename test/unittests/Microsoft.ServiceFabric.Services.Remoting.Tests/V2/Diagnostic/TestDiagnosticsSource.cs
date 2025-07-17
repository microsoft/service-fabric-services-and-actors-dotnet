using System;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{
    internal class TestDiagnosticsSource : IDiagnosticsSource
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

        public void RegisterDiagnosticsSource(IDiagnosticsSource diagnosticsSource)
        {
            throw new NotImplementedException();
        }
    }

    internal class TestDiagnosticsSourceSecond : IDiagnosticsSource
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

        public void RegisterDiagnosticsSource(IDiagnosticsSource diagnosticsSource)
        {
            throw new NotImplementedException();
        }
    }
}
