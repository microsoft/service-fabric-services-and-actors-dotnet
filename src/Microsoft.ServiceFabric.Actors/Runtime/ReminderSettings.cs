// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    /// <summary>
    /// This class provides settings to configure the behavior of reminders. See https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-actors-timers-reminders
    /// </summary>
    public sealed class ReminderSettings
    {
        /// <summary>
        /// Initializes a new instance of the ReminderSettings class.
        /// 
        /// By default the <see cref="Microsoft.ServiceFabric.Actors.Runtime.ReminderSettings.AutoDeleteOneTimeReminders"/> is set to <c>true</c>.
        /// </summary>
        public ReminderSettings()
        {
            this.AutoDeleteOneTimeReminders = true;
        }

        internal ReminderSettings(ReminderSettings reminderSettings)
        {
            this.AutoDeleteOneTimeReminders = reminderSettings.AutoDeleteOneTimeReminders;
        }

        /// <summary>
        /// Gets or sets value indicating if ActorRuntime should automatically delete one-time reminders after
        /// they have fired and completed its callback successfully. One-time reminders refer to reminders whose
        /// <see cref="Microsoft.ServiceFabric.Actors.Runtime.IActorReminder.Period"/> is set to negative value.
        /// </summary>
        /// <remarks>
        /// Note that a reminder is considered to completed successfully only when reminder callback 
        /// <see cref="Microsoft.ServiceFabric.Actors.Runtime.IRemindable.ReceiveReminderAsync"/> completes successfully.
        /// If a failover happens while reminder callback was executing, reminder will fire again on new primary replica.
        /// </remarks>
        /// <value>
        /// The bool value indicating if ActorRuntime should automatically delete a one-time reminders after
        /// it has fired and completed its callback successfully.
        /// </value>
        public bool AutoDeleteOneTimeReminders { get; set; }
    }
}
