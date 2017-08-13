// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Threading;

    internal class ActorReminderState : IActorReminderState
    {
        private readonly ActorReminderData reminder;
        private readonly TimeSpan nextDueTime;

        public ActorReminderState(ActorReminderData reminder, TimeSpan currentLogicalTime, ReminderCompletedData reminderCompletedData)
        {
            this.reminder = reminder;

            if(reminderCompletedData != null)
            {
                this.nextDueTime = ComputeRemainingTime(currentLogicalTime, reminderCompletedData.LogicalTime, reminder.Period);
            }
            else
            {
                this.nextDueTime = ComputeRemainingTime(currentLogicalTime, reminder.LogicalCreationTime, reminder.DueTime);
            }            
        }

        TimeSpan IActorReminderState.RemainingDueTime
        {
            get { return this.nextDueTime; }
        }

        string IActorReminder.Name
        {
            get { return this.reminder.Name; }
        }

        TimeSpan IActorReminder.DueTime
        {
            get { return this.reminder.DueTime; }
        }

        TimeSpan IActorReminder.Period
        {
            get { return this.reminder.Period; }
        }

        byte[] IActorReminder.State
        {
            get { return this.reminder.State; }
        }


        private static TimeSpan ComputeRemainingTime(
            TimeSpan currentLogicalTime,
            TimeSpan createdOrLastCompletedTime,
            TimeSpan dueTimeOrPeriod)
        {
            var elapsedTime = TimeSpan.Zero;

            if(currentLogicalTime > createdOrLastCompletedTime)
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
