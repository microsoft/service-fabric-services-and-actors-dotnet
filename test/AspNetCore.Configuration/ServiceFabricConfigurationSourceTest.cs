// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.Generic;
using System.Fabric;
using Fuzzy;
using Inspector;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.AspNetCore.Configuration;

public abstract class ServiceFabricConfigurationSourceTest
{
    readonly IConfigurationSource sut;

    // Constructor parameters
    readonly ICodePackageActivationContext activationContext = Mock.Of<ICodePackageActivationContext>();
    readonly ServiceFabricConfigurationOptions options = new(fuzzy.String());

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ServiceFabricConfigurationSourceTest() =>
        sut = new ServiceFabricConfigurationSource(activationContext, options);

    public sealed class Build : ServiceFabricConfigurationSourceTest
    {
        readonly IConfigurationBuilder builder = Mock.Of<IConfigurationBuilder>();

        [Fact]
        public void ReturnsConfigurationProviderInitializedFromSource()
        {
            var package = Type<ConfigurationPackage>.Uninitialized();
            _ = Mock.Get(activationContext).Setup(_ => _.GetConfigurationPackageObject(options.PackageName)).Returns(package);
            Mock<Action<ConfigurationPackage, IDictionary<string, string>>> configAction = new();
            options.ConfigAction = configAction.Object;

            IConfigurationProvider provider = sut.Build(builder);
            provider.Load();

            Mock.Get(activationContext).Verify(_ => _.GetConfigurationPackageObject(options.PackageName), Times.Once);
            Mock.Get(activationContext).Verify(_ => _.GetConfigurationPackageObject(It.IsAny<string>()), Times.Once);
            configAction.Verify(_ => _(package, It.IsAny<IDictionary<string, string>>()), Times.Once);
            configAction.Verify(_ => _(It.IsAny<ConfigurationPackage>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }
    }

    public sealed class Constructor : ServiceFabricConfigurationSourceTest
    {
        [Fact]
        public void InitializesActivationContext() =>
            Assert.Same(activationContext, ((ServiceFabricConfigurationSource)sut).ActivationContext);
    }
}
