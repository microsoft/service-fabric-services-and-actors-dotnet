// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Common;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Tests;
using Microsoft.ServiceFabric.Diagnostics;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    public class PerformanceCounterDiagnosticEventsTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly static ActorTypeInformation actorTypeInfo = ActorTypeInformation.Get(typeof(TestActor));
        readonly PerformanceCounterProviderV2 performanceCounterProvider = new PerformanceCounterProviderV2(Guid.NewGuid(), actorTypeInfo);
        readonly IClock clock = Mock.Of<IClock>();

        readonly IDiagnosticEvents sut;

        protected PerformanceCounterDiagnosticEventsTest()
        {
            sut = new PerformanceCounterDiagnosticEvents(performanceCounterProvider, clock);
        }

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
                var exception = Assert.Throws<ArgumentNullException>(() =>
                {
                    new PerformanceCounterDiagnosticEvents(null, clock);
                });
                Assert.Equal("performanceCounterProvider", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullClock()
            {
                var exception = Assert.Throws<ArgumentNullException>(() =>
                {
                    new PerformanceCounterDiagnosticEvents(performanceCounterProvider, null);
                });
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
            readonly DiagnosticsManagerActorContext diagnosticsManagerActorContext = Mock.Of<DiagnosticsManagerActorContext>();

            public OnEvents()
            {
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

                actorMethodCounterInstanceData = new Dictionary<long, PerformanceCounterProvider.CounterInstanceData>();

                var counterInstanceData = new PerformanceCounterProvider.CounterInstanceData { InstanceName = fuzzy.String() };
                counterInstanceData.CounterWriters = new PerformanceCounterProvider.MethodSpecificCounterWriters();
                counterInstanceData.CounterWriters.ActorMethodFrequencyCounterWriter = Mock.Of<ActorMethodFrequencyCounterWriter>();
                counterInstanceData.CounterWriters.ActorMethodExceptionFrequencyCounterWriter = Mock.Of<ActorMethodExceptionFrequencyCounterWriter>();
                counterInstanceData.CounterWriters.ActorMethodExecTimeCounterWriter = Mock.Of<ActorMethodExecTimeCounterWriter>();

                actorMethodCounterInstanceData[interfaceMethodKey] = counterInstanceData;
                performanceCounterProvider.Private().Field<Dictionary<long, PerformanceCounterProvider.CounterInstanceData>>().Set(actorMethodCounterInstanceData);
            }

            [Fact]
            public void WhenCountersNotNeededDoNotEmitAnything()
            {
                sut.ActorOnActivateAsyncStart();
                sut.ActorOnActivateAsyncFinish(DateTime.Now);
                sut.ActorMethodStart(diagnosticsManagerActorContext, actorId, interfaceMethodKey, Services.Remoting.RemotingListenerVersion.V2);
                sut.LoadActorStateStart();
                sut.SaveActorStateStart(fuzzy.ActorId());
                sut.AcquireActorLockStart(diagnosticsManagerActorContext);
                sut.AcquireActorLockFailed(diagnosticsManagerActorContext);
                sut.ActorChangeRole(ReplicaRole.Primary, ReplicaRole.IdleSecondary);
                sut.ActorDeactivated(actorId);

                Mock.Get(actorRequestProcessingTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(actorLockAcquireWaitTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(actorLockHoldTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(actorRequestDeserializationTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(actorResponseSerializationTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(actorOnActivateAsyncTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(actorLoadStateTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(actorOutstandingRequestsCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<long>()), Times.Never);
                Mock.Get(actorLockContentionCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<PendingActorMethodDiagnosticData>()), Times.Never);
                Mock.Get(actorSaveStateTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<ActorStateDiagnosticData>()), Times.Never);
                foreach (var counterInstanceData in actorMethodCounterInstanceData.Values)
                {
                    Mock.Get(counterInstanceData.CounterWriters.ActorMethodFrequencyCounterWriter).Verify(p => p.UpdateCounterValue(), Times.Never);
                    Mock.Get(counterInstanceData.CounterWriters.ActorMethodExceptionFrequencyCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<ActorMethodDiagnosticData>()), Times.Never);
                    Mock.Get(counterInstanceData.CounterWriters.ActorMethodExecTimeCounterWriter).Verify(p => p.UpdateCounterValue(It.IsAny<ActorMethodDiagnosticData>()), Times.Never);

                }
            }
        }

        private class TestActor : Actor, ITestActor
        {
            public TestActor(ActorService actorService, ActorId actorId) : base(actorService, actorId) { }

            public Task TestMethod()
            {
                return Task.CompletedTask;
            }
        }

        private interface ITestActor : IActor
        {
            public Task TestMethod();

        }
    }
}
