// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    internal sealed class Utility
    {
        internal static bool ShouldRetryOperation(
            string currentExceptionId,
            int maxRetryCount,
            ref string lastSeenExceptionId, 
            ref int currentRetryCount)
        {
            if (currentExceptionId == lastSeenExceptionId)
            {
                if (currentRetryCount >= maxRetryCount)
                {
                    // We have retried max number of times.
                    return false;
                }

                ++currentRetryCount;
                return true;
            }

            // The current retriable exception is different from the exception that was last seen, 
            // reset the retry tracking variables
            lastSeenExceptionId = currentExceptionId;
            currentRetryCount = 0;
            return true;
        }
    }
}
