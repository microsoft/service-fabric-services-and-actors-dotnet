using System;
using System.Reflection;
using Inspector;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests
{
    public class ServiceRemotingProviderAttributeTest
    {

        public class StaticConstructor : ServiceRemotingProviderAttributeTest
        {
            [Fact]
            public void InitializesEntryAssembly()
            {
                Assert.Same(Assembly.GetEntryAssembly(), typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Value);
            }
        }

        public class GetProvider : ServiceRemotingProviderAttributeTest, IDisposable
        {
            protected readonly Assembly mockAssemblyWithoutRemotingProviderAttribute = MockAssembly();
            protected readonly Assembly mockAssemblyWithRemotingProviderAttribute;

#if NETFRAMEWORK
            private readonly string expectedExceptionMessagesForMissingRemotingProviderAttribute =
                "To use Service Remoting, the version of the remoting stack must be specified explicitely.";
#endif

            private readonly ServiceRemotingProviderAttribute expectedRemotingProvider =
                new FabricTransportServiceRemotingProviderAttribute();

            public GetProvider()
            {
                this.mockAssemblyWithRemotingProviderAttribute = MockAssembly(this.expectedRemotingProvider);
            }

            public class WithNullArgument : GetProvider 
            {
#if NETFRAMEWORK
                [Fact]
                public void ThrowsExceptionWhenEntryAssemblyIsUnmanagedAssembly()
                {
                    typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(null);

                    var exception = Assert.Throws<InvalidOperationException>(
                        () => ServiceRemotingProviderAttribute.GetProvider());

                    Assert.Equal(this.expectedExceptionMessagesForMissingRemotingProviderAttribute, exception.Message);
                }

                [Fact]
                public void ThrowsExcpetionWhenEntryAssemblyDoesNotHaveProviderAttribute()
                {
                    typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(this.mockAssemblyWithoutRemotingProviderAttribute);

                    var exception = Assert.Throws<InvalidOperationException>(
                        () => ServiceRemotingProviderAttribute.GetProvider());

                    Assert.Equal(this.expectedExceptionMessagesForMissingRemotingProviderAttribute, exception.Message);
                }
#endif

                [Fact]
                public void ReturnsRemotingProviderAttributeOfEntryAssembly()
                {
                    typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(this.mockAssemblyWithRemotingProviderAttribute);

                    ServiceRemotingProviderAttribute provider = ServiceRemotingProviderAttribute.GetProvider();

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
                }

#if NETFRAMEWORK
                [Fact]
                public void ThrowsExceptionWhenTypeHasNoAssemblyProviderAttribute()
                {
                    var types = new Type[] { this.mockTypeWithoutRemotingProviderAssemblyAttribute };
                    typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(null);

                    var exception = Assert.Throws<InvalidOperationException>(
                        () => ServiceRemotingProviderAttribute.GetProvider(types));

                    Assert.Equal(this.expectedExceptionMessagesForMissingRemotingProviderAttribute, exception.Message);
                }
#else
                [Fact]
                public void ReturnsDefaultFabricTransportServiceRemotingProviderWhenTypeHasNoAssemblyProviderAttribute()
                {
                    var result = ServiceRemotingProviderAttribute.GetProvider(new[] { this.mockTypeWithoutRemotingProviderAssemblyAttribute });

                    var expected = new FabricTransportServiceRemotingProviderAttribute();
                    var actual = Assert.IsType<FabricTransportServiceRemotingProviderAttribute>(result);
                    Assert.Equal(expected.RemotingClientVersion, actual.RemotingClientVersion);
                    Assert.Equal(expected.RemotingListenerVersion, actual.RemotingListenerVersion);
                }
#endif

                [Fact]
                public void ReturnsRemotingProviderAttributeOfTypeAssembly()
                {
                    var types = new Type[] { this.mockTypeWithRemotingProviderAssemblyAttribute };

                    var provider = ServiceRemotingProviderAttribute.GetProvider(types);;

                    Assert.Same(this.expectedRemotingProvider, provider);
                }
            }

            public void Dispose()
            {
                typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(Assembly.GetEntryAssembly());
            }

            static Assembly MockAssembly(ServiceRemotingProviderAttribute provider = null)
            {
                var assembly = new Mock<TestAssembly>();
                Attribute[] attributes = provider == null ? new Attribute[0] : new[] { provider };
                assembly.Setup(_ => _.GetCustomAttributes(typeof(ServiceRemotingProviderAttribute), It.IsAny<bool>())).Returns(attributes);
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

