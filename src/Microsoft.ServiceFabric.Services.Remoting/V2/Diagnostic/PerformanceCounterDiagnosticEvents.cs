// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Diagnostics.Util;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class PerformanceCounterDiagnosticEvents : IDiagnosticEvents
    {
        private ServiceRemotingPerformanceCounterProvider performanceCounterProvider;
        private IClock clock;

        private long CalculateMilisecondsSince(DateTime startTime)
        {
            return (long)Math.Floor((clock.UtcNow - startTime).TotalMilliseconds);
        }

        public PerformanceCounterDiagnosticEvents(ServiceRemotingPerformanceCounterProvider performanceCounterProvider, IClock clock)
        {
            this.performanceCounterProvider = performanceCounterProvider ?? throw new ArgumentException(nameof(performanceCounterProvider));
            this.clock = clock ?? throw new ArgumentException(nameof(clock));
        }

        public void OnRequestResponseBegin()
        {
            throw new NotImplementedException();
        }

        public void OnRequestResponseEnd(DateTime startTime)
        {
            throw new NotImplementedException();
        }

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
    }
}
