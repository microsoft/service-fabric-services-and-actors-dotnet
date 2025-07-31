// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Moq;
using Xunit;
using System;
using System.ServiceModel;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Services.Communication;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using Fuzzy;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Client
{
    public abstract class WcfServiceRemotingClientTest
    {
        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);
        
        readonly string errorMessage;

        readonly WcfServiceRemotingClient sut;

        public WcfServiceRemotingClientTest()
        {
            errorMessage = fuzzy.String();

            // Create client exception convertors.
            IEnumerable<V2.Client.IExceptionConvertor> clientExceptionConvertors = new List<V2.Client.IExceptionConvertor>
                {
                    new V2.Client.SystemExceptionConvertor(),
                };

            ExceptionDeserializer exceptionDeserializer = ExceptionDeserializer.CreateDefault(
                clientExceptionConvertors);

            sut = new WcfServiceRemotingClient(
                    null, // wcfClient,
                    null, // serializersManager
                    exceptionDeserializer
                );
        }

        public sealed class RequestResnposeAsync : WcfServiceRemotingClientTest
        {
            [Fact]
            public async Task ThrowsActualExceptionForKnownExceptions()
            {
                // Arrange
                IEnumerable<V2.Runtime.IExceptionConvertor> runtimeExceptionConvertors = new List<V2.Runtime.IExceptionConvertor>
                {
                    new CustomConvertorRuntime(),
                    new V2.Runtime.SystemExceptionConvertor(),
                };

                ExceptionSerializer exceptionSerializer = new ExceptionSerializer(
                    runtimeExceptionConvertors,
                    null
                );

                // Create RemoteException and FaultException
                RemoteException2 systemRemoteException = exceptionSerializer.BuildRemoteException(new NotImplementedException(errorMessage));

                var faultException = new FaultException<RemoteException2>(systemRemoteException);

                IServiceRemotingRequestMessage requestMessageMock = Mock.Of<IServiceRemotingRequestMessage>();

                Mock.Get(requestMessageMock)
                    .Setup(m => m.GetHeader()) // We inject exception here for convenience (ideally, it should be in inner RequestResponseAsync call).
                    .Throws(faultException);

                // Act & Assert
                // Assert that the exception is deserialized correctly and is of type NotImplementedException.
                AggregateException exception = await Assert.ThrowsAsync<AggregateException>(() => sut.RequestResponseAsync(requestMessageMock));
                Exception innerException = exception.Flatten().InnerException;
                Assert.IsType<NotImplementedException>(innerException);
                Assert.Equal(errorMessage, innerException.Message);
            }

            [Fact]
            public async Task ThrowsServiceExceptionForUnknownExceptions()
            {
                // Create runtime exception convertors.
                IEnumerable<V2.Runtime.IExceptionConvertor> runtimeExceptionConvertors = new List<V2.Runtime.IExceptionConvertor>
                    {
                        new CustomConvertorRuntime(),
                    };

                ExceptionSerializer exceptionSerializer = new ExceptionSerializer(
                    runtimeExceptionConvertors,
                    null
                );

                // Create RemoteException and FaultException
                RemoteException2 customRemoteException = exceptionSerializer.BuildRemoteException(new CustomException(errorMessage, "CustomField1", "CustomField2"));

                var faultException = new FaultException<RemoteException2>(customRemoteException);

                IServiceRemotingRequestMessage requestMessageMock = Mock.Of<IServiceRemotingRequestMessage>();

                Mock.Get(requestMessageMock)
                    .Setup(m => m.GetHeader()) // We inject exception here for convenience (ideally, it should be in inner RequestResponseAsync call).
                    .Throws(faultException);

                // Act & Assert
                // Assert that the unknown exception is converted to ServiceException during deserialization.
                AggregateException exception = await Assert.ThrowsAsync<AggregateException>(() => sut.RequestResponseAsync(requestMessageMock));
                Exception innerException = exception.Flatten().InnerException;
                Assert.IsType<ServiceException>(innerException);
                Assert.Equal(errorMessage, innerException.Message);
            }
        }

        internal class CustomConvertorRuntime : ExceptionConvertorBase
        {
            public override bool TryConvertToServiceException(Exception originalException, out ServiceException serviceException)
            {
                serviceException = null;
                if (originalException is CustomException customEx)
                {
                    serviceException = new ServiceException(customEx.GetType().FullName, customEx.Message);
                    serviceException.ActualExceptionStackTrace = originalException.StackTrace;
                    serviceException.ActualExceptionData = new Dictionary<string, string>()
                {
                    { "Field1", customEx.Field1 },
                    { "Field2", customEx.Field2 },
                };

                    return true;
                }

                return false;
            }
        }

        internal class CustomException : Exception
        {
            public CustomException(string message, string field1, string field2)
                : base(message)
            {
                this.Field1 = field1;
                this.Field2 = field2;
            }

            public string Field1 { get; set; }

            public string Field2 { get; set; }
        }
    }
}
