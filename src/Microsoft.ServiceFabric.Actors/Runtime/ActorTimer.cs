// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    internal class ActorTimer : IActorTimer
    {
        private readonly ActorBase owner;
        private readonly TimeSpan dueTime;
        private readonly TimeSpan period;
        private readonly object callbackState;
        private readonly ActorMethodContext callbackMethodContext;

        private Timer timer;
        private Func<object, Task> asyncCallback;

        public ActorTimer(
            ActorBase owner,
            Func<object, Task> asyncCallback,
            object state,
            TimeSpan dueTime,
            TimeSpan period)
        {
            this.owner = owner;
            this.asyncCallback = asyncCallback;
            this.callbackMethodContext = ActorMethodContext.CreateForTimer(this.asyncCallback.GetMethodInfo().Name);
            this.callbackState = state;
            this.period = period;
            this.dueTime = dueTime;

            // Don't capture the current ExecutionContext and its AsyncLocals onto the timer
            bool restoreFlow = false;
            AsyncFlowControl asyncFlowControl = default(AsyncFlowControl);
            try
            {
                if (!ExecutionContext.IsFlowSuppressed())
                {
                    asyncFlowControl = ExecutionContext.SuppressFlow();
                    restoreFlow = true;
                }

                this.timer = new Timer(this.OnTimerCallback);
            }
            finally
            {
                // Restore the current ExecutionContext
                if (restoreFlow)
                {
                    ExecutionContext.RestoreFlow();
                    asyncFlowControl.Undo();
                }
            }

            this.ArmTimer(dueTime);
        }

        ~ActorTimer()
        {
            this.Dispose(false);
        }

        public TimeSpan DueTime
        {
            get { return this.dueTime; }
        }

        public TimeSpan Period
        {
            get { return this.period; }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnTimerCallback(object state)
        {
            Task.Factory.StartNew(this.FireTimerAsync);
        }

        private async Task FireTimerAsync()
        {
            var reschedule = true;

            try
            {
                await this.owner.Manager.DispatchToActorAsync<byte[]>(
                    this.owner.Id,
                    this.callbackMethodContext,
                    false,
                    this.DispatchTimerCallback,
                    Guid.NewGuid().ToString(),
                    true,
                    CancellationToken.None);
            }
            catch (ObjectDisposedException)
            {
                // the actor is disposed, do not reschedule the timer
                reschedule = false;
            }
            catch
            {
                // do nothing
            }

            if (reschedule)
            {
                this.ArmTimer(this.period);
            }
            else
            {
                this.CancelTimer();
            }
        }

        private void CancelTimer()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
                this.asyncCallback = null;
            }
        }

        private async Task<byte[]> DispatchTimerCallback(ActorBase actor, CancellationToken cancellationToken)
        {
            if (!ReferenceEquals(actor, this.owner))
            {
                throw new ObjectDisposedException("actor");
            }

            var callback = this.asyncCallback;
            if (callback != null)
            {
                await callback(this.callbackState);
            }

            return null;
        }

        private void ArmTimer(TimeSpan timeSpan)
        {
            var t = this.timer;
            if (t != null)
            {
                try
                {
                    t.Change(timeSpan, Timeout.InfiniteTimeSpan);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.CancelTimer();
        }
    }
}
