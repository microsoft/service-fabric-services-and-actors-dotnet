// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;

    internal class RCTxExceptionHandler : IExceptionHandler
    {
        public bool TryHandleException(Exception exception, out bool isTransient)
        {
            isTransient = false;
            if (exception is TransactionFaultedException)
            {
                isTransient = true;

                return true;
            }

            return false;
        }
    }
}
