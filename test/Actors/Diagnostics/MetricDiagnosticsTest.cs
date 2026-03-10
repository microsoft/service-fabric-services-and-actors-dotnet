// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Actors.Diagnostics;
using Microsoft.ServiceFabric.Actors.Runtime;
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
        readonly ActorTypeInformation typeInfo = ActorTypeInformation.Get(typeof(TestActor));
        readonly ActorMethodFriendlyNameBuilder nameBuilder;

        readonly IMeter<long> pendingMethodCalls = Mock.Of<IMeter<long>>();
        readonly IMeter<TimeSpan> acquireLockDuration = Mock.Of<IMeter<TimeSpan>>();
        readonly IMeter<TimeSpan> releaseLockDuration = Mock.Of<IMeter<TimeSpan>>();
        readonly IMeter3D<TimeSpan> methodExecutionDuration = Mock.Of<IMeter3D<TimeSpan>>();
        readonly IMeter<TimeSpan> onActivateAsyncDuration = Mock.Of<IMeter<TimeSpan>>();
        readonly IMeter<TimeSpan> requestProcessingDuration = Mock.Of<IMeter<TimeSpan>>();
        readonly IMeter<TimeSpan> loadStateDuration = Mock.Of<IMeter<TimeSpan>>();
        readonly IMeter<TimeSpan> saveStateDuration = Mock.Of<IMeter<TimeSpan>>();

        public MetricDiagnosticsTest()
        {
            Mock.Get(mockLongMeterProvider).Setup(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "PendingMethodCalls"))).Returns(pendingMethodCalls);
            Mock.Get(mockTimeSpanMeterProvider).Setup(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "AcquireLockDuration"))).Returns(acquireLockDuration);
            Mock.Get(mockTimeSpanMeterProvider).Setup(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "ReleaseLockDuration"))).Returns(releaseLockDuration);
            Mock.Get(mockTimeSpanMeterProvider).Setup(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "MethodExecutionDuration"), It.Is<string>(x => x == "MethodName"), It.Is<string>(x => x == "MethodSigniture"), It.Is<string>(x => x == "Exception"))).Returns(methodExecutionDuration);
            Mock.Get(mockTimeSpanMeterProvider).Setup(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "OnActivateAsyncDuration"))).Returns(onActivateAsyncDuration);
            Mock.Get(mockTimeSpanMeterProvider).Setup(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "RequestProcessingDuration"))).Returns(requestProcessingDuration);
            Mock.Get(mockTimeSpanMeterProvider).Setup(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "LoadStateDuration"))).Returns(loadStateDuration);
            Mock.Get(mockTimeSpanMeterProvider).Setup(x => x.CreateMeter(It.Is<string>(x => x == "Actor"), It.Is<string>(x => x == "SaveStateDuration"))).Returns(saveStateDuration);

            nameBuilder = new ActorMethodFriendlyNameBuilder(typeInfo);
            sut = new MetricDiagnostics(mockLongMeterProvider, mockTimeSpanMeterProvider, clock, nameBuilder, typeInfo);
        }

        protected bool DurationsApproximatelyEqual(TimeSpan timeSpan, double durationMilliseconds)
        {
            return Math.Abs(timeSpan.TotalMilliseconds - durationMilliseconds) < 0.0001;
        }

        public class Constructor : MetricDiagnosticsTest
        {
            [Fact]
            public void ThrowsOnNullClock()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new MetricDiagnostics(mockLongMeterProvider, mockTimeSpanMeterProvider, null, nameBuilder, typeInfo));
                Assert.Equal("clock", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullLongMeterProvider()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new MetricDiagnostics(null, mockTimeSpanMeterProvider, clock, nameBuilder, typeInfo));
                Assert.Equal("longMeterProvider", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullTimeSpanMeterProvider()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new MetricDiagnostics(mockLongMeterProvider, null, clock, nameBuilder, typeInfo));
                Assert.Equal("timeSpanProvider", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullNameBuilder()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new MetricDiagnostics(mockLongMeterProvider, mockTimeSpanMeterProvider, clock, null, typeInfo));
                Assert.Equal("nameBuilder", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullTypeInfo()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new MetricDiagnostics(mockLongMeterProvider, mockTimeSpanMeterProvider, clock, nameBuilder, null));
                Assert.Equal("typeInfo", exception.ParamName);
            }
        }

        public class OnEvents : MetricDiagnosticsTest
        {
            readonly DateTime endTime;
            readonly DateTime startTime;
            readonly double durationMilliseconds = fuzzy.Double(0, 5000);
            readonly ActorId actorId = fuzzy.ActorId();
            readonly long interfaceMethodKey = fuzzy.Int64();
            readonly string methodName = fuzzy.String();
            readonly string methodSigniture = fuzzy.String();
            readonly Exception exception = new Exception();

            public OnEvents()
            {
                startTime = DateTime.UtcNow;
                endTime = startTime.AddMilliseconds(durationMilliseconds);

                Mock.Get(clock).Setup(x => x.UtcNow).Returns(endTime);

                Dictionary<long, ActorMethodInfo> actorMethodInfo = new Dictionary<long, ActorMethodInfo>();
                actorMethodInfo[interfaceMethodKey] = new ActorMethodInfo(methodName, methodSigniture);
                sut.Field<IReadOnlyDictionary<long, ActorMethodInfo>>().Set(actorMethodInfo);
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

                Mock.Get(pendingMethodCalls).VerifyNoOtherCalls();
                Mock.Get(acquireLockDuration).VerifyNoOtherCalls();
                Mock.Get(releaseLockDuration).VerifyNoOtherCalls();
                Mock.Get(methodExecutionDuration).VerifyNoOtherCalls();
                Mock.Get(onActivateAsyncDuration).VerifyNoOtherCalls();
                Mock.Get(requestProcessingDuration).VerifyNoOtherCalls();
                Mock.Get(loadStateDuration).VerifyNoOtherCalls();
                Mock.Get(saveStateDuration).VerifyNoOtherCalls();
            }

            public class Lock : OnEvents
            {
                [Fact]
                public void AcquireFinishObserveLockContention()
                {
                    long pendingCalls = fuzzy.Int64();

                    sut.AcquireActorLockFinish(new PendingActorMethodDiagnosticData() { PendingActorMethodCalls = pendingCalls }, startTime);

                    Mock.Get(pendingMethodCalls).Verify(x => x.Record(It.Is<long>(d => d == pendingCalls)), Times.Once);
                }

                [Fact]
                public void AcquireFinishObserveDuration()
                {
                    sut.AcquireActorLockFinish(new PendingActorMethodDiagnosticData(), startTime);

                    Mock.Get(acquireLockDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds))), Times.Once);
                }

                [Fact]
                public void ReleaseObserveHoldDuration()
                {
                    sut.ReleaseActorLock(startTime);

                    Mock.Get(releaseLockDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds))), Times.Once);
                }
            }

            public class Method : OnEvents
            {
                [Fact]
                public void FinishWithExceptionObserveExceptionFrequency()
                {
                    sut.ActorMethodFinish(new ActorMethodDiagnosticData() { Exception = exception, InterfaceMethodKey = interfaceMethodKey }, startTime);

                    Mock.Get(methodExecutionDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds)), It.Is<string>(m => m == methodName), It.Is<string>(m => m == methodSigniture), It.Is<string>(m => m == exception.GetType().Name)), Times.Once);
                }

                [Fact]
                public void FinishWithoutExceptionDoNotObserveExceptionFrequency()
                {
                    sut.ActorMethodFinish(new ActorMethodDiagnosticData() { InterfaceMethodKey = interfaceMethodKey }, startTime);

                    Mock.Get(methodExecutionDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds)), It.Is<string>(m => m == methodName), It.Is<string>(m => m == methodSigniture), It.Is<string>(m => m == "None")), Times.Once);
                }
            }

            public class Activate : OnEvents
            {
                [Fact]
                public void FinishAsyncObserveDuration()
                {
                    sut.ActorOnActivateAsyncFinish(startTime);

                    Mock.Get(onActivateAsyncDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds))), Times.Once);
                }
            }

            public class Request : OnEvents
            {
                [Fact]
                public void FinishObserveProcessingDuration()
                {
                    sut.ActorRequestProcessingFinish(startTime);

                    Mock.Get(requestProcessingDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds))), Times.Once);
                }
            }

            public class State : OnEvents
            {
                [Fact]
                public void LoadFinishObserveDuration()
                {
                    sut.LoadActorStateFinish(startTime);

                    Mock.Get(loadStateDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds))), Times.Once);
                }

                [Fact]
                public void SaveFinishObserveDuration()
                {
                    sut.SaveActorStateFinish(actorId, startTime);

                    Mock.Get(saveStateDuration).Verify(x => x.Record(It.Is<TimeSpan>(d => DurationsApproximatelyEqual(d, durationMilliseconds))), Times.Once);
                }
            }
        }

        public class Dispose : MetricDiagnosticsTest
        {
            [Fact]
            public void DisposesAllMeters()
            {
                sut.Dispose();

                Mock.Get(pendingMethodCalls).Verify(m => m.Dispose(), Times.Once);
                Mock.Get(acquireLockDuration).Verify(m => m.Dispose(), Times.Once);
                Mock.Get(releaseLockDuration).Verify(m => m.Dispose(), Times.Once);
                Mock.Get(methodExecutionDuration).Verify(m => m.Dispose(), Times.Once);
                Mock.Get(onActivateAsyncDuration).Verify(m => m.Dispose(), Times.Once);
                Mock.Get(requestProcessingDuration).Verify(m => m.Dispose(), Times.Once);
                Mock.Get(loadStateDuration).Verify(m => m.Dispose(), Times.Once);
                Mock.Get(saveStateDuration).Verify(m => m.Dispose(), Times.Once);
            }
        }

    }
}
