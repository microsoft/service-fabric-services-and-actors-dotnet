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
    public class TelemetryDiagnosticEventsTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IDiagnosticEvents sut;

        readonly IClock clock = Mock.Of<IClock>();
        readonly IMeterProvider<TimeSpan> meterProvider = new Mock<IMeterProvider<TimeSpan>>() { DefaultValue = DefaultValue.Mock }.Object;

        protected TelemetryDiagnosticEventsTest() => sut = new TelemetryDiagnosticEvents(meterProvider, clock);

        public class Constructor : TelemetryDiagnosticEventsTest
        {
            [Fact]
            public void ThrowsOnNullClock()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new TelemetryDiagnosticEvents(meterProvider, null));
                Assert.Equal("clock", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullMeterProvider()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new TelemetryDiagnosticEvents(null, clock));
                Assert.Equal("meterProvider", exception.ParamName);
            }

            [Fact]
            public void WithParametersCreatesMeters()
            {
                var mockMeterProvider = Mock.Get(meterProvider);

                mockMeterProvider.Verify(x => x.CreateMeter(It.Is<string>(x => x == "Services.Remoting"), It.Is<string>(x => x == "MessageHandler.RequestProcessingTime")), Times.Once);
                mockMeterProvider.Verify(x => x.CreateMeter(It.Is<string>(x => x == "Services.Remoting"), It.Is<string>(x => x == "MessageHandler.RequestDeserializationTime")), Times.Once);
                mockMeterProvider.Verify(x => x.CreateMeter(It.Is<string>(x => x == "Services.Remoting"), It.Is<string>(x => x == "MessageHandler.ResponseSerializationTime")), Times.Once);
            }
        }

        public class OnEvents : TelemetryDiagnosticEventsTest
        {
            readonly IMeter<TimeSpan> mockRequestProcessingTime;
            readonly IMeter<TimeSpan> mockRequestDeserializationTime;
            readonly IMeter<TimeSpan> mockResponseSerializationTime;
            readonly DateTime endTime;
            readonly DateTime startTime;
            readonly double durationMilliseconds = fuzzy.Double(0, 5000);

            public OnEvents()
            {
                mockRequestProcessingTime = sut.Field<IMeter<TimeSpan>>("requestProcessingTime").Value;
                mockRequestDeserializationTime = sut.Field<IMeter<TimeSpan>>("requestDeserializationTime").Value;
                mockResponseSerializationTime = sut.Field<IMeter<TimeSpan>>("responseSerializationTime").Value;

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

        public class Dispose : TelemetryDiagnosticEventsTest
        {
            readonly IMeter<TimeSpan> mockRequestProcessingTime;
            readonly IMeter<TimeSpan> mockRequestDeserializationTime;
            readonly IMeter<TimeSpan> mockResponseSerializationTime;

            public Dispose()
            {
                mockRequestProcessingTime = sut.Field<IMeter<TimeSpan>>("requestProcessingTime").Value;
                mockRequestDeserializationTime = sut.Field<IMeter<TimeSpan>>("requestDeserializationTime").Value;
                mockResponseSerializationTime = sut.Field<IMeter<TimeSpan>>("responseSerializationTime").Value;
            }

            [Fact]
            public void DisposesAllMeters()
            {
                sut.Dispose();

                Mock.Get(mockRequestProcessingTime).Verify(m => m.Dispose(), Times.Once);
                Mock.Get(mockRequestDeserializationTime).Verify(m => m.Dispose(), Times.Once);
                Mock.Get(mockResponseSerializationTime).Verify(m => m.Dispose(), Times.Once);
            }
        }
    }
}
