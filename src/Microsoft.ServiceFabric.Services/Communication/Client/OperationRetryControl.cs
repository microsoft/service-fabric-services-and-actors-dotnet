// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    /// <summary>
    /// Specifies the retry policy for the exceptions got on the communication from client to service.
    /// </summary>
    public class OperationRetryControl
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation should be retried or not.
        /// </summary>
        /// <value>true if the operation should be retried, false if the exception should be thrown to the user</value>
        public bool ShouldRetry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the exception is represents a transient condition.
        /// The transient retriable exceptions are those where the communication channel from client
        /// to service still exists. Non transient retriable exceptions are those where we need to
        /// re-resolve the service endpoint before we retry.
        /// </summary>
        /// <value>
        /// true indicates that this is a transient retriable exception.
        /// false indicates that this is a non transient retriable exception.
        /// </value>
        public bool IsTransient { get; set; }

        /// <summary>
        /// Gets or sets the delay after which the operation should be retried if the ShouldRetry is true.
        /// </summary>
        /// <value>Time delay after which the operation should be retried</value>
        public TimeSpan RetryDelay { get; set; }

        /// <summary>
        /// Gets or sets string that uniquely identifies the exception type.
        /// </summary>
        /// <value>Unique id for this exception.
        /// This id is used to keep track of the number of times this exception is retried</value>
        public string ExceptionId { get; set; }

        /// <summary>
        /// Gets or sets max number of times this operation should be retried if the ShouldRetry is true
        /// </summary>
        /// <value>Max retry count</value>
        public int MaxRetryCount { get; set; }

        /// <summary>
        /// Gets or sets exception to report for the operation, if ShouldRetry is false.
        /// By default this is the same exception as the reported exception, however in some cases the Factory may choose to trasform the reported exception to a more meaningful exception.
        /// </summary>
        /// <value>Exception</value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the delay after which the operation should be retried if the ShouldRetry is true.
        /// </summary>
        /// <value>Time delay after which the operation should be retried</value>
        public Func<int, TimeSpan> GetRetryDelay { get; set; }
    }
}
