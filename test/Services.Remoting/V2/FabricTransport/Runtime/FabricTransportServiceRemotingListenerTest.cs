// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics;
using Microsoft.ServiceFabric.FabricTransport.Runtime;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime
{
    public abstract class FabricTransportServiceRemotingListenerTest
    {
        public class Dispose : FabricTransportServiceRemotingListenerTest
        {
            [Fact]
            public void DisposesMeterProvider()
            {
                var sut = Type<FabricTransportServiceRemotingListener>.Uninitialized();

                var mockMeterProvider = Mock.Of<IMeterProvider<TimeSpan>>();
                var mockFabricTransportListener = Type<FabricTransportListener>.Uninitialized();
                var mockTransportMessageHandler = Type<FabricTransportMessageHandler>.Uninitialized();

                // Set up diagnostic events on the handler so Dispose() doesn't null-ref
                mockTransportMessageHandler.Field<Diagnostic.IDiagnosticEvents>().Set(Mock.Of<Diagnostic.IDiagnosticEvents>());

                sut.Field<IMeterProvider<TimeSpan>>().Set(mockMeterProvider);
                sut.Field<FabricTransportListener>().Set(mockFabricTransportListener);
                sut.Field<FabricTransportMessageHandler>().Set(mockTransportMessageHandler);

                sut.Abort();

                Mock.Get(mockMeterProvider).Verify(m => m.Dispose(), Times.Once);
            }

            [Fact]
            public void DisposesTransportMessageHandler()
            {
                var sut = Type<FabricTransportServiceRemotingListener>.Uninitialized();

                var mockMeterProvider = Mock.Of<IMeterProvider<TimeSpan>>();
                var mockDiagnosticEvents = Mock.Of<Diagnostic.IDiagnosticEvents>();
                var mockFabricTransportListener = Type<FabricTransportListener>.Uninitialized();
                var mockTransportMessageHandler = Type<FabricTransportMessageHandler>.Uninitialized();

                mockTransportMessageHandler.Field<Diagnostic.IDiagnosticEvents>().Set(mockDiagnosticEvents);

                sut.Field<IMeterProvider<TimeSpan>>().Set(mockMeterProvider);
                sut.Field<FabricTransportListener>().Set(mockFabricTransportListener);
                sut.Field<FabricTransportMessageHandler>().Set(mockTransportMessageHandler);

                sut.Abort();

                // Verify handler's Dispose cascades to its diagnosticEvents
                Mock.Get(mockDiagnosticEvents).Verify(d => d.Dispose(), Times.Once);
            }
        }
    }
}
