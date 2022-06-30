// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System.Fabric;
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
                        tempResult = new ExceptionHandlingRetryResult(fabricEx, false, retrySettings, retrySettings.DefaultMaxRetryCountForNonTransientErrors);
                        handled = true;
                        break;
                    default:
                        handled = false;
                        break;
                }
            }

            // TODO: Handle HTTP channel exceptions for retry
            result = tempResult;

            return handled;
        }
    }
}
