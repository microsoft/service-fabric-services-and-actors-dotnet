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
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Wcf;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime.Tests
{
    public abstract class WcfRemotingServiceTest
    {
        // Protected/internal properties for derived classes
        protected Mock<IServiceRemotingMessageHandler> mockMessageHandler;
        protected WcfRemotingListenerSettings listenerSettings;
        internal ServiceRemotingMessageSerializersManager serializersManager;
        internal WcfRemotingService wcfRemotingService;

        public WcfRemotingServiceTest()
        {
            // Initialize mocks
            mockMessageHandler = new Mock<IServiceRemotingMessageHandler>();
            listenerSettings = new WcfRemotingListenerSettings();

            // Create real serializers manager and exception handler (since interfaces are internal)
            serializersManager = new ServiceRemotingMessageSerializersManager(
                new ServiceRemotingDataContractSerializationProvider(),
                null,
                false);

            var exceptionConversionHandler = ExceptionConversionHandler.CreateDefault(
                new List<IExceptionConvertor>(),
                listenerSettings);

            // Create the service under test
            wcfRemotingService = new WcfRemotingService(
                mockMessageHandler.Object,
                serializersManager,
                exceptionConversionHandler,
                listenerSettings);
        }

        public class Constructor : WcfRemotingServiceTest
        {
            [Fact]
            public void WithValidParameters_CreatesInstance()
            {
                // Arrange & Act
                var service = new WcfRemotingService(
                    mockMessageHandler.Object,
                    serializersManager,
                    ExceptionConversionHandler.CreateDefault(
                        new List<IExceptionConvertor>(),
                        listenerSettings),
                    listenerSettings);

                // Assert
                Assert.NotNull(service);
                Assert.IsAssignableFrom<IServiceRemotingContract>(service);
            }
        }

        public class OneWayMessage : WcfRemotingServiceTest
        {
            [Fact]
            public void ThrowsNotImplementedException()
            {
                // Arrange
                var testMessageHeaders = new ArraySegment<byte>(new byte[] { 1, 2, 3 });
                var testRequestBody = new List<ArraySegment<byte>> { new ArraySegment<byte>(new byte[] { 4, 5, 6 }) };

                // Act & Assert
                Assert.Throws<NotImplementedException>(() => 
                    wcfRemotingService.OneWayMessage(testMessageHeaders, testRequestBody));
            }
        }

        public class RequestResponseAsync : WcfRemotingServiceTest
        {
            [Fact]
            public async Task WhenExceptionThrown_ThrowsFaultException()
            {
                // Arrange - Create a message header with an invalid interface ID that will cause an exception
                var headerSerializer = serializersManager.GetHeaderSerializer();
                var invalidHeader = new ServiceRemotingRequestMessageHeader();
                invalidHeader.MethodId = 1;
                invalidHeader.InterfaceId = 9999; // Invalid interface ID that will cause an exception
                
                var serializedHeader = headerSerializer.SerializeRequestHeader(invalidHeader);
                var headerBytes = serializedHeader.GetSendBuffer();

                // Create simple request body
                var requestBody = new List<ArraySegment<byte>> { new ArraySegment<byte>(new byte[0]) };

                // Act & Assert
                var faultException = await Assert.ThrowsAsync<FaultException<RemoteException2>>(
                    () => wcfRemotingService.RequestResponseAsync(headerBytes, requestBody));

                // Verify the fault exception is properly wrapped
                Assert.NotNull(faultException.Detail);
                Assert.Contains("No interface found with this Id", faultException.Detail.Message);
                Assert.NotNull(faultException.Detail.Type);
            }

            [Fact]
            public async Task WithBinaryFormatterSerialization_ThrowsRemoteExceptionFault()
            {
                // Arrange - Create listener settings that use BinaryFormatter for exception serialization
#pragma warning disable 618
                var binaryFormatterSettings = new WcfRemotingListenerSettings()
                {
                    ExceptionSerializationTechnique = FabricTransportRemotingListenerSettings.ExceptionSerialization.BinaryFormatter
                };
#pragma warning restore 618

                var binaryFormatterExceptionHandler = ExceptionConversionHandler.CreateDefault(
                    new List<IExceptionConvertor>(),
                    binaryFormatterSettings);

                // Create a service instance with BinaryFormatter settings
                var serviceWithBinaryFormatter = new WcfRemotingService(
                    mockMessageHandler.Object,
                    serializersManager,
                    binaryFormatterExceptionHandler,
                    binaryFormatterSettings);

                // Create a message header with an invalid interface ID that will cause an exception
                var headerSerializer = serializersManager.GetHeaderSerializer();
                var invalidHeader = new ServiceRemotingRequestMessageHeader();
                invalidHeader.MethodId = 1;
                invalidHeader.InterfaceId = 8888; // Invalid interface ID that will cause an exception
                
                var serializedHeader = headerSerializer.SerializeRequestHeader(invalidHeader);
                var headerBytes = serializedHeader.GetSendBuffer();

                // Create simple request body
                var requestBody = new List<ArraySegment<byte>> { new ArraySegment<byte>(new byte[0]) };

                // Act & Assert
                var faultException = await Assert.ThrowsAsync<FaultException<RemoteException>>(
                    () => serviceWithBinaryFormatter.RequestResponseAsync(headerBytes, requestBody));

                // Verify the fault exception is properly wrapped with RemoteException (not RemoteException2)
                Assert.NotNull(faultException.Detail);
                Assert.NotNull(faultException.Detail.Data);
                Assert.True(faultException.Detail.Data.Count > 0);
                // The exception message is accessible via the FaultException.Message
                Assert.Contains("No interface found with this Id", faultException.Message);
            }
        }
    }
}