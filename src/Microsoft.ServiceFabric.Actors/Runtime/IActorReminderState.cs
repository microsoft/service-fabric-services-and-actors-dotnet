// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;

    /// <summary>
    /// Represents internal state of Actor Reminder.
    /// </summary>
    public interface IActorReminderState : IActorReminder
    {
        /// <summary>
        /// Gets the time when Reminder is next due.
        /// </summary>
        /// <value>Due time as <see cref="System.TimeSpan"/> when the reminder is next due.</value>
        TimeSpan RemainingDueTime { get; }
    }
}
