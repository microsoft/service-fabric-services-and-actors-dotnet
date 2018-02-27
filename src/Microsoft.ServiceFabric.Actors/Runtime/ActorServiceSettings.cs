// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;

    /// <summary>
    /// Settings to configures behavior of Actor Service.
    /// </summary>
    public sealed class ActorServiceSettings
    {
        private ActorGarbageCollectionSettings actorGarbageCollectionSettings = new ActorGarbageCollectionSettings();
        private ActorConcurrencySettings actorConcurrencySettings = new ActorConcurrencySettings();
        private ReminderSettings reminderSettings = new ReminderSettings();

        /// <summary>
        /// Initializes a new instance of the ActorServiceSettings class.
        /// </summary>
        public ActorServiceSettings()
        {
        }

        internal static ActorServiceSettings DeepCopyFromOrDefaultOnNull(ActorServiceSettings other)
        {
            var actorServiceSettings = new ActorServiceSettings();

            if (other == null)
            {
                return actorServiceSettings;
            }

            // deep copy settings.
            actorServiceSettings.actorGarbageCollectionSettings = new ActorGarbageCollectionSettings(other.actorGarbageCollectionSettings);
            actorServiceSettings.actorConcurrencySettings = new ActorConcurrencySettings(other.actorConcurrencySettings);
            actorServiceSettings.reminderSettings = new ReminderSettings(other.reminderSettings);

            return actorServiceSettings;
        }

        /// <summary>
        /// Gets or sets garbage collection settings for the Actor service.
        /// </summary>
        /// <value><see cref="Microsoft.ServiceFabric.Actors.Runtime.ActorGarbageCollectionSettings"/> for the Actor Service.</value>
        public ActorGarbageCollectionSettings ActorGarbageCollectionSettings
        {
            get { return this.actorGarbageCollectionSettings; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.actorGarbageCollectionSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets settings to configure the turn based concurrency lock for actors.
        /// </summary>
        /// <value><see cref="Microsoft.ServiceFabric.Actors.Runtime.ActorConcurrencySettings"/> for the Actor Service.</value>
        public ActorConcurrencySettings ActorConcurrencySettings
        {
            get { return this.actorConcurrencySettings; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.actorConcurrencySettings = value;
            }
        }

        /// <summary>
        /// Gets or sets settings to configure behavior of reminders.
        /// </summary>
        /// <value><see cref="Microsoft.ServiceFabric.Actors.Runtime.ReminderSettings"/> for the Actor Service.</value>
        public ReminderSettings ReminderSettings
        {
            get { return this.reminderSettings; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.reminderSettings = value;
            }
        }
    }
}
