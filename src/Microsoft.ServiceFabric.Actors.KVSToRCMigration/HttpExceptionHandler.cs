// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal class HttpExceptionHandler : Services.Communication.Client.IExceptionHandler
    {
        public bool TryHandleException(ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, out ExceptionHandlingResult result)
        {
            // TODO: Handle HTTP channel exceptions for retry
            result = new ExceptionHandlingThrowResult()
            {
                ExceptionToThrow = exceptionInformation.Exception,
            };

            return false;
        }
    }
}
