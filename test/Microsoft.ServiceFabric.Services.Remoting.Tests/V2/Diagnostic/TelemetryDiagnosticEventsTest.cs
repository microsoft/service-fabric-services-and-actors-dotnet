// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics;
using Microsoft.ServiceFabric.TestFramework;
using Moq;
using Xunit;
using IClock = Microsoft.ServiceFabric.Diagnostics.IClock;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    public class TelemetryDiagnosticEventsTest : MockedMetricsTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IDiagnosticEvents sut;

        readonly IClock clock = Mock.Of<IClock>();
        readonly ServiceContext serviceContext = fuzzy.ServiceContext();

        protected TelemetryDiagnosticEventsTest() => sut = new TelemetryDiagnosticEvents(serviceContext, clock);

        public class Constructor : TelemetryDiagnosticEventsTest
        {
            [Fact]
            public void WithParametersSetsClock()
            {
                var actualClock = sut.Field<IClock>().Value;
                Assert.Equal(clock, actualClock);
            }

            [Fact]
            public void WithParametersSetsMeters()
            {
                var requestProcessingTime = sut.Field<IMeter<TimeSpan>>("requestProcessingTime");
                var requestDeserializationTime = sut.Field<IMeter<TimeSpan>>("requestProcessingTime");
                var responseSerializationTime = sut.Field<IMeter<TimeSpan>>("requestProcessingTime");

                Assert.NotNull(requestProcessingTime);
                Assert.NotNull(requestDeserializationTime);
                Assert.NotNull(responseSerializationTime);
            }

            [Fact]
            public void ThrowsOnNullClock()
            {
                var exception = Assert.Throws<ArgumentNullException>(() =>
                {
                    new TelemetryDiagnosticEvents(serviceContext, null);
                });

                Assert.Equal("clock", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullServiceContext()
            {
                var exception = Assert.Throws<ArgumentNullException>(() =>
                {
                    new TelemetryDiagnosticEvents(null, clock);
                });

                Assert.Equal("serviceContext", exception.ParamName);
            }
        }

        public class OnEvents : TelemetryDiagnosticEventsTest
        {
            readonly IMeter<TimeSpan> mockRequestProcessingTime = Mock.Of<IMeter<TimeSpan>>();
            readonly IMeter<TimeSpan> mockRequestDeserializationTime = Mock.Of<IMeter<TimeSpan>>();
            readonly IMeter<TimeSpan> mockResponseSerializationTime = Mock.Of<IMeter<TimeSpan>>();
            readonly DateTime endTime;
            readonly DateTime startTime;
            readonly double durationMilliseconds = fuzzy.Double(0, 5000);

            public OnEvents()
            {
                sut.Field<IMeter<TimeSpan>>("requestProcessingTime").Set(mockRequestProcessingTime);
                sut.Field<IMeter<TimeSpan>>("requestProcessingTime").Set(mockRequestDeserializationTime);
                sut.Field<IMeter<TimeSpan>>("requestProcessingTime").Set(mockResponseSerializationTime);

                startTime = DateTime.UtcNow;
                endTime = startTime.AddMilliseconds(durationMilliseconds);

                Mock.Get(clock).Setup(x => x.UtcNow).Returns(endTime);
            }

            [Fact]
            public void OnBeginMethodsObserveNothing()
            {
                sut.OnRequestResponseBegin();
                sut.OnCreateTransportMessageBegin();
                sut.OnRemotingRequestBegin();

                Mock.Get(mockRequestProcessingTime).Verify(x => x.Record(It.IsAny<TimeSpan>()), Times.Never);
                Mock.Get(mockRequestDeserializationTime).Verify(x => x.Record(It.IsAny<TimeSpan>()), Times.Never);
                Mock.Get(mockResponseSerializationTime).Verify(x => x.Record(It.IsAny<TimeSpan>()), Times.Never);
            }

            [Fact]
            public void OnRequestEndObserveRequestProcessingTime()
            {
                sut.OnRequestResponseEnd(startTime);

                Mock.Get(mockRequestProcessingTime).Verify(x => x.Record(It.Is<TimeSpan>(ts => Math.Abs(ts.TotalMilliseconds - durationMilliseconds) < 0.0001)), Times.Once);
            }

            [Fact]
            public void OnRemotingRequestEndObserveRequestProcessingTime()
            {
                sut.OnRemotingRequestEnd(startTime);

                Mock.Get(mockRequestDeserializationTime).Verify(x => x.Record(It.Is<TimeSpan>(ts => Math.Abs(ts.TotalMilliseconds - durationMilliseconds) < 0.0001)), Times.Once);
            }

            [Fact]
            public void OnCreateTransportEndObserveRequestProcessingTime()
            {
                sut.OnCreateTransportMessageEnd(startTime);

                Mock.Get(mockResponseSerializationTime).Verify(x => x.Record(It.Is<TimeSpan>(ts => Math.Abs(ts.TotalMilliseconds - durationMilliseconds) < 0.0001)), Times.Once);
            }
        }
    }
}
