// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Threading;

    /// <summary>
    /// Provides the settings to configure the turn based concurrency lock for actors. See https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-actors-introduction for a description of concurrency in actors.
    /// </summary>
    public sealed class ActorConcurrencySettings
    {
        private ActorReentrancyMode reentrancyMode;
        private TimeSpan lockTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorConcurrencySettings"/> class.
        ///
        /// By default the <see cref="ReentrancyMode"/> is <see cref="ActorReentrancyMode.LogicalCallContext"/> with a <see cref="LockTimeout"/> of 60 seconds.
        /// </summary>
        public ActorConcurrencySettings()
        {
            this.reentrancyMode = ActorReentrancyMode.LogicalCallContext;
            this.lockTimeout = TimeSpan.FromSeconds(60);
        }

        internal ActorConcurrencySettings(ActorConcurrencySettings other)
        {
            this.reentrancyMode = other.ReentrancyMode;
            this.lockTimeout = other.lockTimeout;
        }

        /// <summary>
        /// Gets or sets Reentrancy mode for actor method calls.
        /// </summary>
        /// <value><see cref="ActorReentrancyMode"/> for the actor method calls.</value>
        public ActorReentrancyMode ReentrancyMode
        {
            get { return this.reentrancyMode; }
            set { this.reentrancyMode = value; }
        }

        /// <summary>
        /// Gets or sets the timeout for the turn based concurrency lock. If the runtime cannot acquire the lock to dispatch the method
        /// call, it will throw the <see cref="Microsoft.ServiceFabric.Actors.ActorConcurrencyLockTimeoutException"/> exception.
        /// This exception will unwind the logical call chain and the call will retried up to a configured maximum amount of times.
        /// </summary>
        /// <remarks>
        /// The actual timeout value for the concurrency lock can be higher as the runtime will add a random interval to
        /// the supplied value.
        /// </remarks>
        /// <value>Timeout for the turn based concurrency lock. This can be set to <see cref="Timeout.InfiniteTimeSpan"/> to specify waiting forever.</value>
        public TimeSpan LockTimeout
        {
            get
            {
                return this.lockTimeout;
            }

            set
            {
                if ((value != Timeout.InfiniteTimeSpan) && (value < TimeSpan.Zero))
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                if (value == TimeSpan.MaxValue)
                {
                    value = Timeout.InfiniteTimeSpan;
                }

                this.lockTimeout = value;
            }
        }
    }
}
