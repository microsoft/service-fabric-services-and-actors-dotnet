// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    /// <summary>
    /// Specifies the exception handling result when the request from client to service can be retried
    /// </summary>
    public sealed class ExceptionHandlingRetryResult : ExceptionHandlingResult
    {
        private static readonly Random Rand = new Random();

        private readonly bool isTransient;
        private readonly OperationRetrySettings retrySettings;
        private readonly string exceptionId;
        private readonly TimeSpan retryDelay;
        private readonly int maxRetryCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingRetryResult"/> class.
        /// </summary>
        /// <param name="exception">The exception that needs to be retried.</param>
        /// <param name="isTransient">
        /// Indicates if this is a transient retriable exception.
        /// Transient retriable exceptions are those where the communication channel from client
        /// to service still exists.
        /// Non transient retriable exceptions are those where we need to re-resolve the service endpoint
        /// before we retry.
        /// </param>
        /// <param name="retryDelay">The interval to wait before retrying</param>
        /// <param name="maxRetryCount">The maximum number of times the exception given in the exception parameter needs to be retried for.</param>
        public ExceptionHandlingRetryResult(
            Exception exception,
            bool isTransient,
            TimeSpan retryDelay,
            int maxRetryCount)
        {
            this.exceptionId = exception.GetType().FullName;
            this.isTransient = isTransient;
            this.retryDelay = retryDelay;
            this.retrySettings = null;
            this.maxRetryCount = maxRetryCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingRetryResult"/> class.
        /// </summary>
        /// <param name="exceptionId">An identifier for the exception that needs to be retried.</param>
        /// <param name="isTransient">
        /// Indicates if this is a transient retriable exception.
        /// Transient retriable exceptions are those where the communication channel from client
        /// to service still exists.
        /// Non transient retriable exceptions are those where we need to re-resolve the service endpoint
        /// before we retry.
        /// </param>
        /// <param name="retryDelay">The interval to wait before retrying</param>
        /// <param name="maxRetryCount">The maximum number of times the exception identified by the exceptionId parameter needs to be retried for.</param>
        public ExceptionHandlingRetryResult(
            string exceptionId,
            bool isTransient,
            TimeSpan retryDelay,
            int maxRetryCount)
        {
            this.exceptionId = exceptionId;
            this.isTransient = isTransient;
            this.retryDelay = retryDelay;
            this.maxRetryCount = maxRetryCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingRetryResult"/> class.
        /// </summary>
        /// <param name="exception">The exception that needs to be retried.</param>
        /// <param name="isTransient">
        /// Indicates if this is a transient retriable exception.
        /// Transient retriable exceptions are those where the communication channel from client
        /// to service still exists.
        /// Non transient retriable exceptions are those where we need to re-resolve the service endpoint
        /// before we retry.
        /// </param>
        /// <param name="retrySettings">The retrySettings from which the interval to wait before retrying is figured out.</param>
        /// <param name="maxRetryCount">The maximum number of times the exception identified by the exceptionId parameter needs to be retried for.</param>
        public ExceptionHandlingRetryResult(
           Exception exception,
           bool isTransient,
           OperationRetrySettings retrySettings,
           int maxRetryCount)
        {
            this.exceptionId = exception.GetType().FullName;
            this.isTransient = isTransient;
            this.retrySettings = retrySettings;
            this.retryDelay = isTransient ? retrySettings.RetryPolicy.GetNextRetryDelayForTransientErrors(0) :
            retrySettings.RetryPolicy.GetNextRetryDelayForNonTransientErrors(0);
            this.maxRetryCount = maxRetryCount;
        }

        internal ExceptionHandlingRetryResult(
          Exception exception,
          bool isTransient,
          OperationRetrySettings retrySettings)
            : this(exception, isTransient, retrySettings, retrySettings.DefaultMaxRetryCount)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the exception is represents a transient condition.
        /// The transient retriable exceptions are those where the communication channel from client
        /// to service still exists. Non transient retriable exceptions are those where we need to
        /// re-resolve the service endpoint before we retry.
        /// </summary>
        /// <value>
        /// true indicates that this is a transient retriable exception.
        /// false indicates that this is a non transient retriable exception.
        /// </value>
        public bool IsTransient
        {
            get { return this.isTransient; }
        }

        /// <summary>
        /// Gets string that uniquely identifies the exception type.
        /// </summary>
        /// <value>
        /// Unique id for this exception.
        /// This id is used to keep track of the number of times this exception is retried
        /// </value>
        public string ExceptionId
        {
            get { return this.exceptionId; }
        }

        /// <summary>
        /// Gets maximum number of times this exception type needs to be retried before giving up.
        /// The default value is int.MaxValue
        /// </summary>
        /// <value>Max retry count</value>
        public int MaxRetryCount
        {
            get { return this.maxRetryCount; }
        }

        /// <summary>
        /// Gets the time interval after which the operation should be retried.
        /// </summary>
        /// <value>Time delay after which the operation should be retried</value>
        public TimeSpan RetryDelay
        {
            get { return this.retryDelay; }
        }

        /// <summary>
        /// Gets the time interval after which the operation should be retried.
        /// </summary>
        /// <param name="retryAttempt">The retry attempt for which we calculate delay.
        /// </param>
        /// <returns>Time delay after which the operation should be retried</returns>
        public TimeSpan GetRetryDelay(int retryAttempt)
        {
            if (this.retrySettings != null)
            {
                if (this.isTransient)
                {
                    return this.retrySettings.RetryPolicy.GetNextRetryDelayForTransientErrors(retryAttempt);
                }

                return this.retrySettings.RetryPolicy.GetNextRetryDelayForNonTransientErrors(retryAttempt);
            }

            return this.retryDelay;
        }
    }
}
