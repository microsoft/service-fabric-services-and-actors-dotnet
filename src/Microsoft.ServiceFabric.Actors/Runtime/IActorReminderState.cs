// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
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
        /// Time when Reminder is next due.
        /// </summary>
        /// <value>Due time as <see cref="System.TimeSpan"/> when the reminder is next due.</value>
        TimeSpan RemainingDueTime { get; }
    }
}
