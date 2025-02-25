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
            protected readonly Mock<Assembly> mockAssemblyWithoutRemotingProviderAttribute = new Mock<Assembly>();
            protected readonly Mock<Assembly> mockAssemblyWithRemotingProviderAttribute = new Mock<Assembly>();

            private readonly string expectedExceptionMessagesForMissingRemotingProviderAttribute =
                "To use Actor Remoting, the version of the remoting stack must be specified explicitely.";

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
                readonly Mock<Type> mockTypeWithRemotingProviderAssemblyAttribute = new Mock<Type>();
                readonly Mock<Type> mockTypeWithoutRemotingProviderAssemblyAttribute = new Mock<Type>();

                public WithTypeArrayArgument()
                {
                    this.mockTypeWithRemotingProviderAssemblyAttribute
                        .Setup(type => type.Assembly)
                        .Returns(this.mockAssemblyWithRemotingProviderAttribute.Object);
                    this.mockTypeWithoutRemotingProviderAssemblyAttribute
                        .Setup(type => type.Assembly)
                        .Returns(this.mockAssemblyWithoutRemotingProviderAttribute.Object);
                }

                [Fact]
                public void ThrowsExceptionWhenTypeHasNoAssemblyProviderAttribute()
                {
                    var types = new Type[] { this.mockTypeWithoutRemotingProviderAssemblyAttribute.Object };
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(null);

                    var exception = Assert.Throws<InvalidOperationException>(
                        () => ActorRemotingProviderAttribute.GetProvider(types));

                    Assert.Equal(this.expectedExceptionMessagesForMissingRemotingProviderAttribute, exception.Message);
                }

                [Fact]
                public void ReturnsRemotingProviderAttributeOfTypeAssembly()
                {
                    var types = new Type[] { this.mockTypeWithRemotingProviderAssemblyAttribute.Object };

                    ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider(types);

                    Assert.Same(this.expectedRemotingProvider, provider);
                }
            }

            public void Dispose()
            {
                typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(Assembly.GetEntryAssembly());
            }
        }

    }
}
