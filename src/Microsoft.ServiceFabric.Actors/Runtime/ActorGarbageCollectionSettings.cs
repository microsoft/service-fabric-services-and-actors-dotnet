// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;

    /// <summary>
    /// Represents the setting to configure Garbage Collection behavior of Actor Service.
    /// </summary>
    public sealed class ActorGarbageCollectionSettings
    {
        private long scanIntervalInSeconds = 60;
        private long idleTimeoutInSeconds = 3600;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorGarbageCollectionSettings"/> class with the values of the input argument.
        /// </summary>
        public ActorGarbageCollectionSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorGarbageCollectionSettings"/> class.
        /// </summary>
        /// <param name="idleTimeoutInSeconds">Time interval to wait before garbage collecting an actor which is not in use.</param>
        /// <param name="scanIntervalInSeconds">Time interval to run Actor Garbage Collection scan.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <para>When idleTimeoutInSeconds is less than or equal to 0.</para>
        /// <para>When scanIntervalInSeconds is less than or equal to 0.</para>
        /// <para>When idleTimeoutInSeconds is less than scanIntervalInSeconds.</para>
        /// </exception>
        public ActorGarbageCollectionSettings(long idleTimeoutInSeconds, long scanIntervalInSeconds)
        {
            // Verify that values are within acceptable range.
            if (idleTimeoutInSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException("idleTimeoutInSeconds)", SR.ActorGCSettingsValueOutOfRange);
            }

            if (scanIntervalInSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException("scanIntervalInSeconds)", SR.ActorGCSettingsValueOutOfRange);
            }

            if (idleTimeoutInSeconds / scanIntervalInSeconds >= 1)
            {
                this.scanIntervalInSeconds = scanIntervalInSeconds;
                this.idleTimeoutInSeconds = idleTimeoutInSeconds;
            }
            else
            {
                throw new ArgumentOutOfRangeException(SR.ActorGCSettingsNotValid);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorGarbageCollectionSettings"/> class.
        /// </summary>
        /// <param name="settings">The setting of Actor Garbage Collection.</param>
        internal ActorGarbageCollectionSettings(ActorGarbageCollectionSettings settings)
        {
            this.idleTimeoutInSeconds = settings.IdleTimeoutInSeconds;
            this.scanIntervalInSeconds = settings.ScanIntervalInSeconds;
        }

        /// <summary>
        /// Gets the time interval to run Actor Garbage Collection scan.
        /// </summary>
        /// <value>The time interval in <see cref="long"/> to run Actor Garbage Collection scan.</value>
        public long ScanIntervalInSeconds
        {
            get { return this.scanIntervalInSeconds; }
        }

        /// <summary>
        /// Gets the time interval to wait before garbage collecting an actor which is not in use.
        /// </summary>
        /// <value>The time interval in <see cref="long"/> to wait before garbage collecting an actor which is not in use.</value>
        public long IdleTimeoutInSeconds
        {
            get { return this.idleTimeoutInSeconds; }
        }
    }
}
