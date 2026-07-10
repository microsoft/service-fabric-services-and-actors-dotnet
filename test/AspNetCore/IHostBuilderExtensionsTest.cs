// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Fuzzy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    public class IHostBuilderExtensionsTest
    {
        readonly AspNetCoreCommunicationListener listener;
        readonly Mock<IHostBuilder> builder;
        Action<HostBuilderContext, IServiceCollection> capturedConfigureServices;
        int configureServicesCount;

        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        IHostBuilderExtensionsTest()
        {
            StatelessServiceContext context = fuzzy.StatelessServiceContext();
            listener = new KestrelCommunicationListener(context, (Func<string, AspNetCoreCommunicationListener, IHost>)BuildHost);

            builder = new Mock<IHostBuilder>();
            _ = builder.SetupGet(_ => _.Properties).Returns(new Dictionary<object, object>());
            _ = builder
                .Setup(_ => _.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()))
                .Callback<Action<HostBuilderContext, IServiceCollection>>(action =>
                {
                    capturedConfigureServices = action;
                    configureServicesCount++;
                })
                .Returns(builder.Object);
        }

        public sealed class UseServiceFabricIntegration_IHostBuilder : IHostBuilderExtensionsTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenHostBuilderIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ((IHostBuilder)null).UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None));
                Assert.Equal("hostBuilder", exception.ParamName);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenListenerIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => builder.Object.UseServiceFabricIntegration(null, ServiceFabricIntegrationOptions.None));
                Assert.Equal("listener", exception.ParamName);
            }

            [Fact]
            public void RegistersStartupFilterWithoutUrlSuffixWhenOptionsIsNone()
            {
                builder.Object.UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None);
                Assert.Equal(1, configureServicesCount);
                Assert.Empty(listener.UrlSuffix);
            }

            [Fact]
            public void RegistersStartupFilterWithUrlSuffixWhenOptionsIsUseUniqueServiceUrl()
            {
                builder.Object.UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl);
                Assert.Equal(1, configureServicesCount);
                Assert.NotEmpty(listener.UrlSuffix);
            }

            [Fact]
            public void IsIdempotentWhenCalledMultipleTimes()
            {
                builder.Object.UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None);
                builder.Object.UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None);

                Assert.Equal(1, configureServicesCount);
            }

            [Fact]
            public void ReturnsGivenBuilder()
            {
                IHostBuilder result = builder.Object.UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None);

                Assert.Same(builder.Object, result);
            }

            [Fact]
            public void RegistersStartupFilter()
            {
                builder.Object.UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None);

                var services = new ServiceCollection();
                capturedConfigureServices(null, services);

                Assert.Single(services, d => d.ServiceType == typeof(IStartupFilter));
            }
        }

        static IHost BuildHost(string url, AspNetCoreCommunicationListener listener) =>
            Mock.Of<IHost>();
    }
}
