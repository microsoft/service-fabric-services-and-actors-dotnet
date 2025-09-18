// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Xunit;

namespace Microsoft.ServiceFabric.Services
{
    public abstract class FabricTransportRemotingListenerSettingsTest
    {
        public FabricTransportRemotingListenerSettingsTest()
        {
        }

        public sealed class Constructor : FabricTransportRemotingListenerSettingsTest
        {
            [Fact]
            public void ShouldInitializeRemotingExceptionDepthToDefaultValue()
            {
                // Act
                var settings = new FabricTransportRemotingListenerSettings();

                // Assert
                Assert.Equal(ExceptionSerializer.DefaultRemotingExceptionDepth, settings.RemotingExceptionDepth);
            }

            [Fact]
            public void ShouldImplementIExceptionSerializerSettings()
            {
                // Act
                var settings = new FabricTransportRemotingListenerSettings();

                // Assert
                Assert.IsAssignableFrom<IExceptionSerializerSettings>(settings);
            }
        }

        public sealed class RemotingExceptionDepth : FabricTransportRemotingListenerSettingsTest
        {
            [Fact]
            public void ShouldReturnSetValue()
            {
                // Arrange
                var settings = new FabricTransportRemotingListenerSettings();
                const int expectedDepth = 5;

                // Act
                settings.RemotingExceptionDepth = expectedDepth;

                // Assert
                Assert.Equal(expectedDepth, settings.RemotingExceptionDepth);
            }

            [Fact]
            public void ShouldSetToMaxValueWhenValueIsZero()
            {
                // Arrange
                var settings = new FabricTransportRemotingListenerSettings();

                // Act
                settings.RemotingExceptionDepth = 0;

                // Assert
                Assert.Equal(int.MaxValue, settings.RemotingExceptionDepth);
            }

            [Fact]
            public void ShouldSetToMaxValueWhenValueIsNegative()
            {
                // Arrange
                var settings = new FabricTransportRemotingListenerSettings();

                // Act
                settings.RemotingExceptionDepth = -1;

                // Assert
                Assert.Equal(int.MaxValue, settings.RemotingExceptionDepth);
            }

            [Fact]
            public void ShouldAcceptPositiveValues()
            {
                // Arrange
                var settings = new FabricTransportRemotingListenerSettings();
                const int expectedDepth = 10;

                // Act
                settings.RemotingExceptionDepth = expectedDepth;

                // Assert
                Assert.Equal(expectedDepth, settings.RemotingExceptionDepth);
            }
        }
    }
}
