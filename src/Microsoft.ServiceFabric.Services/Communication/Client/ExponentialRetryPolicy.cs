// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    /// <summary>
    /// Specifies the exponential backoff policy for retrying requests on exceptions in the communication channel between
    /// client and service replicas.
    /// </summary>
    public class ExponentialRetryPolicy : IRetryPolicy
    {
        private const int MaxDelayMultiplier = 9;
        private const int SameDelayRequestCounter = 3;
        private static readonly RandomGenerator RandomGenerator = new RandomGenerator();

        private readonly int totalNumberOfRetry;
        private readonly TimeSpan clientRetryTimeout;
        private readonly TimeSpan maxRetryJitter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialRetryPolicy"/> class with the supplied settings.
        /// and the  default values for the other retry settings.
        /// The default values for maxRetryJitterForTransientErrors, maxRetryJitterForNonTransientErrors
        /// are 2 seconds. The default value for MaxRetryCount is 10.
        /// </summary>
        /// <param name="defaultMaxRetryCount">
        /// Specifies the maximum number of times to retry.
        /// </param>
        /// <param name="clientRetryTimeout">Specifies the max timeout for the client side retry logic.</param>
        public ExponentialRetryPolicy(
            int defaultMaxRetryCount,
            TimeSpan clientRetryTimeout)
        {
            this.clientRetryTimeout = clientRetryTimeout;
            this.maxRetryJitter = TimeSpan.FromSeconds(2);
            this.totalNumberOfRetry = defaultMaxRetryCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialRetryPolicy"/> class with the supplied settings.
        /// </summary>
        /// <param name="maxRetryJitter">
        /// Specifies the maximum jitter to use for Errors.
        /// </param>
        /// <param name="clientRetryTimeout">Specifies the max timeout for the client side retry logic.</param>
        /// <param name="defaultMaxRetryCount">
        /// Specifies the maximum number of times to retry.
        /// </param>
        internal ExponentialRetryPolicy(
            int defaultMaxRetryCount,
            TimeSpan maxRetryJitter,
            TimeSpan clientRetryTimeout)
        {
            this.totalNumberOfRetry = defaultMaxRetryCount;
            this.clientRetryTimeout = clientRetryTimeout;
            this.maxRetryJitter = maxRetryJitter;
        }

        /// <inheritdoc/>
        public int TotalNumberOfRetries { get => this.totalNumberOfRetry; }

        /// <inheritdoc/>
        public TimeSpan ClientRetryTimeout { get => this.clientRetryTimeout; }

        /// <inheritdoc/>
        public TimeSpan GetNextRetryDelay(RetryDelayParameters retryDelayParameters)
        {
            // This we are doing to increase delay gradually . For every 3 consecutive retrries, delay wuld be same.
            int delayMultiplier = retryDelayParameters.RetryAttempt / SameDelayRequestCounter;

            // Capping the Max Retry Time to nearest 5 mins ~ Pow(2,9)
            if (delayMultiplier >= MaxDelayMultiplier)
            {
                delayMultiplier = MaxDelayMultiplier;
            }

            return TimeSpan.FromSeconds((this.maxRetryJitter.TotalSeconds * RandomGenerator.NextDouble()) + (1 << delayMultiplier));
        }
    }
}
