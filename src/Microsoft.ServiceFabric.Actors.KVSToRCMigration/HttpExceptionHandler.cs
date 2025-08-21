// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System.Fabric;
    using System.Net.Http;
    using System.Net.Sockets;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal class HttpExceptionHandler : Services.Communication.Client.IExceptionHandler
    {
        public bool TryHandleException(ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, out ExceptionHandlingResult result)
        {
            ExceptionHandlingResult tempResult = new ExceptionHandlingThrowResult
            {
                ExceptionToThrow = exceptionInformation.Exception,
            };

            bool handled = false;
            if (exceptionInformation.Exception is FabricException)
            {
                var fabricEx = exceptionInformation.Exception as FabricException;
                switch (fabricEx.ErrorCode)
                {
                    case FabricErrorCode.NotPrimary:
                    case FabricErrorCode.NotReadable:
                    case FabricErrorCode.FabricEndpointNotFound:
                        tempResult = new ExceptionHandlingRetryResult(fabricEx, false, retrySettings, retrySettings.DefaultMaxRetryCountForTransientErrors);
                        handled = true;
                        break;
                    default:
                        handled = false;
                        break;
                }

                if (fabricEx is FabricTransientException
                    || (fabricEx.Data.Contains("ActualExceptionType") && (string)fabricEx.Data["ActualExceptionType"] == typeof(FabricTransientException).FullName))
                {
                    switch (fabricEx.ErrorCode)
                    {
                        case FabricErrorCode.DatabaseMigrationInProgress:
                            handled = true;
                            new ExceptionHandlingThrowResult { ExceptionToThrow = fabricEx };
                            break;
                        default:
                            new ExceptionHandlingRetryResult(fabricEx, true, retrySettings, retrySettings.DefaultMaxRetryCountForNonTransientErrors);
                            handled = true;
                            break;
                    }
                }
            }
            else if (exceptionInformation.Exception is HttpRequestException)
            {
                if (exceptionInformation.Exception.InnerException != null && exceptionInformation.Exception.InnerException is SocketException)
                {
                    if (((SocketException)exceptionInformation.Exception.InnerException).SocketErrorCode == SocketError.ConnectionRefused)
                    {
                        tempResult = new ExceptionHandlingRetryResult(exceptionInformation.Exception, false, retrySettings, retrySettings.DefaultMaxRetryCountForNonTransientErrors);
                        handled = true;
                    }
                }
            }

            result = tempResult;
            return handled;
        }
    }
}
