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

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Client
{
    public class WcfServiceRemotingClientTest
    {
        [Fact]
        public async Task GivenSupportedExceptionType_WhenReceived_IsRecognized()
        {
            // Create client and runtime exception convertors.
            IEnumerable<V2.Client.IExceptionConvertor> clientExceptionConvertors = null;

            IEnumerable<V2.Runtime.IExceptionConvertor> runtimeExceptionConvertors = new List<IExceptionConvertor>
            {
                new CustomConvertorRuntime(),
                new SystemExceptionConvertor(),
            };

            ExceptionSerializer exceptionSerializer = new ExceptionSerializer(
                runtimeExceptionConvertors,
                null
            );

            // Create RemoteException and FaultException
            RemoteException2 systemRemoteException = exceptionSerializer.BuildRemoteException(new NotImplementedException("foo exception"));

            FaultException<RemoteException2> faultException = new FaultException<RemoteException2>(systemRemoteException);

            WcfServiceRemotingClient client = new WcfServiceRemotingClient(
                null, // wcfClient,
                null, // serializersManager
                clientExceptionConvertors
            );

            IServiceRemotingRequestMessage requestMessageMock = Mock.Of<IServiceRemotingRequestMessage>();

            Mock.Get(requestMessageMock)
                .Setup(m => m.GetHeader()) // We inject exception here for convenience (ideally, it should be in inner RequestResponseAsync call).
                .Throws(faultException);

            AggregateException exception = await Assert.ThrowsAsync<AggregateException>(() => client.RequestResponseAsync(requestMessageMock));
            Exception innerException = exception.Flatten().InnerException;
            Assert.IsType<NotImplementedException>(innerException);
            Assert.Equal("foo exception", innerException.Message);
        }

        [Fact]
        public async Task GivenSupportedExceptionType_WhenReceived_FallbacksToServiceException()
        {
            // Create client and runtime exception convertors.
            IEnumerable<V2.Client.IExceptionConvertor> clientExceptionConvertors = null;

            IEnumerable<V2.Runtime.IExceptionConvertor> runtimeExceptionConvertors = new List<IExceptionConvertor>
            {
                new CustomConvertorRuntime(),
            };

            ExceptionSerializer exceptionSerializer = new ExceptionSerializer(
                runtimeExceptionConvertors,
                null
            );
                
            // Create RemoteException and FaultException
            RemoteException2 customRemoteException = exceptionSerializer.BuildRemoteException(new CustomException("CustomEx", "CustomField1", "CustomField2"));

            FaultException<RemoteException2> faultException = new FaultException<RemoteException2>(customRemoteException);

            WcfServiceRemotingClient client = new WcfServiceRemotingClient(
                null, // wcfClient,
                null, // serializersManager
                clientExceptionConvertors
            );

            IServiceRemotingRequestMessage requestMessageMock = Mock.Of<IServiceRemotingRequestMessage>();

            Mock.Get(requestMessageMock)
                .Setup(m => m.GetHeader()) // We inject exception here for convenience (ideally, it should be in inner RequestResponseAsync call).
                .Throws(faultException);

            AggregateException exception = await Assert.ThrowsAsync<AggregateException>(() => client.RequestResponseAsync(requestMessageMock));
            Exception innerException = exception.Flatten().InnerException;
            Assert.IsType<ServiceException>(innerException);
            Assert.Equal("CustomEx", innerException.Message);
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
