// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Xunit;

    /// <summary>
    /// Tests for RemoteException.
    /// </summary>
    public class RemoteExceptionTest
    {
        /// <summary>
        /// SerializableExceptionStream Test.
        /// </summary>
        [Fact]
        public static void SerializableExceptionStreamTest()
        {
            var segments = RemoteException.FromException(new InvalidOperationException("Testing")).Data;
            var msgStream = new SegmentedReadMemoryStream(segments);

            Exception ex;
            var isdeserialzied = RemoteException.ToException(msgStream, out ex);
            Assert.True(isdeserialzied);
            Assert.True(ex is InvalidOperationException);
        }

        /// <summary>
        /// NonSerializableExceptionStream Test.
        /// </summary>
        [Fact]
        public static void NonSerializableExceptionStreamTest()
        {
            var segments = RemoteException.FromException(new NonSerializableException()).Data;

            var msgStream = new SegmentedReadMemoryStream(segments);
            Exception ex;
            var isdeserialized = RemoteException.ToException(msgStream, out ex);
            Assert.True(isdeserialized);
            Assert.True(ex is ServiceException);
        }

        private class NonSerializableException : Exception
        {
        }
    }
}
