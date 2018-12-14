// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    internal sealed class Utility
    {
        internal static bool ShouldRetryOperation(
            string currentExceptionId,
            int maxRetryCount,
            ref string lastSeenExceptionId,
            ref int currentRetryCount)
        {
            if (maxRetryCount == 0)
            {
                return false;
            }

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
            currentRetryCount = 1;
            return true;
        }
    }
}
