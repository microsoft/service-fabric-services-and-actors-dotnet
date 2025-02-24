using System;
using System.Reflection;
using Microsoft.ServiceFabric.Actors.Remoting;
using Xunit;
using Inspector;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;

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

                    Assert.Equal(ActorRemotingProviderAttribute.DefaultRemotingProviderExceptionMessage, exception.Message);
                }

                [Fact]
                public void ThrowsExcpetionWhenEntryAssemblyDoesNotHaveProviderAttribute()
                {
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(new MockAssemblyWithoutRemotingProviderAttribute());

                    var exception = Assert.Throws<InvalidOperationException>(() =>
                    {
                        ActorRemotingProviderAttribute.GetProvider();
                    });

                    Assert.Equal(ActorRemotingProviderAttribute.DefaultRemotingProviderExceptionMessage, exception.Message);
                }

                [Fact]
                public void DoesNotThrowExcpetionWhenEntryAssemblyHasProviderAttribute()
                {
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(new MockAssemblyWithRemotingProviderAttribute());

                    ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider();

                    Assert.IsType<FabricTransportActorRemotingProviderAttribute>(provider);
                }
            }

            public class WithTypeArrayArgument : GetProvider
            {
                [Fact]
                public void ThrowsExceptionWhenTypeHasNoAssemblyProviderAttribute()
                {
                    var types = new Type[] { new MockTypeWithoutAssemblyProviderAttribute() };
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(null);

                    var exception = Assert.Throws<InvalidOperationException>(() =>
                    {
                        ActorRemotingProviderAttribute.GetProvider();
                    });

                    Assert.Equal(ActorRemotingProviderAttribute.DefaultRemotingProviderExceptionMessage, exception.Message);
                }

                [Fact]
                public void DoesNotThrowExceptionWhenTypeHasAssemblyProviderAttribute()
                {
                    var types = new Type[] { new MockTypeWithAssemblyProviderAttribute() };

                    ActorRemotingProviderAttribute provider = ActorRemotingProviderAttribute.GetProvider(types);

                    Assert.IsType<FabricTransportActorRemotingProviderAttribute>(provider);
                }

                public class MockTypeWithAssemblyProviderAttribute : MockBaseType
                {
                    public override Assembly Assembly { get => new MockAssemblyWithRemotingProviderAttribute(); }
                }

                public class MockTypeWithoutAssemblyProviderAttribute : MockBaseType
                {
                    public override Assembly Assembly { get => new MockAssemblyWithoutRemotingProviderAttribute(); }
                }
            }

            public class MockAssemblyWithoutRemotingProviderAttribute : Assembly
            {
                public override object[] GetCustomAttributes(Type attributeType, bool inherit)
                {
                    return new Attribute[] { };
                }
            }

            public class MockAssemblyWithRemotingProviderAttribute : Assembly
            {
                public override object[] GetCustomAttributes(Type attributeType, bool inherit)
                {
                    return new Attribute[] { new FabricTransportActorRemotingProviderAttribute() };
                }
            }

            public void Dispose()
            {
                typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(Assembly.GetEntryAssembly());
            }
        }

    }
}
