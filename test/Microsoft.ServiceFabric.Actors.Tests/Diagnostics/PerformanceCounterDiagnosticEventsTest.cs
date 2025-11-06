// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Common;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Actors.Diagnostics.PerformanceCounters;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Tests;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Services.Remoting;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    public class PerformanceCounterDiagnosticEventsTest
    {
        readonly static IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IDiagnostics sut;

        readonly IClock clock = Mock.Of<IClock>();

        readonly static ActorTypeInformation actorTypeInfo = ActorTypeInformation.Get(typeof(TestActor));
        readonly PerformanceCounterProviderV2 performanceCounterProvider = new PerformanceCounterProviderV2(Guid.NewGuid(), actorTypeInfo);

        protected PerformanceCounterDiagnosticEventsTest() => sut = new PerformanceCounterDiagnosticEvents(performanceCounterProvider, clock);

        public class Constructor : PerformanceCounterDiagnosticEventsTest
        {
            [Fact]
            public void WithParametersSetsValue()
            {
                var providerField = sut.Field<PerformanceCounterProviderV2>().Value;
                Assert.Equal(performanceCounterProvider, providerField);

                var clockField = sut.Field<IClock>().Value;
                Assert.Equal(clock, clockField);
            }

            [Fact]
            public void ThrowsOnNullProvider()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new PerformanceCounterDiagnosticEvents(null, clock));
                Assert.Equal("performanceCounterProvider", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullClock()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new PerformanceCounterDiagnosticEvents(performanceCounterProvider, null));
                Assert.Equal("clock", exception.ParamName);
            }
        }

        public class OnEvents : PerformanceCounterDiagnosticEventsTest
        {
            readonly FabricAverageCount64PerformanceCounterWriter actorRequestProcessingTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            readonly FabricAverageCount64PerformanceCounterWriter actorLockAcquireWaitTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            readonly FabricAverageCount64PerformanceCounterWriter actorLockHoldTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            readonly FabricAverageCount64PerformanceCounterWriter actorRequestDeserializationTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            readonly FabricAverageCount64PerformanceCounterWriter actorResponseSerializationTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            readonly FabricAverageCount64PerformanceCounterWriter actorOnActivateAsyncTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            readonly FabricAverageCount64PerformanceCounterWriter actorLoadStateTimeCounterWriter = Mock.Of<FabricAverageCount64PerformanceCounterWriter>();
            readonly FabricNumberOfItems64PerformanceCounterWriter actorOutstandingRequestsCounterWriter = Mock.Of<FabricNumberOfItems64PerformanceCounterWriter>();
            readonly ActorLockContentionCounterWriter actorLockContentionCounterWriter = Mock.Of<ActorLockContentionCounterWriter>();
            readonly ActorSaveStateTimeCounterWriter actorSaveStateTimeCounterWriter = Mock.Of<ActorSaveStateTimeCounterWriter>();
            readonly Dictionary<long, PerformanceCounterProvider.CounterInstanceData> actorMethodCounterInstanceData;

            readonly long interfaceMethodKey = fuzzy.Int64();
            readonly ActorId actorId = fuzzy.ActorId();
            readonly DateTime startTime = DateTime.Now;
            readonly DateTime endTime;
            readonly long operationDurationMillis = fuzzy.Int64().Between(100, 2000);

            public OnEvents()
            {
                actorMethodCounterInstanceData = new Dictionary<long, PerformanceCounterProvider.CounterInstanceData>();
                var counterInstanceData = new PerformanceCounterProvider.CounterInstanceData { InstanceName = fuzzy.String() };
                counterInstanceData.CounterWriters = new PerformanceCounterProvider.MethodSpecificCounterWriters();
                counterInstanceData.CounterWriters.ActorMethodFrequencyCounterWriter = Mock.Of<ActorMethodFrequencyCounterWriter>();
                counterInstanceData.CounterWriters.ActorMethodExceptionFrequencyCounterWriter = Mock.Of<ActorMethodExceptionFrequencyCounterWriter>();
                counterInstanceData.CounterWriters.ActorMethodExecTimeCounterWriter = Mock.Of<ActorMethodExecTimeCounterWriter>();
                actorMethodCounterInstanceData[interfaceMethodKey] = counterInstanceData;

                performanceCounterProvider.Private().Field<Dictionary<long, PerformanceCounterProvider.CounterInstanceData>>().Set(actorMethodCounterInstanceData);
                performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorRequestProcessingTimeCounterWriter)).Set(actorRequestProcessingTimeCounterWriter);
                performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorLockAcquireWaitTimeCounterWriter)).Set(actorLockAcquireWaitTimeCounterWriter);
                performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorLockHoldTimeCounterWriter)).Set(actorLockHoldTimeCounterWriter);
                performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorRequestDeserializationTimeCounterWriter)).Set(actorRequestDeserializationTimeCounterWriter);
                performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorResponseSerializationTimeCounterWriter)).Set(actorResponseSerializationTimeCounterWriter);
                performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorOnActivateAsyncTimeCounterWriter)).Set(actorOnActivateAsyncTimeCounterWriter);
                performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorLoadStateTimeCounterWriter)).Set(actorLoadStateTimeCounterWriter);
                performanceCounterProvider.Field<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorOutstandingRequestsCounterWriter)).Set(actorOutstandingRequestsCounterWriter);
                performanceCounterProvider.Field<ActorLockContentionCounterWriter>(nameof(performanceCounterProvider.actorLockContentionCounterWriter)).Set(actorLockContentionCounterWriter);
                performanceCounterProvider.Field<ActorSaveStateTimeCounterWriter>(nameof(performanceCounterProvider.actorSaveStateTimeCounterWriter)).Set(actorSaveStateTimeCounterWriter);

                endTime = startTime + TimeSpan.FromMilliseconds(operationDurationMillis);
                Mock.Get(clock).Setup(clock => clock.UtcNow).Returns(endTime);
            }

            public class WithNoPerfCounters : OnEvents
            {
                [Fact]
                public void EmitsNothingWhenCountersNotNeeded()
                {
                    sut.ActorOnActivateAsyncStart();
                    sut.ActorMethodStart(actorId, interfaceMethodKey);
                    sut.LoadActorStateStart();
                    sut.SaveActorStateStart(actorId);
                    sut.ActorChangeRole(ReplicaRole.Primary, ReplicaRole.IdleSecondary);
                    sut.ActorActivated(actorId);
                    sut.ActorDeactivated(actorId);

                    Mock.Get(actorRequestProcessingTimeCounterWriter).VerifyNoOtherCalls();
                    Mock.Get(actorLockAcquireWaitTimeCounterWriter).VerifyNoOtherCalls();
                    Mock.Get(actorLockHoldTimeCounterWriter).VerifyNoOtherCalls();
                    Mock.Get(actorRequestDeserializationTimeCounterWriter).VerifyNoOtherCalls();
                    Mock.Get(actorResponseSerializationTimeCounterWriter).VerifyNoOtherCalls();
                    Mock.Get(actorOnActivateAsyncTimeCounterWriter).VerifyNoOtherCalls();
                    Mock.Get(actorLoadStateTimeCounterWriter).VerifyNoOtherCalls();
                    Mock.Get(actorOutstandingRequestsCounterWriter).VerifyNoOtherCalls();
                    Mock.Get(actorLockContentionCounterWriter).VerifyNoOtherCalls();
                    Mock.Get(actorSaveStateTimeCounterWriter).VerifyNoOtherCalls();

                    foreach (var counterInstanceData in actorMethodCounterInstanceData.Values)
                    {
                        Mock.Get(counterInstanceData.CounterWriters.ActorMethodFrequencyCounterWriter).VerifyNoOtherCalls();
                        Mock.Get(counterInstanceData.CounterWriters.ActorMethodExceptionFrequencyCounterWriter).VerifyNoOtherCalls();
                        Mock.Get(counterInstanceData.CounterWriters.ActorMethodExecTimeCounterWriter).VerifyNoOtherCalls();
                    }
                }
            }

            public class ActorState : OnEvents
            {
                [Fact]
                public void SaveEmitsPerfCounter()
                {
                    ActorStateDiagnosticData expectedDiagnoticData = new ActorStateDiagnosticData() { ActorId = actorId, OperationTime = TimeSpan.FromMilliseconds(operationDurationMillis) };

                    sut.SaveActorStateFinish(actorId, startTime);

                    Mock.Get(actorSaveStateTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.Is<ActorStateDiagnosticData>(data => data.Equals(expectedDiagnoticData))), Times.Once);
                }

                [Fact]
                public void SaveEmitsNothingWhenCounterNull()
                {
                    performanceCounterProvider.Field<ActorSaveStateTimeCounterWriter>(nameof(performanceCounterProvider.actorSaveStateTimeCounterWriter)).Set(null);

                    sut.SaveActorStateFinish(actorId, startTime);

                    Mock.Get(actorSaveStateTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<ActorStateDiagnosticData>()), Times.Never);

                    performanceCounterProvider.Field<ActorSaveStateTimeCounterWriter>(nameof(performanceCounterProvider.actorSaveStateTimeCounterWriter)).Set(actorSaveStateTimeCounterWriter);
                }

                [Fact]
                public void LoadEmitsPerfCounter()
                {
                    sut.LoadActorStateFinish(startTime);

                    Mock.Get(actorLoadStateTimeCounterWriter).Verify(p => p.UpdateCounterValue(operationDurationMillis), Times.Once);
                }

                [Fact]
                public void LoadEmitsNothingWhenCounterNull()
                {
                    performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorLoadStateTimeCounterWriter)).Set(null);

                    sut.LoadActorStateFinish(startTime);

                    Mock.Get(actorLoadStateTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);

                    performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorLoadStateTimeCounterWriter)).Set(actorLoadStateTimeCounterWriter);
                }
            }

            public class RequestProcessing : OnEvents
            {
                [Fact]
                public void StartEmitsPerfCounter()
                {
                    sut.ActorRequestProcessingStart();

                    Mock.Get(actorOutstandingRequestsCounterWriter).Verify(p => p.UpdateCounterValue(1), Times.Once);
                }

                [Fact]
                public void StartEmitsNothingWhenCounterNull()
                {
                    performanceCounterProvider.Field<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorOutstandingRequestsCounterWriter)).Set(null);

                    sut.ActorRequestProcessingStart();

                    Mock.Get(actorOutstandingRequestsCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);

                    performanceCounterProvider.Field<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorOutstandingRequestsCounterWriter)).Set(actorOutstandingRequestsCounterWriter);
                }

                [Fact]
                public void EndEmitsPerfCounter()
                {
                    sut.ActorRequestProcessingFinish(startTime);

                    Mock.Get(actorOutstandingRequestsCounterWriter).Verify(p => p.UpdateCounterValue(-1), Times.Once);
                    Mock.Get(actorRequestProcessingTimeCounterWriter).Verify(p => p.UpdateCounterValue(operationDurationMillis), Times.Once);
                }

                [Fact]
                public void EndEmitsNothingWhenCounterNull()
                {
                    performanceCounterProvider.Field<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorOutstandingRequestsCounterWriter)).Set(null);
                    performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorRequestProcessingTimeCounterWriter)).Set(null);

                    sut.ActorRequestProcessingFinish(startTime);

                    Mock.Get(actorOutstandingRequestsCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                    Mock.Get(actorRequestProcessingTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);

                    performanceCounterProvider.Field<FabricNumberOfItems64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorOutstandingRequestsCounterWriter)).Set(actorOutstandingRequestsCounterWriter);
                    performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorRequestProcessingTimeCounterWriter)).Set(actorRequestProcessingTimeCounterWriter);
                }
            }

            public class ActorLock : OnEvents
            {
                readonly long pendingMethodCalls = fuzzy.Int64();
                readonly long pendingMethodCallsDelta = fuzzy.Int64();
                readonly PendingActorMethodDiagnosticData pendingMethodData;

                public ActorLock() => pendingMethodData = new PendingActorMethodDiagnosticData() { ActorId = actorId, PendingActorMethodCalls = pendingMethodCalls, PendingActorMethodCallsDelta = pendingMethodCallsDelta };

                [Fact]
                public void ReleasedEmitsPerfCounter()
                {
                    sut.ReleaseActorLock(startTime);

                    Mock.Get(actorLockHoldTimeCounterWriter).Verify(p => p.UpdateCounterValue(operationDurationMillis), Times.Once);
                }

                [Fact]
                public void ReleasedEmitsNothingWhenCounterNull()
                {
                    performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorLockHoldTimeCounterWriter)).Set(null);

                    sut.ReleaseActorLock(startTime);

                    Mock.Get(actorLockHoldTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);

                    performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorLockHoldTimeCounterWriter)).Set(actorLockHoldTimeCounterWriter);
                }

                [Fact]
                public void AcquiredEmitsPerfCounter()
                {
                    sut.AcquireActorLockFinish(pendingMethodData, startTime);

                    Mock.Get(actorLockAcquireWaitTimeCounterWriter).Verify(p => p.UpdateCounterValue(operationDurationMillis), Times.Once);
                    Mock.Get(actorLockContentionCounterWriter).Verify(p => p.UpdateCounterValue(It.Is<PendingActorMethodDiagnosticData>(p => p.Equals(pendingMethodData))), Times.Once);
                }

                [Fact]
                public void AcquiredEmitsNothingWhenCountersNull()
                {
                    performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorLockAcquireWaitTimeCounterWriter)).Set(null);
                    performanceCounterProvider.Field<ActorLockContentionCounterWriter>(nameof(performanceCounterProvider.actorLockContentionCounterWriter)).Set(null);

                    sut.AcquireActorLockFinish(pendingMethodData, startTime);

                    Mock.Get(actorLockAcquireWaitTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                    Mock.Get(actorLockContentionCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<PendingActorMethodDiagnosticData>()), Times.Never);

                    performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorLockAcquireWaitTimeCounterWriter)).Set(actorLockAcquireWaitTimeCounterWriter);
                    performanceCounterProvider.Field<ActorLockContentionCounterWriter>(nameof(performanceCounterProvider.actorLockContentionCounterWriter)).Set(actorLockContentionCounterWriter);
                }
            }

            public class ActorActivatedAsync : OnEvents
            {
                [Fact]
                public void EmitsPerfCounter()
                {
                    sut.ActorOnActivateAsyncFinish(startTime);

                    Mock.Get(actorOnActivateAsyncTimeCounterWriter).Verify(p => p.UpdateCounterValue(operationDurationMillis), Times.Once);
                }

                [Fact]
                public void ReleasedEmitsNothingWhenCounterNull()
                {
                    performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorOnActivateAsyncTimeCounterWriter)).Set(null);

                    sut.ActorOnActivateAsyncFinish(startTime);

                    Mock.Get(actorOnActivateAsyncTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);

                    performanceCounterProvider.Field<FabricAverageCount64PerformanceCounterWriter>(nameof(performanceCounterProvider.actorOnActivateAsyncTimeCounterWriter)).Set(actorOnActivateAsyncTimeCounterWriter);
                }
            }

            public class ActorMethod : OnEvents
            {
                readonly PerformanceCounterProvider.MethodSpecificCounterWriters methodCounters;
                readonly ActorMethodFrequencyCounterWriter actorMethodFrequencyCounterWriter;
                readonly ActorMethodExceptionFrequencyCounterWriter actorMethodExceptionFrequencyCounterWriter;
                readonly ActorMethodExecTimeCounterWriter actorMethodExecTimeCounterWriter;
                readonly ActorMethodDiagnosticData diagnoticData;

                public ActorMethod()
                {
                    // store references to counter writer Mocks for easier access
                    methodCounters = actorMethodCounterInstanceData[interfaceMethodKey].CounterWriters;
                    actorMethodFrequencyCounterWriter = methodCounters.ActorMethodFrequencyCounterWriter;
                    actorMethodExceptionFrequencyCounterWriter = methodCounters.ActorMethodExceptionFrequencyCounterWriter;
                    actorMethodExecTimeCounterWriter = methodCounters.ActorMethodExecTimeCounterWriter;

                    diagnoticData = new ActorMethodDiagnosticData() { ActorId = actorId, InterfaceMethodKey = interfaceMethodKey, Exception = new Exception(), RemotingListener = RemotingListenerVersion.V2 };
                    diagnoticData.MethodExecutionTime = TimeSpan.FromMilliseconds(operationDurationMillis);
                }

                [Fact]
                public void FinishEmitsPerfCounter()
                {
                    sut.ActorMethodFinish(diagnoticData, startTime);

                    Mock.Get(actorMethodFrequencyCounterWriter).Verify(p => p.UpdateCounterValue(), Times.Once);
                    Mock.Get(actorMethodExceptionFrequencyCounterWriter).Verify(p => p.UpdateCounterValue(It.Is<ActorMethodDiagnosticData>(p => p.Equals(diagnoticData))), Times.Once);
                    Mock.Get(actorMethodExecTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.Is<ActorMethodDiagnosticData>(p => p.Equals(diagnoticData))), Times.Once);
                }

                [Fact]
                public void FinishEmitsNothingWhenCounterNull()
                {
                    methodCounters.Property<ActorMethodFrequencyCounterWriter>(nameof(methodCounters.ActorMethodFrequencyCounterWriter)).Set(null);
                    methodCounters.Property<ActorMethodExceptionFrequencyCounterWriter>(nameof(methodCounters.ActorMethodExceptionFrequencyCounterWriter)).Set(null);
                    methodCounters.Property<ActorMethodExecTimeCounterWriter>(nameof(methodCounters.ActorMethodExecTimeCounterWriter)).Set(null);

                    sut.ActorMethodFinish(diagnoticData, startTime);

                    Mock.Get(actorMethodFrequencyCounterWriter).Verify(p => p.UpdateCounterValue(), Times.Never);
                    Mock.Get(actorMethodExceptionFrequencyCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<ActorMethodDiagnosticData>()), Times.Never);
                    Mock.Get(actorMethodExecTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<ActorMethodDiagnosticData>()), Times.Never);

                    methodCounters.Property<ActorMethodFrequencyCounterWriter>(nameof(methodCounters.ActorMethodFrequencyCounterWriter)).Set(actorMethodFrequencyCounterWriter);
                    methodCounters.Property<ActorMethodExceptionFrequencyCounterWriter>(nameof(methodCounters.ActorMethodExceptionFrequencyCounterWriter)).Set(actorMethodExceptionFrequencyCounterWriter);
                    methodCounters.Property<ActorMethodExecTimeCounterWriter>(nameof(methodCounters.ActorMethodExecTimeCounterWriter)).Set(actorMethodExecTimeCounterWriter);
                }
            }
        }
    }
}
