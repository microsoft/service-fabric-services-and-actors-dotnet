// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    /// <summary>
    /// Specifies the retry policy for retrying requests on exceptions in the communication channel between
    /// client and service replicas.
    /// </summary>
    public interface IRetryPolicy
    {
        /// <summary>
        /// Gets the maximum number of times to retry.
        /// </summary>
        /// <value>Maximum number of times to retry a specific exception.</value>
         int TotalNumberOfRetries { get; }

        /// <summary>
        /// Gets the timeout for the client side retry.
        /// </summary>
        /// <value>Amount of time we retry before throwing the OperationCancelledException to the user Api.</value>
         TimeSpan ClientRetryTimeout { get; }

        /// <summary>
        /// Gets the maximum interval to back-off before retrying in-case of Transient errors.
        /// </summary>
        /// <param name="retryDelayParameters">
        ///  The retry delay parameters to be used to calculate retry.
        /// </param>
        /// <returns>Maximum retry interval to back-off on transient errors.</returns>
         TimeSpan GetNextRetryDelay(RetryDelayParameters retryDelayParameters);
    }
}
