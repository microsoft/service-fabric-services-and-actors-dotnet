// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
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
        private readonly TimeSpan maxRetryBackoffIntervalOnNonTransientErrors;
        private readonly TimeSpan maxRetryBackoffIntervalOnTransientErrors;
        private readonly int defaultMaxRetryCountForTransientErrors;
        private readonly int defaultMaxRetryCountForNonTransientErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationRetrySettings"/> class
        /// with default values for the retry settings.
        /// The default values for MaxRetryBackoffIntervalOnTransientErrors, NonTransientErrors
        /// are 2 seconds. The default value for MaxRetryCount is 10.
        /// </summary>
        public OperationRetrySettings()
            : this(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), 10)
        {
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
        /// <param name="defaultMaxRetryCountForTransientErrors">
        /// Specifies the maximum number of times to retry for transient errors.
        /// </param>
        /// <param name="defaultMaxRetryCountForNonTransientErrors">
        /// Specifies the maximum number of times to retry for non-transient errors.
        /// </param>
        public OperationRetrySettings(
            TimeSpan maxRetryBackoffIntervalOnTransientErrors,
            TimeSpan maxRetryBackoffIntervalOnNonTransientErrors,
            int defaultMaxRetryCountForTransientErrors,
            int defaultMaxRetryCountForNonTransientErrors = int.MaxValue)
        {
            this.maxRetryBackoffIntervalOnTransientErrors = maxRetryBackoffIntervalOnTransientErrors;
            this.maxRetryBackoffIntervalOnNonTransientErrors = maxRetryBackoffIntervalOnNonTransientErrors;
            this.defaultMaxRetryCountForTransientErrors = defaultMaxRetryCountForTransientErrors;
            this.defaultMaxRetryCountForNonTransientErrors = defaultMaxRetryCountForNonTransientErrors;
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
        /// Gets the maximum interval to back-off before retrying in-case of Non transient errors
        /// </summary>
        /// <value>Maximum retry interval to back-off on non transient errors</value>
        public TimeSpan MaxRetryBackoffIntervalOnNonTransientErrors
        {
            get { return this.maxRetryBackoffIntervalOnNonTransientErrors; }
        }

        /// <summary>
        /// Gets the maximum number of times to retry for transient errors.
        /// </summary>
        /// <value>Maximum number of times to retry a specific exception.</value>
        public int DefaultMaxRetryCountForTransientErrors
        {
            get { return this.defaultMaxRetryCountForTransientErrors; }
        }

        /// <summary>
        /// Gets the maximum number of times to retry for non -transient errors.
        /// </summary>
        /// <value>Maximum number of times to retry a specific non-transient exception.</value>
        public int DefaultMaxRetryCountForNonTransientErrors
        {
            get { return this.defaultMaxRetryCountForNonTransientErrors; }
        }
    }
}
