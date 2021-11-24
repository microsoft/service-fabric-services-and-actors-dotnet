// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.ExceptionConvertors
{
    using System.Collections.Generic;
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Xunit;

    /// <summary>
    /// FabricExceptionConvertor test.
    /// </summary>
    public class FabricExceptionConvertorTest
    {
        private static List<Microsoft.ServiceFabric.Services.Remoting.V2.Runtime.IExceptionConvertor> runtimeConvertors
            = new List<Microsoft.ServiceFabric.Services.Remoting.V2.Runtime.IExceptionConvertor>() { new Microsoft.ServiceFabric.Services.Remoting.V2.Runtime.FabricExceptionConvertor() };

        private static Microsoft.ServiceFabric.Services.Remoting.V2.Runtime.ExceptionConvertorHelper runtimeHelper
            = new Microsoft.ServiceFabric.Services.Remoting.V2.Runtime.ExceptionConvertorHelper(runtimeConvertors);

        private static List<Microsoft.ServiceFabric.Services.Remoting.V2.Client.IExceptionConvertor> clientConvertors
            = new List<Microsoft.ServiceFabric.Services.Remoting.V2.Client.IExceptionConvertor>() { new Microsoft.ServiceFabric.Services.Remoting.V2.Client.FabricExceptionConvertor() };

        private static Microsoft.ServiceFabric.Services.Remoting.V2.Client.ExceptionConvertorHelper clientHelper
            = new Microsoft.ServiceFabric.Services.Remoting.V2.Client.ExceptionConvertorHelper(clientConvertors);

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
        public static void FabricExceptionSerializationTest()
        {
            foreach (var exception in fabricExceptions)
            {
                var svcEx = runtimeHelper.ToServiceException(exception);
                var remoteEx = runtimeHelper.ToRemoteException(svcEx);
                var remoteExWrapper = new RemoteException2Wrapper()
                {
                    Exceptions = new List<RemoteException2>() { remoteEx },
                };

                var serializedData = runtimeHelper.SerializeRemoteException(remoteExWrapper);
                var msgStream = new SegmentedReadMemoryStream(serializedData);

                var isdeserialzied = clientHelper.TryDeserializeRemoteException(msgStream, out var resultWrapper);
                Assert.True(isdeserialzied);
                var resultSvcEx = clientHelper.FromRemoteException(resultWrapper.Exceptions[0]);
                var resultFabricEx = clientHelper.FromServiceException(resultSvcEx);

                Assert.Equal(resultFabricEx.GetType(), exception.GetType());
                Assert.Equal(resultFabricEx.Message, exception.Message);
                Assert.Equal(resultFabricEx.HResult, exception.HResult);
            }
        }
    }
}
