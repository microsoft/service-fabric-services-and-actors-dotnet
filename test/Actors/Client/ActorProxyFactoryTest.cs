// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Actors.Remoting.V2.Client;
using Microsoft.ServiceFabric.Actors.Tests;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Client;

public abstract class ActorProxyFactoryTest
{
    readonly ActorProxyFactory sut;

    // Constructor parameters
    readonly Func<IServiceRemotingCallbackMessageHandler, IServiceRemotingClientFactory> createServiceRemotingClientFactory;
    readonly OperationRetrySettings retrySettings = new();

    readonly IServiceRemotingClientFactory factory = new Mock<IServiceRemotingClientFactory> { DefaultValue = DefaultValue.Mock }.Object;

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ActorProxyFactoryTest()
    {
        createServiceRemotingClientFactory = _ => factory;
        sut = new ActorProxyFactory(createServiceRemotingClientFactory, retrySettings);
    }

    public sealed class Constructor_FuncOfIServiceRemotingCallbackMessageHandlerOfIServiceRemotingClientFactory_OperationRetrySettings : ActorProxyFactoryTest
    {
        [Fact]
        public void CreatesProxyFactoryWithGivenFuncAndRetrySettings()
        {
            // The V2 ActorProxyFactory is internal, so inspect its private state to verify the V1 constructor
            // forwarded both parameters into it. The Func behavior is further exercised indirectly by the
            // CreateActorProxy tests, which require the Func to return a usable IServiceRemotingClientFactory.
            var v2 = sut.Field<Remoting.V2.Client.ActorProxyFactory>().Value;
            Assert.NotNull(v2);
            Assert.Same(
                createServiceRemotingClientFactory,
                v2.Field<Func<IServiceRemotingCallbackMessageHandler, IServiceRemotingClientFactory>>().Value);
            Assert.Same(retrySettings, v2.Field<OperationRetrySettings>().Value);
        }
    }

    public sealed class Constructor_OperationRetrySettings : ActorProxyFactoryTest
    {
        [Fact]
        public void StoresRetrySettingsAndLeavesProxyFactoryNull()
        {
            var sut = new ActorProxyFactory(retrySettings);

            Assert.Same(retrySettings, sut.Field<OperationRetrySettings>().Value);
            Assert.Null(sut.Field<Remoting.V2.Client.ActorProxyFactory>().Value);
        }
    }

    public sealed class CreateActorProxy_ActorId_String_String_String : ActorProxyFactoryTest
    {
        readonly ActorId actorId = fuzzy.ActorId();
        readonly string applicationName = "fabric:/" + fuzzy.String().LettersOrDigits();
        readonly string serviceName = fuzzy.String().LettersOrDigits();
        readonly string listenerName = fuzzy.String();

        [Fact]
        public void ReturnsProxyWithActorIdAndServiceUriBuiltFromApplicationNameAndServiceName()
        {
            var expectedUri = new Uri($"{applicationName}/{serviceName}");

            var proxy = (IActorProxy)sut.CreateActorProxy<IFactoryTestActor>(actorId, applicationName, serviceName, listenerName);

            Assert.Same(actorId, proxy.ActorId);
            Assert.Equal(expectedUri, proxy.ActorServicePartitionClientV2.ServiceUri);
            Assert.Same(listenerName, proxy.ActorServicePartitionClientV2.ListenerName);
            Assert.Same(factory, proxy.ActorServicePartitionClientV2.Factory);
        }

        [Fact]
        public void LazilyCreatesV2ProxyFactoryFromDefaultProviderAndForwardsRetrySettings()
        {
            // Exercises the lazy-init branch in GetOrSetProxyFactory reachable only when ActorProxyFactory is
            // constructed without a Func. proxyFactoryV2 is assigned before the V2 factory invokes the default
            // provider's CreateServiceRemotingClientFactory delegate, which may or may not fail depending on
            // whether the Service Fabric runtime is available; either outcome leaves proxyFactoryV2 assigned.
            var sut = new ActorProxyFactory(retrySettings);

            try { sut.CreateActorProxy<IFactoryTestActor>(actorId, applicationName, serviceName, listenerName); }
            catch { }

            var v2 = sut.Field<Remoting.V2.Client.ActorProxyFactory>().Value;
            Assert.NotNull(v2);
            Assert.Same(retrySettings, v2.Field<OperationRetrySettings>().Value);
        }

        [Fact]
        public void SubstitutesDefaultListenerNameWhenLazyInitOverrodeItAndCallerSuppliedNull()
        {
            // Exercises the override branch in OverrideListenerNameIfConditionMet: lazy init via the no-Func
            // constructor sets overrideListenerName=true and defaultListenerName from the default provider
            // (FabricTransportActorRemotingProviderAttribute => V2 => DefaultV2listenerName). The V2 factory
            // built by lazy init is wired to the default provider whose factory delegate requires the Service
            // Fabric runtime, so it is replaced with one bound to the mock client factory before creating the
            // proxy whose ListenerName is then observed.
            var sut = new ActorProxyFactory(retrySettings);
            try { sut.CreateActorProxy<IFactoryTestActor>(actorId, applicationName, serviceName, listenerName); }
            catch { }

            sut.Field<Remoting.V2.Client.ActorProxyFactory>()
                .Set(new Remoting.V2.Client.ActorProxyFactory(createServiceRemotingClientFactory, retrySettings));

            var proxy = (IActorProxy)sut.CreateActorProxy<IFactoryTestActor>(actorId, applicationName, serviceName, listenerName: null);

            Assert.Equal(ServiceRemotingProviderAttribute.DefaultV2listenerName, proxy.ActorServicePartitionClientV2.ListenerName);
        }

        [Fact]
        public void SubstitutesWrappedMessageStackListenerNameWhenOverrideDefaultIsSetForV2_1()
        {
            // Exercises the V2_1 branch of OverrideDefaultListenerName. The default sut's proxyFactoryV2 is
            // already non-null (constructed with a Func), so CreateActorProxy skips lazy init and applies the
            // override configured here. Reaching this branch via the lazy-init path would require an actor
            // interface from an assembly carrying an ActorRemotingProviderAttribute with V2_1, which would
            // change the provider seen by the other tests in this assembly.
            OverrideDefaultListenerName(RemotingClientVersion.V2_1);
            var proxy = (IActorProxy)sut.CreateActorProxy<IFactoryTestActor>(actorId, applicationName, serviceName, listenerName: null);
            Assert.Equal(ServiceRemotingProviderAttribute.DefaultWrappedMessageStackListenerName, proxy.ActorServicePartitionClientV2.ListenerName);
        }

        [Fact]
        public void PreservesListenerNameWhenOverrideDefaultIsSetAndCallerSuppliedNonNull()
        {
            // Exercises the guard branch in OverrideListenerNameIfConditionMet: when the override is enabled
            // but the caller supplies a non-empty listenerName, the caller-supplied value must be preserved.
            OverrideDefaultListenerName(RemotingClientVersion.V2_1);

            var proxy = (IActorProxy)sut.CreateActorProxy<IFactoryTestActor>(actorId, applicationName, serviceName, listenerName);

            Assert.Same(listenerName, proxy.ActorServicePartitionClientV2.ListenerName);
        }
    }

    public sealed class CreateActorProxy_Type_Uri_ActorId_String : ActorProxyFactoryTest
    {
        readonly Type actorInterfaceType = typeof(IFactoryTestActor);
        readonly Uri serviceUri = fuzzy.Uri();
        readonly ActorId actorId = fuzzy.ActorId();
        readonly string listenerName = fuzzy.String();

        [Fact]
        public void ReturnsProxyWithGivenServiceUriActorIdAndListenerName()
        {
            var proxy = (IActorProxy)sut.CreateActorProxy(actorInterfaceType, serviceUri, actorId, listenerName);

            Assert.Same(actorId, proxy.ActorId);
            Assert.Same(serviceUri, proxy.ActorServicePartitionClientV2.ServiceUri);
            Assert.Same(listenerName, proxy.ActorServicePartitionClientV2.ListenerName);
            Assert.Same(factory, proxy.ActorServicePartitionClientV2.Factory);
        }

        [Fact]
        public void SubstitutesDefaultListenerNameWhenOverrideDefaultIsSetAndCallerSuppliedNull()
        {
            // Exercises the override-on branch in OverrideListenerNameIfConditionMet for this overload.
            OverrideDefaultListenerName(RemotingClientVersion.V2);

            var proxy = (IActorProxy)sut.CreateActorProxy(actorInterfaceType, serviceUri, actorId, listenerName: null);

            Assert.Equal(ServiceRemotingProviderAttribute.DefaultV2listenerName, proxy.ActorServicePartitionClientV2.ListenerName);
        }
    }

    public sealed class CreateActorProxy_Uri_ActorId_String : ActorProxyFactoryTest
    {
        readonly Uri serviceUri = fuzzy.Uri();
        readonly ActorId actorId = fuzzy.ActorId();
        readonly string listenerName = fuzzy.String();

        [Fact]
        public void ReturnsProxyWithGivenServiceUriActorIdAndListenerName()
        {
            var proxy = (IActorProxy)sut.CreateActorProxy<IFactoryTestActor>(serviceUri, actorId, listenerName);

            Assert.Same(actorId, proxy.ActorId);
            Assert.Same(serviceUri, proxy.ActorServicePartitionClientV2.ServiceUri);
            Assert.Same(listenerName, proxy.ActorServicePartitionClientV2.ListenerName);
            Assert.Same(factory, proxy.ActorServicePartitionClientV2.Factory);
        }

        [Fact]
        public void SubstitutesDefaultListenerNameWhenOverrideDefaultIsSetAndCallerSuppliedNull()
        {
            // Exercises the override-on branch in OverrideListenerNameIfConditionMet for this overload.
            OverrideDefaultListenerName(RemotingClientVersion.V2);

            var proxy = (IActorProxy)sut.CreateActorProxy<IFactoryTestActor>(serviceUri, actorId, listenerName: null);

            Assert.Equal(ServiceRemotingProviderAttribute.DefaultV2listenerName, proxy.ActorServicePartitionClientV2.ListenerName);
        }

        [Fact]
        public void SubstitutesDefaultListenerNameWhenOverrideDefaultIsSetAndCallerSuppliedEmpty()
        {
            // Exercises the string.IsNullOrEmpty empty-string branch in OverrideListenerNameIfConditionMet.
            // OverrideListenerNameIfConditionMet is shared across all CreateActorProxy/CreateActorServiceProxy
            // overloads, so a single representative test here covers the empty-string case for all of them.
            OverrideDefaultListenerName(RemotingClientVersion.V2);

            var proxy = (IActorProxy)sut.CreateActorProxy<IFactoryTestActor>(serviceUri, actorId, listenerName: "");

            Assert.Equal(ServiceRemotingProviderAttribute.DefaultV2listenerName, proxy.ActorServicePartitionClientV2.ListenerName);
        }
    }

    public sealed class CreateActorServiceProxy_Uri_ActorId_String : ActorProxyFactoryTest
    {
        readonly Uri serviceUri = fuzzy.Uri();
        readonly ActorId actorId = fuzzy.ActorId();
        readonly string listenerName = fuzzy.String();

        [Fact]
        public void ReturnsProxyWithPartitionKeyDerivedFromActorId()
        {
            var proxy = (IServiceProxy)sut.CreateActorServiceProxy<IActorService>(serviceUri, actorId, listenerName);

            Assert.Same(serviceUri, proxy.ServicePartitionClient2.ServiceUri);
            Assert.Same(listenerName, proxy.ServicePartitionClient2.ListenerName);
            Assert.Equal(actorId.GetPartitionKey(), proxy.ServicePartitionClient2.PartitionKey.Value);
            Assert.Same(factory, proxy.ServicePartitionClient2.Factory);
        }

        [Fact]
        public void SubstitutesDefaultListenerNameWhenOverrideDefaultIsSetAndCallerSuppliedNull()
        {
            // Exercises the override-on branch in OverrideListenerNameIfConditionMet for this overload.
            OverrideDefaultListenerName(RemotingClientVersion.V2);

            var proxy = (IServiceProxy)sut.CreateActorServiceProxy<IActorService>(serviceUri, actorId, listenerName: null);

            Assert.Equal(ServiceRemotingProviderAttribute.DefaultV2listenerName, proxy.ServicePartitionClient2.ListenerName);
        }
    }

    public sealed class CreateActorServiceProxy_Uri_Int64_String : ActorProxyFactoryTest
    {
        readonly Uri serviceUri = fuzzy.Uri();
        readonly long partitionKey = fuzzy.Int64();
        readonly string listenerName = fuzzy.String();

        [Fact]
        public void ReturnsProxyWithGivenServiceUriListenerNameAndPartitionKey()
        {
            var proxy = (IServiceProxy)sut.CreateActorServiceProxy<IActorService>(serviceUri, partitionKey, listenerName);

            Assert.Same(serviceUri, proxy.ServicePartitionClient2.ServiceUri);
            Assert.Same(listenerName, proxy.ServicePartitionClient2.ListenerName);
            Assert.Equal(partitionKey, proxy.ServicePartitionClient2.PartitionKey.Value);
            Assert.Same(factory, proxy.ServicePartitionClient2.Factory);
        }

        [Fact]
        public void SubstitutesDefaultListenerNameWhenOverrideDefaultIsSetAndCallerSuppliedNull()
        {
            // Exercises the override-on branch in OverrideListenerNameIfConditionMet for this overload.
            OverrideDefaultListenerName(RemotingClientVersion.V2);

            var proxy = (IServiceProxy)sut.CreateActorServiceProxy<IActorService>(serviceUri, partitionKey, listenerName: null);

            Assert.Equal(ServiceRemotingProviderAttribute.DefaultV2listenerName, proxy.ServicePartitionClient2.ListenerName);
        }
    }

    public sealed class Dispose : ActorProxyFactoryTest
    {
        [Fact]
        public void DoesNothingWhenProxyFactoryWasNeverCreated()
        {
            var sut = new ActorProxyFactory();
            sut.Dispose();
            Assert.Null(sut.Field<Remoting.V2.Client.ActorProxyFactory>().Value);
        }

        [Fact(Explicit = true)] // TODO: SUT testability limitation. V2 Dispose only acts on FabricTransportActorRemotingClientFactory, which requires the runtime.
        public void DelegatesToUnderlyingProxyFactoryAndLeavesItInPlace()
        {
            // V1 Dispose delegates to V2 Dispose when the V2 field is non-null, but V2 Dispose only disposes
            // the remoting client factory when it is a FabricTransportActorRemotingClientFactory. That type
            // cannot be constructed without the Service Fabric runtime and its Dispose is non-virtual, so
            // the delegated call has no observable effect on a mock IServiceRemotingClientFactory. Removing
            // the entire body of V1 Dispose leaves any assertion on the V2 field unchanged, so this branch
            // cannot be verified by a test that fails when the product code is sabotaged.
            throw new NotImplementedException();
        }
    }

    // TODO: Replace with sut.Method<Action<RemotingClientVersion>>() once Inspector handles open generic
    // method definitions. Inspector 0.3.12 iterates all instance methods and calls Delegate.CreateDelegate
    // on each, which throws ArgumentException for the generic CreateActorProxy<TActorInterface> overloads
    // declared on this type. See https://github.com/olegsych/inspector/issues/5.
    void OverrideDefaultListenerName(RemotingClientVersion version) =>
        sut.Method("OverrideDefaultListenerName").Invoke(version);
}
