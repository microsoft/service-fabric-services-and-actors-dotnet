// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Fuzzy;
using Microsoft.ServiceFabric.Actors.Remoting.V2.Client;
using Microsoft.ServiceFabric.Actors.Tests;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Client;

public abstract class ActorProxyTest
{
    readonly ActorProxy sut = new TestProxy();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class Initialize : ActorProxyTest
    {
        // Method parameters
        readonly ActorServicePartitionClient client;
        readonly IServiceRemotingMessageBodyFactory serviceRemotingMessageBodyFactory = Mock.Of<IServiceRemotingMessageBodyFactory>();

        readonly ActorId actorId = fuzzy.ActorId();

        public Initialize()
        {
            Mock<IServiceRemotingClientFactory> factory = new() { DefaultValue = DefaultValue.Mock };
            client = new ActorServicePartitionClient(factory.Object, fuzzy.Uri(), actorId);
        }

        [Fact]
        public void StoresParametersAccessibleViaProperties()
        {
            sut.Initialize(client, serviceRemotingMessageBodyFactory);

            Assert.Same(client, sut.ActorServicePartitionClientV2);
            Assert.Same(actorId, sut.ActorId);
            Assert.Same(serviceRemotingMessageBodyFactory, sut.ServiceRemotingMessageBodyFactory);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Initialize doesn't validate client.
        public void ThrowsArgumentNullExceptionWhenClientIsNull()
        {
            // ActorProxy.Initialize stores client without validation. The defect surfaces later as a
            // NullReferenceException from the ActorId getter, which dereferences servicePartitionClientV2.
            var exception = Assert.Throws<ArgumentNullException>(() => sut.Initialize(null, serviceRemotingMessageBodyFactory));
            Assert.Equal(nameof(client), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Initialize doesn't validate serviceRemotingMessageBodyFactory.
        public void ThrowsArgumentNullExceptionWhenServiceRemotingMessageBodyFactoryIsNull()
        {
            // ActorProxy.Initialize forwards a null factory to InitializeV2 without validation. The defect
            // surfaces later as a NullReferenceException from ProxyBase.CreateRequestMessageBodyV2.
            var exception = Assert.Throws<ArgumentNullException>(() => sut.Initialize(client, null));
            Assert.Equal(nameof(serviceRemotingMessageBodyFactory), exception.ParamName);
        }
    }

    sealed class TestProxy : ActorProxy
    {
        protected override object GetReturnValue(int interfaceId, int methodId, object responseBody) => null;
    }
}
