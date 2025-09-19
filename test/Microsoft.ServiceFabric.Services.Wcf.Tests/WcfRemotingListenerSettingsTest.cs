// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Wcf.Remoting.V2.Wcf.Runtime
{
    public abstract class WcfRemotingListenerSettingsTest
    {
        readonly WcfRemotingListenerSettings sut = new WcfRemotingListenerSettings();

        public sealed class Constructor : WcfRemotingListenerSettingsTest
        {
            [Fact]
            public void ShouldInitializeRemotingExceptionDepthToDefaultValue()
            {
                Assert.Equal(ExceptionSerializer.DefaultRemotingExceptionDepth, sut.RemotingExceptionDepth);
            }

            [Fact]
            public void ShouldImplementIExceptionSerializerSettings()
            {
                Assert.IsAssignableFrom<IExceptionSerializerSettings>(sut);
            }
        }

        public sealed class RemotingExceptionDepth : WcfRemotingListenerSettingsTest
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
