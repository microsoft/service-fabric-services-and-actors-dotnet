// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using Fuzzy;
using Inspector;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.AspNetCore.Configuration;

public abstract class ServiceFabricConfigurationProviderTest
{
    readonly ServiceFabricConfigurationProvider sut;

    // Constructor parameters
    readonly Mock<ICodePackageActivationContext> activationContext = new();
    readonly ServiceFabricConfigurationOptions options;

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);
    readonly string packageName = fuzzy.String();
    readonly Mock<Action<ConfigurationPackage, IDictionary<string, string>>> configAction = new();

    ServiceFabricConfigurationProviderTest()
    {
        options = new(packageName) { ConfigAction = configAction.Object };
        sut = new ServiceFabricConfigurationProvider(activationContext.Object, options);
    }

    public sealed class Constructor : ServiceFabricConfigurationProviderTest
    {
        readonly ConfigurationPackage matching;

        public Constructor() =>
            matching = Package(packageName);

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor doesn't validate activationContext.
        public void ThrowsArgumentNullExceptionWhenActivationContextIsNull()
        {
            // The constructor stores activationContext without a null check and then dereferences it
            // when subscribing to ConfigurationPackageModifiedEvent, causing NullReferenceException
            // instead of an ArgumentNullException naming the offending parameter.
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ServiceFabricConfigurationProvider(null, options));
            Assert.Equal(sut.Constructor().Parameter<ICodePackageActivationContext>().Name, exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenOptionsIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ServiceFabricConfigurationProvider(activationContext.Object, null));
            Assert.Equal(sut.Constructor().Parameter<ServiceFabricConfigurationOptions>().Name, exception.ParamName);
        }

        [Fact]
        public void LoadsPackageAndNotifiesChangeWhenAddedPackageNameMatches() =>
            VerifyLoadsPackageAndNotifiesChange(RaiseAdded);

        [Fact]
        public void LoadsPackageAndNotifiesChangeWhenModifiedPackageNameMatches() =>
            VerifyLoadsPackageAndNotifiesChange(RaiseModified);

        [Fact]
        public void IgnoresAddedPackageWhenNameDoesNotMatch() =>
            VerifyIgnoresPackageWhenNameDoesNotMatch(RaiseAdded);

        [Fact]
        public void IgnoresModifiedPackageWhenNameDoesNotMatch() =>
            VerifyIgnoresPackageWhenNameDoesNotMatch(RaiseModified);

        [Fact]
        public void ThrowsArgumentNullExceptionWhenAddedPackageDescriptionIsNull() =>
            VerifyThrowsArgumentNullExceptionWhenPackageDescriptionIsNull(RaiseAdded);

        [Fact]
        public void ThrowsArgumentNullExceptionWhenModifiedPackageDescriptionIsNull() =>
            VerifyThrowsArgumentNullExceptionWhenPackageDescriptionIsNull(RaiseModified);

        [Fact(Explicit = true)] // TODO: SUT bug. HandleNewPackage dereferences package without a null check.
        public void ThrowsArgumentNullExceptionWhenAddedPackageIsNull() =>
            // HandleNewPackage accesses package.Description immediately, throwing NullReferenceException
            // instead of an ArgumentNullException naming the offending parameter.
            VerifyThrowsArgumentNullExceptionWhenPackageIsNull(RaiseAdded);

        [Fact(Explicit = true)] // TODO: SUT bug. HandleNewPackage dereferences package without a null check.
        public void ThrowsArgumentNullExceptionWhenModifiedPackageIsNull() =>
            // HandleNewPackage accesses package.Description immediately, throwing NullReferenceException
            // instead of an ArgumentNullException naming the offending parameter.
            VerifyThrowsArgumentNullExceptionWhenPackageIsNull(RaiseModified);

        void VerifyLoadsPackageAndNotifiesChange(Action<ConfigurationPackage> raiseEvent)
        {
            string staleKey = fuzzy.String();
            sut.Set(staleKey, fuzzy.String());
            string key = staleKey + fuzzy.String();
            string value = fuzzy.String();
            _ = configAction.Setup(_ => _(matching, It.IsAny<IDictionary<string, string>>()))
                .Callback((ConfigurationPackage _, IDictionary<string, string> data) => data[key] = value);
            IChangeToken token = sut.GetReloadToken();

            raiseEvent(matching);

            configAction.Verify(_ => _(matching, It.IsAny<IDictionary<string, string>>()), Times.Once);
            configAction.Verify(_ => _(It.IsAny<ConfigurationPackage>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
            Assert.False(sut.TryGet(staleKey, out _));
            Assert.True(sut.TryGet(key, out string actual));
            Assert.Same(value, actual);
            Assert.True(token.HasChanged);
        }

        void VerifyIgnoresPackageWhenNameDoesNotMatch(Action<ConfigurationPackage> raiseEvent)
        {
            ConfigurationPackage other = Package(packageName + fuzzy.String());
            string existingKey = fuzzy.String();
            string existingValue = fuzzy.String();
            sut.Set(existingKey, existingValue);
            IChangeToken token = sut.GetReloadToken();

            raiseEvent(other);

            configAction.Verify(
                _ => _(It.IsAny<ConfigurationPackage>(), It.IsAny<IDictionary<string, string>>()),
                Times.Never);
            Assert.False(token.HasChanged);
            Assert.True(sut.TryGet(existingKey, out string actual));
            Assert.Equal(existingValue, actual);
        }

        void VerifyThrowsArgumentNullExceptionWhenPackageDescriptionIsNull(Action<ConfigurationPackage> raiseEvent)
        {
            var package = Type<ConfigurationPackage>.Uninitialized();

            var exception = Assert.Throws<ArgumentNullException>(() => raiseEvent(package));
            Assert.Equal($"package.{nameof(ConfigurationPackage.Description)}", exception.ParamName);
        }

        void VerifyThrowsArgumentNullExceptionWhenPackageIsNull(Action<ConfigurationPackage> raiseEvent)
        {
            var exception = Assert.Throws<ArgumentNullException>(() => raiseEvent(null));
            Assert.Equal("package", exception.ParamName);
        }

        void RaiseAdded(ConfigurationPackage package) =>
            activationContext.Raise(_ => _.ConfigurationPackageAddedEvent += null,
                new PackageAddedEventArgs<ConfigurationPackage> { Package = package });

        void RaiseModified(ConfigurationPackage newPackage) =>
            activationContext.Raise(_ => _.ConfigurationPackageModifiedEvent += null,
                new PackageModifiedEventArgs<ConfigurationPackage> { NewPackage = newPackage });
    }

    public sealed class Load : ServiceFabricConfigurationProviderTest
    {
        readonly ConfigurationPackage package;

        public Load()
        {
            package = Package(packageName);
            _ = activationContext.Setup(_ => _.GetConfigurationPackageObject(packageName)).Returns(package);
        }

        [Fact]
        public void LoadsPackageFromActivationContextAndPopulatesData()
        {
            string key = fuzzy.String();
            string value = fuzzy.String();
            _ = configAction.Setup(_ => _(package, It.IsAny<IDictionary<string, string>>()))
                .Callback((ConfigurationPackage _, IDictionary<string, string> data) => data[key] = value);
            IChangeToken token = sut.GetReloadToken();

            sut.Load();

            activationContext.Verify(_ => _.GetConfigurationPackageObject(It.IsAny<string>()), Times.Once);
            configAction.Verify(_ => _(package, It.IsAny<IDictionary<string, string>>()), Times.Once);
            configAction.Verify(_ => _(It.IsAny<ConfigurationPackage>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
            Assert.True(sut.TryGet(key, out string actual));
            Assert.Same(value, actual);
            Assert.False(token.HasChanged);
        }

        [Fact]
        public void PreservesExistingData()
        {
            string existingKey = fuzzy.String();
            string existingValue = fuzzy.String();
            sut.Set(existingKey, existingValue);

            sut.Load();

            Assert.True(sut.TryGet(existingKey, out string preserved));
            Assert.Same(existingValue, preserved);
        }
    }

    static ConfigurationPackage Package(string name)
    {
        var desc = Type<ConfigurationPackageDescription>.Uninitialized();
        desc.Property<string>(nameof(ConfigurationPackageDescription.Name)).Set(name);
        var pkg = Type<ConfigurationPackage>.Uninitialized();
        pkg.Property<ConfigurationPackageDescription>().Set(desc);
        return pkg;
    }
}
