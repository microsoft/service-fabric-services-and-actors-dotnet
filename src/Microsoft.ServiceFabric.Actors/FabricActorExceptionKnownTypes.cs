// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Actors.Migration.Exceptions;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Communication;

namespace Microsoft.ServiceFabric.Actors
{
    internal class FabricActorExceptionKnownTypes
    {
#pragma warning disable SA1401 // Fields should be private
        public static IDictionary<string, ConvertorFuncs> ServiceExceptionConvertors =
#pragma warning restore SA1401 // Fields should be private
            new Dictionary<string, ConvertorFuncs>()
            {
                {
                    "Microsoft.ServiceFabric.Actors.Runtime.DuplicateMessageException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<DuplicateMessageException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "Microsoft.ServiceFabric.Actors.InvalidReentrantCallException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<InvalidReentrantCallException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "Microsoft.ServiceFabric.Actors.ReminderNotFoundException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ReminderNotFoundException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "Microsoft.ServiceFabric.Actors.ReentrancyModeDisallowedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ReentrancyModeDisallowedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "Microsoft.ServiceFabric.Actors.ReentrantActorInvalidStateException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ReentrantActorInvalidStateException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "Microsoft.ServiceFabric.Actors.ActorConcurrencyLockTimeoutException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ActorConcurrencyLockTimeoutException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "Microsoft.ServiceFabric.Actors.ActorDeletedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ActorDeletedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "Microsoft.ServiceFabric.Actors.ReminderLoadInProgressException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ReminderLoadInProgressException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "Microsoft.ServiceFabric.Actors.Migration.Exceptions.ActorCallsDisallowedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ActorCallsDisallowedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "Microsoft.ServiceFabric.Actors.Migration.Exceptions.InvalidMigrationConfigException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<InvalidMigrationConfigException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "Microsoft.ServiceFabric.Actors.Migration.Exceptions.InvalidMigrationStateProviderException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<InvalidMigrationStateProviderException>(svcEx, innerEx),
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
            var firstInnerEx = innerExceptions == null || innerExceptions.Length == 0 ? null : innerExceptions[0];
            T originalEx = (T)Activator.CreateInstance(typeof(T), new object[] { serviceException.Message, firstInnerEx });

            // HResult property setter is public only starting netcore 3.0
            originalEx.Data.Add("RemoteHResult", serviceException.ActualExceptionData["HResult"]);
            originalEx.Data.Add("RemoteFabricErrorCode", (FabricErrorCode)long.Parse(serviceException.ActualExceptionData["HResult"]));
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
