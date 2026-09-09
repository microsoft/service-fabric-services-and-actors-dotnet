// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport.Client;

public abstract class FabricTransportCallbackHandlerBrokerTest
{
    readonly NativeFabricTransport.IFabricTransportCallbackMessageHandler sut;
    readonly IFabricTransportCallbackMessageHandler callImpl = Mock.Of<IFabricTransportCallbackMessageHandler>();

    FabricTransportCallbackHandlerBrokerTest() =>
        sut = new FabricTransportCallbackHandlerBroker(callImpl);

    public sealed class Constructor: FabricTransportCallbackHandlerBrokerTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate callImpl.
        public void ThrowsArgumentNullExceptionWhenCallImplIsNull()
        {
            // The constructor stores callImpl without a null check, so a null argument is accepted here and only
            // dereferenced later by HandleOneWay, producing NullReferenceException instead of ArgumentNullException.
            var exception = Assert.Throws<ArgumentNullException>(() => new FabricTransportCallbackHandlerBroker(null));
            Assert.Equal(nameof(callImpl), exception.ParamName);
        }
    }

    public sealed class HandleOneWay: FabricTransportCallbackHandlerBrokerTest
    {
        readonly NativeFabricTransport.IFabricTransportMessage message = Mock.Of<NativeFabricTransport.IFabricTransportMessage>();

        [Fact]
        public void InvokesOneWayMessageOnCallImplWithConvertedMessage()
        {
            sut.HandleOneWay(message);

            Mock.Get(message).Verify(
                _ => _.GetHeaderAndBodyBuffer(out It.Ref<IntPtr>.IsAny, out It.Ref<uint>.IsAny, out It.Ref<IntPtr>.IsAny),
                Times.Once);
            Mock.Get(callImpl).Verify(_ => _.OneWayMessage(It.IsAny<FabricTransportMessage>()), Times.Once);
        }
    }
}
