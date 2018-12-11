// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    /// <summary>
    /// Specifies the policy for retrying requests on exceptions in the communication channel between
    /// client and service replicas.
    /// </summary>
    public sealed class OperationRetrySettings
    {
        private readonly RetryPolicy retryPolicy;
        private readonly TimeSpan clientRetryTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationRetrySettings"/> class
        /// with default values for the retry settings.
        /// The default values for MaxRetryBackoffIntervalOnTransientErrors, NonTransientErrors
        /// are 2 seconds. The default value for MaxRetryCount is 10.
        /// </summary>
        public OperationRetrySettings()
            : this(new ExponentialRetryPolicy(10, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), default(TimeSpan)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationRetrySettings"/> class
        /// with default values for the retry settings.
        /// The default values for MaxRetryBackoffIntervalOnTransientErrors, NonTransientErrors
        /// are 2 seconds. The default value for MaxRetryCount is 10.
        /// </summary>
        /// <param name="clientRetryTimeout">Specifies the maximum time client retries the call before quitting</param>
        public OperationRetrySettings(TimeSpan clientRetryTimeout)
            : this()
        {
            this.clientRetryTimeout = clientRetryTimeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationRetrySettings"/> class
        /// with the retry Policy supplied.
        /// </summary>
        /// <param name="retryPolicy">Specifies the Retry Policy to be used for the communication between client and service.
        /// </param>
        public OperationRetrySettings(RetryPolicy retryPolicy)
        {
            this.retryPolicy = retryPolicy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationRetrySettings"/> class with the supplied settings.
        /// </summary>
        /// <param name="maxRetryBackoffIntervalOnTransientErrors">
        /// Specifies the maximum interval to back-off before retrying incase of Transient errors
        /// </param>
        /// <param name="maxRetryBackoffIntervalOnNonTransientErrors">
        /// Specifies the maximum interval to back-off before retrying incase of Non transient errors
        /// </param>
        /// <param name="defaultMaxRetryCount">
        /// Specifies the maximum number of times to retry.
        /// </param>
        public OperationRetrySettings(
            TimeSpan maxRetryBackoffIntervalOnTransientErrors,
            TimeSpan maxRetryBackoffIntervalOnNonTransientErrors,
            int defaultMaxRetryCount)
            : this(new ConstantRetryPolicy(
                maxRetryBackoffIntervalOnTransientErrors,
                maxRetryBackoffIntervalOnNonTransientErrors,
                defaultMaxRetryCount,
                default(TimeSpan)))
        {
        }

        /// <summary>
        /// Gets the maximum interval to back-off before retrying in-case of Transient errors
        /// </summary>
        /// <value>Maximum retry interval to back-off on transient errors</value>
        public TimeSpan MaxRetryBackoffIntervalOnTransientErrors
        {
            get
            {
                var contantpolicy = this.retryPolicy as ConstantRetryPolicy;
                if (contantpolicy != null)
                {
                    return contantpolicy.MaxRetryBackoffIntervalOnTransientErrors;
                }

                throw new NotSupportedException("This retry Policy doesn't support this functionality");
            }
        }

        /// <summary>
        /// Gets the maximum interval to back-off before retrying in-case of Non transient errors
        /// </summary>
        /// <value>Maximum retry interval to back-off on non transient errors</value>
        public TimeSpan MaxRetryBackoffIntervalOnNonTransientErrors
        {
            get
            {
                var contantpolicy = this.retryPolicy as ConstantRetryPolicy;
                if (contantpolicy != null)
                {
                    return contantpolicy.MaxRetryBackoffIntervalOnNonTransientErrors;
                }

                throw new NotSupportedException("This retry Policy doesn't support this functionality ");
            }
        }

        /// <summary>
        /// Gets the maximum number of times to retry.
        /// </summary>
        /// <value>Maximum number of times to retry a specific exception.</value>
        public int DefaultMaxRetryCount
        {
            get { return this.retryPolicy.TotalNumberOfRetry; }
        }

        /// <summary>
        /// Gets the timeout for the client side retry.
        /// </summary>
        /// <value>Amount of time we retry before throwing the OperationCancelledException to the user Api.</value>
        public TimeSpan ClientRetryTimeout
        {
            get { return this.retryPolicy.ClientRetryTimeout; }
        }

        /// <summary>
        /// Gets the Retry Policy to be used for the communication between client and service.
        /// </summary>
        public RetryPolicy RetryPolicy
        {
            get
            {
                return this.retryPolicy;
            }
        }
    }
}
