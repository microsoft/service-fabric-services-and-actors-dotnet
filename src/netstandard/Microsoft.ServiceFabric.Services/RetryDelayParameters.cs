// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    public class RetryDelayParameters
    {
        private readonly int retryAttempt;
        private bool isTransient;

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
        /// Gets the maximum number of times to retry.
        /// </summary>
        /// <value>Maximum number of times to retry a specific exception.</value>
        public int RetryAttempt
        {
            get { return this.retryAttempt; }
        }
    }
}
