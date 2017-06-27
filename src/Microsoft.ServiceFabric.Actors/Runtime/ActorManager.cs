﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Common.Tracing;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Diagnostics;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Actors.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Common;

    internal sealed class ActorManager : IActorManager
    {
        private const string TraceType = "ActorManager";
        private const string ReceiveReminderMethodName = "ReceiveReminderAsync";

        private readonly string traceId;
        private readonly ActorService actorService;
        private readonly Random random = new Random();
        private readonly ActorMethodContext reminderMethodContext;
        private readonly ConcurrentDictionary<ActorId, ActorBase> activeActors;
        private readonly ConcurrentDictionary<ActorId, ConcurrentDictionary<string, ActorReminder>> remindersByActorId;
        private readonly DiagnosticsEventManager diagnosticsEventManager;

        private IDiagnosticsManager diagnosticsManager;
        private bool isClosed;
        private IActorEventManager eventManager;
        private Timer gcTimer;
        private Task loadRemindersTask;

        private IActorStateProvider StateProvider
        {
            get { return this.actorService.StateProvider; }
        }

        private IActorActivator ActorActivator
        {
            get { return this.actorService.ActorActivator; }
        }

        internal ActorManager(ActorService actorService)
        {
            this.actorService = actorService;
            this.traceId = actorService.Context.TraceId;
            this.diagnosticsManager = new DiagnosticsManager(actorService);
            this.diagnosticsEventManager = this.diagnosticsManager.DiagnosticsEventManager;
            this.eventManager = new ActorEventManager(actorService.ActorTypeInformation);

            this.isClosed = false;
            this.activeActors = new ConcurrentDictionary<ActorId, ActorBase>();
            this.remindersByActorId = new ConcurrentDictionary<ActorId, ConcurrentDictionary<string, ActorReminder>>();
            this.reminderMethodContext = ActorMethodContext.CreateForReminder(ReceiveReminderMethodName);
            this.gcTimer = new Timer(this.RunGarbageCollection, null, Timeout.Infinite, Timeout.Infinite);
        }

        #region IActorManager Implementation

        public ActorService ActorService
        {
            get { return this.actorService; }
        }

        #region Actor Diagnostics

        public DiagnosticsEventManager DiagnosticsEventManager
        {
            get { return this.diagnosticsEventManager; }
        }

        #endregion

        #region Actor Manager Life Cycle

        public Task OpenAsync(IServicePartition partition, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Opening ...");

            this.ThrowIfClosed();
            this.ArmGcTimer();

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Opened.");

            return TaskDone.Done;
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Closing ...");
            this.isClosed = true;

            await this.CleanupRemindersAsync();
            this.DisposeDiagnosticsManager();

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Closed.");
        }

        public void Abort()
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Abort.");

            this.isClosed = true;
            this.DisposeDiagnosticsManager();
        }

        #endregion

        #region Actor Method Dispatch

        public Task<byte[]> InvokeAsync(
            ActorId actorId,
            int interfaceId,
            int methodId,
            string callContext,
            byte[] requestMsgBody,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var methodDispatcher = this.actorService.MethodDispatcherMap.GetDispatcher(interfaceId, methodId);
            var actorMethodName = methodDispatcher.GetMethodName(methodId);
            var actorMethodContext = ActorMethodContext.CreateForActor(actorMethodName);

            var deserializationStartTime = DateTime.UtcNow;
            var requestBody = methodDispatcher.DeserializeRequestMessageBody(requestMsgBody);
            this.DiagnosticsEventManager.ActorRequestDeserializationFinish(deserializationStartTime);

            return this.DispatchToActorAsync(
                actorId: actorId,
                actorMethodContext: actorMethodContext,
                createIfRequired: true,
                actorFunc:
                    (actor, innerCancellationToken) =>
                        this.ActorMethodDispatch(methodDispatcher, actor, interfaceId, methodId, requestBody, innerCancellationToken),
                callContext: callContext,
                timerCall: false,
                cancellationToken: cancellationToken);
        }

        public async Task<byte[]> DispatchToActorAsync(
            ActorId actorId,
            ActorMethodContext actorMethodContext,
            bool createIfRequired,
            Func<ActorBase, CancellationToken, Task<byte[]>> actorFunc,
            string callContext,
            bool timerCall,
            CancellationToken cancellationToken)
        {
            ExceptionDispatchInfo exceptionInfo = null;
            Exception exception = null;
            byte[] retval = null;


            // get activeActor from the activeActors table
            using (var actorUseScope = this.GetActor(actorId, createIfRequired, timerCall))
            {
                var actor = actorUseScope.Actor;

                //
                // START: CRITICAL CODE
                //
                // Emit diagnostic info - before acquiring actor lock
                var lockAcquireStartTime = this.DiagnosticsEventManager.AcquireActorLockStart(actor);
                DateTime? lockAcquireFinishTime = null;
                try
                {
                    await actor.ConcurrencyLock.Acquire(callContext, (async innerActor => await this.HandleDirtyStateAsync(innerActor)), cancellationToken);
                }
                catch
                {
                    // Emit diagnostic info - failed to acquire actor lock
                    this.DiagnosticsEventManager.AcquireActorLockFailed(actor);
                    throw;
                }
                //
                // WARNING: DO NOT PUT ANY CODE BETWEEN CONCURRENCY LOCK ACQUIRE AND TRY
                // THE LOCK NEEDS TO BE RELEASED IF THERE IS ANY EXCEPTION 
                // 
                try
                {
                    // Emit diagnostic info - after acquiring actor lock
                    lockAcquireFinishTime = this.DiagnosticsEventManager.AcquireActorLockFinish(actor, lockAcquireStartTime);

                    retval = await this.DispatchToActorConcurrencyLockHeldAsync(actorId, actorMethodContext, actor, actorFunc, callContext, cancellationToken);
                }
                catch (Exception e)
                {
                    exception = e;
                    try
                    {
                        exceptionInfo = ExceptionDispatchInfo.Capture(e);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                //
                // WARNING: DO NOT PUT ANY CODE BELOW BEFORE THE LOCK IS RELEASED
                // BECAUSE WE ARE NOT INSIDE A TRY-CATCH BLOCK
                //
                // Signal that current execution is finished on this actor 
                // since there is no call pending or this was the first actor call in the callContext.
                await actor.ConcurrencyLock.ReleaseContext(callContext);

                // Emit diagnostic info - after releasing actor lock
                this.DiagnosticsEventManager.ReleaseActorLock(lockAcquireFinishTime);

                //
                // END: CRITICAL CODE
                //

                if (exceptionInfo != null)
                {
                    exceptionInfo.Throw();
                }
                else if (exception != null)
                {
                    throw exception;
                }

                return retval;
            }
        }

        #endregion

        #region Actor Events

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
            return (TEvent) (object) this.eventManager.GetActorEventProxy(actorId, typeof(TEvent));
        }

        #endregion

        #region Actor Reminders

        public bool HasRemindersLoaded
        {
            get { return (this.loadRemindersTask != null && this.loadRemindersTask.IsCompleted); }
        }

        public async Task<IActorReminder> RegisterOrUpdateReminderAsync(
            ActorId actorId,
            string reminderName,
            byte[] state,
            TimeSpan dueTime,
            TimeSpan period,
            bool saveState = true)
        {
            var reminder = new ActorReminder(actorId, this, reminderName, state, dueTime, period);
            await this.RegisterOrUpdateReminderAsync(reminder, dueTime, saveState);

            return reminder;
        }

        public IActorReminder GetReminder(string reminderName, ActorId actorId)
        {
            ConcurrentDictionary<string, ActorReminder> actorReminders;
            if (this.remindersByActorId.TryGetValue(actorId, out actorReminders))
            {
                ActorReminder reminder;
                if (actorReminders.TryGetValue(reminderName, out reminder))
                {
                    return reminder;
                }

                throw new ReminderNotFoundException(string.Format(SR.ReminderNotFound, reminderName, actorId));
            }

            throw new ReminderNotFoundException(string.Format(SR.ReminderNotFound, reminderName, actorId));
        }

        public async Task UnregisterReminderAsync(string reminderName, ActorId actorId, bool removeFromStateProvider)
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Unregistering reminder for actor {0}, reminderName {1}", actorId, reminderName);

            if (removeFromStateProvider)
            {
                await this.StateProvider.DeleteReminderAsync(actorId, reminderName);
            }

            ConcurrentDictionary<string, ActorReminder> actorReminders;
            if (this.remindersByActorId.TryGetValue(actorId, out actorReminders))
            {
                ActorReminder reminder;
                if (actorReminders.TryRemove(reminderName, out reminder))
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
            this.loadRemindersTask = this.LoadRemindersAsync(cancellationToken);
            return this.loadRemindersTask;
        }

        public async void FireReminder(ActorReminder reminder)
        {
            var rearmTimer = true;

            try
            {
                using (var actorScope = this.GetActor(reminder.OwnerActorId, true, false))
                {
                    var actorBase = actorScope.Actor;

                    // If Actor is deleted, reminder should not be fired or armed again.
                    // Its an optimization so that we don't fire the reminder if the actor
                    // is marked for deletion.
                    if (actorBase.MarkedForDeletion)
                    {
                        rearmTimer = false;
                        return;
                    }

                    if (this.actorService.ActorTypeInformation.IsRemindable)
                    {
                        var actor = (IRemindable) actorBase;

                        await this.DispatchToActorAsync(
                            reminder.OwnerActorId,
                            this.reminderMethodContext,
                            false,
                            (async (a, cancellationTkn) =>
                            {
                                await actor.ReceiveReminderAsync(reminder.Name, reminder.State, reminder.DueTime, reminder.Period);

                                return null;
                            }),
                            Guid.NewGuid().ToString(),
                            false,
                            CancellationToken.None);
                    }
                }
            }
            catch (ActorDeletedException)
            {
                rearmTimer = false;
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.traceId,
                    "Firing reminder {0} for actor {1} caused exception: {2}",
                    reminder.Name,
                    reminder.OwnerActorId,
                    e.ToString());
            }

            if (rearmTimer)
            {
                await this.UpdateReminderLastCompletedTimeAsync(reminder);
                reminder.ArmTimer(reminder.Period);
            }
        }

        #endregion

        #region Actor Query

        public async Task DeleteActorAsync(string callContext, ActorId actorId, CancellationToken cancellationToken)
        {
            ExceptionDispatchInfo exceptionInfo = null;

            if (!this.HasRemindersLoaded)
            {
                throw new ReminderLoadInProgressException(string.Format(CultureInfo.CurrentCulture, SR.DeleteActorConflictWithLoadReminders, actorId));
            }

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "DeleteActorAsync: Delete call received for actor {0}", actorId);


            // Uses ActorConcurrencyLock to synchronize with other actor calls.
            // If the Actor is active, its ActorConcurrencyLock is used for synchronization.
            // If the actor is inactive, a Dummy Actor instance is created and its ActorConcurrencyLock is used for synchronization.
            using (var actorUseScope = this.GetActor(actorId: actorId, createIfRequired: true, timerCall: false, createDummyActor: true))
            {
                var actor = actorUseScope.Actor;

                await
                    actor.ConcurrencyLock.Acquire(
                        callContext,
                        (async innerActor => await this.HandleDirtyStateAsync(innerActor)),
                        ActorReentrancyMode.Disallowed,
                        cancellationToken);

                ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "DeleteActorAsync: Acquired ReentrancyGuard for actor {0}.", actorId);

                // If Actor is already marked for deletion by other delete call, do not try to delete it again.
                if (actor.MarkedForDeletion)
                {
                    ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.traceId,
                        "DeleteActorAsync: Actor {0} is already marked for deletion, returning without processing this delete call.",
                        actorId);
                }
                else
                {
                    try
                    {
                        this.ThrowIfClosed();
                        cancellationToken.ThrowIfCancellationRequested();
                        actor.MarkedForDeletion = true;

                        // Removes actor state (and reminders) first and then unregister reminders as RemoveActorState can throw
                        // and in this case reminders should not be unregistered.
                        ActorTrace.Source.WriteInfoWithId(
                            TraceType,
                            this.traceId,
                            "DeleteActorAsync: Removing actor state and reminders for Actor {0}.",
                            actor.Id);

                        await this.StateProvider.RemoveActorAsync(actorId, cancellationToken);

                        ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "DeleteActorAsync: Unregistering all reminders for Actor {0}.", actor.Id);

                        ConcurrentDictionary<string, ActorReminder> actorReminders;
                        if (this.remindersByActorId.TryGetValue(actorId, out actorReminders))
                        {
                            var reminderNames = actorReminders.Values.Select(r => r.Name).ToList().AsReadOnly();

                            foreach (var reminderName in reminderNames)
                            {
                                await this.UnregisterReminderAsync(reminderName, actor.Id, false);
                            }
                        }

                        ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "DeleteActorAsync: Clearing event subscriptions for actor {0}.", actorId);

                        await this.eventManager.ClearAllSubscriptions(actorId);
                    }
                    catch (Exception e)
                    {
                        ActorTrace.Source.WriteInfoWithId(
                            TraceType,
                            this.traceId,
                            "DeleteActorAsync: Removing state for actor {0} caused exception {1}, {2}.",
                            actorId,
                            e.Message,
                            e.StackTrace);

                        exceptionInfo = ExceptionDispatchInfo.Capture(e);
                    }

                    try
                    {
                        // Deactivate must happen outside of above try catch to avoid scenarios
                        // in which Remove actor state and reminder from state provider throws.

                        ActorBase removedActor;
                        if (this.activeActors.TryRemove(actorId, out removedActor))
                        {
                            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "DeleteActorAsync: Deactivating actor {0}", actorId);

                            await this.DeactivateActorAsync(removedActor);

                            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "DeleteActorAsync: Completed Deactivation of actor {0}", actorId);
                        }
                    }
                    catch (Exception e)
                    {
                        // Catch exception as ReentrantGuard must be released.
                        ActorTrace.Source.WriteInfoWithId(
                            TraceType,
                            this.traceId,
                            "DeleteActorAsync: Deactivating actor {0} caused exception {1}, {2}.",
                            actorId,
                            e.Message,
                            e.StackTrace);

                        exceptionInfo = ExceptionDispatchInfo.Capture(e);
                    }
                }

                await actor.ConcurrencyLock.ReleaseContext(callContext);

                if (exceptionInfo != null)
                {
                    exceptionInfo.Throw();
                }

                return;
            }
        }

        /// <summary>
        /// Returns Actors list by querying state provider for Actors.
        /// </summary>
        public async Task<PagedResult<ActorInformation>> GetActorsFromStateProvider(ContinuationToken continuationToken, CancellationToken cancellationToken)
        {
            // Gets the Actors list from State provider and mark them Active or Inactive.
            const int maxCount = PagedResult<ActorInformation>.MaxItemsToReturn;
            var queryResult = await this.StateProvider.GetActorsAsync(maxCount, continuationToken, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var actorInfos = queryResult.Items.Select(x => new ActorInformation(x, this.activeActors.ContainsKey(x))).ToList();

            return new PagedResult<ActorInformation>() {Items = actorInfos, ContinuationToken = queryResult.ContinuationToken};
        }

        #endregion

        #region Actor Tracing

        public string GetActorTraceId(ActorId actorId)
        {
            return ActorTrace.GetTraceIdForActor(this.actorService.Context.PartitionId, this.actorService.Context.ReplicaId, actorId);
        }

        public FabricEvents.ExtensionsEvents TraceSource
        {
            get { return ActorTrace.Source; }
        }

        #endregion

        #endregion

        #region Actor Method Dispatch Helper Methods

        private Task<byte[]> ActorMethodDispatch(
            ActorMethodDispatcherBase methodDispatcher,
            ActorBase actor,
            int interfaceId,
            int methodId,
            object requestBody,
            CancellationToken innerCancellationToken)
        {
            long actorInterfaceMethodKey = DiagnosticsEventManager.GetInterfaceMethodKey((uint) interfaceId, (uint) methodId);
            this.DiagnosticsEventManager.ActorMethodStart(actorInterfaceMethodKey, actor);

            Task<object> dispatchTask;
            try
            {
                dispatchTask = methodDispatcher.DispatchAsync(actor, methodId, requestBody, innerCancellationToken);
            }
            catch (Exception e)
            {
                this.DiagnosticsEventManager.ActorMethodFinish(actorInterfaceMethodKey, actor, e);
                throw;
            }

            return dispatchTask.ContinueWith(
                t =>
                {
                    object responseMsgBody = null;
                    try
                    {
                        responseMsgBody = t.GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        this.DiagnosticsEventManager.ActorMethodFinish(actorInterfaceMethodKey, actor, e);
                        throw;
                    }
                    this.DiagnosticsEventManager.ActorMethodFinish(actorInterfaceMethodKey, actor, null);

                    var serializationStartTime = DateTime.UtcNow;
                    var serializedResponse = methodDispatcher.SerializeResponseMessageBody(responseMsgBody);
                    this.DiagnosticsEventManager.ActorResponseSerializationFinish(serializationStartTime);

                    return serializedResponse;
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        private async Task<byte[]> DispatchToActorConcurrencyLockHeldAsync(
            ActorId actorId,
            ActorMethodContext actorMethodContext,
            ActorBase actor,
            Func<ActorBase, CancellationToken, Task<byte[]>> actorFunc,
            string callContext,
            CancellationToken cancellationToken)
        {
            byte[] retval = null;

            // If this actor has been deleted or is a dummy actor, then calls must be made on new object.
            if (actor.MarkedForDeletion || actor.IsDummy)
            {
                // Deleted Actor, Method calls will be retried by Actor Proxy.
                throw new ActorDeletedException(string.Format(CultureInfo.CurrentCulture, SR.ActorDeletedExceptionMessage, actorId));
            }

            this.ThrowIfClosed();

            // Sets the incoming callContext as the new logical call context, before
            // making calls to the actor, so that when the actor makes call this context
            // flows through.
            ActorLogicalCallContext.Set(callContext);

            // Initializes the actor if needed.
            if (ShouldInitialize(actor))
            {
                await this.InitializeAsync(actor);
            }
            try
            {
                // Invokes the function of the actor.
                await this.OnPreInvokeAsync(actor, actorMethodContext);
                retval = await actorFunc.Invoke(actor, cancellationToken);
                await this.OnPostInvokeAsync(actor, actorMethodContext);
            }
            catch
            {
                actor.OnInvokeFailedInternal();
                throw;
            }

            return retval;
        }

        #endregion

        #region Actor Lifecycle and State Management Helper Methods

        internal ActorUseScope GetActor(ActorId actorId, bool createIfRequired, bool timerCall, bool createDummyActor = false)
        {
            return createIfRequired ? this.GetOrCreateActor(actorId, timerCall, createDummyActor) : this.GetExistingActor(actorId, timerCall);
        }

        private ActorUseScope GetExistingActor(ActorId actorId, bool timerUse)
        {
            ActorUseScope scope = null;
            ActorBase activeActor;
            if (this.activeActors.TryGetValue(actorId, out activeActor))
            {
                scope = ActorUseScope.TryCreate(activeActor, timerUse);
            }

            if (scope == null)
            {
                throw new ObjectDisposedException("actor");
            }

            return scope;
        }

        private ActorUseScope GetOrCreateActor(ActorId actorId, bool timerUse, bool createDummyActor)
        {
            ActorUseScope scope = null;
            var sw = new SpinWait();
            while (scope == null)
            {
                ActorBase actor;
                if (!this.activeActors.TryGetValue(actorId, out actor))
                {
                    actor = this.activeActors.GetOrAdd(actorId, l => this.CreateActor(actorId, createDummyActor));
                }

                scope = ActorUseScope.TryCreate(actor, timerUse);
                if (scope == null)
                {
                    sw.SpinOnce();
                }
            }

            return scope;
        }

        private ActorBase CreateActor(ActorId actorId, bool createDummyActor)
        {
            var actor = createDummyActor ? this.CreateDummyActor(actorId) : this.ActorActivator.Activate(this.actorService, actorId);
            return actor;
        }

        private static bool ShouldInitialize(ActorBase actor)
        {
            return (actor.IsInitialized != true);
        }

        private async Task InitializeAsync(ActorBase actor)
        {
            actor.IsInitialized = false;

            await this.OnPreActivateAsync(actor);

            await actor.OnActivateInternalAsync();

            actor.IsInitialized = true;

            await this.OnPostActivateAsync(actor);

            this.DiagnosticsEventManager.ActorActivated(actor);
        }

        internal Task OnPreActivateAsync(ActorBase actor)
        {
            return ResetStateAsync(actor);
        }

        internal async Task OnPostActivateAsync(ActorBase actor)
        {
            await this.StateProvider.ActorActivatedAsync(actor.Id);
            await actor.OnPostActivateAsync();
        }

        internal async Task HandleDirtyStateAsync(ActorBase actor)
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Reloading state for Actor {0}, since IsDirty state is {1}", actor.Id, actor.IsDirty);

            await ResetStateAsync(actor);
            actor.IsDirty = false;
        }

        internal Task RemoveActorStateAsync(ActorBase actor)
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Deleting state for Actor {0}, as delete actor call was made for it.", actor.Id);

            return this.StateProvider.RemoveActorAsync(actor.Id);
        }

        private static Task ResetStateAsync(ActorBase actor)
        {
            return actor.ResetStateAsyncInternal();
        }

        private static Task SaveStateAsync(ActorBase actor)
        {
            return actor.SaveStateAsyncInternal();
        }

        internal ActorBase CreateDummyActor(ActorId actorId)
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Creating DummyActor to delete inactive actor {0}", actorId);

            return new DummyActor(this.actorService, actorId);
        }

        internal async Task OnPreInvokeAsync(ActorBase actor, ActorMethodContext actorMethodContext)
        {
            this.ThrowIfClosed();
            await actor.OnPreActorMethodAsyncInternal(actorMethodContext);
        }

        internal async Task OnPostInvokeAsync(ActorBase actor, ActorMethodContext actorMethodContext)
        {
            this.ThrowIfClosed();
            await actor.OnPostActorMethodAsyncInternal(actorMethodContext);
            await SaveStateAsync(actor);
        }

        #endregion

        #region Actor Garbage Collection

        private void ArmGcTimer()
        {
            if (this.gcTimer != null)
            {
                // Marks active actors for early collection rather than waiting till idleTimeout if the host is closed.
                if (this.isClosed)
                {
                    foreach (var activeActor in this.activeActors)
                    {
                        activeActor.Value.GcHandler.MarkForEarlyCollection();
                    }
                }

                if (this.isClosed && (this.activeActors.Count == 0))
                {
                    // All actors are garbaged collected and we are closed
                    // no need to schedule the timer again.
                    this.gcTimer.Dispose();
                    this.gcTimer = null;
                }
                else
                {
                    // Adds some randomness to gcTimer firing to handle scenarios in which a Timer firing at exact same interval as
                    // gcTimer can potentially keep the Actor alive forever.
                    double scanIntervalInMilliseconds = 1000 * this.actorService.Settings.ActorGarbageCollectionSettings.ScanIntervalInSeconds;
                    scanIntervalInMilliseconds += (scanIntervalInMilliseconds * this.random.Next(0, 10)) / 100;
                    this.gcTimer.Change(TimeSpan.FromMilliseconds(scanIntervalInMilliseconds), TimeSpan.FromMilliseconds(-1));
                }
            }
        }

        private void RunGarbageCollection(object state)
        {
            var deactivatedActors = new List<ActorBase>();

            foreach (var activeActor in this.activeActors)
            {
                if (activeActor.Value.GcHandler.TryCollect())
                {
                    ActorBase deactivatedActor;
                    if (this.activeActors.TryRemove(activeActor.Key, out deactivatedActor))
                    {
                        deactivatedActors.Add(deactivatedActor);
                    }
                }
            }

            if (deactivatedActors.Count > 0)
            {
#pragma warning disable 4014
                // Lets the thread continue and deactivate the actors 
                // in asynchronous manner and then reset the GC timer.
                this.DeactivateActorsAsync(deactivatedActors);
#pragma warning restore 4014
            }

            this.ArmGcTimer();
        }

        // Disables the ReSharper once UnusedMethodReturnValue.Local.
        private Task DeactivateActorsAsync(IEnumerable<ActorBase> deactivatedActors)
        {
            var deactivateTasks = new List<Task>();
            foreach (var a in deactivatedActors)
            {
                try
                {
                    deactivateTasks.Add(this.DeactivateActorAsync(a));
                }
                catch
                {
                    // ignored
                }
            }

            return Task.WhenAll(deactivateTasks.ToArray());
        }

        private Task OnPostDeactivateAsync(ActorBase actor)
        {
            return TaskDone.Done;
        }

        private async Task DeactivateActorAsync(ActorBase actor)
        {
            if (actor.IsInitialized && !(actor.IsDummy))
            {
                await actor.OnDeactivateInternalAsync();

                await this.OnPostDeactivateAsync(actor);

                this.DiagnosticsEventManager.ActorDeactivated(actor);
            }
        }

        #endregion

        #region Helper Methods

        private void ThrowIfClosed()
        {
            if (this.isClosed)
            {
                throw new FabricNotPrimaryException();
            }
        }

        private void DisposeDiagnosticsManager()
        {
            if (this.diagnosticsManager != null)
            {
                this.diagnosticsManager.Dispose();
                this.diagnosticsManager = null;
            }
        }

        private async Task CleanupRemindersAsync()
        {
            try
            {
                if (this.loadRemindersTask != null)
                {
                    await this.loadRemindersTask;
                }
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteErrorWithId(TraceType, this.traceId, "Wait for loadRemindersTask failed, exception {0}", e.ToString());
            }

            foreach (var reminders in this.remindersByActorId.Values)
            {
                foreach (var reminder in reminders.Values)
                {
                    reminder.Dispose();
                }
            }
        }

        private async Task LoadRemindersAsync(CancellationToken cancellationToken)
        {
            var reminders = await this.StateProvider.LoadRemindersAsync(cancellationToken);

            if (reminders.Count > 0 && !this.actorService.ActorTypeInformation.IsRemindable)
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.traceId,
                    "LoadRemindersAsync: ActorStateProvider has {0} reminders but actor is not remindable.",
                    reminders.Count);

                return;
            }

            if (this.actorService.ActorTypeInformation.IsRemindable)
            {
                foreach (var actorReminders in reminders)
                {
                    var actorId = actorReminders.Key;

                    try
                    {
                        foreach (var reminderState in actorReminders.Value)
                        {
                            await this.RegisterOrUpdateReminderAsync(actorId, reminderState, false);
                        }
                    }
                    catch
                    {
                        ActorTrace.Source.WriteInfoWithId(
                            TraceType,
                            this.traceId,
                            "Exception encountered while configuring reminder for ActorID {0}.",
                            actorId.ToString());
                    }
                }
            }
        }

        private Task RegisterOrUpdateReminderAsync(ActorId actorId, IActorReminderState reminderState, bool saveState = true)
        {
            var reminder = new ActorReminder(actorId, this, reminderState);
            return this.RegisterOrUpdateReminderAsync(reminder, reminderState.RemainingDueTime, saveState);
        }

        private async Task RegisterOrUpdateReminderAsync(ActorReminder actorReminder, TimeSpan remainingDueTime, bool saveState = true)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "Registering reminder for actor: ({0}), reminderName: ({1}), remainingDueTime: ({2}),  saveState {3}",
                actorReminder.OwnerActorId,
                actorReminder.Name,
                remainingDueTime,
                saveState);

            var reminderDictionary = this.remindersByActorId.GetOrAdd(actorReminder.OwnerActorId, k => new ConcurrentDictionary<string, ActorReminder>());

            reminderDictionary.AddOrUpdate(
                actorReminder.Name,
                actorReminder,
                (k, v) =>
                {
                    v.CancelTimer();
                    return actorReminder;
                });

            try
            {
                if (saveState)
                {
                    await this.StateProvider.SaveReminderAsync(actorReminder.OwnerActorId, actorReminder);
                }

                actorReminder.ArmTimer(remainingDueTime);
            }
            catch
            {
                actorReminder.CancelTimer();
                reminderDictionary.TryRemove(actorReminder.Name, out actorReminder);

                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    this.traceId,
                    "Failed to register reminder for actor {0}, reminderName {1}, saveState {2}",
                    actorReminder.OwnerActorId,
                    actorReminder.Name,
                    saveState);

                throw;
            }
        }

        private async Task UpdateReminderLastCompletedTimeAsync(ActorReminder reminder)
        {
            try
            {
                await this.StateProvider.ReminderCallbackCompletedAsync(reminder.OwnerActorId, reminder, CancellationToken.None);
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.traceId,
                    "Failed to update last completed time for ActorId: ({0}), ReminderName: ({1}). Exception: {2}",
                    reminder.OwnerActorId,
                    reminder.Name,
                    ex.ToString());
            }
        }

        #endregion
    }
}
