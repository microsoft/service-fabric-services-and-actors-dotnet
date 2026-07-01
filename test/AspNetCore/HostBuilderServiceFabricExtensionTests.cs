// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    /// <summary>
    /// Test class for WebHostBuilderServiceFabricExtension with IHostBuilder.
    /// </summary>
    public class HostBuilderServiceFabricExtensionTests
    {
        private readonly AspNetCoreCommunicationListener listener;
        private readonly Mock<IHostBuilder> builder;
        private Action<HostBuilderContext, IServiceCollection> capturedConfigureServices;

        /// <summary>
        /// Used by tests to check if services were configured by WebHostBuilderServiceFabricExtension.UseServiceFabricIntegration.
        /// </summary>
        private int configureServicesCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostBuilderServiceFabricExtensionTests"/> class.
        /// </summary>
        public HostBuilderServiceFabricExtensionTests()
        {
            var context = TestMocksRepository.GetMockStatelessServiceContext();
            this.listener = new KestrelCommunicationListener(context, (uri, listen) => this.BuildFunc(uri, listen));

            this.builder = new Mock<IHostBuilder>();
            this.builder.SetupGet(_ => _.Properties).Returns(new Dictionary<object, object>());
            this.builder
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
        /// Verify WebHostBuilderExtension for ServiceFabricIntegrationOptions.None.
        /// </summary>
        [Fact]
        public void VerifyWithServiceFabricIntegrationOptions_None()
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
        /// Verify WebHostBuilderExtension for ServiceFabricIntegrationOptions.UseUniqueServiceUrl.
        /// </summary>
        [Fact]
        public void VerifyWithServiceFabricIntegrationOptions_UseUniqueServiceUrl()
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

        private IHost BuildFunc(string url, AspNetCoreCommunicationListener listener)
        {
            return new Mock<IHost>().Object;
        }
    }
}
