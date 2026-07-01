// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    /// <summary>
    /// Test class for WebHostBuilderServiceFabricExtension.
    /// </summary>
    public class WebHostBuilderServiceFabricExtensionTests
    {
        private readonly Dictionary<string, string> settings;
        private readonly AspNetCoreCommunicationListener listener;
        private readonly IWebHostBuilder builder;

        /// <summary>
        /// Used by test to check if services were configured by WebHostBuilderServiceFabricExtension.UseServiceFabricIntegration.
        /// </summary>
        private bool servicesConfigured;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHostBuilderServiceFabricExtensionTests"/> class.
        /// </summary>
        public WebHostBuilderServiceFabricExtensionTests()
        {
            this.settings = new Dictionary<string, string>();

            // create mock IWebHostBuilder to test functionality of Service Fabric WebHostBuilder extension
            var mockBuilder = new Mock<IWebHostBuilder>();

            // setup call backs for Getting and setting settings.
            mockBuilder.Setup(y => y.GetSetting(It.IsAny<string>())).Returns<string>(name =>
            {
                this.settings.TryGetValue(name, out var value);
                return value;
            });

            mockBuilder.Setup(y => y.UseSetting(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((name, value) =>
            {
                this.settings.Add(name, value);
            });

            mockBuilder.Setup(y => y.ConfigureServices(It.IsAny<Action<IServiceCollection>>())).Callback(() => this.servicesConfigured = true);

            this.builder = mockBuilder.Object;
            this.servicesConfigured = false;

            var context = TestMocksRepository.GetMockStatelessServiceContext();
            this.listener = new KestrelCommunicationListener(context, (uri, listen) => this.BuildFunc(uri, listen));
        }

        /// <summary>
        /// Verify WebHostBuilderExtension for ServiceFabricIntegrationOptions.None.
        /// </summary>
        [Fact]
        public void VerifyWithServiceFabricIntegrationOptions_None()
        {
            this.builder.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.None);
            Assert.True(this.servicesConfigured, "services are configured.");
            Assert.Empty(this.listener.UrlSuffix); // listener is not Configured to use UniqueServiceUrl.

            // Call the UseServiceFabricIntegration() again and verify that its dual invocation, doesn't have adverse affect.
            this.builder.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.None);
            Assert.True(this.servicesConfigured, "services are configured.");
            Assert.Empty(this.listener.UrlSuffix); // listener is not Configured to use UniqueServiceUrl.
        }

        /// <summary>
        /// Verify WebHostBuilderExtension for ServiceFabricIntegrationOptions.UseUniqueServiceUrl.
        /// </summary>
        [Fact]
        public void VerifyWithServiceFabricIntegrationOptions_UseUniqueServiceUrl()
        {
            // ServiceFabricIntegrationOptions.None doesn't adds middleware and doesn't configures listener to use UrlSuffix.
            this.builder.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl);
            Assert.True(this.servicesConfigured, "services are configured.");
            Assert.NotEmpty(this.listener.UrlSuffix); // listener is Configured to use UniqueServiceUrl.

            // Call the UseServiceFabricIntegration() again and verify that its dual invocation, doesn't have adverse affect.
            this.builder.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl);
            Assert.True(this.servicesConfigured, "services are configured.");
            Assert.NotEmpty(this.listener.UrlSuffix); // listener is Configured to use UniqueServiceUrl.
        }

        private IWebHost BuildFunc(string url, AspNetCoreCommunicationListener listener)
        {
            var mockServerAddressFeature = new Mock<IServerAddressesFeature>();
            mockServerAddressFeature.Setup(y => y.Addresses).Returns(new string[] { url });
            var featureCollection = new FeatureCollection();
            featureCollection.Set(mockServerAddressFeature.Object);

            // Create mock IWebHost and set required things used by this test.
            var mockWebHost = new Mock<IWebHost>();
            return mockWebHost.Object;
        }

        /// <summary>
        /// Tests for UseServiceFabricIntegration overload with Microsoft.Extensions.Hosting.IHostBuilder.
        /// </summary>
        public sealed class UseServiceFabricIntegration_IHostBuilder
        {
            private readonly AspNetCoreCommunicationListener listener;
            private readonly Mock<IHostBuilder> builder;
            private Action<HostBuilderContext, IServiceCollection> capturedConfigureServices;

            /// <summary>
            /// Used by tests to check if services were configured by WebHostBuilderServiceFabricExtension.UseServiceFabricIntegration.
            /// </summary>
            private int configureServicesCount;

            /// <summary>
            /// Initializes a new instance of the <see cref="UseServiceFabricIntegration_IHostBuilder"/> class.
            /// </summary>
            public UseServiceFabricIntegration_IHostBuilder()
            {
                ServiceContext context = TestMocksRepository.GetMockStatelessServiceContext();
                this.listener = new KestrelCommunicationListener(context, (Func<string, AspNetCoreCommunicationListener, IHost>)this.BuildHost);

                this.builder = new Mock<IHostBuilder>();
                _ = this.builder.SetupGet(_ => _.Properties).Returns(new Dictionary<object, object>());
                _ = this.builder
                    .Setup(_ => _.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()))
                    .Callback<Action<HostBuilderContext, IServiceCollection>>(action =>
                    {
                        this.capturedConfigureServices = action;
                        this.configureServicesCount++;
                    })
                    .Returns(this.builder.Object);
            }

            /// <summary>
            /// ArgumentNullException is thrown when hostBuilder is null.
            /// </summary>
            [Fact]
            public void ThrowsArgumentNullExceptionWhenHostBuilderIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ((IHostBuilder)null).UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.None));
                Assert.Equal("hostBuilder", exception.ParamName);
            }

            /// <summary>
            /// ArgumentNullException is thrown when listener is null.
            /// </summary>
            [Fact]
            public void ThrowsArgumentNullExceptionWhenListenerIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => this.builder.Object.UseServiceFabricIntegration(null, ServiceFabricIntegrationOptions.None));
                Assert.Equal("listener", exception.ParamName);
            }

            /// <summary>
            /// Registers startup filter without url suffix when options are None.
            /// </summary>
            [Fact]
            public void RegistersStartupFilterWithoutUrlSuffixWhenOptionsIsNone()
            {
                this.builder.Object.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.None);
                Assert.Equal(1, this.configureServicesCount);
                Assert.Empty(this.listener.UrlSuffix);

                // Call UseServiceFabricIntegration() again and verify idempotency.
                this.builder.Object.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.None);
                Assert.Equal(1, this.configureServicesCount);
                Assert.Empty(this.listener.UrlSuffix);
            }

            /// <summary>
            /// Registers startup filter with url suffix when options are UseUniqueServiceUrl.
            /// </summary>
            [Fact]
            public void RegistersStartupFilterWithUrlSuffixWhenOptionsIsUseUniqueServiceUrl()
            {
                this.builder.Object.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl);
                Assert.Equal(1, this.configureServicesCount);
                Assert.NotEmpty(this.listener.UrlSuffix);

                // Call UseServiceFabricIntegration() again and verify idempotency.
                this.builder.Object.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl);
                Assert.Equal(1, this.configureServicesCount);
                Assert.NotEmpty(this.listener.UrlSuffix);
            }

            /// <summary>
            /// Verify that the ConfigureServices delegate registers an IStartupFilter.
            /// </summary>
            [Fact]
            public void RegistersStartupFilter()
            {
                this.builder.Object.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.None);

                var services = new ServiceCollection();
                this.capturedConfigureServices(null, services);

                Assert.Single(services, d => d.ServiceType == typeof(IStartupFilter));
            }

            private IHost BuildHost(string url, AspNetCoreCommunicationListener listener)
            {
                return Mock.Of<IHost>();
            }
        }
    }
}
