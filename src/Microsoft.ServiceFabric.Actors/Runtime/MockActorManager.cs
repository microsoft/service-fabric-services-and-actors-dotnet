// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Diagnostics;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Services.Common;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    internal sealed class MockActorManager : IActorManager
    {
        private readonly ActorService actorService;
        private readonly ConcurrentDictionary<ActorId, ConcurrentDictionary<string, ActorReminder>> remindersByActorId;
        private readonly ActorEventSource traceSource;

        private IDiagnosticsManager diagnosticsManager;
        private IActorEventManager eventManager;

        internal MockActorManager(ActorService actorService)
        {
            this.actorService = actorService;
            this.diagnosticsManager = new MockDiagnosticsManager(actorService);
            this.eventManager = new MockActorEventManager(actorService.ActorTypeInformation);
            this.remindersByActorId = new ConcurrentDictionary<ActorId, ConcurrentDictionary<string, ActorReminder>>();
            this.traceSource = ActorEventSource.Instance;
            this.IsClosed = false;
        }

        public bool IsClosed { get; private set; }

        public ActorService ActorService
        {
            get { return this.actorService; }
        }

        public bool HasRemindersLoaded
        {
            get
            {
                return true;
            }
        }

        public ActorEventSource TraceSource => this.traceSource;

        public DiagnosticsEventManager DiagnosticsEventManager
        {
            get { return this.diagnosticsManager.DiagnosticsEventManager; }
        }

        private IActorStateProvider StateProvider
        {
            get { return this.actorService.StateProvider; }
        }

        public Task OpenAsync(IServicePartition partition, CancellationToken cancellationToken)
        {
            return TaskDone.Done;
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            this.IsClosed = true;

            return TaskDone.Done;
        }

        public void Abort()
        {
            this.IsClosed = true;
        }

        public Task<byte[]> InvokeAsync(
            ActorId actorId,
            int interfaceId,
            int methodId,
            string callContext,
            byte[] requestMsgBody,
            CancellationToken cancellationToken)
        {
            return TaskDone<byte[]>.Done;
        }

        public Task<IServiceRemotingResponseMessageBody> InvokeAsync(
            ActorId actorId,
            int interfaceId,
            int methodId,
            string callContext,
            IServiceRemotingRequestMessageBody requestMsgBody,
            IServiceRemotingMessageBodyFactory remotingMessageBodyFactory,
            CancellationToken cancellationToken)
        {
            return TaskDone<IServiceRemotingResponseMessageBody>.Done;
        }

        public Task<T> DispatchToActorAsync<T>(
            ActorId actorId,
            ActorMethodContext actorMethodContext,
            bool createIfRequired,
            Func<ActorBase, CancellationToken, Task<T>> actorFunc,
            string callContext,
            bool timerCall,
            CancellationToken cancellationToken)
        {
            return TaskDone<T>.Done;
        }

        public Task SubscribeAsync(ActorId actorId, int eventInterfaceId, IActorEventSubscriberProxy subscriber)
        {
            return this.eventManager.SubscribeAsync(actorId, eventInterfaceId, subscriber);
        }

        public Task UnsubscribeAsync(ActorId actorId, int eventInterfaceId, Guid subscriberId)
        {
            return this.eventManager.UnsubscribeAsync(actorId, eventInterfaceId, subscriberId);
        }

        public TEvent GetEvent<TEvent>(ActorId actorId)
        {
            return (TEvent)(object)this.eventManager.GetActorEventProxy(actorId, typeof(TEvent));
        }

        public async Task<IActorReminder> RegisterOrUpdateReminderAsync(
            ActorId actorId,
            string reminderName,
            byte[] state,
            TimeSpan dueTime,
            TimeSpan period,
            bool saveState = true)
        {
            var reminderDictionary = this.remindersByActorId.GetOrAdd(
                actorId,
                k => new ConcurrentDictionary<string, ActorReminder>());

            var reminder = new ActorReminder(actorId, this, reminderName, state, dueTime, period);

            reminderDictionary.AddOrUpdate(
                reminderName,
                reminder,
                (k, v) =>
                {
                    v.CancelTimer();
                    return reminder;
                });
            try
            {
                if (saveState)
                {
                    await this.StateProvider.SaveReminderAsync(actorId, reminder);
                }
            }
            catch
            {
                reminder.CancelTimer();
                reminderDictionary.TryRemove(reminderName, out reminder);

                throw;
            }

            return reminder;
        }

        public IActorReminder GetReminder(string reminderName, ActorId actorId)
        {
            if (this.remindersByActorId.TryGetValue(actorId, out var actorReminders))
            {
                if (actorReminders.TryGetValue(reminderName, out var reminder))
                {
                    return reminder;
                }

                throw new ReminderNotFoundException(string.Format(SR.ReminderNotFound, reminderName, actorId));
            }

            throw new ReminderNotFoundException(string.Format(SR.ReminderNotFound, reminderName, actorId));
        }

        public async Task UnregisterReminderAsync(
            string reminderName,
            ActorId actorId,
            bool removeFromStateProvider)
        {
            if (removeFromStateProvider)
            {
                await this.StateProvider.DeleteReminderAsync(actorId, reminderName);
            }

            if (this.remindersByActorId.TryGetValue(actorId, out var actorReminders))
            {
                if (actorReminders.TryRemove(reminderName, out var reminder))
                {
                    reminder.Dispose();
                }
                else
                {
                    throw new ReminderNotFoundException(string.Format(SR.ReminderNotFound, reminderName, actorId));
                }
            }
            else
            {
                throw new ReminderNotFoundException(string.Format(SR.ReminderNotFound, reminderName, actorId));
            }
        }

        public Task StartLoadingRemindersAsync(CancellationToken cancellationToken)
        {
            return TaskDone.Done;
        }

        public Task FireReminderAsync(ActorReminder reminder)
        {
            // no-op.
            // Reminders don't fire in mock version.
            return Task.FromResult(true);
        }

        public Task DeleteActorAsync(
            string callContext,
            ActorId actorId,
            CancellationToken cancellationToken)
        {
            return TaskDone.Done;
        }

        public Task<PagedResult<ActorInformation>> GetActorsFromStateProvider(
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new PagedResult<ActorInformation>());
        }

        public string GetActorTraceId(ActorId actorId)
        {
            return string.Empty;
        }
    }
}
