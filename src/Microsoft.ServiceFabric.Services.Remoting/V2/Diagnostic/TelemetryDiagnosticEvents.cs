// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
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

        public TelemetryDiagnosticEvents(ServiceContext serviceContext, IClock clock)
        {
            _ = serviceContext ?? throw new ArgumentNullException(nameof(serviceContext));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));

            var timeSpanMeterProvider = new TimeSpanMeterProvider(serviceContext);
            this.requestProcessingTime = timeSpanMeterProvider.CreateMeter("Services.Remoting", "MessageHandler.RequestProcessingTime");
            this.requestDeserializationTime = timeSpanMeterProvider.CreateMeter("Services.Remoting", "MessageHandler.RequestDeserializationTime");
            this.responseSerializationTime = timeSpanMeterProvider.CreateMeter("Services.Remoting", "MessageHandler.ResponseSerializationTime");
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
