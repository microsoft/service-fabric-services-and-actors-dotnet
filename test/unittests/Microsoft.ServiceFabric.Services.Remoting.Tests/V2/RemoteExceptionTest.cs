// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
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
        public static async Task SerializableExceptionStreamTest()
        {
            IEnumerable<Remoting.V2.Runtime.IExceptionConvertor> runtimeConvertors = new Remoting.V2.Runtime.IExceptionConvertor[]
            {
                new Remoting.V2.Runtime.SystemExceptionConvertor(),
            };

            IEnumerable<Remoting.V2.Client.IExceptionConvertor> clientConvertors = new Remoting.V2.Client.IExceptionConvertor[]
            {
                new Remoting.V2.Client.SystemExceptionConvertor(),
            };

            var exceptionSerializer = new ExceptionSerializer(runtimeConvertors, null);
            var exceptionDeserializer = new ExceptionDeserializer(clientConvertors);

            Exception ex = new InvalidOperationException("Testing");

            var segments = exceptionSerializer.SerializeRemoteException(ex);
            var msgStream = new SegmentedReadMemoryStream(segments);

            try
            {
                await exceptionDeserializer.DeserializeRemoteExceptionAndThrowAsync(msgStream);
                Assert.Fail("Expected exception not thrown.");
            }
            catch (Exception e)
            {
                Assert.True(e is AggregateException);
                Assert.True(e.InnerException is InvalidOperationException, "InnerException is not of type InvalidOperationException");
                Assert.Equal("Testing", e.InnerException.Message);
            }
        }

        /// <summary>
        /// NonSerializableExceptionStream Test
        /// </summary>
        [Fact]
        public static async Task NonSerializableExceptionStreamTest()
        {
            IEnumerable<Remoting.V2.Runtime.IExceptionConvertor> runtimeConvertors = new Remoting.V2.Runtime.IExceptionConvertor[]
            {
                            new Remoting.V2.Runtime.SystemExceptionConvertor(),
                            new DefaultExceptionConvertor(),
            };

            IEnumerable<Remoting.V2.Client.IExceptionConvertor> clientConvertors = new Remoting.V2.Client.IExceptionConvertor[]
            {
                new Remoting.V2.Client.SystemExceptionConvertor(),
            };

            var exceptionSerializer = new ExceptionSerializer(runtimeConvertors, new FabricTransportRemotingListenerSettings() { RemotingExceptionDepth = 1 });
            var exceptionDeserializer = new ExceptionDeserializer(clientConvertors);

            Exception ex = new FabricServiceNotFoundException("Testing");

            var segments = exceptionSerializer.SerializeRemoteException(ex);
            var msgStream = new SegmentedReadMemoryStream(segments);

            try
            {
                await exceptionDeserializer.DeserializeRemoteExceptionAndThrowAsync(msgStream);
                Assert.Fail("Expected exception not thrown.");
            }
            catch (Exception e)
            {
                
                Assert.True(e is AggregateException);
                Assert.True(e.InnerException is ServiceException, "InnerException is not of type ServiceException");
                Assert.Equal("Testing", e.InnerException.Message);
            }
        }
    }
}
