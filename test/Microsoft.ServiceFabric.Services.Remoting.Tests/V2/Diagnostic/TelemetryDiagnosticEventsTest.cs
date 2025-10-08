// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics;
using Microsoft.ServiceFabric.TestFramework;
using Moq;
using Xunit;
using IClock = Microsoft.ServiceFabric.Diagnostics.IClock;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    public class TelemetryDiagnosticEventsTest : MockedTelemetryTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        TelemetryDiagnosticEvents sut;

        readonly IClock clock = Mock.Of<IClock>();

        protected TelemetryDiagnosticEventsTest() => sut = new TelemetryDiagnosticEvents(fuzzy.ServiceContext(), clock);

        public class Class : TelemetryDiagnosticEventsTest
        {
            [Fact]
            public void ImplementsIDiagnosticsEvents()
            {
                Assert.True(typeof(IDiagnosticEvents).IsAssignableFrom(sut.GetType()));
            }
        }

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
                var requestProcessingTime = sut.Field<IMeter<TimeSpan>>(nameof(sut.requestProcessingTime));
                var requestDeserializationTime = sut.Field<IMeter<TimeSpan>>(nameof(sut.requestDeserializationTime));
                var responseSerializationTime = sut.Field<IMeter<TimeSpan>>(nameof(sut.responseSerializationTime));

                Assert.NotNull(requestProcessingTime);
                Assert.NotNull(requestDeserializationTime);
                Assert.NotNull(responseSerializationTime);
            }

            [Fact]
            public void ThrowsOnNullClock()
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    new TelemetryDiagnosticEvents(fuzzy.ServiceContext(), null);
                });
            }

            [Fact]
            public void ThrowsOnNullServiceContext()
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    new TelemetryDiagnosticEvents(null, clock);
                });
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
                sut.Field<IMeter<TimeSpan>>(nameof(sut.requestProcessingTime)).Set(mockRequestProcessingTime);
                sut.Field<IMeter<TimeSpan>>(nameof(sut.requestDeserializationTime)).Set(mockRequestDeserializationTime);
                sut.Field<IMeter<TimeSpan>>(nameof(sut.responseSerializationTime)).Set(mockResponseSerializationTime);

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
