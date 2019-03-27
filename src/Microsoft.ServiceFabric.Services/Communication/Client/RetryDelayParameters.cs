// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    /// <summary>
    /// Specifies the Retry Paremeters for calculating retry delay for communication between client and services.</summary>
    public class RetryDelayParameters
    {
        private readonly int retryAttempt;
        private bool isTransient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryDelayParameters"/> class with the supplied settings.
        /// </summary>
        /// <param name="retryAttempt">Specifies the number of times request has been retried.</param>
        /// <param name="isTransient">Specifies whether its a transient condition or not</param>
        public RetryDelayParameters(int retryAttempt, bool isTransient)
        {
            this.retryAttempt = retryAttempt;
            this.isTransient = isTransient;
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
        /// Gets the number of times request has been retried.
        /// </summary>
        /// <value>Number of times request has been retried .</value>
        public int RetryAttempt
        {
            get { return this.retryAttempt; }
        }
    }
}
