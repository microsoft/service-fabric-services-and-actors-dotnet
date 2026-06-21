// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric;
using System.Linq;
using Fuzzy;
using Inspector;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.AspNetCore;

public abstract class WebHostBuilderServiceFabricExtensionTest
{
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class UseServiceFabricIntegration : WebHostBuilderServiceFabricExtensionTest
    {
        // Method parameters
        readonly Mock<IWebHostBuilder> hostBuilder = new();
        readonly AspNetCoreCommunicationListener listener = new TestCommunicationListener(fuzzy.StatelessServiceContext());
        readonly ServiceFabricIntegrationOptions options = fuzzy.Enum<ServiceFabricIntegrationOptions>();

        // Wire-format configuration key the SUT contracts on; intentionally a literal, not nameof(...), because the
        // key is independent of the method name and must remain stable across renames for back-compat.
        const string settingName = "UseServiceFabricIntegration";
        static readonly string settingValue = true.ToString();

        [Fact]
        public void ReturnsHostBuilder()
        {
            IWebHostBuilder actual = hostBuilder.Object.UseServiceFabricIntegration(listener, options);

            Assert.Same(hostBuilder.Object, actual);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenHostBuilderIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => WebHostBuilderServiceFabricExtension.UseServiceFabricIntegration(null, listener, options));
            Assert.Equal(nameof(hostBuilder), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. UseServiceFabricIntegration does not validate listener and throws NullReferenceException.
        public void ThrowsArgumentNullExceptionWhenListenerIsNull()
        {
            // UseServiceFabricIntegration dereferences listener without a null check, so calling it with a null listener
            // throws NullReferenceException instead of the expected ArgumentNullException.
            var exception = Assert.Throws<ArgumentNullException>(() => hostBuilder.Object.UseServiceFabricIntegration(null, ServiceFabricIntegrationOptions.UseUniqueServiceUrl));
            Assert.Equal(nameof(listener), exception.ParamName);
        }

        [Fact]
        public void ReturnsHostBuilderWithoutReconfiguringWhenSettingIsAlreadyTrue()
        {
            _ = hostBuilder.Setup(_ => _.GetSetting(settingName)).Returns(settingValue);

            IWebHostBuilder actual = hostBuilder.Object.UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl);

            Assert.Same(hostBuilder.Object, actual);
            hostBuilder.Verify(_ => _.GetSetting(It.IsAny<string>()), Times.Once);
            hostBuilder.Verify(_ => _.UseSetting(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            hostBuilder.Verify(_ => _.ConfigureServices(It.IsAny<Action<IServiceCollection>>()), Times.Never);
            Assert.Empty(listener.UrlSuffix);
        }

        [Fact]
        public void MarksHostBuilderToPreventDoubleConfiguration()
        {
            _ = hostBuilder.Object.UseServiceFabricIntegration(listener, options);

            hostBuilder.Verify(_ => _.UseSetting(settingName, settingValue), Times.Once);
            hostBuilder.Verify(_ => _.UseSetting(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(ServiceFabricIntegrationOptions.UseUniqueServiceUrl)]
        [InlineData(ServiceFabricIntegrationOptions.UseUniqueServiceUrl | ServiceFabricIntegrationOptions.UseReverseProxyIntegration)]
        public void ConfiguresListenerToUseUniqueServiceUrlWhenOptionsHasUseUniqueServiceUrlFlag(ServiceFabricIntegrationOptions options)
        {
            _ = hostBuilder.Object.UseServiceFabricIntegration(listener, options);

            Assert.NotEmpty(listener.UrlSuffix);
        }

        [Theory]
        [InlineData(ServiceFabricIntegrationOptions.None)]
        [InlineData(ServiceFabricIntegrationOptions.UseReverseProxyIntegration)]
        public void DoesNotConfigureListenerToUseUniqueServiceUrlWhenOptionsDoesNotHaveUseUniqueServiceUrlFlag(ServiceFabricIntegrationOptions options)
        {
            _ = hostBuilder.Object.UseServiceFabricIntegration(listener, options);

            Assert.Empty(listener.UrlSuffix);
        }

        [Fact]
        public void RegistersServiceFabricSetupFilterAsSingleton()
        {
            ServiceDescriptor descriptor = InvokeAndCaptureStartupFilterDescriptor(options);

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        }

        [Theory]
        [InlineData(ServiceFabricIntegrationOptions.None)]
        [InlineData(ServiceFabricIntegrationOptions.UseUniqueServiceUrl)]
        [InlineData(ServiceFabricIntegrationOptions.UseReverseProxyIntegration)]
        [InlineData(ServiceFabricIntegrationOptions.UseUniqueServiceUrl | ServiceFabricIntegrationOptions.UseReverseProxyIntegration)]
        public void RegistersServiceFabricSetupFilterWithListenerUrlSuffixAndOptions(ServiceFabricIntegrationOptions options)
        {
            ServiceDescriptor descriptor = InvokeAndCaptureStartupFilterDescriptor(options);

            var filter = (ServiceFabricSetupFilter)descriptor.ImplementationInstance;
            Assert.Same(listener.UrlSuffix, filter.Field<string>().Value);
            Assert.Equal(options, filter.Field<ServiceFabricIntegrationOptions>().Value);
        }

        ServiceDescriptor InvokeAndCaptureStartupFilterDescriptor(ServiceFabricIntegrationOptions options)
        {
            Action<IServiceCollection> captured = null;
            _ = hostBuilder
                .Setup(_ => _.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
                .Callback<Action<IServiceCollection>>(a => captured = a);

            _ = hostBuilder.Object.UseServiceFabricIntegration(listener, options);

            hostBuilder.Verify(_ => _.ConfigureServices(It.IsAny<Action<IServiceCollection>>()), Times.Once);
            Assert.NotNull(captured);
            ServiceCollection services = new();
            captured(services);
            return services.Single(_ => _.ServiceType == typeof(IStartupFilter));
        }

        sealed class TestCommunicationListener(ServiceContext serviceContext)
            : AspNetCoreCommunicationListener(serviceContext, (_, _) => Mock.Of<IWebHost>())
        {
            protected internal override string GetListenerUrl() => string.Empty;
        }
    }
}
