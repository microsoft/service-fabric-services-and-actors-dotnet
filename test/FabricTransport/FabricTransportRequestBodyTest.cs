// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport;

public abstract class FabricTransportRequestBodyTest
{
    readonly FabricTransportRequestBody sut;
    readonly IEnumerable<ArraySegment<byte>> sendBuffers = Mock.Of<IEnumerable<ArraySegment<byte>>>();
    readonly Action disposeAction = Mock.Of<Action>();

    FabricTransportRequestBodyTest() =>
        sut = new FabricTransportRequestBody(sendBuffers, disposeAction);

    public sealed class Dispose : FabricTransportRequestBodyTest
    {
        [Fact]
        public void InvokesDisposeAction()
        {
            sut.Dispose();
            Mock.Get(disposeAction).Verify(_ => _(), Times.Once);
        }

        [Fact]
        public void DoesNotThrowWhenDisposeActionIsNull() =>
            new FabricTransportRequestBody(sendBuffers, null).Dispose();
    }

    public sealed class GetBodyBuffers : FabricTransportRequestBodyTest
    {
        [Fact]
        public void ReturnsSendBuffers() =>
            Assert.Same(sendBuffers, sut.GetBodyBuffers());
    }

    public sealed class GetRecievedStream : FabricTransportRequestBodyTest
    {
        new readonly FabricTransportRequestBody sut;
        readonly Stream recievedStream = Mock.Of<Stream>(); // Spelling matches SUT parameter

        public GetRecievedStream() =>
            sut = new FabricTransportRequestBody(recievedStream);

        [Fact]
        public void ReturnsRecievedStream() =>
            Assert.Same(recievedStream, sut.GetRecievedStream());
    }
}
