// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.ServiceFabric.FabricTransport.Runtime;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport
{
    public class ServiceCommunicationTest
    {
        [Fact]
        public static void ReadTransportSettingsFromConfig()
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "ServiceCommunicationTestSettings.xml");
            var settings = FabricTransportSettings.LoadFrom("TestServiceListenerTransportSettings", fileName);
            Assert.True(settings.OperationTimeout.TotalSeconds == 2);
            Assert.True(settings.KeepAliveTimeout.TotalSeconds == 1000);
            Assert.True(settings.MaxConcurrentCalls == 5);
            Assert.True(settings.MaxQueueSize == 35);

            Assert.True(settings.SecurityCredentials.CredentialType == CredentialType.X509);
            var x509securityCredentail = (X509Credentials) settings.SecurityCredentials;
            Assert.True(x509securityCredentail.FindType == X509FindType.FindBySubjectName);
            Assert.True(x509securityCredentail.FindValue.Equals("CN=alice.server.servicefabric.azure.test"));
            Assert.True(x509securityCredentail.StoreName.Equals(X509Credentials.StoreNameDefault));
            Assert.True(x509securityCredentail.ProtectionLevel == ProtectionLevel.EncryptAndSign);
            Assert.True(x509securityCredentail.StoreLocation == StoreLocation.LocalMachine);
            var commonNames = new List<string>() {"alice.server.servicefabric.azure.test"};
            Assert.True(x509securityCredentail.RemoteCommonNames.SequenceEqual(commonNames));
        }

        [Fact]
        public static void ReadDefaultTransportSettingsFromConfigWhenSectionIsNotPresent()
        {
            var settings = FabricTransportSettings.GetDefault("Dummy");
            Assert.True(settings.OperationTimeout.TotalSeconds == TimeSpan.FromMinutes(5).TotalSeconds);
            Assert.True(settings.KeepAliveTimeout.TotalSeconds == TimeSpan.Zero.TotalSeconds);
            Assert.True(settings.MaxConcurrentCalls == 0);
            Assert.True(settings.MaxQueueSize == 10000);
            Assert.True(settings.SecurityCredentials.CredentialType == CredentialType.None);
        }

        [Fact]
        public static void TestReadSettingsUsingTryLoadFrom()
        {
            FabricTransportSettings settings = null;
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "ServiceCommunicationTestSettings.xml");
            var isSucceeded = FabricTransportSettings.TryLoadFrom("TestServiceListenerTransportSettings", out settings,
                fileName);
            Assert.True(isSucceeded);
            Assert.True(settings.OperationTimeout.TotalSeconds == 2);
            Assert.True(settings.KeepAliveTimeout.TotalSeconds == 1000);
            Assert.True(settings.MaxConcurrentCalls == 5);
            Assert.True(settings.MaxQueueSize == 35);
            FabricTransportSettings settings2 = null;
            isSucceeded = FabricTransportSettings.TryLoadFrom("TestServiceListener", out settings2, fileName);
            Assert.False(isSucceeded);
            FabricTransportListenerSettings listenerSettings;
            isSucceeded = FabricTransportListenerSettings.TryLoadFrom("TestServiceListenerTransportSettings",
                out listenerSettings, "Config2");
            Assert.False(isSucceeded);
        }

        [Fact]
        public static void ReadTransportSettingsFromConfigWhenSectionOrFileIsNotPresent()
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "ServiceCommunicationTestSettings.xml");
            try
            {
                var settings = FabricTransportSettings.LoadFrom("TestServiceListener", fileName);
                Assert.Fail("Exception should be thrown as Section is Not Present");
            }
            catch (ArgumentException)
            {
                //Expected Exception
            }
            try
            {
                var settings = FabricTransportSettings.LoadFrom("TestServiceListener");
                Assert.Fail("Exception should be thrown as FileName is Not Present");
            }
            catch (ArgumentException)
            {
                //Expected Exception
            }
        }

        [Fact]
        public static void ReadTransportListenerSettingsFromConfigPackage()
        {
            try
            {
                var settings = FabricTransportListenerSettings.LoadFrom("TestServiceListener", "Config1");
                Assert.Fail("Exception should be thrown as ConfigPackage is Not Present");
            }
            catch (ArgumentException)
            {
                //Expected Exception
            }

            try
            {
                var settings = FabricTransportListenerSettings.LoadFrom("TestServiceListener");
                Assert.Fail("Exception should be thrown as Section is Not Present");
            }
            catch (ArgumentException)
            {
                //Expected Exception
            }
        }
    }
}
