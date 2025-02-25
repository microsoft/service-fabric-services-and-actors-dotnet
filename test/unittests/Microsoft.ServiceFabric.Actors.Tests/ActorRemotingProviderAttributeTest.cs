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
            protected readonly Mock<TestAssembly> mockAssemblyWithoutRemotingProviderAttribute = new Mock<TestAssembly>();
            protected readonly Mock<TestAssembly> mockAssemblyWithRemotingProviderAttribute = new Mock<TestAssembly>();

#if NETFRAMEWORK
            private readonly string expectedExceptionMessagesForMissingRemotingProviderAttribute =
                "To use Actor Remoting, the version of the remoting stack must be specified explicitely.";
#endif

            private readonly FabricTransportActorRemotingProviderAttribute expectedRemotingProvider =
                new FabricTransportActorRemotingProviderAttribute();

            public GetProvider()
            {
                this.mockAssemblyWithoutRemotingProviderAttribute
                    .Setup(assembly => assembly.GetCustomAttributes(It.IsAny<Type>(), It.IsAny<bool>()))
                    .Returns(new Attribute[0]);
                this.mockAssemblyWithRemotingProviderAttribute
                    .Setup(assembly => assembly.GetCustomAttributes(It.IsAny<Type>(), It.IsAny<bool>()))
                    .Returns(new Attribute[] { this.expectedRemotingProvider });
            }

            public class WithNullArgument : GetProvider
            {
#if NETFRAMEWORK
                [Fact]
                public void ThrowsExceptionWhenEntryAssemblyIsUnmanagedAssembly()
                {
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(null);

                    var exception = Assert.Throws<InvalidOperationException>(
                        () => ActorRemotingProviderAttribute.GetProvider());

                    Assert.Equal(this.expectedExceptionMessagesForMissingRemotingProviderAttribute, exception.Message);
                }

                [Fact]
                public void ThrowsExcpetionWhenEntryAssemblyDoesNotHaveProviderAttribute()
                {
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(this.mockAssemblyWithoutRemotingProviderAttribute.Object);

                    var exception = Assert.Throws<InvalidOperationException>(
                        () => ActorRemotingProviderAttribute.GetProvider());

                    Assert.Equal(this.expectedExceptionMessagesForMissingRemotingProviderAttribute, exception.Message);
                }
#endif

                [Fact]
                public void ReturnsRemotingProviderAttributeOfEntryAssembly()
                {
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(this.mockAssemblyWithRemotingProviderAttribute.Object);

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
                    this.mockTypeWithRemotingProviderAssemblyAttribute = MockType(this.mockAssemblyWithRemotingProviderAttribute.Object);
                    this.mockTypeWithoutRemotingProviderAssemblyAttribute = MockType(this.mockAssemblyWithoutRemotingProviderAttribute.Object);

                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(null);
                }

#if NETFRAMEWORK
                [Fact]
                public void ThrowsExceptionWhenTypeHasNoAssemblyProviderAttribute()
                {
                    var types = new Type[] { this.mockTypeWithoutRemotingProviderAssemblyAttribute };

                    var exception = Assert.Throws<InvalidOperationException>(
                        () => ActorRemotingProviderAttribute.GetProvider(types));

                    Assert.Equal(this.expectedExceptionMessagesForMissingRemotingProviderAttribute, exception.Message);
                }
#else
                [Fact]
                public void ReturnsDefaultFabricTransportActorRemotingProviderWhenTypeHasNoAssemblyProviderAttribute()
                {
                    var result = ActorRemotingProviderAttribute.GetProvider(new[] { this.mockTypeWithoutRemotingProviderAssemblyAttribute });

                    var expected = new FabricTransportActorRemotingProviderAttribute();
                    var actual = Assert.IsType<FabricTransportActorRemotingProviderAttribute>(result);
                    Assert.Equal(expected.RemotingClientVersion, actual.RemotingClientVersion);
                    Assert.Equal(expected.RemotingListenerVersion, actual.RemotingListenerVersion);
                }
#endif

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

            public class TestAssembly : Assembly { }
        }
    }
}
