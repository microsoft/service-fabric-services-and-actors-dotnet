// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    /// <summary>
    /// Specifies the retry policy for retrying requests on exceptions in the communication channel between
    /// client and service replicas.
    /// </summary>
    public abstract class RetryPolicy
    {
        /// <summary>
        /// Gets the maximum number of times to retry.
        /// </summary>
        /// <value>Maximum number of times to retry a specific exception.</value>
        public abstract int TotalNumberOfRetry { get; }

        /// <summary>
        /// Gets the timeout for the client side retry.
        /// </summary>
        /// <value>Amount of time we retry before throwing the OperationCancelledException to the user Api.</value>
        public abstract TimeSpan ClientRetryTimeout { get; }

        /// <summary>
        /// Gets the maximum interval to back-off before retrying in-case of Transient errors
        /// </summary>
        /// <param name="retryAttempt">
        ///  The retry attempt for which we calculate delay.
        /// </param>
        /// <returns>Maximum retry interval to back-off on transient errors</returns>
        public abstract TimeSpan GetNextRetryDelayForTransientErrors(int retryAttempt);

        /// <summary>
        /// Gets the maximum interval to back-off before retrying in-case of Non transient errors
        /// </summary>
        /// <param name="retryAttempt">
        ///  The retry attempt for which we calculate delay.
        /// </param>
        /// <returns>Maximum retry interval to back-off on non-transient errors</returns>
        public abstract TimeSpan GetNextRetryDelayForNonTransientErrors(int retryAttempt);
    }
}
