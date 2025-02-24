using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Reflection;
using Inspector;
using Microsoft.ServiceFabric.Services.Remoting.Tests;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Moq;

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
            protected Mock<Assembly> mockAssemblyWithoutRemotingProviderAttribute = new Mock<Assembly>();
            protected Mock<Assembly> mockAssemblyWithRemotingProviderAttribute = new Mock<Assembly>();

            public GetProvider()
            {
                this.mockAssemblyWithoutRemotingProviderAttribute
                    .Setup(assembly => assembly.GetCustomAttributes(It.IsAny<Type>(), It.IsAny<bool>()))
                    .Returns(new Attribute[] { });
                this.mockAssemblyWithRemotingProviderAttribute
                    .Setup(assembly => assembly.GetCustomAttributes(It.IsAny<Type>(), It.IsAny<bool>()))
                    .Returns(new Attribute[] { new FabricTransportServiceRemotingProviderAttribute() });
            }

            public class WithNullArgument : GetProvider 
            {
                [Fact]
                public void ThrowsExceptionWhenEntryAssselbyIsNull()
                {
                    typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(null);

                    var exception = Assert.Throws<InvalidOperationException>(() => 
                    { 
                        ServiceRemotingProviderAttribute.GetProvider(); 
                    });

                    Assert.Equal(ServiceRemotingProviderAttribute.DefaultRemotingProviderExceptionMessage, exception.Message);
                }

                [Fact]
                public void ThrowsExcpetionWhenEntryAssemblyDoesNotHaveProviderAttribute()
                {
                    typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(this.mockAssemblyWithoutRemotingProviderAttribute.Object);
                    
                    var exception = Assert.Throws<InvalidOperationException>(() => 
                    { 
                        ServiceRemotingProviderAttribute.GetProvider(); 
                    });

                    Assert.Equal(ServiceRemotingProviderAttribute.DefaultRemotingProviderExceptionMessage, exception.Message);
                }

                [Fact]
                public void DoesNotThrowExcpetionWhenEntryAssemblyHasProviderAttribute()
                {
                    typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(this.mockAssemblyWithRemotingProviderAttribute.Object);

                    ServiceRemotingProviderAttribute provider = ServiceRemotingProviderAttribute.GetProvider();

                    Assert.IsType<FabricTransportServiceRemotingProviderAttribute>(provider);
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
                    typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(null);

                    var exception = Assert.Throws<InvalidOperationException>(() => 
                    { 
                        ServiceRemotingProviderAttribute.GetProvider(types);
                    });

                    Assert.Equal(ServiceRemotingProviderAttribute.DefaultRemotingProviderExceptionMessage, exception.Message);
                }

                [Fact]
                public void DoesNotThrowExceptionWhenTypeHasAssemblyProviderAttribute()
                {
                    var types = new Type[] { this.mockTypeWithAssemblyProviderAttribute.Object };

                    ServiceRemotingProviderAttribute provider = ServiceRemotingProviderAttribute.GetProvider(types);;

                    Assert.IsType<FabricTransportServiceRemotingProviderAttribute>(provider);
                }
            }

            public void Dispose()
            {
                typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(Assembly.GetEntryAssembly());
            }
        }
    }
}

