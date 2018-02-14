// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    internal class ActorReminder : IActorReminder
    {
        private const string TraceType = "ActorReminder";

        private readonly TimeSpan MinTimePeriod = Timeout.InfiniteTimeSpan;
        private readonly ActorId ownerActorId;
        private readonly IActorManager actorManager;
        private readonly string name;
        private readonly TimeSpan dueTime;
        private readonly TimeSpan period;
        private readonly byte[] state;

        private Timer timer;

        public ActorReminder(ActorId actorId, IActorManager actorManager, IActorReminder reminder)
            : this(
                  actorId,
                  actorManager,
                  reminder.Name,
                  reminder.State,
                  reminder.DueTime,
                  reminder.Period)
        {
        }

        public ActorReminder(
            ActorId actorId,
            IActorManager actorManager,
            string reminderName,
            byte[] reminderState,
            TimeSpan reminderDueTime,
            TimeSpan reminderPeriod)
        {
            ValidateDueTime("DueTime", reminderDueTime);
            ValidatePeriod("Period", reminderPeriod);

            this.actorManager = actorManager;
            this.ownerActorId = actorId;
            this.name = reminderName;
            this.dueTime = reminderDueTime;
            this.period = reminderPeriod;
            this.state = reminderState;

            this.timer = new Timer(this.OnReminderCallback);
        }

        internal ActorId OwnerActorId
        {
            get { return this.ownerActorId; }
        }

        internal bool IsValid()
        {
            return (this.timer != null);
        }

        #region IActorReminder Members

        public string Name
        {
            get { return this.name; }
        }

        public byte[] State
        {
            get { return this.state; }
        }

        public TimeSpan DueTime
        {
            get { return this.dueTime; }
        }

        public TimeSpan Period
        {
            get { return this.period; }
        }

        #endregion

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ActorReminder()
        {
            this.Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.CancelTimer();
        }

        internal void CancelTimer()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }
        }

        private void OnReminderCallback(object reminderState)
        {
            Task.Run(() => { this.actorManager.FireReminderAsync(this); });
        }

        internal void ArmTimer(TimeSpan newDueTime)
        {
            var snap = this.timer;
            if (snap != null)
            {
                try
                {
                    snap.Change(newDueTime, Timeout.InfiniteTimeSpan);
                }
                catch (Exception e)
                {
                    this.actorManager.TraceSource.WriteErrorWithId(
                        TraceType,
                        this.actorManager.GetActorTraceId(this.OwnerActorId),
                        "Failed to arm timer for reminder {0} exception {1}",
                        this.Name,
                        e);
                }
            }
        }

        private void ValidateDueTime(string argName, TimeSpan value)
        {
            if (value < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    argName,
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.TimerArgumentOutOfRange,
                        this.MinTimePeriod.TotalMilliseconds,
                        TimeSpan.MaxValue.TotalMilliseconds));
            }
        }

        private void ValidatePeriod(string argName, TimeSpan value)
        {
            if (value < this.MinTimePeriod)
            {
                throw new ArgumentOutOfRangeException(
                    argName,
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.TimerArgumentOutOfRange,
                        this.MinTimePeriod.TotalMilliseconds,
                        TimeSpan.MaxValue.TotalMilliseconds));
            }
        }
    }
}
