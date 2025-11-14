// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks;
using Inspector;
using Microsoft.ServiceFabric.Actors.Diagnostics;
using Microsoft.ServiceFabric.Actors.Remoting.V2;
using Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Tests;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors
{
    public class ActorServiceRemotingDispatcherTest
    {
        readonly internal ActorServiceRemotingDispatcher sut;

        readonly internal IDiagnostics diagnosticEvents = Mock.Of<IDiagnostics>();
        readonly internal IClock clock = Mock.Of<IClock>();

        readonly DateTime startTime = DateTime.Now;

        public ActorServiceRemotingDispatcherTest()
        {
            ActorService actorService = TestMocksRepository.GetActorService<TestActor>();
            actorService.InitializeInternal(new ActorMethodFriendlyNameBuilder(actorService.ActorTypeInformation));
            Mock.Get(clock).Setup(clock => clock.UtcNow).Returns(startTime);

            sut = new ActorServiceRemotingDispatcher(actorService, Mock.Of<IServiceRemotingMessageBodyFactory>());
            sut.Field<IClock>().Set(clock);
            sut.Field<IDiagnostics>().Set(diagnosticEvents);
        }

        public class Diagnostics : ActorServiceRemotingDispatcherTest
        {
            readonly Func<IActorRemotingMessageHeaders, IServiceRemotingRequestMessageBody, CancellationToken, Task<IServiceRemotingResponseMessageBody>> handleActorMethodDispatchAsync;

            public Diagnostics()
            {
                handleActorMethodDispatchAsync = sut.Method<Func<IActorRemotingMessageHeaders, IServiceRemotingRequestMessageBody, CancellationToken, Task<IServiceRemotingResponseMessageBody>>>("HandleActorMethodDispatchAsync");
            }

            [Fact]
            public void EmitDiagnosticsOnHandleActorMethodDispatchAsync1()
            {
                handleActorMethodDispatchAsync.Invoke(Mock.Of<IActorRemotingMessageHeaders>(), Mock.Of<IServiceRemotingRequestMessageBody>(), TestContext.Current.CancellationToken);

                Mock.Get(diagnosticEvents).Verify(d => d.ActorRequestProcessingStart(), Times.Once);
                Mock.Get(diagnosticEvents).Verify(d => d.ActorRequestProcessingFinish(startTime), Times.Once);
            }
        }
    }
}
