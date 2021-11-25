// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.ExceptionConvertors
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Xunit;

    /// <summary>
    /// FabricExceptionConvertor test.
    /// </summary>
    public class FabricExceptionConvertorTest
    {
        private static List<Remoting.V2.Runtime.IExceptionConvertor> runtimeConvertors
            = new List<Remoting.V2.Runtime.IExceptionConvertor>()
            {
                new Remoting.V2.Runtime.FabricExceptionConvertor(),
                new Remoting.V2.Runtime.SystemExceptionConvertor(),
                new Remoting.V2.Runtime.ExceptionConvertorHelper.DefaultExceptionConvetor(),
            };

        private static Remoting.V2.Runtime.ExceptionConvertorHelper runtimeHelper
            = new Remoting.V2.Runtime.ExceptionConvertorHelper(runtimeConvertors, 2);

        private static List<Remoting.V2.Client.IExceptionConvertor> clientConvertors
            = new List<Remoting.V2.Client.IExceptionConvertor>()
            {
                new Remoting.V2.Client.SystemExceptionConvertor(),
                new Remoting.V2.Client.FabricExceptionConvertor(),
            };

        private static Remoting.V2.Client.ExceptionConvertorHelper clientHelper
            = new Remoting.V2.Client.ExceptionConvertorHelper(clientConvertors);

        private static List<FabricException> fabricExceptions = new List<FabricException>()
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

        /// <summary>
        /// Known types test.
        /// </summary>
        [Fact]
        public static void KnownFabricExceptionSerializationTest()
        {
            foreach (var exception in fabricExceptions)
            {
                var serializedData = runtimeHelper.SerializeRemoteException(exception);
                var msgStream = new SegmentedReadMemoryStream(serializedData);

                var isdeserialized = clientHelper.TryDeserializeRemoteException(msgStream, out Exception resultFabricEx);
                Assert.True(isdeserialized);
                Assert.Equal(resultFabricEx.GetType(), exception.GetType());
                Assert.Equal(resultFabricEx.Message, exception.Message);
                Assert.Equal(resultFabricEx.HResult, exception.HResult);
            }
        }

        /// <summary>
        /// Unknown types test.
        /// </summary>
        [Fact]
        public static void UnknownFabricExceptionSerializationTest()
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

            var serializedData = runtimeHelper.SerializeRemoteException(customFabricEx);
            var msgStream = new SegmentedReadMemoryStream(serializedData);

            var isdeserialized = clientHelper.TryDeserializeRemoteException(msgStream, out Exception resultFabricEx);
            Assert.True(isdeserialized);

            Assert.True(resultFabricEx.GetType().Equals(typeof(ServiceException)));
            var resultSvcEx1 = resultFabricEx as ServiceException;
            Assert.Equal(resultSvcEx1.ActualExceptionType, customFabricEx.GetType().ToString());
            Assert.Equal(resultSvcEx1.Message, customFabricEx.Message);
            Assert.Equal(resultSvcEx1.ActualExceptionData["FabricErrorCode"], ((long)customFabricEx.ErrorCode).ToString());
            Assert.Equal(resultSvcEx1.ActualExceptionStackTrace, customFabricEx.StackTrace);
        }

        private static void ThrowFabricException()
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
