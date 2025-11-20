// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Actors.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics.Metrics;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Tests.Diagnostics
{
    public class MetricDiagnosticsTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IDiagnostics sut;

        readonly IClock clock = Mock.Of<IClock>();
        readonly IMeterProvider<TimeSpan> mockTimeSpanMeterProvider = new Mock<IMeterProvider<TimeSpan>>() { DefaultValue = DefaultValue.Mock }.Object;
        readonly IMeterProvider<long> mockLongMeterProvider = new Mock<IMeterProvider<long>>() { DefaultValue = DefaultValue.Mock }.Object;

        public MetricDiagnosticsTest() => sut = new MetricDiagnostics(mockLongMeterProvider, mockTimeSpanMeterProvider, clock);

        protected bool DurationsApproximatelyEqual(TimeSpan timeSpan, double durationMilliseconds)
        {
            return Math.Abs(timeSpan.TotalMilliseconds - durationMilliseconds) < 0.0001;
        }

        public class Constructor : MetricDiagnosticsTest
        {
            [Fact]
            public void ThrowsOnNullClock()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new MetricDiagnostics(mockLongMeterProvider, mockTimeSpanMeterProvider, null));
                Assert.Equal("clock", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullLongMeterProvider()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new MetricDiagnostics(null, mockTimeSpanMeterProvider, clock));
                Assert.Equal("longMeterProvider", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullTimeSpanMeterProvider()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new MetricDiagnostics(mockLongMeterProvider, null, clock));
                Assert.Equal("timeSpanProvider", exception.ParamName);
            }

            [Fact]
            public void WithParametersCreatesMeters()
            {
                Mock.Get(mockLongMeterProvider).Verify(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "ActorLockContention")), Times.Once);
                Mock.Get(mockTimeSpanMeterProvider).Verify(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "AcquireLockDuration")), Times.Once);
                Mock.Get(mockTimeSpanMeterProvider).Verify(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "ReleaseLockDuration")), Times.Once);
                Mock.Get(mockLongMeterProvider).Verify(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "MethodExceptionCount"), It.Is<string>(x => x == "MethodId")), Times.Once);
                Mock.Get(mockTimeSpanMeterProvider).Verify(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "MethodExecutionDuration"), It.Is<string>(x => x == "MethodId")), Times.Once);
                Mock.Get(mockTimeSpanMeterProvider).Verify(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "OnActivateAsyncDuration")), Times.Once);
                Mock.Get(mockTimeSpanMeterProvider).Verify(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "RequestProcessingDuration")), Times.Once);
                Mock.Get(mockTimeSpanMeterProvider).Verify(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "LoadStateDuration")), Times.Once);
                Mock.Get(mockTimeSpanMeterProvider).Verify(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "SaveStateDuration")), Times.Once);
            }
        }

        public class OnEvents : MetricDiagnosticsTest
        {
            readonly IMeter<long> mockActorLockContention;
            readonly IMeter<TimeSpan> mockAcquireLockDuration;
            readonly IMeter<TimeSpan> mockReleaseLockDuration;
            readonly IMeter1D<long> mockMethodExceptionCount;
            readonly IMeter1D<TimeSpan> mockMethodExecutionDuration;
            readonly IMeter<TimeSpan> mockOnActivateAsyncDuration;
            readonly IMeter<TimeSpan> mockRequestProcessingDuration;
            readonly IMeter<TimeSpan> mockLoadStateDuration;
            readonly IMeter<TimeSpan> mockSaveStateDuration;

            readonly DateTime endTime;
            readonly DateTime startTime;
            readonly double durationMilliseconds = fuzzy.Double(0, 5000);
            readonly ActorId actorId = fuzzy.ActorId();
            readonly long interfaceMethodKey = fuzzy.Int64();

            public OnEvents()
            {
                mockActorLockContention = sut.Field<IMeter<long>>("actorLockContention").Value;
                mockAcquireLockDuration = sut.Field<IMeter<TimeSpan>>("acquireLockDuration").Value;
                mockReleaseLockDuration = sut.Field<IMeter<TimeSpan>>("releaseLockDuration").Value;
                mockMethodExceptionCount = sut.Field<IMeter1D<long>>("methodExceptionCount").Value;
                mockMethodExecutionDuration = sut.Field<IMeter1D<TimeSpan>>("methodExecutionDuration").Value;
                mockOnActivateAsyncDuration = sut.Field<IMeter<TimeSpan>>("onActivateAsyncDuration").Value;
                mockRequestProcessingDuration = sut.Field<IMeter<TimeSpan>>("requestProcessingDuration").Value;
                mockLoadStateDuration = sut.Field<IMeter<TimeSpan>>("loadStateDuration").Value;
                mockSaveStateDuration = sut.Field<IMeter<TimeSpan>>("saveStateDuration").Value;

                startTime = DateTime.UtcNow;
                endTime = startTime.AddMilliseconds(durationMilliseconds);

                Mock.Get(clock).Setup(x => x.UtcNow).Returns(endTime);
            }

            [Fact]
            public void EmitNothingWhenNotNeeded()
            {
                sut.ActorActivated(actorId);
                sut.ActorChangeRole(ReplicaRole.Primary, ReplicaRole.IdleSecondary);
                sut.ActorDeactivated(actorId);
                sut.ActorMethodStart(actorId, interfaceMethodKey);
                sut.ActorOnActivateAsyncStart();
                sut.ActorRequestProcessingStart();
                sut.LoadActorStateStart();
                sut.SaveActorStateStart(actorId);

                Mock.Get(mockActorLockContention).VerifyNoOtherCalls();
                Mock.Get(mockAcquireLockDuration).VerifyNoOtherCalls();
                Mock.Get(mockReleaseLockDuration).VerifyNoOtherCalls();
                Mock.Get(mockMethodExceptionCount).VerifyNoOtherCalls();
                Mock.Get(mockMethodExecutionDuration).VerifyNoOtherCalls();
                Mock.Get(mockOnActivateAsyncDuration).VerifyNoOtherCalls();
                Mock.Get(mockRequestProcessingDuration).VerifyNoOtherCalls();
                Mock.Get(mockLoadStateDuration).VerifyNoOtherCalls();
                Mock.Get(mockSaveStateDuration).VerifyNoOtherCalls();
            }

            public class Lock : OnEvents
            {
                [Fact]
                public void AcquireFinishObserveLockContention()
                {
                    long pendingCalls = fuzzy.Int64();

                    sut.AcquireActorLockFinish(new PendingActorMethodDiagnosticData() { PendingActorMethodCalls = pendingCalls }, startTime);

                    Mock.Get(mockActorLockContention).Verify(x => x.Record(It.Is<long>(d => d == pendingCalls)), Times.Once);
                }

                [Fact]
                public void AcquireFinishObserveDuration()
                {
                    sut.AcquireActorLockFinish(new PendingActorMethodDiagnosticData(), startTime);

                    Mock.Get(mockAcquireLockDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds))), Times.Once);
                }

                [Fact]
                public void ReleaseObserveHoldDuration()
                {
                    sut.ReleaseActorLock(startTime);

                    Mock.Get(mockReleaseLockDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds))), Times.Once);
                }
            }

            public class Method : OnEvents
            {
                readonly int methodId = fuzzy.Int32();

                [Fact]
                public void FinishWithExceptionObserveExceptionFrequency()
                {
                    sut.ActorMethodFinish(new ActorMethodDiagnosticData() { Exception = new Exception(), MethodId = methodId }, startTime);

                    Mock.Get(mockMethodExceptionCount).Verify(x => x.Record(It.Is<long>(d => d == 1), It.Is<string>(m => m == methodId.ToString())), Times.Once);
                }

                [Fact]
                public void FinishWithoutExceptionDNotObserveExceptionFrequency()
                {
                    sut.ActorMethodFinish(new ActorMethodDiagnosticData(), startTime);

                    Mock.Get(mockMethodExceptionCount).VerifyNoOtherCalls();
                }

                [Fact]
                public void FinishObserveExecutionDuration()
                {
                    sut.ActorMethodFinish(new ActorMethodDiagnosticData() { Exception = new Exception(), MethodId = methodId }, startTime);

                    Mock.Get(mockMethodExecutionDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds)), It.Is<string>(m => m == methodId.ToString())), Times.Once);
                }
            }

            public class Activate : OnEvents
            {
                [Fact]
                public void FinishAsyncObserveDuration()
                {
                    sut.ActorOnActivateAsyncFinish(startTime);

                    Mock.Get(mockOnActivateAsyncDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds))), Times.Once);
                }
            }

            public class Request : OnEvents
            {
                [Fact]
                public void FinishObserveProcessingDuration()
                {
                    sut.ActorRequestProcessingFinish(startTime);

                    Mock.Get(mockRequestProcessingDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds))), Times.Once);
                }
            }

            public class State : OnEvents
            {
                [Fact]
                public void LoadFinishObserveDuration()
                {
                    sut.LoadActorStateFinish(startTime);

                    Mock.Get(mockLoadStateDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds))), Times.Once);
                }

                [Fact]
                public void SaveFinishObserveDuration()
                {
                    sut.SaveActorStateFinish(actorId, startTime);

                    Mock.Get(mockSaveStateDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds))), Times.Once);
                }
            }
        }

    }
}
