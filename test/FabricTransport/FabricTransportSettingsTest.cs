// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport;

[WindowsOnly("Can't load libFabricCommon.so on Linux.")]
public abstract class FabricTransportSettingsTest
{
    readonly string settingsFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ServiceCommunicationTestSettings.xml");

    public sealed class LoadFrom: FabricTransportSettingsTest
    {
        [Fact]
        public void ReadTransportSettingsFromConfig()
        {
            var settings = FabricTransportSettings.LoadFrom("TestServiceListenerTransportSettings", settingsFile);

            Assert.Equal(2, settings.OperationTimeout.TotalSeconds);
            Assert.Equal(1000, settings.KeepAliveTimeout.TotalSeconds);
            Assert.Equal(5, settings.MaxConcurrentCalls);
            Assert.Equal(35, settings.MaxQueueSize);
            Assert.Equal(CredentialType.X509, settings.SecurityCredentials.CredentialType);
            var credentials = (X509Credentials)settings.SecurityCredentials;
            Assert.Equal(X509FindType.FindBySubjectName, credentials.FindType);
            Assert.Equal("CN=alice.server.servicefabric.azure.test", credentials.FindValue);
            Assert.Equal(X509Credentials.StoreNameDefault, credentials.StoreName);
            Assert.Equal(ProtectionLevel.EncryptAndSign, credentials.ProtectionLevel);
            Assert.Equal(StoreLocation.LocalMachine, credentials.StoreLocation);
            Assert.Equal(["alice.server.servicefabric.azure.test"], credentials.RemoteCommonNames);
        }

        [Fact]
        public void ThrowsExceptionWhenSectionDoesNotExistInSpecifiedFile() =>
            Assert.Throws<ArgumentException>(() => FabricTransportSettings.LoadFrom("TestServiceListener", settingsFile));

        [Fact]
        public void ThrowsExceptionWhenSectionDoesNotExist() =>
            Assert.Throws<ArgumentException>(() => FabricTransportSettings.LoadFrom("TestServiceListener"));
    }

    public sealed class GetDefault: FabricTransportSettingsTest
    {
        [Fact]
        public static void ReadDefaultTransportSettingsFromConfigWhenSectionIsNotPresent()
        {
            var settings = FabricTransportSettings.GetDefault("Dummy");

            Assert.Equal(TimeSpan.FromMinutes(5), settings.OperationTimeout);
            Assert.Equal(TimeSpan.Zero, settings.KeepAliveTimeout);
            Assert.Equal(0, settings.MaxConcurrentCalls);
            Assert.Equal(10000, settings.MaxQueueSize);
            Assert.Equal(CredentialType.None, settings.SecurityCredentials.CredentialType);
        }
    }

    public sealed class TryLoadFrom: FabricTransportSettingsTest
    {
        [Fact]
        public void TestReadSettingsUsingTryLoadFrom()
        {
            bool isSucceeded = FabricTransportSettings.TryLoadFrom("TestServiceListenerTransportSettings", out FabricTransportSettings settings, settingsFile);

            Assert.True(isSucceeded);
            Assert.Equal(2, settings.OperationTimeout.TotalSeconds);
            Assert.Equal(1000, settings.KeepAliveTimeout.TotalSeconds);
            Assert.Equal(5, settings.MaxConcurrentCalls);
            Assert.Equal(35, settings.MaxQueueSize);
        }

        [Fact]
        public void ReturnsFalseWhenSectionDoesNotExistInGivenFile() =>
            Assert.False(FabricTransportSettings.TryLoadFrom("TestServiceListener", out _, settingsFile));
    }
}
