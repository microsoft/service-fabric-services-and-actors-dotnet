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
        private static readonly RandomGenerator RandomGenerator = new RandomGenerator();

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
        /// <param name="clientRetryTimeout">Specifies the max timeout for the client side retry logic</param>
        public ExponentialRetryPolicy(
            int defaultMaxRetryCount,
            TimeSpan clientRetryTimeout)
        {
            this.ClientRetryTimeout = clientRetryTimeout;
            this.maxRetryJitter = TimeSpan.FromSeconds(2);
            this.TotalNumberOfRetries = defaultMaxRetryCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialRetryPolicy"/> class with the supplied settings.
        /// </summary>
        /// <param name="maxRetryJitter">
        /// Specifies the maximum jitter to use for Errors
        /// </param>
        /// <param name="clientRetryTimeout">Specifies the max timeout for the client side retry logic</param>
        /// <param name="defaultMaxRetryCount">
        /// Specifies the maximum number of times to retry.
        /// </param>
        internal ExponentialRetryPolicy(
            int defaultMaxRetryCount,
            TimeSpan maxRetryJitter,
            TimeSpan clientRetryTimeout)
        {
            this.TotalNumberOfRetries = defaultMaxRetryCount;
            this.ClientRetryTimeout = clientRetryTimeout;
            this.maxRetryJitter = maxRetryJitter;
        }

        /// <summary>
        /// Gets or sets the maximum multiplier for delay.  MaxDelay equals BaseRetryDelay * (2 to the power of MaxDelayMultiplier).
        /// </summary>
        public static int MaxDelayMultiplier { get; set; } = 9;

        /// <summary>
        /// Gets or sets the number of requests to use the same delay in a row. This slows the exponential backoff.
        /// </summary>
        public static int SameDelayRequestCounter { get; set; } = 3;

        /// <inheritdoc/>
        public int TotalNumberOfRetries { get; }

        /// <inheritdoc/>
        public TimeSpan ClientRetryTimeout { get; }

        /// <summary>
        /// Gets or sets the initial delay before retrying. All retries would be exponentially increasing from this value.
        /// </summary>
        public TimeSpan BaseRetryDelay { get; set; } = TimeSpan.FromSeconds(1);

        /// <inheritdoc/>
        public TimeSpan GetNextRetryDelay(RetryDelayParameters retryDelayParameters)
        {
            // This we are doing to increase delay gradually . For every SameDelayRequestCounter consecutive retrries, delay would be same.
            int delayMultiplier = retryDelayParameters.RetryAttempt / SameDelayRequestCounter;

            // Capping the Max Retry Time to BaseRetryDelay * Pow(2,MaxDelayMultiplier).
            if (delayMultiplier >= MaxDelayMultiplier)
            {
                delayMultiplier = MaxDelayMultiplier;
            }

            return TimeSpan.FromMilliseconds((this.maxRetryJitter.TotalMilliseconds * RandomGenerator.NextDouble()) + ((int)this.BaseRetryDelay.TotalMilliseconds << delayMultiplier));
        }
    }
}
