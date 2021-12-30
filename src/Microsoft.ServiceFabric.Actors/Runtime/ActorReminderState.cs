// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading;

    [DataContract(Name = "ActorReminderState")]
    public class ActorReminderState : IActorReminderState
    {
        [DataMember(Name = "Reminder", Order = 0, IsRequired = true)]
        private readonly ActorReminderData reminder;
        [DataMember(Name = "NextDueTime", Order = 1, IsRequired = true)]
        private readonly TimeSpan nextDueTime;

        public ActorReminderState(ActorReminderData reminder, TimeSpan currentLogicalTime, ReminderCompletedData reminderCompletedData)
        {
            this.reminder = reminder;

            if (reminderCompletedData != null)
            {
                this.nextDueTime = ComputeRemainingTime(currentLogicalTime, reminderCompletedData.LogicalTime, reminder.Period);
            }
            else
            {
                this.nextDueTime = ComputeRemainingTime(currentLogicalTime, reminder.LogicalCreationTime, reminder.DueTime);
            }
        }

        public TimeSpan RemainingDueTime
        {
            get { return this.nextDueTime; }
        }

        public string Name
        {
            get { return this.reminder.Name; }
        }

        public TimeSpan DueTime
        {
            get { return this.reminder.DueTime; }
        }

        public TimeSpan Period
        {
            get { return this.reminder.Period; }
        }

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
