// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime;

public abstract class FabricTransportRequestContextTest
{
    readonly FabricTransportRequestContext sut;

    // Constructor parameters
    readonly string clientId = fuzzy.String();
    readonly Mock<Func<string, FabricTransportCallbackClient>> getCallBack = new(); // Casing matches SUT parameter name

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    FabricTransportRequestContextTest() =>
        sut = new FabricTransportRequestContext(clientId, getCallBack.Object);

    public sealed class Constructor : FabricTransportRequestContextTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate getCallBack.
        public void ThrowsArgumentNullExceptionWhenGetCallBackIsNull()
        {
            // The constructor silently stores null in the callback field. GetCallbackClient then throws
            // NullReferenceException far from the original caller, violating the rule in csharp.instructions.md
            // requiring ArgumentException over low-level exceptions for invalid arguments.
            var exception = Assert.Throws<ArgumentNullException>(() => new FabricTransportRequestContext(clientId, null));
            Assert.Equal(nameof(getCallBack), exception.ParamName);
        }
    }

    public sealed class ClientId : FabricTransportRequestContextTest
    {
        [Fact]
        public void ReturnsClientIdProvidedToConstructor() =>
            Assert.Same(clientId, sut.ClientId);
    }

    public sealed class GetCallbackClient : FabricTransportRequestContextTest
    {
        readonly FabricTransportCallbackClient callbackClient = Type<FabricTransportCallbackClient>.Uninitialized();

        public GetCallbackClient() =>
            _ = getCallBack.Setup(_ => _(clientId)).Returns(callbackClient);

        [Fact]
        public void ReturnsCallbackClientReturnedByCallbackInvokedWithClientId()
        {
            Assert.Same(callbackClient, sut.GetCallbackClient());
            getCallBack.Verify(_ => _(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void CachesCallbackClientAcrossCalls()
        {
            FabricTransportCallbackClient first = sut.GetCallbackClient();
            FabricTransportCallbackClient second = sut.GetCallbackClient();

            Assert.Same(first, second);
            getCallBack.Verify(_ => _(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ReturnsNullWhenCallbackReturnsNull()
        {
            _ = getCallBack.Setup(_ => _(clientId)).Returns((FabricTransportCallbackClient)null);

            Assert.Null(sut.GetCallbackClient());
            getCallBack.Verify(_ => _(clientId), Times.Once);
            getCallBack.Verify(_ => _(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void DoesNotCacheNullResultAndRetriesCallbackOnNextCall()
        {
            _ = getCallBack.Setup(_ => _(clientId)).Returns((FabricTransportCallbackClient)null);

            _ = sut.GetCallbackClient();
            _ = sut.GetCallbackClient();

            getCallBack.Verify(_ => _(clientId), Times.Exactly(2));
            getCallBack.Verify(_ => _(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
