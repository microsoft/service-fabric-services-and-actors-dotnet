// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class PerformanceCounterDiagnosticEvents : IDiagnosticEvents
    {

        private AbstractFabricCounterWriterWrapper serviceRequestProcessingTimeCounterWriter;
        private AbstractFabricCounterWriterWrapper serviceRequestDeserializationTimeCounterWriter;
        private AbstractFabricCounterWriterWrapper serviceResponseSerializationTimeCounterWriter;
        private AbstractFabricCounterWriterWrapper serviceOutstandingRequestsCounterWriter;

        public PerformanceCounterDiagnosticEvents(IServiceRemotingPerformanceCounterWriterProvider performanceCounterProvider)
        {
            if(performanceCounterProvider == null)
                throw new ArgumentException(nameof(performanceCounterProvider));

            this.serviceRequestProcessingTimeCounterWriter = new FabricAverageCountPerformanceCounterWrapper(
                performanceCounterProvider.ServiceRequestProcessingTimeCounterWriter);
            this.serviceRequestDeserializationTimeCounterWriter = new FabricAverageCountPerformanceCounterWrapper(
                performanceCounterProvider.ServiceRequestDeserializationTimeCounterWriter);
            this.serviceResponseSerializationTimeCounterWriter = new FabricAverageCountPerformanceCounterWrapper(
                performanceCounterProvider.ServiceResponseSerializationTimeCounterWriter);
            this.serviceOutstandingRequestsCounterWriter = new FabricNumberOfItemsPerformanceCounterWrapper(
                performanceCounterProvider.ServiceOutstandingRequestsCounterWriter);
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
