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
                    typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(new MockAssemblyWithoutRemotingProviderAttribute());
                    
                    var exception = Assert.Throws<InvalidOperationException>(() => 
                    { 
                        ServiceRemotingProviderAttribute.GetProvider(); 
                    });

                    Assert.Equal(ServiceRemotingProviderAttribute.DefaultRemotingProviderExceptionMessage, exception.Message);
                }

                [Fact]
                public void DoesNotThrowExcpetionWhenEntryAssemblyHasProviderAttribute()
                {
                    typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(new MockAssemblyWithRemotingProviderAttribute());

                    ServiceRemotingProviderAttribute provider = ServiceRemotingProviderAttribute.GetProvider();

                    Assert.IsType<FabricTransportServiceRemotingProviderAttribute>(provider);
                }
            }

            public class WithTypeArrayArgument : GetProvider 
            {
                [Fact]
                public void ThrowsExceptionWhenTypeHasNoAssemblyProviderAttribute()
                {
                    var types = new Type[] { new MockTypeWithoutAssemblyProviderAttribute() };
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
                    var types = new Type[] { new MockTypeWithAssemblyProviderAttribute() };

                    ServiceRemotingProviderAttribute provider = ServiceRemotingProviderAttribute.GetProvider(types);;

                    Assert.IsType<FabricTransportServiceRemotingProviderAttribute>(provider);
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
                    return new Attribute[] { new FabricTransportServiceRemotingProviderAttribute() };
                }
            }

            public void Dispose()
            {
                typeof(ServiceRemotingProviderAttribute).Field<Assembly>().Set(Assembly.GetEntryAssembly());
            }
        }
    }
}

