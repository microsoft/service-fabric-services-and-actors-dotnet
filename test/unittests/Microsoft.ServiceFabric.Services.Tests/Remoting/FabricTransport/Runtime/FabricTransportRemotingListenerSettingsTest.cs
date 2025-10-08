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
        readonly FabricTransportRemotingListenerSettings sut = new FabricTransportRemotingListenerSettings();
        public sealed class Constructor : FabricTransportRemotingListenerSettingsTest
        {
            [Fact]
            public void ShouldInitializeRemotingExceptionDepthToDefaultValue()
            {
                Assert.Equal(ExceptionConversionHandler.DefaultRemotingExceptionDepth, sut.RemotingExceptionDepth);
            }

            [Fact]
            public void ShouldImplementIExceptionSerializerSettings()
            {
                Assert.IsAssignableFrom<IExceptionSerializerSettings>(sut);
            }
        }

        public sealed class RemotingExceptionDepth : FabricTransportRemotingListenerSettingsTest
        {
            [Fact]
            public void ShouldReturnSetValue()
            {
                const int expectedDepth = 5;

                sut.RemotingExceptionDepth = expectedDepth;

                Assert.Equal(expectedDepth, sut.RemotingExceptionDepth);
            }

            [Fact]
            public void ShouldSetToMaxValueWhenValueIsZero()
            {
                sut.RemotingExceptionDepth = 0;

                Assert.Equal(int.MaxValue, sut.RemotingExceptionDepth);
            }

            [Fact]
            public void ShouldSetToMaxValueWhenValueIsNegative()
            {
                sut.RemotingExceptionDepth = -1;

                Assert.Equal(int.MaxValue, sut.RemotingExceptionDepth);
            }

            [Fact]
            public void ShouldAcceptPositiveValues()
            {
                const int expectedDepth = 10;

                sut.RemotingExceptionDepth = expectedDepth;

                Assert.Equal(expectedDepth, sut.RemotingExceptionDepth);
            }
        }
    }
}
