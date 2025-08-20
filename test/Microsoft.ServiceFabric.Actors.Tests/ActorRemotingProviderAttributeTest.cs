using System;
using System.Reflection;
using Inspector;
using Microsoft.ServiceFabric.Actors.Remoting;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Tests
{
    public class ActorRemotingProviderAttributeTest
    {
        public class StaticConstructor : ActorRemotingProviderAttributeTest
        {
            [Fact]
            public void InitializesEntryAssembly()
            {
                Assert.Same(Assembly.GetEntryAssembly(), typeof(ActorRemotingProviderAttribute).Field<Assembly>().Value);
            }
        }

        public class GetProvider : ActorRemotingProviderAttributeTest, IDisposable
        {
            protected readonly Assembly mockAssemblyWithoutRemotingProviderAttribute = MockAssembly();
            protected readonly Assembly mockAssemblyWithRemotingProviderAttribute;
            private readonly ActorRemotingProviderAttribute expectedRemotingProvider =
                new FabricTransportActorRemotingProviderAttribute();

            public GetProvider()
            {
                this.mockAssemblyWithRemotingProviderAttribute = MockAssembly(this.expectedRemotingProvider);
            }

            public class WithNullArgument : GetProvider
            {
                [Fact]
                public void ReturnsRemotingProviderAttributeOfEntryAssembly()
                {
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(this.mockAssemblyWithRemotingProviderAttribute);

                    ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider();

                    Assert.Same(this.expectedRemotingProvider, provider);
                }
            }

            public class WithTypeArrayArgument : GetProvider
            {
                readonly Type mockTypeWithRemotingProviderAssemblyAttribute; 
                readonly Type mockTypeWithoutRemotingProviderAssemblyAttribute;

                public WithTypeArrayArgument()
                {
                    this.mockTypeWithRemotingProviderAssemblyAttribute = MockType(this.mockAssemblyWithRemotingProviderAttribute);
                    this.mockTypeWithoutRemotingProviderAssemblyAttribute = MockType(this.mockAssemblyWithoutRemotingProviderAttribute);

                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(null);
                }

                [Fact]
                public void ReturnsDefaultFabricTransportActorRemotingProviderWhenTypeHasNoAssemblyProviderAttribute()
                {
                    var result = ActorRemotingProviderAttribute.GetProvider(new[] { this.mockTypeWithoutRemotingProviderAssemblyAttribute });

                    var expected = new FabricTransportActorRemotingProviderAttribute();
                    var actual = Assert.IsType<FabricTransportActorRemotingProviderAttribute>(result);
                    Assert.Equal(expected.RemotingClientVersion, actual.RemotingClientVersion);
                    Assert.Equal(expected.RemotingListenerVersion, actual.RemotingListenerVersion);
                }

                [Fact]
                public void ReturnsRemotingProviderAttributeOfTypeAssembly()
                {
                    var types = new Type[] { this.mockTypeWithRemotingProviderAssemblyAttribute };

                    ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider(types);

                    Assert.Same(this.expectedRemotingProvider, provider);
                }
            }

            public void Dispose()
            {
                typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(Assembly.GetEntryAssembly());
            }

            static Assembly MockAssembly(ActorRemotingProviderAttribute provider = null)
            {
                var assembly = new Mock<TestAssembly>();
                Attribute[] attributes = provider == null ? new Attribute[0] : new[] { provider };
                assembly.Setup(_ => _.GetCustomAttributes(typeof(ActorRemotingProviderAttribute), It.IsAny<bool>())).Returns(attributes);
                return assembly.Object;
            }

            static Type MockType(Assembly assembly)
            {
                var type = new Mock<Type>();
                type.Setup(_ => _.Assembly).Returns(assembly);
#if NETFRAMEWORK
                var reflectableType = type.As<IReflectableType>();
                reflectableType.Setup(_ => _.GetTypeInfo()).Returns(MockTypeInfo(assembly));
#endif
                return type.Object;
            }

#if NETFRAMEWORK
            static TypeInfo MockTypeInfo(Assembly assembly)
            {
                var typeInfo = new Mock<TypeDelegator>();
                typeInfo.Setup(_ => _.Assembly).Returns(assembly);
                return typeInfo.Object;
            }
#endif

            // Make Assembly concrete to enable mocking on NetFx
            public class TestAssembly : Assembly { }
        }
    }
}
