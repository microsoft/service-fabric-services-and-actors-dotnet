// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Inspector;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using Moq;
using Xunit;
using ClientVersion = Microsoft.ServiceFabric.Services.Remoting.RemotingClientVersion;
using ListenerVersion = Microsoft.ServiceFabric.Services.Remoting.RemotingListenerVersion;

namespace Microsoft.ServiceFabric.Actors.Remoting;

public abstract class ActorRemotingProviderAttributeTest
{
    readonly ActorRemotingProviderAttribute sut = new TestActorRemotingProviderAttribute();

    public sealed class Constructor : ActorRemotingProviderAttributeTest
    {
        [Fact]
        public void InitializesPropertiesToV2_1()
        {
            Assert.Equal(ClientVersion.V2_1, sut.RemotingClientVersion);
            Assert.Equal(ListenerVersion.V2_1, sut.RemotingListenerVersion);
        }
    }

    public sealed class GetProvider : ActorRemotingProviderAttributeTest, IDisposable
    {
        // Method parameters
        IEnumerable<Type> types;

        readonly Assembly assemblyWithoutAttribute = MockAssembly();
        readonly Assembly assemblyWithAttribute;
        readonly ActorRemotingProviderAttribute expected = new FabricTransportActorRemotingProviderAttribute();

        public GetProvider()
        {
            assemblyWithAttribute = MockAssembly(expected);
            typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(null);
        }

        void IDisposable.Dispose() =>
            typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(Assembly.GetEntryAssembly());

        [Fact]
        public void ReturnsRemotingProviderAttributeOfTypeAssembly()
        {
            types = [MockType(assemblyWithAttribute)];

            ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider(types);

            Assert.Same(expected, provider);
        }

        [Fact]
        public void ReturnsRemotingProviderAttributeOfLaterTypeWhenEarlierTypeAssemblyHasNoAttribute()
        {
            types = [MockType(assemblyWithoutAttribute), MockType(assemblyWithAttribute)];

            ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider(types);

            Assert.Same(expected, provider);
        }

        [Fact]
        public void ReturnsRemotingProviderAttributeOfEarlierTypeWhenMultipleTypeAssembliesHaveAttributes()
        {
            var otherProvider = new FabricTransportActorRemotingProviderAttribute();
            types = [MockType(assemblyWithAttribute), MockType(MockAssembly(otherProvider))];

            ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider(types);

            Assert.Same(expected, provider);
        }

        [Fact]
        public void ReturnsRemotingProviderAttributeOfTypeAssemblyWhenEntryAssemblyAlsoHasAttribute()
        {
            types = [MockType(assemblyWithAttribute)];
            var entryProvider = new FabricTransportActorRemotingProviderAttribute();
            typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(MockAssembly(entryProvider));

            ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider(types);

            Assert.Same(expected, provider);
        }

        [Fact]
        public void ReturnsRemotingProviderAttributeOfEntryAssemblyWhenTypesIsNull()
        {
            typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(assemblyWithAttribute);

            ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider(types);

            Assert.Same(expected, provider);
        }

        [Fact]
        public void ReturnsRemotingProviderAttributeOfEntryAssemblyWhenTypesIsEmpty()
        {
            types = [];
            typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(assemblyWithAttribute);

            ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider(types);

            Assert.Same(expected, provider);
        }

        [Fact]
        public void ReturnsRemotingProviderAttributeOfEntryAssemblyWhenNoTypeAssemblyHasAttribute()
        {
            types = [MockType(assemblyWithoutAttribute)];
            typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(assemblyWithAttribute);

            ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider(types);

            Assert.Same(expected, provider);
        }

        [Fact]
        public void ReturnsDefaultFabricTransportActorRemotingProviderWhenTypeHasNoAssemblyProviderAttribute()
        {
            types = [MockType(assemblyWithoutAttribute)];

            ActorRemotingProviderAttribute result = ActorRemotingProviderAttribute.GetProvider(types);

            Assert.Same(typeof(FabricTransportActorRemotingProviderAttribute), result.GetType());
        }

        [Fact]
        public void ReturnsDefaultFabricTransportActorRemotingProviderWhenNeitherTypeAssemblyNorEntryAssemblyHasAttribute()
        {
            types = [MockType(assemblyWithoutAttribute)];
            typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(assemblyWithoutAttribute);

            ActorRemotingProviderAttribute result = ActorRemotingProviderAttribute.GetProvider(types);

            Assert.Same(typeof(FabricTransportActorRemotingProviderAttribute), result.GetType());
        }

        [Fact]
        public void ReturnsDefaultFabricTransportActorRemotingProviderWhenTypesIsNullAndEntryAssemblyIsNull()
        {
            ActorRemotingProviderAttribute result = ActorRemotingProviderAttribute.GetProvider(types);

            Assert.Same(typeof(FabricTransportActorRemotingProviderAttribute), result.GetType());
        }

        static Assembly MockAssembly(ActorRemotingProviderAttribute provider = null)
        {
            var assembly = new Mock<TestAssembly>();
            Attribute[] attributes = provider == null ? [] : [provider];
            _ = assembly.Setup(_ => _.GetCustomAttributes(typeof(ActorRemotingProviderAttribute), true)).Returns(attributes);
            return assembly.Object;
        }

        static Type MockType(Assembly assembly)
        {
            var type = new Mock<Type>();
            _ = type.Setup(_ => _.Assembly).Returns(assembly);
#if NETFRAMEWORK
            Mock<IReflectableType> reflectableType = type.As<IReflectableType>();
            _ = reflectableType.Setup(_ => _.GetTypeInfo()).Returns(MockTypeInfo(assembly));
#endif
            return type.Object;
        }

#if NETFRAMEWORK
        static TypeInfo MockTypeInfo(Assembly assembly)
        {
            var typeInfo = new Mock<TypeDelegator>();
            _ = typeInfo.Setup(_ => _.Assembly).Returns(assembly);
            return typeInfo.Object;
        }
#endif

        // Make Assembly concrete to enable mocking on NetFx
        internal class TestAssembly : Assembly { }
    }

    public sealed class RemotingClientVersion : ActorRemotingProviderAttributeTest
    {
        [Theory, InlineData(ClientVersion.V2), InlineData(ClientVersion.V2_1)]
        public void SetsValue(ClientVersion value)
        {
            sut.RemotingClientVersion = value == ClientVersion.V2 ? ClientVersion.V2_1 : ClientVersion.V2;
            sut.RemotingClientVersion = value;
            Assert.Equal(value, sut.RemotingClientVersion);
        }
    }

    public sealed class RemotingListenerVersion : ActorRemotingProviderAttributeTest
    {
        [Theory, InlineData(ListenerVersion.V2), InlineData(ListenerVersion.V2_1)]
        public void SetsValue(ListenerVersion value)
        {
            sut.RemotingListenerVersion = value == ListenerVersion.V2 ? ListenerVersion.V2_1 : ListenerVersion.V2;
            sut.RemotingListenerVersion = value;
            Assert.Equal(value, sut.RemotingListenerVersion);
        }
    }

    sealed class TestActorRemotingProviderAttribute : ActorRemotingProviderAttribute
    {
        public override Dictionary<string, Func<ActorService, IServiceRemotingListener>> CreateServiceRemotingListeners() =>
            throw new NotImplementedException();

        public override IServiceRemotingClientFactory CreateServiceRemotingClientFactory(IServiceRemotingCallbackMessageHandler callbackMessageHandler) =>
            throw new NotImplementedException();
    }
}
