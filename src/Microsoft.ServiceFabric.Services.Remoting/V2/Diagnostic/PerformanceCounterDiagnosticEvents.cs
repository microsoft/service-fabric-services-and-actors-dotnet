// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Diagnostics.Util;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class PerformanceCounterDiagnosticEvents : IDiagnosticEvents, IDisposable
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
            if(performanceCounterProvider.ServiceOutstandingRequestsCounterWriter != null)
            {
                performanceCounterProvider.ServiceOutstandingRequestsCounterWriter.UpdateCounterValue(1);
            }
        }

        public void OnRequestResponseEnd(DateTime startTime)
        {
            if (performanceCounterProvider.ServiceOutstandingRequestsCounterWriter != null)
            {
                performanceCounterProvider.ServiceOutstandingRequestsCounterWriter.UpdateCounterValue(-1);
            }
            if (performanceCounterProvider.ServiceRequestProcessingTimeCounterWriter != null)
            {
                performanceCounterProvider.ServiceRequestProcessingTimeCounterWriter.UpdateCounterValue(
                    CalculateMilisecondsSince(startTime));
            }
        }

        public void OnCreateTransportMessageBegin()
        {
            // Intentionally left blank, since we don't track transport message begin in performance counters.
        }

        public void OnCreateTransportMessageEnd(DateTime startTime)
        {
            if(performanceCounterProvider.ServiceRequestDeserializationTimeCounterWriter != null)
            {
                performanceCounterProvider.ServiceRequestDeserializationTimeCounterWriter.UpdateCounterValue(
                    CalculateMilisecondsSince(startTime));
            }
        }

        public void OnRemotingRequestBegin()
        {
            // Intentionally left blank, since we don't track remoting request begin in performance counters.
        }

        public void OnRemotingRequestEnd(DateTime startTime)
        {
            if(performanceCounterProvider.ServiceResponseSerializationTimeCounterWriter != null)
            {
                performanceCounterProvider.ServiceResponseSerializationTimeCounterWriter.UpdateCounterValue(
                    CalculateMilisecondsSince(startTime));
            }
        }

        public void Dispose()
        {
            if(performanceCounterProvider != null)
            {
                performanceCounterProvider.Dispose();
            }
        }
    }
}
