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
using Microsoft.ServiceFabric.Services.Remoting;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Tests.Diagnostics
{
    public class EventSourceDiagnosticsTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IDiagnostics sut;

        readonly IClock clock = Mock.Of<IClock>();
        readonly ActorFrameworkEventSource eventSource;

        readonly ActorTypeInformation typeInfo = ActorTypeInformation.Get(typeof(TestActor));
        readonly ActorMethodFriendlyNameBuilder nameBuilder;
        readonly ServiceContext serviceContext = fuzzy.ServiceContext();

        public EventSourceDiagnosticsTest()
        {
            eventSource = Mock.Of<ActorFrameworkEventSource>();
            nameBuilder = new ActorMethodFriendlyNameBuilder(typeInfo);

            sut = new EventSourceDiagnostics(eventSource, clock, serviceContext, nameBuilder, typeInfo);

            Mock.Get(eventSource).Setup(eventSource => eventSource.IsActorSaveStateStartEventEnabled()).Returns(true);
            Mock.Get(eventSource).Setup(eventSource => eventSource.IsActorSaveStateStopEventEnabled()).Returns(true);
            Mock.Get(eventSource).Setup(eventSource => eventSource.IsPendingMethodCallsEventEnabled()).Returns(true);
            Mock.Get(eventSource).Setup(eventSource => eventSource.IsActorMethodStartEventEnabled()).Returns(true);
            Mock.Get(eventSource).Setup(eventSource => eventSource.IsActorMethodStopEventEnabled()).Returns(true);
        }

        public class Constructor : EventSourceDiagnosticsTest
        {
            [Fact]
            public void WithParametersSetsValue()
            {
                var serviceContextField = sut.Field<ServiceContext>().Value;
                Assert.Equal(serviceContext, serviceContextField);

                var clockField = sut.Field<IClock>().Value;
                Assert.Equal(clock, clockField);

                var eventSourceField = sut.Field<ActorFrameworkEventSource>().Value;
                Assert.Equal(eventSource, eventSourceField);

                var actorType = sut.Field<string>().Value;
                Assert.Equal(typeInfo.ImplementationType.ToString(), actorType);
            }

            [Fact]
            public void ThrowsOnNullTypeInfo()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new EventSourceDiagnostics(eventSource, clock, serviceContext, nameBuilder, null));
                Assert.Equal("typeInfo", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullNameBuilder()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new EventSourceDiagnostics(eventSource, clock, serviceContext, null, typeInfo));
                Assert.Equal("nameBuilder", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullClock()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new EventSourceDiagnostics(eventSource, null, serviceContext, nameBuilder, typeInfo));
                Assert.Equal("clock", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullEventSource()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new EventSourceDiagnostics(null, clock, serviceContext, nameBuilder, typeInfo));
                Assert.Equal("eventSource", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullServiceContext()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new EventSourceDiagnostics(eventSource, clock, null, nameBuilder, typeInfo));
                Assert.Equal("serviceContext", exception.ParamName);
            }
        }

        public class OnEvents : EventSourceDiagnosticsTest
        {
            readonly long interfaceMethodKey = fuzzy.Int64();
            readonly ActorId actorId = fuzzy.ActorId();
            readonly DateTime startTime = DateTime.Now;
            readonly DateTime endTime;
            readonly long operationDurationMillis = fuzzy.Int64().Between(100, 2000);
            readonly string actorType;
            readonly long ticks;

            public OnEvents()
            {
                actorType = typeInfo.ImplementationType.ToString();

                ticks = TimeSpan.FromMilliseconds(operationDurationMillis).Ticks;
                endTime = startTime + TimeSpan.FromMilliseconds(operationDurationMillis);
                Mock.Get(clock).Setup(clock => clock.UtcNow).Returns(endTime);
            }

            public class WithNoTraces : OnEvents
            {
                [Fact]
                public void TracesNothingWhenNotNeeded()
                {
                    sut.ActorRequestProcessingStart();
                    sut.ActorRequestProcessingFinish(startTime);
                    sut.ActorOnActivateAsyncStart();
                    sut.ActorOnActivateAsyncFinish(startTime);
                    sut.LoadActorStateStart();
                    sut.LoadActorStateFinish(startTime);
                    sut.ReleaseActorLock(startTime);

                    Mock.Get(eventSource).VerifyNoOtherCalls();
                }
            }

            public class ChangeRole : OnEvents
            {
                [Fact]
                public void WhenPrimaryTracesFromPrimaryChange()
                {
                    sut.ActorChangeRole(ReplicaRole.Primary, ReplicaRole.IdleSecondary);

                    Mock.Get(eventSource).Verify(p => p.ReplicaChangeRoleFromPrimary(serviceContext), Times.Once);
                }

                [Fact]
                public void WhenNotPrimaryTracesToPrimaryChange()
                {
                    sut.ActorChangeRole(ReplicaRole.IdleSecondary, ReplicaRole.Primary);

                    Mock.Get(eventSource).Verify(p => p.ReplicaChangeRoleToPrimary(serviceContext), Times.Once);
                }
            }

            public class ActorActivation : OnEvents
            {
                [Fact]
                public void TracesActivation()
                {
                    sut.ActorActivated(actorId);

                    Mock.Get(eventSource).Verify(p => p.ActorActivated(actorType, actorId, serviceContext), Times.Once);
                }

                [Fact]
                public void TracesDeactivation()
                {
                    sut.ActorDeactivated(actorId);

                    Mock.Get(eventSource).Verify(p => p.ActorDeactivated(actorType, actorId, serviceContext), Times.Once);
                }
            }

            public class SaveState : OnEvents
            {
                [Fact]
                public void StartTraceIfEnabled()
                {
                    sut.SaveActorStateStart(actorId);

                    Mock.Get(eventSource).Verify(p => p.ActorSaveStateStart(actorType, actorId, serviceContext), Times.Once);
                }

                [Fact]
                public void StartNoTraceIfDisabled()
                {
                    Mock.Get(eventSource).Setup(eventSource => eventSource.IsActorSaveStateStartEventEnabled()).Returns(false);

                    sut.SaveActorStateStart(actorId);

                    Mock.Get(eventSource).Verify(p => p.IsActorSaveStateStartEventEnabled(), Times.Once);
                    Mock.Get(eventSource).VerifyNoOtherCalls();
                }

                [Fact]
                public void FinishTraceIfEnabled()
                {
                    sut.SaveActorStateFinish(actorId, startTime);

                    Mock.Get(eventSource).Verify(p => p.ActorSaveStateStop(ticks, actorType, actorId, serviceContext), Times.Once);
                }

                [Fact]
                public void FinishNoTraceIfDisabled()
                {
                    Mock.Get(eventSource).Setup(eventSource => eventSource.IsActorSaveStateStopEventEnabled()).Returns(false);

                    sut.SaveActorStateFinish(actorId, startTime);

                    Mock.Get(eventSource).Verify(p => p.IsActorSaveStateStopEventEnabled(), Times.Once);
                    Mock.Get(eventSource).VerifyNoOtherCalls();
                }
            }

            public class AcquireLock : OnEvents
            {
                readonly PendingActorMethodDiagnosticData pendingActorMethodDiagnosticData;

                public AcquireLock() => pendingActorMethodDiagnosticData = new PendingActorMethodDiagnosticData() { PendingActorMethodCalls = fuzzy.Int64(), ActorId = actorId };

                [Fact]
                public void TracesIfEnabled()
                {
                    sut.AcquireActorLockFinish(pendingActorMethodDiagnosticData, startTime);

                    Mock.Get(eventSource).Verify(p => p.ActorMethodCallsWaitingForLock(pendingActorMethodDiagnosticData.PendingActorMethodCalls, actorType, actorId, serviceContext), Times.Once);
                }

                [Fact]
                public void DoesNotTraceIfDisabled()
                {
                    Mock.Get(eventSource).Setup(eventSource => eventSource.IsPendingMethodCallsEventEnabled()).Returns(false);

                    sut.AcquireActorLockFinish(pendingActorMethodDiagnosticData, startTime);

                    Mock.Get(eventSource).Verify(p => p.IsPendingMethodCallsEventEnabled(), Times.Once);
                    Mock.Get(eventSource).VerifyNoOtherCalls();
                }
            }

            public class Method : OnEvents
            {
                readonly Dictionary<long, ActorMethodInfo> actorMethodInfo = new Dictionary<long, ActorMethodInfo>();
                ActorMethodDiagnosticData diagnosticData;

                public Method()
                {
                    actorMethodInfo[interfaceMethodKey] = new ActorMethodInfo(fuzzy.String(), fuzzy.String());
                    sut.Field<IReadOnlyDictionary<long, ActorMethodInfo>>().Set(actorMethodInfo);

                    diagnosticData = new ActorMethodDiagnosticData() { ActorId = actorId, InterfaceMethodKey = interfaceMethodKey, Exception = null, RemotingListener = RemotingListenerVersion.V2 };
                }

                [Fact]
                public void StartTracesIfEnabled()
                {
                    sut.ActorMethodStart(actorId, interfaceMethodKey);

                    Mock.Get(eventSource).Verify(p => p.ActorMethodStart(actorMethodInfo[interfaceMethodKey].methodName, actorMethodInfo[interfaceMethodKey].methodSignature, actorType, actorId, serviceContext), Times.Once);
                }

                [Fact]
                public void StartDoesNotTraceIfDisabled()
                {
                    Mock.Get(eventSource).Setup(eventSource => eventSource.IsActorMethodStartEventEnabled()).Returns(false);

                    sut.ActorMethodStart(actorId, interfaceMethodKey);

                    Mock.Get(eventSource).Verify(p => p.IsActorMethodStartEventEnabled(), Times.Once);
                    Mock.Get(eventSource).VerifyNoOtherCalls();
                }

                [Fact]
                public void FinishTracesIfEnabledAndNoException()
                {
                    sut.ActorMethodFinish(diagnosticData, startTime);

                    Mock.Get(eventSource).Verify(p => p.ActorMethodStop(ticks, actorMethodInfo[interfaceMethodKey].methodName, actorMethodInfo[interfaceMethodKey].methodSignature, actorType, actorId, serviceContext), Times.Once);
                }

                [Fact]
                public void FinishTracesIfDisabledAndException()
                {
                    diagnosticData.Exception = Mock.Of<Exception>();

                    sut.ActorMethodFinish(diagnosticData, startTime);

                    Mock.Get(eventSource).Verify(p => p.ActorMethodThrewException(diagnosticData.Exception.ToString(), ticks, actorMethodInfo[interfaceMethodKey].methodName, actorMethodInfo[interfaceMethodKey].methodSignature, actorType, actorId, serviceContext), Times.Once);
                }

                [Fact]
                public void FinishDoesNotTraceIfDisabledAndNoException()
                {
                    Mock.Get(eventSource).Setup(eventSource => eventSource.IsActorMethodStopEventEnabled()).Returns(false);

                    sut.ActorMethodFinish(diagnosticData, startTime);

                    Mock.Get(eventSource).Verify(p => p.IsActorMethodStopEventEnabled(), Times.Once);
                    Mock.Get(eventSource).VerifyNoOtherCalls();
                }
            }
        }
    }
}
