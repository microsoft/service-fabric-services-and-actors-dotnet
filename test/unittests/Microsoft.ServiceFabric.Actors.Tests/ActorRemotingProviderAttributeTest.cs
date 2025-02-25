using System;
using System.Reflection;
using Microsoft.ServiceFabric.Actors.Remoting;
using Xunit;
using Inspector;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Moq;

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
            protected Mock<Assembly> mockAssemblyWithoutRemotingProviderAttribute = new Mock<Assembly>();
            protected Mock<Assembly> mockAssemblyWithRemotingProviderAttribute = new Mock<Assembly>();

            private readonly string expectedExceptionMessagesForMissingRemotingProviderAttribute =
                "To use Actor Remoting, the version of the remoting stack must be specified explicitely.";

            public GetProvider()
            {
                this.mockAssemblyWithoutRemotingProviderAttribute
                    .Setup(assembly => assembly.GetCustomAttributes(It.IsAny<Type>(), It.IsAny<bool>()))
                    .Returns(new Attribute[] { });
                this.mockAssemblyWithRemotingProviderAttribute
                    .Setup(assembly => assembly.GetCustomAttributes(It.IsAny<Type>(), It.IsAny<bool>()))
                    .Returns(new Attribute[] { new FabricTransportActorRemotingProviderAttribute() });
            }

            public class WithNullArgument : GetProvider
            {
                [Fact]
                public void ThrowsExceptionWhenEntryAssselbyIsNull()
                {
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(null);

                    var exception = Assert.Throws<InvalidOperationException>(() =>
                    {
                        ActorRemotingProviderAttribute.GetProvider();
                    });

                    Assert.Equal(this.expectedExceptionMessagesForMissingRemotingProviderAttribute, exception.Message);
                }

                [Fact]
                public void ThrowsExcpetionWhenEntryAssemblyDoesNotHaveProviderAttribute()
                {
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(this.mockAssemblyWithoutRemotingProviderAttribute.Object);

                    var exception = Assert.Throws<InvalidOperationException>(() =>
                    {
                        ActorRemotingProviderAttribute.GetProvider();
                    });

                    Assert.Equal(this.expectedExceptionMessagesForMissingRemotingProviderAttribute, exception.Message);
                }

                [Fact]
                public void DoesNotThrowExcpetionWhenEntryAssemblyHasProviderAttribute()
                {
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(this.mockAssemblyWithRemotingProviderAttribute.Object);

                    ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider();

                    Assert.IsType<FabricTransportActorRemotingProviderAttribute>(provider);
                }
            }

            public class WithTypeArrayArgument : GetProvider
            {
                Mock<Type> mockTypeWithAssemblyProviderAttribute = new Mock<Type>();
                Mock<Type> mockTypeWithoutAssemblyProviderAttribute = new Mock<Type>();

                public WithTypeArrayArgument()
                {
                    this.mockTypeWithAssemblyProviderAttribute
                        .Setup(type => type.Assembly)
                        .Returns(this.mockAssemblyWithRemotingProviderAttribute.Object);
                    this.mockTypeWithoutAssemblyProviderAttribute
                        .Setup(type => type.Assembly)
                        .Returns(this.mockAssemblyWithoutRemotingProviderAttribute.Object);
                }

                [Fact]
                public void ThrowsExceptionWhenTypeHasNoAssemblyProviderAttribute()
                {
                    var types = new Type[] { this.mockTypeWithoutAssemblyProviderAttribute.Object };
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(null);

                    var exception = Assert.Throws<InvalidOperationException>(() =>
                    {
                        ActorRemotingProviderAttribute.GetProvider();
                    });

                    Assert.Equal(this.expectedExceptionMessagesForMissingRemotingProviderAttribute, exception.Message);
                }

                [Fact]
                public void DoesNotThrowExceptionWhenTypeHasAssemblyProviderAttribute()
                {
                    var types = new Type[] { this.mockTypeWithAssemblyProviderAttribute.Object };

                    ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider(types);

                    Assert.IsType<FabricTransportActorRemotingProviderAttribute>(provider);
                }
            }

            public void Dispose()
            {
                typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(Assembly.GetEntryAssembly());
            }
        }

    }
}
