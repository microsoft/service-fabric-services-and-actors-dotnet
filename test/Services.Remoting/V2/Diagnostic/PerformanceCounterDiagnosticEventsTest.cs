// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric.Common;
using FluentAssertions.Extensions;
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
        readonly IClock clock = Mock.Of<IClock>();

        readonly IDiagnosticEvents sut;

        protected PerformanceCounterDiagnosticEventsTest() => sut = new PerformanceCounterDiagnosticEvents(performanceCounterProvider, clock);


        public class Constructor : PerformanceCounterDiagnosticEventsTest
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
                    new PerformanceCounterDiagnosticEvents(null, clock);
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

        public class OnEvent : PerformanceCounterDiagnosticEventsTest
        {
            private FabricAverageCount64PerformanceCounterWriter requestProcessingTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            private FabricAverageCount64PerformanceCounterWriter requestDeserializationTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            private FabricAverageCount64PerformanceCounterWriter responseSerializationTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            private FabricNumberOfItems64PerformanceCounterWriter outstandingRequestsCounterWriter = Mock.Of<FabricNumberOfItems64PerformanceCounterWriter>();

            public OnEvent()
            {
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceRequestProcessingTimeCounterWriter))
                    .Set(requestProcessingTimeCounterWriter);
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceRequestDeserializationTimeCounterWriter))
                    .Set(requestDeserializationTimeCounterWriter);
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceResponseSerializationTimeCounterWriter))
                    .Set(responseSerializationTimeCounterWriter);
                performanceCounterProvider.Property<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceOutstandingRequestsCounterWriter))
                    .Set(outstandingRequestsCounterWriter);
            }

            [Fact]
            public void RequestBeginIncrementCounter()
            {
                sut.OnRequestResponseBegin();

                Mock.Get(outstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Once);
            }

            [Fact]
            public void RequestBeginIgnoreIfWriterNull()
            {
                performanceCounterProvider.Property<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceOutstandingRequestsCounterWriter))
                    .Set(null);

                sut.OnRequestResponseBegin();

                Mock.Get(outstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                performanceCounterProvider.Property<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceOutstandingRequestsCounterWriter))
                    .Set(outstandingRequestsCounterWriter);
            }

            [Fact]
            public void RequestEndDecrementCounterAndObserveProcessingTime()
            {
                DateTime requestStartTime = DateTime.UtcNow;
                Mock.Get(clock).Setup(x => x.UtcNow).Returns(requestStartTime.AddMilliseconds(100));

                sut.OnRequestResponseEnd(requestStartTime);

                Mock.Get(outstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(-1), Times.Once);
                Mock.Get(requestProcessingTimeCounterWriter).Verify(x => x.UpdateCounterValue(100), Times.Once);
            }

            [Fact]
            public void RequestEndIgnoreIfAnyWritersNull()
            {
                performanceCounterProvider.Property<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceOutstandingRequestsCounterWriter))
                    .Set(null);
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceRequestProcessingTimeCounterWriter))
                    .Set(null);

                sut.OnRequestResponseEnd(DateTime.UtcNow);

                Mock.Get(outstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(requestProcessingTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                performanceCounterProvider.Property<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceOutstandingRequestsCounterWriter))
                    .Set(outstandingRequestsCounterWriter);
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceRequestProcessingTimeCounterWriter))
                    .Set(requestProcessingTimeCounterWriter);
            }

            [Fact]
            public void RemotingMessageBeginObserveNothing()
            {
                sut.OnRemotingRequestBegin();

                Mock.Get(requestProcessingTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(requestDeserializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(responseSerializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(outstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
            }

            [Fact]
            public void RemotingMessageEndObserveSerializationTime()
            {
                DateTime requestStartTime = DateTime.UtcNow;
                Mock.Get(clock).Setup(x => x.UtcNow).Returns(requestStartTime.AddMilliseconds(100));

                sut.OnRemotingRequestEnd(requestStartTime);

                Mock.Get(responseSerializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(100), Times.Once);
            }

            [Theory]
            [InlineData(0, 0)]
            [InlineData(50, 0)] // currently 0 due to precision limitations of DateTime class
            [InlineData(100000, 0)]
            [InlineData(400000, 0)]
            [InlineData(600000, 1)]
            [InlineData(990000, 1)]
            [InlineData(1100000, 1)]
            [InlineData(1600000, 2)]
            public void RemotingMessageEndObserveShortSerializationTime(long elapsedNanoseconds, long trackedElapsedMilliseconds)
            {
                DateTime requestStartTime = DateTime.UtcNow;
                Mock.Get(clock).Setup(x => x.UtcNow).Returns(requestStartTime.AddNanoseconds(elapsedNanoseconds));

                sut.OnRemotingRequestEnd(requestStartTime);

                Mock.Get(responseSerializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(trackedElapsedMilliseconds), Times.Once);
            }

            [Fact]
            public void RemotingMessageEndIgnoreIfWriterNull()
            {
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceResponseSerializationTimeCounterWriter))
                   .Set(null);

                sut.OnRemotingRequestEnd(DateTime.UtcNow);

                Mock.Get(responseSerializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceResponseSerializationTimeCounterWriter))
                    .Set(responseSerializationTimeCounterWriter);
            }

            [Fact]
            public void TransportMessageBeginObserveNothing()
            {
                sut.OnCreateTransportMessageBegin();

                Mock.Get(requestProcessingTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(requestDeserializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(responseSerializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(outstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
            }

            [Fact]
            public void TransportMessageEndObserveDeserializationTime()
            {
                DateTime requestStartTime = DateTime.UtcNow;
                Mock.Get(clock).Setup(x => x.UtcNow).Returns(requestStartTime.AddMilliseconds(100));

                sut.OnCreateTransportMessageEnd(requestStartTime);
                Mock.Get(requestDeserializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(100), Times.Once);
            }

            [Theory]
            [InlineData(0, 0)]
            [InlineData(50, 0)] // currently 0 due to precision limitations of DateTime class
            [InlineData(100000, 0)]
            [InlineData(400000, 0)]
            [InlineData(600000, 1)]
            [InlineData(990000, 1)]
            [InlineData(1100000, 1)]
            [InlineData(1600000, 2)]
            public void TransportMessageEndObserveShortDeserializationTime(long elapsedNanoseconds, long trackedElapsedMilliseconds)
            {
                DateTime requestStartTime = DateTime.UtcNow;
                Mock.Get(clock).Setup(x => x.UtcNow).Returns(requestStartTime.AddNanoseconds(elapsedNanoseconds));

                sut.OnCreateTransportMessageEnd(requestStartTime);

                Mock.Get(requestDeserializationTimeCounterWriter).Verify(x => x.UpdateCounterValue(trackedElapsedMilliseconds), Times.Once);
            }

            [Fact]
            public void TransportMessageEndIgnoreIfWriterNull()
            {
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceRequestDeserializationTimeCounterWriter))
                   .Set(null);

                sut.OnCreateTransportMessageEnd(DateTime.UtcNow);

                Mock.Get(outstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                performanceCounterProvider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.ServiceRequestDeserializationTimeCounterWriter))
                    .Set(requestDeserializationTimeCounterWriter);
            }
        }

        public class Dispose : PerformanceCounterDiagnosticEventsTest
        {
            [Fact]
            public void DoesNotThrow()
            {
                sut.Dispose();
            }
        }
    }
}
