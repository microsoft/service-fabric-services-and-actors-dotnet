// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Microsoft.ServiceFabric.FabricTransport.Runtime;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport;

[WindowsOnly("Can't load libFabricCommon.so on Linux.")]
public abstract class FabricTransportListenerSettingsTest
{
    public sealed class LoadFrom: FabricTransportListenerSettingsTest
    {
        [Fact]
        public void ThrowsExceptionWhenCodePackageDoesNotExist() => 
            Assert.Throws<ArgumentException>(() => FabricTransportListenerSettings.LoadFrom("TestServiceListener", "Config1"));

        [Fact]
        public static void ThrowsExceptionWhenSectionDoesNotExist() =>
            Assert.Throws<ArgumentException>(() => FabricTransportListenerSettings.LoadFrom("TestServiceListener"));
    }

    public sealed class TryLoadFrom: FabricTransportListenerSettingsTest
    {
        [Fact]
        public void ReturnsFalseWhenConfigurationPackageDoesNotExist() =>
            Assert.False(FabricTransportListenerSettings.TryLoadFrom("TestServiceListenerTransportSettings", out _, "Config2"));
    }
}
