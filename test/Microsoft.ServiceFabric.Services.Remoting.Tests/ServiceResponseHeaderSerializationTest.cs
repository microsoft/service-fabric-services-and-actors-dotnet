// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Tests
{
    using System;
    using System.Text;
    using FluentAssertions;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Xunit;

    /// <summary>
    /// Tests for SErviceResponseHeader serialization.
    /// </summary>
    public class ServiceResponseHeaderSerializationTest
    {
        /// <summary>
        /// Test serialization with empty headers.
        /// </summary>
        [Fact]
        public void SerializeEmptyHeaders()
        {
            var headers = new ServiceRemotingResponseMessageHeader();
            var serializer = new ServiceRemotingMessageHeaderSerializer(new BufferPoolManager());
            var serializedHeader = serializer.SerializeResponseHeader(headers);
            serializedHeader.Should().BeNull();
        }

        /// <summary>
        /// Test serialization with non-empty headers.
        /// </summary>
        [Fact]
        public void SerializeNotEmptyHeaders()
        {
            var headers = new ServiceRemotingResponseMessageHeader();
            var data = "My Headers";
            headers.AddHeader("CustomHeader", Encoding.ASCII.GetBytes(data));
            var serializer = new ServiceRemotingMessageHeaderSerializer(new BufferPoolManager());
            var serializedHeader = serializer.SerializeResponseHeader(headers);
            serializedHeader.Should().NotBeNull();
            serializedHeader.GetSendBuffer().Count.Should().BeGreaterThan(0);
        }
    }
}
