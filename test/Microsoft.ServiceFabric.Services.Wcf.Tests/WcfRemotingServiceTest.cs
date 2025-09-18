// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using Fuzzy;
using System.Runtime.Remoting;
using Microsoft.ServiceFabric.Services.Communication;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime.Tests
{
    public abstract class WcfRemotingServiceTest
    {
        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public class RequestResponseAsyncMethod
        {
            readonly string errorMessage;

            public RequestResponseAsyncMethod()
            {
                errorMessage = fuzzy.String();
            }

            [Fact]
            public async Task ReturnsActualExceptionTypeForKnownExceptions()
            {
                // Arrange
                IServiceRemotingMessageHandler mockHandler = Mock.Of<IServiceRemotingMessageHandler>();

                var mockHeaderSerializer = new Mock<IServiceRemotingMessageHeaderSerializer>();
                mockHeaderSerializer.Setup(h => h.DeserializeRequestHeaders(It.IsAny<IMessageHeader>()))
                    .Throws(new InvalidOperationException(errorMessage));

                IEnumerable<V2.Runtime.IExceptionConvertor> runtimeExceptionConvertors = new List<V2.Runtime.IExceptionConvertor>
                {
                    new V2.Runtime.SystemExceptionConvertor(),
                };
                ExceptionSerializer exceptionSerializer = new ExceptionSerializer(runtimeExceptionConvertors, new WcfRemotingListenerSettings());

                var serializersManager = new ServiceRemotingMessageSerializersManager(
                    null, // serializationProvider
                    mockHeaderSerializer.Object,
                    false);

                WcfRemotingService service = new WcfRemotingService(
                    mockHandler,
                    serializersManager,
                    exceptionSerializer);

                // Act
                var faultException = await Assert.ThrowsAsync<FaultException<RemoteException2>>(async () =>
                    await service.RequestResponseAsync(
                        new ArraySegment<byte>(new byte[] { 1, 2, 3 }),
                        new List<ArraySegment<byte>> { new ArraySegment<byte>(new byte[] { 4, 5, 6 }) }));

                // Assert
                IEnumerable<V2.Client.IExceptionConvertor> clientExceptionConvertors = new List<V2.Client.IExceptionConvertor>
                {
                    new V2.Client.SystemExceptionConvertor(),
                };

                // Create an ExceptionDeserializer to inspect the remote exception
                var deserializer = new ExceptionDeserializer(clientExceptionConvertors);

                Exception exception = deserializer.ConvertRemoteException(faultException.Detail);
                Assert.IsType<AggregateException>(exception);

                Exception innerException = ((AggregateException)exception).Flatten().InnerException;
                Assert.IsType<InvalidOperationException>(innerException);
                Assert.Equal(errorMessage, innerException.Message);
            }

            [Fact]
            public async Task ReturnsServiceExceptionForUnknownExceptions()
            {
                // Arrange
                IServiceRemotingMessageHandler mockHandler = Mock.Of<IServiceRemotingMessageHandler>();
                var mockHeaderSerializer = new Mock<IServiceRemotingMessageHeaderSerializer>();
                mockHeaderSerializer.Setup(h => h.DeserializeRequestHeaders(It.IsAny<IMessageHeader>()))
                    .Throws(new ServerException(errorMessage));

                IEnumerable<V2.Runtime.IExceptionConvertor> runtimeExceptionConvertors = new List<V2.Runtime.IExceptionConvertor>
                {
                    new V2.Runtime.SystemExceptionConvertor(),
                    new V2.Runtime.DefaultExceptionConvertor(),
                };
                ExceptionSerializer exceptionSerializer = new ExceptionSerializer(runtimeExceptionConvertors, new WcfRemotingListenerSettings());

                var serializersManager = new ServiceRemotingMessageSerializersManager(
                    null, // serializationProvider
                    mockHeaderSerializer.Object,
                    false);

                WcfRemotingService service = new WcfRemotingService(
                    mockHandler,
                    serializersManager,
                    exceptionSerializer);

                // Act
                var faultException = await Assert.ThrowsAsync<FaultException<RemoteException2>>(async () =>
                    await service.RequestResponseAsync(
                        new ArraySegment<byte>(new byte[] { 1, 2, 3 }),
                        new List<ArraySegment<byte>> { new ArraySegment<byte>(new byte[] { 4, 5, 6 }) }));

                // Assert
                IEnumerable<V2.Client.IExceptionConvertor> clientExceptionConvertors = new List<V2.Client.IExceptionConvertor>
                {
                    new V2.Client.SystemExceptionConvertor(),
                };

                // Create an ExceptionDeserializer to inspect the remote exception
                var deserializer = new ExceptionDeserializer(clientExceptionConvertors);

                Exception exception = deserializer.ConvertRemoteException(faultException.Detail);
                Assert.IsType<AggregateException>(exception);

                Exception innerException = ((AggregateException)exception).Flatten().InnerException;
                Assert.IsType<ServiceException>(innerException);
                Assert.Equal(errorMessage, innerException.Message);
            }
        }
    }
}
