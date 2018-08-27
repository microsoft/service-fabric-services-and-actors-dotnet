// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.FabricTransport
{
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal class ExceptionHandler : IExceptionHandler
    {
        bool IExceptionHandler.TryHandleException(
            ExceptionInformation exceptionInformation,
            OperationRetrySettings retrySettings,
            out ExceptionHandlingResult result)
        {
            if (exceptionInformation.Exception is FabricException fabricException)
            {
                return TryHandleFabricException(
                    fabricException,
                    retrySettings,
                    out result);
            }

            result = null;
            return false;
        }

        private static bool TryHandleFabricException(
            FabricException fabricException,
            OperationRetrySettings retrySettings,
            out ExceptionHandlingResult result)
        {
            if ((fabricException is FabricCannotConnectException) ||
                (fabricException is FabricEndpointNotFoundException))
            {
                result = new ExceptionHandlingRetryResult(
                    fabricException,
                    false,
                    retrySettings,
                    retrySettings.DefaultMaxRetryCountForNonTransientErrors);
                return true;
            }

            if (fabricException.ErrorCode.Equals(FabricErrorCode.ServiceTooBusy))
            {
                result = new ExceptionHandlingRetryResult(
                    fabricException,
                    true,
                    retrySettings,
                    int.MaxValue);
                return true;
            }

            result = null;
            return false;
        }
    }
}
