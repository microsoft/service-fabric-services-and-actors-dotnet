// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
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
            this.listener = new KestrelCommunicationListener(context, (uri, listen) => Mock.Of<IHost>());
        }

        /// <summary>
        /// Verify WebHostBuilderExtension for ServiceFabricIntegrationOptions.None.
        /// </summary>
        [Fact]
        public void VerifyWithServiceFabricIntegrationOptions_None()
        {
            this.builder.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.None);
            this.servicesConfigured.Should().BeTrue("services are configured.");
            this.listener.UrlSuffix.Should().BeEmpty("listener is not Configured to use UniqueServiceUrl.");

            // Call the UseServiceFabricIntegration() again and verify that its dual invocation, doesn't have adverse affect.
            this.builder.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.None);
            this.servicesConfigured.Should().BeTrue("services are configured.");
            this.listener.UrlSuffix.Should().BeEmpty("listener is not Configured to use UniqueServiceUrl.");
        }

        /// <summary>
        /// Verify WebHostBuilderExtension for ServiceFabricIntegrationOptions.UseUniqueServiceUrl.
        /// </summary>
        [Fact]
        public void VerifyWithServiceFabricIntegrationOptions_UseUniqueServiceUrl()
        {
            // ServiceFabricIntegrationOptions.None doesn't adds middleware and doesn't configures listener to use UrlSuffix.
            this.builder.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl);
            this.servicesConfigured.Should().BeTrue("services are configured.");
            this.listener.UrlSuffix.Should().NotBeEmpty("listener is Configured to use UniqueServiceUrl.");

            // Call the UseServiceFabricIntegration() again and verify that its dual invocation, doesn't have adverse affect.
            this.builder.UseServiceFabricIntegration(this.listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl);
            this.servicesConfigured.Should().BeTrue("services are configured.");
            this.listener.UrlSuffix.Should().NotBeEmpty("listener is Configured to use UniqueServiceUrl.");
        }
    }
}
