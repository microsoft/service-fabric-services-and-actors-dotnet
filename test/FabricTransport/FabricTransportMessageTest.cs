// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Moq;
using Xunit;
using static Microsoft.ServiceFabric.FabricTransport.NativeFabricTransport;

namespace Microsoft.ServiceFabric.FabricTransport;

public abstract class FabricTransportMessageTest
{
    readonly FabricTransportMessage sut;

    // Constructor parameters
    readonly FabricTransportRequestHeader requestHeader;
    readonly FabricTransportRequestBody requestBody;
    readonly IFabricTransportMessage nativeInterfaceRoot = Mock.Of<IFabricTransportMessage>();

    readonly Mock<Action> headerDispose = new();
    readonly Mock<Action> bodyDispose = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    FabricTransportMessageTest()
    {
        requestHeader = new FabricTransportRequestHeader(new ArraySegment<byte>(fuzzy.Array(fuzzy.Byte)), headerDispose.Object);
        requestBody = new FabricTransportRequestBody([new ArraySegment<byte>(fuzzy.Array(fuzzy.Byte))], bodyDispose.Object);
        sut = new FabricTransportMessage(requestHeader, requestBody, nativeInterfaceRoot);
    }

    public sealed class Dispose : FabricTransportMessageTest
    {
        [Fact]
        public void InvokesDisposeOnRequestBodyAndRequestHeader()
        {
            sut.Dispose();
            bodyDispose.Verify(_ => _(), Times.Once);
            headerDispose.Verify(_ => _(), Times.Once);
        }

        [Fact(Explicit = true)] // TODO: SUT testability limitation. Cannot mock native COM release.
        public void ReleasesNativeInterfaceRoot() =>
            // nativeInterfaceRoot.SafeReleaseComObject() is gated on Marshal.IsComObject and is a no-op
            // for managed mocks. The SUT exposes no other observable signal that the COM release branch executed.
            throw new NotImplementedException();

        [Fact]
        public void InvokesDisposeOnRequestBodyWhenRequestHeaderIsNull()
        {
            FabricTransportMessage sut = new(null, requestBody);
            sut.Dispose();
            bodyDispose.Verify(_ => _(), Times.Once);
        }

        [Fact]
        public void InvokesDisposeOnRequestHeaderWhenRequestBodyIsNull()
        {
            FabricTransportMessage sut = new(requestHeader, null);
            sut.Dispose();
            headerDispose.Verify(_ => _(), Times.Once);
        }
    }

    public sealed class GetBody : FabricTransportMessageTest
    {
        [Fact]
        public void ReturnsRequestBodyPassedToConstructor() =>
            Assert.Same(requestBody, sut.GetBody());
    }

    public sealed class GetHeader : FabricTransportMessageTest
    {
        [Fact]
        public void ReturnsRequestHeaderPassedToConstructor() =>
            Assert.Same(requestHeader, sut.GetHeader());
    }
}
