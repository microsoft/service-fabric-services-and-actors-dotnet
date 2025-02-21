using System;
using System.Reflection;
using Microsoft.ServiceFabric.Actors.Remoting;
using Xunit;
using Inspector;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

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

            public class WithNullArgument : GetProvider
            {
                [Fact]
                public void ThrowsExceptionWhenEntryAssselbyIsNull()
                {
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(null);

                    Assert.Throws<InvalidOperationException>(() => { ActorRemotingProviderAttribute.GetProvider(); });
                }

                [Fact]
                public void ThrowsExcpetionWhenEntryAssemblyDoesNotHaveProviderAttribute()
                {
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(new MockAssemblyWithoutRemotingProviderAttribute());
                    Assert.Throws<InvalidOperationException>(() => { ActorRemotingProviderAttribute.GetProvider(); });
                }

                [Fact]
                public void DoesNotThrowExcpetionWhenEntryAssemblyHasProviderAttribute()
                {
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(new MockAssemblyWithRemotingProviderAttribute());

                    Exception exception = Record.Exception(() => { ActorRemotingProviderAttribute.GetProvider(); });

                    Assert.Null(exception);
                }
            }

            public class WithTypeArrayArgument : GetProvider
            {

                public class MockTypeWithAssemblyProviderAttribute : MockBaseType
                {
                    public override Assembly Assembly { get => new MockAssemblyWithRemotingProviderAttribute(); }
                }

                public class MockTypeWithoutAssemblyProviderAttribute : MockBaseType
                {
                    public override Assembly Assembly { get => new MockAssemblyWithoutRemotingProviderAttribute(); }
                }

                [Fact]
                public void ThrowsExceptionWhenTypeHasNoAssemblyProviderAttribute()
                {
                    var types = new Type[] { new MockTypeWithoutAssemblyProviderAttribute() };
                    typeof(ActorRemotingProviderAttribute).Field<Assembly>().Set(null);

                    Assert.Throws<InvalidOperationException>(() => { ActorRemotingProviderAttribute.GetProvider(types); });
                }

                [Fact]
                public void DoesNotThrowExceptionWhenTypeHasAssemblyProviderAttribute()
                {
                    var types = new Type[] { new MockTypeWithAssemblyProviderAttribute() };

                    Exception exception = Record.Exception(() => { ActorRemotingProviderAttribute.GetProvider(types); });

                    Assert.Null(exception);
                }
            }

        }

    }
}
