// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Util;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{
    public class PerformanceCounterDiagnosticEventsTest
    {
        internal readonly ServiceRemotingPerformanceCounterProvider performanceCounterProvider = new ServiceRemotingPerformanceCounterProvider(Guid.NewGuid(), 0);
        internal readonly IClock mockClock = Mock.Of<IClock>();
        internal PerformanceCounterDiagnosticEvents sut;

        public PerformanceCounterDiagnosticEventsTest()
        {
            this.sut = new PerformanceCounterDiagnosticEvents(performanceCounterProvider, mockClock);
        }

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
                var providerField = sut.Field<ServiceRemotingPerformanceCounterProvider>("performanceCounterProvider").Value;
                Assert.NotNull(providerField);
                Assert.IsAssignableFrom<ServiceRemotingPerformanceCounterProvider>(providerField);

                var clock = sut.Field<IClock>("clock").Value;
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
            //private AbstractFabricCounterWriterWrapper mockRequestProcessingTimeCounterWriter = Mock.Of<AbstractFabricCounterWriterWrapper>();
            //private AbstractFabricCounterWriterWrapper mockRequestDeserializationTimeCounterWriter = Mock.Of<AbstractFabricCounterWriterWrapper>();
            //private AbstractFabricCounterWriterWrapper mockResponseSerializationTimeCounterWriter = Mock.Of<AbstractFabricCounterWriterWrapper>();
            //private AbstractFabricCounterWriterWrapper mockOutstandingRequestsCounterWriter = Mock.Of<AbstractFabricCounterWriterWrapper>();

            //public OnEvent()
            //{
            //    sut.Field<AbstractFabricCounterWriterWrapper>("serviceRequestProcessingTimeCounterWriter").Set(mockRequestProcessingTimeCounterWriter);
            //    sut.Field<AbstractFabricCounterWriterWrapper>("serviceRequestDeserializationTimeCounterWriter").Set(mockRequestDeserializationTimeCounterWriter);
            //    sut.Field<AbstractFabricCounterWriterWrapper>("serviceResponseSerializationTimeCounterWriter").Set(mockResponseSerializationTimeCounterWriter);
            //    sut.Field<AbstractFabricCounterWriterWrapper>("serviceOutstandingRequestsCounterWriter").Set(mockOutstandingRequestsCounterWriter);

            //    var provider = new ServiceRemotingPerformanceCounterProvider(1, 1);
            //    provider.Property<FabricAverageCount64PerformanceCounterWriter>(nameof(provider.ServiceRequestProcessingTimeCounterWriter))
            //        .Set(mockRequestProcessingTimeCounterWriter);

            //}

            //[Fact] 
            //public void RequestBeginIncrementCounter() 
            //{                 
                
            //    sut.OnRequestResponseBegin();

            //    Mock.Get(mockOutstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Once);
            //}

            //[Fact]
            //public void RequestBeginIgnoreIfWriterNull()
            //{
            //    sut.Field<AbstractFabricCounterWriterWrapper>("serviceOutstandingRequestsCounterWriter").Set(null);
            //    sut.OnRequestResponseBegin();

            //    Mock.Get(mockOutstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);

            //    sut.Field<AbstractFabricCounterWriterWrapper>("serviceOutstandingRequestsCounterWriter").Set(mockRequestProcessingTimeCounterWriter);
            //}

            //[Fact]
            //public void RequestEndDecrementCounterAndUpdateProcessingTime()
            //{
            //    DateTime requestStartTime = DateTime.UtcNow;
            //    Mock.Get(mockClock).Setup(x => x.UtcNow).Returns(requestStartTime.AddMilliseconds(100));
            //    sut.OnRequestResponseEnd(requestStartTime);

            //    Mock.Get(mockOutstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(-1), Times.Once);
            //    Mock.Get(mockRequestProcessingTimeCounterWriter).Verify(x => x.UpdateCounterValue(100), Times.Once);
            //}

            //[Fact]
            //public void RequestEndIgnoreIfAnyWritersNull()
            //{
            //    sut.Field<AbstractFabricCounterWriterWrapper>("serviceOutstandingRequestsCounterWriter").Set(null);
            //    sut.Field<AbstractFabricCounterWriterWrapper>("serviceRequestProcessingTimeCounterWriter").Set(null);
            //    sut.OnRequestResponseEnd(DateTime.UtcNow);

            //    Mock.Get(mockOutstandingRequestsCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);
            //    Mock.Get(mockRequestProcessingTimeCounterWriter).Verify(x => x.UpdateCounterValue(It.IsAny<long>()), Times.Never);

            //    sut.Field<AbstractFabricCounterWriterWrapper>("serviceOutstandingRequestsCounterWriter").Set(mockOutstandingRequestsCounterWriter);
            //    sut.Field<AbstractFabricCounterWriterWrapper>("serviceRequestProcessingTimeCounterWriter").Set(mockRequestProcessingTimeCounterWriter);
            //}
        }
    }
}
