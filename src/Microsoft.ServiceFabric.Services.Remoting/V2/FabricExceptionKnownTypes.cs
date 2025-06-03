// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    internal class FabricExceptionKnownTypes
    {
#pragma warning disable SA1401 // Fields should be private
        public static IDictionary<string, ConvertorFuncs> ServiceExceptionConvertors =
#pragma warning restore SA1401 // Fields should be private
            new Dictionary<string, ConvertorFuncs>()
            {
                {
                    "System.Fabric.FabricException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricInsufficientMaxLoadCapacityException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricInsufficientMaxLoadCapacityException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricInvalidPartitionKeyException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricInvalidPartitionKeyException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricElementAlreadyExistsException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricElementAlreadyExistsException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricElementNotFoundException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricElementNotFoundException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricNotPrimaryException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricNotPrimaryException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricTransientException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricTransientException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricObjectClosedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricObjectClosedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricConnectionDeniedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricConnectionDeniedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricServerAuthenticationFailedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricServerAuthenticationFailedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricSkipRestoreOperationException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricSkipRestoreOperationException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricInvalidAddressException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricInvalidAddressException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricInvalidAtomicGroupException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricInvalidAtomicGroupException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricMissingFullBackupException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricMissingFullBackupException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricNotReadableException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricNotReadableException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricImageStoreException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricImageStoreException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricBackupInProgressException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricBackupInProgressException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricBackupDirectoryNotEmptyException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricBackupDirectoryNotEmptyException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricBackupNotFoundException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricBackupNotFoundException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricReplicationOperationTooLargeException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricReplicationOperationTooLargeException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricServiceNotFoundException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricServiceNotFoundException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricCannotConnectException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricCannotConnectException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricMessageTooLargeException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricMessageTooLargeException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricEndpointNotFoundException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricEndpointNotFoundException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricDeleteBackupFileFailedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricDeleteBackupFileFailedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricInvalidTestCommandStateException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricInvalidTestCommandStateException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricTestCommandOperationIdAlreadyExistsException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricTestCommandOperationIdAlreadyExistsException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricChaosAlreadyRunningException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricChaosAlreadyRunningException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricChaosEngineException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricChaosEngineException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricRestoreSafeCheckFailedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricRestoreSafeCheckFailedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricInvalidPartitionSelectorException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricInvalidPartitionSelectorException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricInvalidReplicaSelectorException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricInvalidReplicaSelectorException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricInvalidForStatefulServicesException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricInvalidForStatefulServicesException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricInvalidForStatelessServicesException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricInvalidForStatelessServicesException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricOnlyValidForStatefulPersistentServicesException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricOnlyValidForStatefulPersistentServicesException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricPeriodicBackupNotEnabledException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricPeriodicBackupNotEnabledException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Fabric.FabricValidationException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricValidationException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime.FabricTransportCallbackNotFoundException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FabricTransportCallbackNotFoundException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
            };

        private static ServiceException ToServiceException(FabricException fabricEx)
        {
            var serviceException = new ServiceException(fabricEx.GetType().FullName, fabricEx.Message);
            serviceException.ActualExceptionStackTrace = fabricEx.StackTrace;
            serviceException.ActualExceptionData = new Dictionary<string, string>()
            {
                { "HResult", fabricEx.HResult.ToString() },
                { "FabricErrorCode", ((long)fabricEx.ErrorCode).ToString() },
            };

            return serviceException;
        }

        private static T FromServiceException<T>(ServiceException serviceException, params Exception[] innerExceptions)
            where T : FabricException
        {
            var args = new List<object>();
            var firstInnerEx = innerExceptions == null || innerExceptions.Length == 0 ? null : innerExceptions[0];
            if (typeof(T) == typeof(FabricTransportCallbackNotFoundException))
            {
                args.Add(serviceException.Message);
            }
            else if (typeof(T) == typeof(FabricInsufficientMaxLoadCapacityException)
                || typeof(T) == typeof(FabricMissingFullBackupException)
                || typeof(T) == typeof(FabricNotReadableException)
                || typeof(T) == typeof(FabricBackupInProgressException)
                || typeof(T) == typeof(FabricBackupDirectoryNotEmptyException)
                || typeof(T) == typeof(FabricBackupNotFoundException)
                || typeof(T) == typeof(FabricReplicationOperationTooLargeException)
                || typeof(T) == typeof(FabricServiceNotFoundException)
                || typeof(T) == typeof(FabricServerAuthenticationFailedException)
                || typeof(T) == typeof(FabricMessageTooLargeException)
                || typeof(T) == typeof(FabricEndpointNotFoundException)
                || typeof(T) == typeof(FabricDeleteBackupFileFailedException)
                || typeof(T) == typeof(FabricInvalidTestCommandStateException)
                || typeof(T) == typeof(FabricTestCommandOperationIdAlreadyExistsException)
                || typeof(T) == typeof(FabricChaosAlreadyRunningException)
                || typeof(T) == typeof(FabricChaosEngineException)
                || typeof(T) == typeof(FabricRestoreSafeCheckFailedException)
                || typeof(T) == typeof(FabricPeriodicBackupNotEnabledException))
            {
                args.Add(serviceException.Message);
                args.Add(firstInnerEx);
            }
            else if (typeof(T) == typeof(FabricInvalidPartitionSelectorException)
                || typeof(T) == typeof(FabricInvalidReplicaSelectorException))
            {
                args.Add(serviceException.Message);
                if (firstInnerEx != null)
                {
                    args.Add(firstInnerEx);
                }
                else
                {
                    args.Add((FabricErrorCode)long.Parse(serviceException.ActualExceptionData["FabricErrorCode"]));
                }
            }
            else
            {
                args.Add(serviceException.Message);
                args.Add(firstInnerEx);
                args.Add((FabricErrorCode)long.Parse(serviceException.ActualExceptionData["FabricErrorCode"]));
            }

            T originalEx = (T)Activator.CreateInstance(typeof(T), args.ToArray());

            // HResult property setter is public only starting netcore 3.0
            originalEx.Data.Add("RemoteHResult", serviceException.ActualExceptionData["HResult"]);
            originalEx.Data.Add("RemoteFabricErrorCode", serviceException.ActualExceptionData["FabricErrorCode"]);
            originalEx.Data.Add("RemoteStackTrace", serviceException.ActualExceptionStackTrace);

            return originalEx;
        }

        private static Exception[] GetInnerExceptions(Exception exception)
        {
            return exception.InnerException != null ? new Exception[] { exception.InnerException } : null;
        }

        internal class ConvertorFuncs
        {
            public Func<FabricException, ServiceException> ToServiceExFunc { get; set; }

            public Func<ServiceException, Exception[], FabricException> FromServiceExFunc { get; set; }

            public Func<FabricException, Exception[]> InnerExFunc { get; set; }
        }
    }
}
