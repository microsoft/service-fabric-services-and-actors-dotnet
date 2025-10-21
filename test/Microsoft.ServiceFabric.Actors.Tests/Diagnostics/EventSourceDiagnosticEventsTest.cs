// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
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
    public class EventSourceDiagnosticEventsTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IClock clock = Mock.Of<IClock>();
        readonly ActorTypeInformation typeInfo = ActorTypeInformation.Get(typeof(TestActor));
        readonly ActorMethodFriendlyNameBuilder nameBuilder;
        readonly ServiceContext serviceContext = fuzzy.ServiceContext();
        readonly ActorFrameworkEventSource eventSource = Mock.Of<ActorFrameworkEventSource>();

        readonly IDiagnosticEvents sut;

        public EventSourceDiagnosticEventsTest()
        {
            nameBuilder = new ActorMethodFriendlyNameBuilder(typeInfo);
            sut = new EventSourceDiagnosticEvents(eventSource, clock, serviceContext, nameBuilder, typeInfo);
        }

        public class Constructor : EventSourceDiagnosticEventsTest
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
                var exception = Assert.Throws<ArgumentNullException>(() =>
                {
                    new EventSourceDiagnosticEvents(eventSource, clock, serviceContext, nameBuilder, null);
                });
                Assert.Equal("typeInformation", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullNameBuilder()
            {
                var exception = Assert.Throws<ArgumentNullException>(() =>
                {
                    new EventSourceDiagnosticEvents(eventSource, clock, serviceContext, null, typeInfo);
                });
                Assert.Equal("nameBuilder", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullClock()
            {
                var exception = Assert.Throws<ArgumentNullException>(() =>
                {
                    new EventSourceDiagnosticEvents(eventSource, null, serviceContext, nameBuilder, typeInfo);
                });
                Assert.Equal("clock", exception.ParamName);
            }

            [Fact]
            public void ThrowsOnNullEventSource()
            {
                var exception = Assert.Throws<ArgumentNullException>(() =>
                {
                    new EventSourceDiagnosticEvents(null, clock, serviceContext, nameBuilder, typeInfo);
                });
                Assert.Equal("eventSource", exception.ParamName);
            }

            [Fact]
            public void InitializedMethodInfos()
            {
                // TODO
            }
        }

        public class OnEvents : EventSourceDiagnosticEventsTest
        {
            readonly long interfaceMethodKey = fuzzy.Int64();
            readonly ActorId actorId = fuzzy.ActorId();
            readonly DiagnosticsManagerActorContext diagnosticsManagerActorContext = Mock.Of<DiagnosticsManagerActorContext>();
            readonly DateTime startTime;
            readonly DateTime endTime;
            readonly long operationDurationMillis = fuzzy.Int64().Between(100, 2000);
            readonly RemotingListenerVersion remotingListener = RemotingListenerVersion.V2;

            public class WitnNoPerfCounters : OnEvents
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
                    sut.AcquireActorLockStart(diagnosticsManagerActorContext);
                    sut.AcquireActorLockFailed(diagnosticsManagerActorContext);
                    sut.AcquireActorLockFinishPreProcess(diagnosticsManagerActorContext, startTime, actorId);
                    sut.ReleaseActorLock(startTime);

                    Mock.Get(eventSource).VerifyNoOtherCalls();
                }
            }


        }


    }
}
