// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    /// <summary>
    /// Specifies the exponential backoff policy for retrying requests on exceptions in the communication channel between
    /// client and service replicas.
    /// </summary>
    public class ExponentialRetryPolicy : RetryPolicy
    {
        private static readonly Random Rand = new Random();

        private readonly int totalNumberOfRetry;
        private readonly TimeSpan clientRetryTimeout;
        private readonly TimeSpan maxRetryJitterForTransientErrors;
        private readonly TimeSpan maxRetryJitterForNonTransientErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialRetryPolicy"/> class with the supplied settings.
        /// </summary>
        /// <param name="maxRetryJitterForTransientErrors">
        /// Specifies the maximum jitter to use for Transient Errors
        /// </param>
        /// <param name="maxRetryJitterForNonTransientErrors">
        /// Specifies the maximum jitter to use for Non Transient Errors
        /// </param>
        /// <param name="clientRetryTimeout">Specifies the max timeout for the client side retry logic</param>
        /// <param name="defaultMaxRetryCount">
        /// Specifies the maximum number of times to retry.
        /// </param>
        public ExponentialRetryPolicy(
            int defaultMaxRetryCount,
            TimeSpan maxRetryJitterForTransientErrors,
            TimeSpan maxRetryJitterForNonTransientErrors,
            TimeSpan clientRetryTimeout)
        {
            this.totalNumberOfRetry = defaultMaxRetryCount;
            this.clientRetryTimeout = clientRetryTimeout;
            this.maxRetryJitterForTransientErrors = maxRetryJitterForTransientErrors;
            this.maxRetryJitterForNonTransientErrors = maxRetryJitterForNonTransientErrors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialRetryPolicy"/> class with the supplied settings.
        /// and the  default values for the other retry settings.
        /// The default values for maxRetryJitterForTransientErrors, maxRetryJitterForNonTransientErrors
        /// are 2 seconds. The default value for MaxRetryCount is 10.
        /// </summary>
        /// <param name="clientRetryTimeout">Specifies the max timeout for the client side retry logic</param>
        public ExponentialRetryPolicy(
          TimeSpan clientRetryTimeout)
        {
            this.clientRetryTimeout = clientRetryTimeout;
            this.maxRetryJitterForNonTransientErrors = TimeSpan.FromSeconds(2);
            this.maxRetryJitterForTransientErrors = TimeSpan.FromSeconds(2);
            this.totalNumberOfRetry = 10;
        }

        /// <inheritdoc/>
        public override int TotalNumberOfRetry { get => this.totalNumberOfRetry; }

        /// <inheritdoc/>
        public override TimeSpan ClientRetryTimeout { get => this.clientRetryTimeout; }

        /// <inheritdoc/>
        public override TimeSpan GetNextRetryDelayForNonTransientErrors(int retryAttempt)
        {
            return TimeSpan.FromSeconds((this.maxRetryJitterForNonTransientErrors.TotalSeconds * Rand.NextDouble()) + (1 << retryAttempt));
        }

        /// <inheritdoc/>
        public override TimeSpan GetNextRetryDelayForTransientErrors(int retryAttempt)
        {
            return TimeSpan.FromSeconds((this.maxRetryJitterForTransientErrors.TotalSeconds * Rand.NextDouble()) + (1 << retryAttempt));
        }
    }
}
