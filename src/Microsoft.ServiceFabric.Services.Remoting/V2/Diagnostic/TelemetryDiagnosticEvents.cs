// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics.Metrics;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class TelemetryDiagnosticEvents : IDiagnosticEvents
    {
        readonly IClock clock;

        internal readonly IMeter<TimeSpan> requestProcessingTime;
        internal readonly IMeter<TimeSpan> requestDeserializationTime;
        internal readonly IMeter<TimeSpan> responseSerializationTime;

        public TelemetryDiagnosticEvents(IMeterProvider<TimeSpan> meterProvider, IClock clock)
        {
            _ = meterProvider ?? throw new ArgumentNullException(nameof(meterProvider));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));

            this.requestProcessingTime = meterProvider.CreateMeter("Services.Remoting", "MessageHandler.RequestProcessingTime");
            this.requestDeserializationTime = meterProvider.CreateMeter("Services.Remoting", "MessageHandler.RequestDeserializationTime");
            this.responseSerializationTime = meterProvider.CreateMeter("Services.Remoting", "MessageHandler.ResponseSerializationTime");
        }

        public void OnCreateTransportMessageBegin()
        {
            // Intentionally left blank, since we don't observe this
        }

        public void OnCreateTransportMessageEnd(DateTime startTime)
        {
            responseSerializationTime.Record(clock.UtcNow - startTime);
        }

        public void OnRemotingRequestBegin()
        {
            // Intentionally left blank, since we don't observe this
        }

        public void OnRemotingRequestEnd(DateTime startTime)
        {
            requestDeserializationTime.Record(clock.UtcNow - startTime);
        }

        public void OnRequestResponseBegin()
        {
            // Intentionally left blank, since we don't observe this
        }

        public void OnRequestResponseEnd(DateTime startTime)
        {
            requestProcessingTime.Record(clock.UtcNow - startTime);
        }
    }
}
