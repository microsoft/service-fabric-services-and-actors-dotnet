// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class PerformanceCounterDiagnosticEvents : IDiagnosticEvents
    {
        public void OnCreateTransportMessageBegin()
        {
            throw new NotImplementedException();
        }

        public void OnCreateTransportMessageEnd(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public void OnRemotingRequestBegin()
        {
            throw new NotImplementedException();
        }

        public void OnRemotingRequestEnd(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public void OnRequestResponseBegin()
        {
            throw new NotImplementedException();
        }

        public void OnRequestResponseEnd(DateTime startTime)
        {
            throw new NotImplementedException();
        }
    }
}
