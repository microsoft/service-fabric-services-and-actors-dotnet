using System;
using System.Fabric;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics.Metrics;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    public class TelemetryDiagnosticEventsTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IClock clock = Mock.Of<IClock>();
        readonly ServiceContext serviceContext = fuzzy.ServiceContext();
        TelemetryDiagnosticEvents sut;

        protected TelemetryDiagnosticEventsTest()
        {
            typeof(MeterProvider<TimeSpan>).Field<Func<IFabricMeterProvider>>().Set(() => new Mock<IFabricMeterProvider>() { DefaultValue = DefaultValue.Mock }.Object);

            sut = new TelemetryDiagnosticEvents(serviceContext, clock);
        }

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
                    new TelemetryDiagnosticEvents(serviceContext, null);
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
            readonly DateTime currentTime;
            readonly double durationMilliseconds = fuzzy.Double(0, 5000);

            public OnEvents()
            {
                sut.Field<IMeter<TimeSpan>>(nameof(sut.requestProcessingTime)).Set(mockRequestProcessingTime);
                sut.Field<IMeter<TimeSpan>>(nameof(sut.requestDeserializationTime)).Set(mockRequestDeserializationTime);
                sut.Field<IMeter<TimeSpan>>(nameof(sut.responseSerializationTime)).Set(mockResponseSerializationTime);

                this.currentTime = DateTime.UtcNow;
                Mock.Get(clock).Setup(x => x.UtcNow).Returns(currentTime);
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
                var startTime = currentTime.AddMilliseconds(-durationMilliseconds);

                sut.OnRequestResponseEnd(startTime);

                Mock.Get(mockRequestProcessingTime).Verify(x => x.Record(It.Is<TimeSpan>(ts => Math.Abs(ts.TotalMilliseconds - durationMilliseconds) < 0.0001)), Times.Once);
            }

            [Fact]
            public void OnRemotingRequestEndObserveRequestProcessingTime()
            {
                var startTime = currentTime.AddMilliseconds(-durationMilliseconds);

                sut.OnRemotingRequestEnd(startTime);

                Mock.Get(mockRequestDeserializationTime).Verify(x => x.Record(It.Is<TimeSpan>(ts => Math.Abs(ts.TotalMilliseconds - durationMilliseconds) < 0.0001)), Times.Once);
            }

            [Fact]
            public void OnCreateTransportEndObserveRequestProcessingTime()
            {
                var startTime = currentTime.AddMilliseconds(-durationMilliseconds);

                sut.OnCreateTransportMessageEnd(startTime);

                Mock.Get(mockResponseSerializationTime).Verify(x => x.Record(It.Is<TimeSpan>(ts => Math.Abs(ts.TotalMilliseconds - durationMilliseconds) < 0.0001)), Times.Once);
            }
        }


    }
}
