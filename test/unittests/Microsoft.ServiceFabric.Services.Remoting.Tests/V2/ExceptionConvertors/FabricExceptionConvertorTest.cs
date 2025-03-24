// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.ExceptionConvertors
{
    public class FabricExceptionConvertorTest
    {
        static readonly IEnumerable<IExceptionConvertor> runtimeConvertors = new IExceptionConvertor[]
            {
                new FabricExceptionConvertor(),
                new SystemExceptionConvertor(),
                new DefaultExceptionConvertor(),
            };

        static readonly ExceptionSerializer runtimeHandler = new ExceptionSerializer(
            runtimeConvertors, new FabricTransportRemotingListenerSettings { RemotingExceptionDepth = 2 });

        static readonly IEnumerable<Remoting.V2.Client.IExceptionConvertor> clientConvertors = new Remoting.V2.Client.IExceptionConvertor[]
            {
                new Remoting.V2.Client.SystemExceptionConvertor(),
                new Remoting.V2.Client.FabricExceptionConvertor(),
            };

        static readonly Remoting.V2.Client.ExceptionDeserializer clientHandler = new Remoting.V2.Client.ExceptionDeserializer(clientConvertors);

        static readonly IEnumerable<FabricException> fabricExceptions = new FabricException[]
        {
            new FabricException("FabricException"),
            new FabricInvalidPartitionKeyException("FabricInvalidPartitionKeyException"),
            new FabricElementAlreadyExistsException("FabricElementAlreadyExistsException"),
            new FabricElementNotFoundException("FabricElementNotFoundException"),
            new FabricNotPrimaryException("FabricNotPrimaryException"),
            new FabricTransientException("FabricTransientException"),
            new FabricObjectClosedException("FabricObjectClosedException"),
            new FabricConnectionDeniedException("FabricConnectionDeniedException"),
            new FabricServerAuthenticationFailedException("FabricServerAuthenticationFailedException"),
            new FabricInvalidAddressException("FabricInvalidAddressException"),
            new FabricInvalidAtomicGroupException("FabricInvalidAtomicGroupException"),
            new FabricMissingFullBackupException("FabricMissingFullBackupException"),
            new FabricNotReadableException("FabricNotReadableException"),
            new FabricImageStoreException("FabricImageStoreException"),
            new FabricBackupInProgressException("FabricBackupInProgressException"),
            new FabricBackupDirectoryNotEmptyException("FabricBackupDirectoryNotEmptyException"),
            new FabricReplicationOperationTooLargeException("FabricReplicationOperationTooLargeException"),
            new FabricServiceNotFoundException("FabricServiceNotFoundException"),
            new FabricCannotConnectException("FabricCannotConnectException"),
            new FabricMessageTooLargeException("FabricMessageTooLargeException"),
            new FabricEndpointNotFoundException("FabricEndpointNotFoundException"),
            new FabricDeleteBackupFileFailedException("FabricDeleteBackupFileFailedException"),
            new FabricInvalidTestCommandStateException("FabricInvalidTestCommandStateException"),
            new FabricTestCommandOperationIdAlreadyExistsException("FabricTestCommandOperationIdAlreadyExistsException"),
            new FabricChaosAlreadyRunningException("FabricChaosAlreadyRunningException"),
            new FabricChaosEngineException("FabricChaosEngineException"),
            new FabricRestoreSafeCheckFailedException("FabricRestoreSafeCheckFailedException"),
            new FabricInvalidPartitionSelectorException("FabricInvalidPartitionSelectorException"),
            new FabricInvalidReplicaSelectorException("FabricInvalidReplicaSelectorException"),
            new FabricInvalidForStatefulServicesException("FabricInvalidForStatefulServicesException"),
            new FabricInvalidForStatelessServicesException("FabricInvalidForStatelessServicesException"),
            new FabricOnlyValidForStatefulPersistentServicesException("FabricOnlyValidForStatefulPersistentServicesException"),
            new FabricPeriodicBackupNotEnabledException("FabricPeriodicBackupNotEnabledException"),
            new FabricValidationException("FabricValidationException"),
            new FabricTransportCallbackNotFoundException("FabricTransportCallbackNotFoundException"),
        };

        [Fact]
        public static async Task KnownFabricExceptionSerializationTest()
        {
            foreach (FabricException exception in fabricExceptions)
            {
                List<ArraySegment<byte>> serializedData = runtimeHandler.SerializeRemoteException(exception);
                using var msgStream = new SegmentedReadMemoryStream(serializedData);

                Exception resultFabricEx = null;
                try
                {
                    await clientHandler.DeserializeRemoteExceptionAndThrowAsync(msgStream);
                }
                catch (AggregateException ex)
                {
                    resultFabricEx = ex.InnerException;
                }

                Assert.True(resultFabricEx != null);
                Assert.Equal(resultFabricEx.GetType(), exception.GetType());
                Assert.Equal(resultFabricEx.Message, exception.Message);
                Assert.Equal(resultFabricEx.HResult, exception.HResult);
            }
        }

        [Fact]
        public static async Task UnknownFabricExceptionSerializationTest()
        {
            FabricException customFabricEx = null;
            try
            {
                ThrowFabricException();
            }
            catch (FabricException ex)
            {
                customFabricEx = ex;
            }

            List<ArraySegment<byte>> serializedData = runtimeHandler.SerializeRemoteException(customFabricEx);
            using var msgStream = new SegmentedReadMemoryStream(serializedData);

            Exception resultFabricEx = null;
            try
            {
                await clientHandler.DeserializeRemoteExceptionAndThrowAsync(msgStream);
            }
            catch (AggregateException ex)
            {
                resultFabricEx = ex.InnerException;
            }

            Assert.True(resultFabricEx != null);

            Assert.True(resultFabricEx.GetType().Equals(typeof(ServiceException)));
            var resultSvcEx1 = resultFabricEx as ServiceException;
            Assert.Equal(resultSvcEx1.ActualExceptionType, customFabricEx.GetType().ToString());
            Assert.Equal(resultSvcEx1.Message, customFabricEx.Message);
            Assert.Equal(resultSvcEx1.ActualExceptionData["FabricErrorCode"], ((long)customFabricEx.ErrorCode).ToString());
            Assert.Equal(resultSvcEx1.ActualExceptionStackTrace, customFabricEx.StackTrace);
        }

        static void ThrowFabricException()
        {
            throw new MyFabricException("Thrown from ThrowFabricException");
        }

        internal class MyFabricException : FabricException
        {
            public MyFabricException(string msg)
                : base(msg)
            {
            }
        }
    }
}
