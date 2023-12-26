// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Threading;

    /// <summary>
    /// Represents state of Actor Reminder.
    /// </summary>
    [DataContract(Name = "ActorReminderState")]
    public class ActorReminderState : IActorReminderState
    {
        private const string TraceType = "ActorReminderState";

        [DataMember(Name = "Reminder", Order = 0, IsRequired = true)]
        private readonly ActorReminderData reminder;

        [DataMember(Name = "NextDueTime", Order = 1, IsRequired = true)]
        private readonly TimeSpan nextDueTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorReminderState"/> class
        /// </summary>
        /// <param name="reminder">Reminder data.</param>
        /// <param name="currentLogicalTime">Current Logical time.</param>
        /// <param name="reminderCompletedData">Reminder completed data.</param>
        internal ActorReminderState(ActorReminderData reminder, TimeSpan currentLogicalTime, ReminderCompletedData reminderCompletedData)
        {
            this.reminder = reminder;

            if (reminderCompletedData != null)
            {
                this.nextDueTime = ComputeRemainingTime(currentLogicalTime, reminderCompletedData.LogicalTime, reminder.Period);
                ActorTrace.Source.WriteInfo(
                    TraceType,
                    "Next Due Time for Reminder: ({0}), Actor: ({1}), Reminder Completed Time: ({2}), Period: ({3}) is ({4})",
                    reminder.Name,
                    reminder.ActorId.GetStorageKey(),
                    reminderCompletedData.LogicalTime,
                    reminder.Period,
                    this.nextDueTime);
            }
            else
            {
                this.nextDueTime = ComputeRemainingTime(currentLogicalTime, reminder.LogicalCreationTime, reminder.DueTime);
                ActorTrace.Source.WriteInfo(
                    TraceType,
                    "Next Due Time for Reminder: ({0}), Actor: ({1}), Reminder Creation Time: ({2}), Due Time: ({3}) is ({4})",
                    reminder.Name,
                    reminder.ActorId.GetStorageKey(),
                    reminder.LogicalCreationTime,
                    reminder.DueTime,
                    this.nextDueTime);
            }
        }

        /// <summary>
        /// Gets the remaining due time for the reminder.
        /// </summary>
        public TimeSpan RemainingDueTime
        {
            get { return this.nextDueTime; }
        }

        /// <summary>
        /// Gets the name of the reminder.
        /// </summary>
        public string Name
        {
            get { return this.reminder.Name; }
        }

        /// <summary>
        /// Gets the duetime configured for the reminder.
        /// </summary>
        public TimeSpan DueTime
        {
            get { return this.reminder.DueTime; }
        }

        /// <summary>
        /// Gets the period configured for the reminder.
        /// </summary>
        public TimeSpan Period
        {
            get { return this.reminder.Period; }
        }

        /// <summary>
        /// Gets the user state stored with the rmeinder.
        /// </summary>
        public byte[] State
        {
            get { return this.reminder.State; }
        }

        private static TimeSpan ComputeRemainingTime(
            TimeSpan currentLogicalTime,
            TimeSpan createdOrLastCompletedTime,
            TimeSpan dueTimeOrPeriod)
        {
            var elapsedTime = TimeSpan.Zero;

            if (currentLogicalTime > createdOrLastCompletedTime)
            {
                elapsedTime = currentLogicalTime - createdOrLastCompletedTime;
            }

            // If reminder has negative DueTime or Period, it is not intended to fire again.
            // Skip computing remaining time.
            if (dueTimeOrPeriod < TimeSpan.Zero)
            {
                return Timeout.InfiniteTimeSpan;
            }

            var remainingTime = TimeSpan.Zero;

            if (dueTimeOrPeriod > elapsedTime)
            {
                remainingTime = dueTimeOrPeriod - elapsedTime;
            }

            return remainingTime;
        }
    }
}
