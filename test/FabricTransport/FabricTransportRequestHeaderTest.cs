// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.IO;
using Fuzzy;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport;

public abstract class FabricTransportRequestHeaderTest
{
    readonly FabricTransportRequestHeader sut;

    // Constructor parameters
    readonly ArraySegment<byte> requestHeaderBuffer;
    readonly Action disposeAction = Mock.Of<Action>();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    FabricTransportRequestHeaderTest()
    {
        // Use a sliced segment with non-zero offset and count smaller than the array remainder, so the test
        // detects accidental normalization of Offset/Count by the SUT. The default Array length leaves ample
        // room for the derived offset and count without additional length constraints.
        byte[] array = fuzzy.Array(fuzzy.Byte);
        int offset = fuzzy.Int32().Between(1, array.Length - 2);
        int count = fuzzy.Int32().Between(1, array.Length - offset - 1);
        requestHeaderBuffer = new ArraySegment<byte>(array, offset, count);
        sut = new FabricTransportRequestHeader(requestHeaderBuffer, disposeAction);
    }

    public sealed class Dispose : FabricTransportRequestHeaderTest
    {
        [Fact]
        public void InvokesDisposeAction()
        {
            sut.Dispose();
            Mock.Get(disposeAction).Verify(_ => _(), Times.Once);
        }

        [Fact]
        public void DoesNotThrowWhenDisposeActionIsNull() =>
            new FabricTransportRequestHeader(requestHeaderBuffer, null).Dispose();
    }

    public sealed class GetRecievedStream : FabricTransportRequestHeaderTest
    {
        new readonly FabricTransportRequestHeader sut;
        readonly Stream recievedHeaderStream = Mock.Of<Stream>(); // Spelling matches SUT parameter

        public GetRecievedStream() =>
            sut = new FabricTransportRequestHeader(recievedHeaderStream);

        [Fact]
        public void ReturnsRecievedHeaderStream() =>
            Assert.Same(recievedHeaderStream, sut.GetRecievedStream());
    }

    public sealed class GetSendBuffer : FabricTransportRequestHeaderTest
    {
        [Fact]
        public void ReturnsRequestHeaderBuffer() =>
            Assert.StrictEqual(requestHeaderBuffer, sut.GetSendBuffer());
    }
}
