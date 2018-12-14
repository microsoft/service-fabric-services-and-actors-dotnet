// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    /// <summary>
    /// Specifies the constant retry policy for retrying requests on exceptions in the communication channel between
    /// client and service replicas.
    /// </summary>
    public class ConstantRetryPolicy : IRetryPolicy
    {
        private static readonly Random Rand = new Random();

        private readonly TimeSpan maxRetryBackoffIntervalOnNonTransientErrors;
        private readonly int maxRetryCountOnNonTransientErrors;
        private readonly TimeSpan maxRetryBackoffIntervalOnTransientErrors;
        private readonly int totalNumberOfRetries;
        private readonly TimeSpan clientRetryTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantRetryPolicy"/> class with the supplied settings.
        /// </summary>
        /// <param name="maxRetryBackoffIntervalOnTransientErrors">
        /// Specifies the maximum interval to back-off before retrying incase of Transient errors
        /// </param>
        /// <param name="maxRetryBackoffIntervalOnNonTransientErrors">
        /// Specifies the maximum interval to back-off before retrying incase of Non transient errors
        /// </param>
        /// <param name="maxRetryCountOnNonTransientErrors">Specifies the max number of times to retry On Non-Transient Errors</param>
        /// <param name="maxRetryCount">
        /// Specifies the maximum number of times to retry.
        /// </param>
        /// <param name="clientRetryTimeout">Specifies the maximum time client retries the call before quitting</param>
        public ConstantRetryPolicy(
            TimeSpan maxRetryBackoffIntervalOnTransientErrors,
            TimeSpan maxRetryBackoffIntervalOnNonTransientErrors,
            int maxRetryCountOnNonTransientErrors,
            int maxRetryCount,
            TimeSpan clientRetryTimeout)
        {
            this.maxRetryBackoffIntervalOnTransientErrors = maxRetryBackoffIntervalOnTransientErrors;
            this.maxRetryBackoffIntervalOnNonTransientErrors = maxRetryBackoffIntervalOnNonTransientErrors;
            this.maxRetryCountOnNonTransientErrors = maxRetryCountOnNonTransientErrors;
            this.totalNumberOfRetries = maxRetryCount;
            this.clientRetryTimeout = clientRetryTimeout;
        }

        /// <inheritdoc/>
        public int TotalNumberOfRetries
        {
            get { return this.totalNumberOfRetries; }
        }

        /// <inheritdoc/>
        public TimeSpan ClientRetryTimeout
        {
            get { return this.clientRetryTimeout; }
        }

        /// <summary>
        /// Gets the maximum interval to back-off before retrying in-case of Non transient errors
        /// </summary>
        /// <value>Maximum retry interval to back-off on non transient errors</value>
        public TimeSpan MaxRetryBackoffIntervalOnNonTransientErrors
        {
            get { return this.maxRetryBackoffIntervalOnNonTransientErrors; }
        }

        /// <summary>
        /// Gets the maximum interval to back-off before retrying in-case of Transient errors
        /// </summary>
        /// <value>Maximum retry interval to back-off on transient errors</value>
        public TimeSpan MaxRetryBackoffIntervalOnTransientErrors
        {
            get { return this.maxRetryBackoffIntervalOnTransientErrors; }
        }

        /// <summary>
        /// Gets the maximum number of times to retry for non -transient errors.
        /// </summary>
        /// <value>Maximum number of times to retry a specific non-transient exception.</value>
        public int MaxRetryCountOnNonTransientErrors => this.maxRetryCountOnNonTransientErrors;

        /// <inheritdoc/>
        public TimeSpan GetNextRetryDelay(RetryDelayParameters retryDelayParameters)
        {
            if (retryDelayParameters.IsTransient)
            {
                return new TimeSpan((long)(this.maxRetryBackoffIntervalOnTransientErrors.Ticks * Rand.NextDouble()));
            }

            return new TimeSpan((long)(this.maxRetryBackoffIntervalOnNonTransientErrors.Ticks * Rand.NextDouble()));
        }
    }
}
