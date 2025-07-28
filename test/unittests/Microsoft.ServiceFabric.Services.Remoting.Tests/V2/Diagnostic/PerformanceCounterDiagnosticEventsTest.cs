// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric.Common;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{
    public class PerformanceCounterDiagnosticEventsTest
    {
        readonly ServiceRemotingPerformanceCounterProvider performanceCounterProvider = new ServiceRemotingPerformanceCounterProvider(Guid.NewGuid(), 0);
        readonly IClock mockClock = Mock.Of<IClock>();
        PerformanceCounterDiagnosticEvents sut;

        protected PerformanceCounterDiagnosticEventsTest() => sut = new PerformanceCounterDiagnosticEvents(performanceCounterProvider, mockClock);

        public class Class : PerformanceCounterDiagnosticEventsTest
        {
            [Fact]
            public void ImplementsIDiagnosticsEvents()
            {
                var performanceCounterDiagnosticsSourceType = typeof(PerformanceCounterDiagnosticEvents);
                var iDiagnosticsSourceType = typeof(IDiagnosticEvents);

                Assert.True(iDiagnosticsSourceType.IsAssignableFrom(performanceCounterDiagnosticsSourceType));
            }
        }

        public class Constructor: PerformanceCounterDiagnosticEventsTest
        {
            [Fact]
            public void WithParametersPresent()
            {
                var sutType = typeof(PerformanceCounterDiagnosticEvents);
                var expectedParameterTypes = new[] { typeof(ServiceRemotingPerformanceCounterProvider), typeof(IClock) };
                var constructor = sutType.GetConstructor(expectedParameterTypes);
                Assert.NotNull(constructor);
            }

            [Fact]
            public void WithParametersSetsValue()
            {
                var providerField = sut.Field<ServiceRemotingPerformanceCounterProvider>().Value;
                Assert.NotNull(providerField);
                Assert.IsAssignableFrom<ServiceRemotingPerformanceCounterProvider>(providerField);

                var clock = sut.Field<IClock>().Value;
                Assert.NotNull(clock);
                Assert.IsAssignableFrom<IClock>(clock);
            }

            [Fact]
            public void ThrowsOnNullProvider()
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    new PerformanceCounterDiagnosticEvents(null, mockClock);
                });
            }

            [Fact]
            public void ThrowsOnNullClock()
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    new PerformanceCounterDiagnosticEvents(performanceCounterProvider, null);
                });
            }
        }

        public class OnEvent: PerformanceCounterDiagnosticEventsTest
        {
            private FabricAverageCount64PerformanceCounterWriter mockRequestProcessingTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            private FabricAverageCount64PerformanceCounterWriter mockRequestDeserializationTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            private FabricAverageCount64PerformanceCounterWriter mockResponseSerializationTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            private FabricNumberOfItems64PerformanceCounterWriter mockOutstandingRequestsCounterWriter = Mock.Of<FabricNumberOfItems64PerformanceCounterWriter>();

            public OnEvent()
            {
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceRequestProcessingTimeCounterWriter))
                    .Set(mockRequestProcessingTimeCounterWriter);
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceRequestDeserializationTimeCounterWriter))
                    .Set(mockRequestDeserializationTimeCounterWriter);
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceResponseSerializationTimeCounterWriter))
                    .Set(mockResponseSerializationTimeCounterWriter);
                performanceCounterProvider.Property<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceOutstandingRequestsCounterWriter))
                    .Set(mockOutstandingRequestsCounterWriter); 
            }

            [Fact]
            public void RequestBeginIncrementCounter()
            {
                sut.OnRequestResponseBegin();

                Mock.Get(mockOutstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Once);
            }

            [Fact]
            public void RequestBeginIgnoreIfWriterNull()
            {
                performanceCounterProvider.Property<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceOutstandingRequestsCounterWriter))
                    .Set(null); 
                
                sut.OnRequestResponseBegin();

                Mock.Get(mockOutstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                performanceCounterProvider.Property<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceOutstandingRequestsCounterWriter))
                    .Set(mockOutstandingRequestsCounterWriter);
            }

            [Fact]
            public void RequestEndDecrementCounterAndObserveProcessingTime()
            {
                DateTime requestStartTime = DateTime.UtcNow;
                Mock.Get(mockClock).Setup(x => x.UtcNow).Returns(requestStartTime.AddMilliseconds(100));

                sut.OnRequestResponseEnd(requestStartTime);

                Mock.Get(mockOutstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(-1), Times.Once);
                Mock.Get(mockRequestProcessingTimeCounterWriter).Verify(x => x.UpdateCounterValue(100), Times.Once);
            }

            [Fact]
            public void RequestEndIgnoreIfAnyWritersNull()
            {
                performanceCounterProvider.Property<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceOutstandingRequestsCounterWriter))
                    .Set(null);
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceRequestProcessingTimeCounterWriter))
                    .Set(null);

                sut.OnRequestResponseEnd(DateTime.UtcNow);

                Mock.Get(mockOutstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(mockRequestProcessingTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                performanceCounterProvider.Property<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceOutstandingRequestsCounterWriter))
                    .Set(mockOutstandingRequestsCounterWriter);
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceRequestProcessingTimeCounterWriter))
                    .Set(mockRequestProcessingTimeCounterWriter);
            }

            [Fact]
            public void RemotingMessageBeginObserveNothing()
            {
                sut.OnRemotingRequestBegin();

                Mock.Get(mockRequestProcessingTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(mockRequestDeserializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(mockResponseSerializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(mockOutstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
            }

            [Fact]
            public void RemotingMessageEndObserveSerializationTime()
            {
                DateTime requestStartTime = DateTime.UtcNow;
                Mock.Get(mockClock).Setup(x => x.UtcNow).Returns(requestStartTime.AddMilliseconds(100));

                sut.OnRemotingRequestEnd(requestStartTime);

                Mock.Get(mockResponseSerializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(100), Times.Once);
            }

            [Fact]
            public void RemotingMessageEndIgnoreIfWriterNull()
            {
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceResponseSerializationTimeCounterWriter))
                   .Set(null);

                sut.OnRemotingRequestEnd(DateTime.UtcNow);

                Mock.Get(mockResponseSerializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceResponseSerializationTimeCounterWriter))
                    .Set(mockResponseSerializationTimeCounterWriter);
            }

            [Fact]
            public void TransportMessageBeginObserveNothing()
            {
                sut.OnCreateTransportMessageBegin();

                Mock.Get(mockRequestProcessingTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(mockRequestDeserializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(mockResponseSerializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(mockOutstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
            }

            [Fact]
            public void TransportMessageEndObserveDeserializationTime()
            {
                DateTime requestStartTime = DateTime.UtcNow;
                Mock.Get(mockClock).Setup(x => x.UtcNow).Returns(requestStartTime.AddMilliseconds(100));

                sut.OnCreateTransportMessageEnd(requestStartTime);
                Mock.Get(mockRequestDeserializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(100), Times.Once);
            }

            [Fact]
            public void TransportMessageEndIgnoreIfWriterNull()
            {
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceRequestDeserializationTimeCounterWriter))
                   .Set(null);

                sut.OnCreateTransportMessageEnd(DateTime.UtcNow);

                Mock.Get(mockOutstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceRequestDeserializationTimeCounterWriter))
                    .Set(mockRequestDeserializationTimeCounterWriter);
            }
        }
    }
}
