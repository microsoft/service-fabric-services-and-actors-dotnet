// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class ResolvedServicePartitionClientTest
{
    readonly ResolvedServicePartitionClient sut;
    readonly ResolvedServicePartitionClient other = new()
    {
        Rsp = Type<ResolvedServicePartition>.Uninitialized(),
        Client = Mock.Of<ICommunicationClient>(),
    };

    protected ResolvedServicePartitionClientTest() => sut = new(other);

    public sealed class Client : ResolvedServicePartitionClientTest
    {
        [Fact]
        public void ReturnsValuePreviouslySet()
        {
            var client = Mock.Of<ICommunicationClient>();
            sut.Client = client;
            Assert.Same(client, sut.Client);
        }
    }

    public sealed class Constructor : ResolvedServicePartitionClientTest
    {
        [Fact]
        public void InitializesProperties()
        {
            var sut = new ResolvedServicePartitionClient();
            Assert.Null(sut.Rsp);
            Assert.Null(sut.Client);
        }
    }

    public sealed class Constructor_ResolvedServicePartitionClient : ResolvedServicePartitionClientTest
    {
        [Fact]
        public void CopiesRspAndClientFromOther()
        {
            Assert.Same(other.Rsp, sut.Rsp);
            Assert.Same(other.Client, sut.Client);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Copy constructor should throw ArgumentNullException when other is null
        public void ThrowsArgumentNullExceptionWhenOtherIsNull()
        {
            // The copy constructor dereferences `other` without validating it, so passing null currently throws
            // NullReferenceException. The desired behavior is to throw ArgumentNullException with ParamName "other",
            // matching the public API contract for null arguments. Enable this test once the SUT is fixed.
            var thrown = Assert.Throws<ArgumentNullException>(() => new ResolvedServicePartitionClient(null));
            Assert.Equal("other", thrown.ParamName);
        }
    }

    public sealed class Rsp : ResolvedServicePartitionClientTest
    {
        [Fact]
        public void ReturnsValuePreviouslySet()
        {
            var rsp = Type<ResolvedServicePartition>.Uninitialized();
            sut.Rsp = rsp;
            Assert.Same(rsp, sut.Rsp);
        }
    }
}
