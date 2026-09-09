// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport.Client;

public abstract class FabricTransportClientConnectionEventHandlerBrokerTest
{
    readonly NativeFabricTransport.IFabricTransportClientEventHandler sut;

    // Constructor parameters
    readonly Mock<IFabricTransportClientEventHandler> clientConnectionHandler = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    FabricTransportClientConnectionEventHandlerBrokerTest() =>
        sut = new FabricTransportClientConnectionEventHandlerBroker(clientConnectionHandler.Object);

    public sealed class Constructor: FabricTransportClientConnectionEventHandlerBrokerTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate clientConnectionHandler.
        public void ThrowsArgumentNullExceptionWhenClientConnectionHandlerIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new FabricTransportClientConnectionEventHandlerBroker(null));
            Assert.Equal(nameof(clientConnectionHandler), exception.ParamName);
        }
    }

    public sealed class OnConnected: FabricTransportClientConnectionEventHandlerBrokerTest
    {
        readonly IntPtr connectionAddress = new(fuzzy.Int32());

        [Fact]
        public void CallsClientConnectionHandlerOnConnected()
        {
            sut.OnConnected(connectionAddress);

            clientConnectionHandler.Verify(_ => _.OnConnected(), Times.Once);
            clientConnectionHandler.VerifyNoOtherCalls();
        }
    }

    public sealed class OnDisconnected: FabricTransportClientConnectionEventHandlerBrokerTest
    {
        readonly IntPtr connectionAddress = new(fuzzy.Int32());
        readonly int errorCode = fuzzy.Int32();

        [Fact]
        public void CallsClientConnectionHandlerOnDisconnected()
        {
            sut.OnDisconnected(connectionAddress, errorCode);

            clientConnectionHandler.Verify(_ => _.OnDisconnected(), Times.Once);
            clientConnectionHandler.VerifyNoOtherCalls();
        }
    }
}
