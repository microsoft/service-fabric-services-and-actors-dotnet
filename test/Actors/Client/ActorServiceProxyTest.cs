// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Client;

public abstract class ActorServiceProxyTest : IDisposable
{
    readonly ActorServiceProxy sut = new();

    readonly Remoting.V2.Client.ActorProxyFactory previousV2;
    readonly IServiceRemotingClientFactory factory = new Mock<IServiceRemotingClientFactory> { DefaultValue = DefaultValue.Mock }.Object;

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ActorServiceProxyTest()
    {
        // ActorServiceProxy delegates to ActorProxy.DefaultProxyFactory, whose default-constructed
        // proxyFactoryV2 is null and lazy-initialized from the default ActorRemotingProvider which
        // requires the Service Fabric runtime. Substitute a V2 factory wired to a mock client factory
        // and restore it after each test so the shared static state stays clean.
        var field = ActorProxy.DefaultProxyFactory.Field<Remoting.V2.Client.ActorProxyFactory>();
        previousV2 = field.Value;
        field.Set(new Remoting.V2.Client.ActorProxyFactory(_ => factory));
    }

    void IDisposable.Dispose() =>
        ActorProxy.DefaultProxyFactory.Field<Remoting.V2.Client.ActorProxyFactory>().Set(previousV2);

    public sealed class CreateOfIActorService_Uri_ActorId_String : ActorServiceProxyTest
    {
        // Method parameters
        readonly Uri serviceUri = fuzzy.Uri();
        readonly ActorId actorId;
        readonly string listenerName = fuzzy.String();

        readonly long partitionKey = fuzzy.Int64();

        public CreateOfIActorService_Uri_ActorId_String() =>
            actorId = new ActorId(partitionKey);

        [Fact]
        public void ReturnsServiceProxyWithGivenServiceUriListenerNameAndPartitionKeyDerivedFromActorId()
        {
            var proxy = (IServiceProxy)ActorServiceProxy.Create<IActorService>(serviceUri, actorId, listenerName);

            Assert.Same(serviceUri, proxy.ServicePartitionClient2.ServiceUri);
            Assert.Same(listenerName, proxy.ServicePartitionClient2.ListenerName);
            Assert.Equal(partitionKey, proxy.ServicePartitionClient2.PartitionKey.Value);
            Assert.Same(factory, proxy.ServicePartitionClient2.Factory);
        }
    }

    public sealed class CreateOfIActorService_Uri_Int64_String : ActorServiceProxyTest
    {
        readonly Uri serviceUri = fuzzy.Uri();
        readonly long partitionKey = fuzzy.Int64();
        readonly string listenerName = fuzzy.String();

        [Fact]
        public void ReturnsServiceProxyWithGivenServiceUriPartitionKeyAndListenerName()
        {
            var proxy = (IServiceProxy)ActorServiceProxy.Create<IActorService>(serviceUri, partitionKey, listenerName);

            Assert.Same(serviceUri, proxy.ServicePartitionClient2.ServiceUri);
            Assert.Same(listenerName, proxy.ServicePartitionClient2.ListenerName);
            Assert.Equal(partitionKey, proxy.ServicePartitionClient2.PartitionKey.Value);
            Assert.Same(factory, proxy.ServicePartitionClient2.Factory);
        }
    }

    public sealed class Create_Uri_ActorId_String : ActorServiceProxyTest
    {
        // Method parameters
        readonly Uri serviceUri = fuzzy.Uri();
        readonly ActorId actorId;
        readonly string listenerName = fuzzy.String();

        readonly long partitionKey = fuzzy.Int64();

        public Create_Uri_ActorId_String() =>
            actorId = new ActorId(partitionKey);

        [Fact]
        public void ReturnsActorServiceWithGivenServiceUriListenerNameAndPartitionKeyDerivedFromActorId()
        {
            var proxy = (IServiceProxy)ActorServiceProxy.Create(serviceUri, actorId, listenerName);

            Assert.Same(serviceUri, proxy.ServicePartitionClient2.ServiceUri);
            Assert.Same(listenerName, proxy.ServicePartitionClient2.ListenerName);
            Assert.Equal(partitionKey, proxy.ServicePartitionClient2.PartitionKey.Value);
            Assert.Same(factory, proxy.ServicePartitionClient2.Factory);
        }
    }

    public sealed class Create_Uri_Int64_String : ActorServiceProxyTest
    {
        readonly Uri serviceUri = fuzzy.Uri();
        readonly long partitionKey = fuzzy.Int64();
        readonly string listenerName = fuzzy.String();

        [Fact]
        public void ReturnsActorServiceWithGivenServiceUriPartitionKeyAndListenerName()
        {
            var proxy = (IServiceProxy)ActorServiceProxy.Create(serviceUri, partitionKey, listenerName);

            Assert.Same(serviceUri, proxy.ServicePartitionClient2.ServiceUri);
            Assert.Same(listenerName, proxy.ServicePartitionClient2.ListenerName);
            Assert.Equal(partitionKey, proxy.ServicePartitionClient2.PartitionKey.Value);
            Assert.Same(factory, proxy.ServicePartitionClient2.Factory);
        }
    }
}
