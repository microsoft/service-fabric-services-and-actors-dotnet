// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Diagnostics;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Services.Common;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using SR = Microsoft.ServiceFabric.Actors.SR;

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
        private readonly IActorEventManager eventManager;

        private Timer gcTimer;
        private Task loadRemindersTask;

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

        private IActorStateProvider StateProvider
        {
            get { return this.actorService.StateProvider; }
        }

        private IActorActivator ActorActivator
        {
            get { return this.actorService.ActorActivator; }
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
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Aborting...");

            this.isClosed = true;

            this.CleanupRemindersAsync().ContinueWith(t => t.Exception);
            this.DisposeDiagnosticsManager();

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Aborted.");
        }

        public bool IsClosed
        {
            get { return this.isClosed; }
        }

        #endregion

        #region Actor Method Dispatch

#if !DotNetCoreClr
        public Task<byte[]> InvokeAsync(
            ActorId actorId,
            int interfaceId,
            int methodId,
            string callContext,
            byte[] requestMsgBody,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.ThrowIfClosed();

            var methodDispatcher = this.actorService.MethodDispatcherMapV1.GetDispatcher(interfaceId, methodId);
            var actorMethodName = methodDispatcher.GetMethodName(methodId);
            var actorMethodContext = ActorMethodContext.CreateForActor(actorMethodName);

            var deserializationStartTime = DateTime.UtcNow;
            var requestBody = methodDispatcher.DeserializeRequestMessageBody(requestMsgBody);
            this.DiagnosticsEventManager.ActorRequestDeserializationFinish(deserializationStartTime);

            return this.DispatchToActorAsync<byte[]>(
                actorId: actorId,
                actorMethodContext: actorMethodContext,
                createIfRequired: true,
                actorFunc:
                    (actor, innerCancellationToken) =>
                        this.ActorMethodDispatch(methodDispatcher, actor, interfaceId, methodId, requestBody,
                            innerCancellationToken),
                callContext: callContext,
                timerCall: false,
                cancellationToken: cancellationToken);
        }
#endif

        public async Task<T> DispatchToActorAsync<T>(
            ActorId actorId,
            ActorMethodContext actorMethodContext,
            bool createIfRequired,
            Func<ActorBase, CancellationToken, Task<T>> actorFunc,
            string callContext,
            bool timerCall,
            CancellationToken cancellationToken)
        {
            this.ThrowIfClosed();

            ExceptionDispatchInfo exceptionInfo = null;
            Exception exception = null;
            var retval = default(T);


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
                    await
                        actor.ConcurrencyLock.Acquire(callContext,
                            (async innerActor => await this.HandleDirtyStateAsync(innerActor)), cancellationToken);
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
                    lockAcquireFinishTime = this.DiagnosticsEventManager.AcquireActorLockFinish(actor,
                        lockAcquireStartTime);

                    retval =
                        await
                            this.DispatchToActorConcurrencyLockHeldAsync<T>(actorId, actorMethodContext, actor,
                                actorFunc, callContext, cancellationToken);
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
                // signal that current execution is finished on this actor 
                // since there is no call pending or this was the first actor call in the callContext
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

        public Task<IServiceRemotingResponseMessageBody> InvokeAsync(ActorId actorId, int interfaceId, int methodId,
            string callContext,
            IServiceRemotingRequestMessageBody requestMsgBody,
            IServiceRemotingMessageBodyFactory remotingMessageBodyFactory,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.ThrowIfClosed();

            var methodDispatcher = this.actorService.MethodDispatcherMapV2.GetDispatcher(interfaceId, methodId);
            var actorMethodName = methodDispatcher.GetMethodName(methodId);
            var actorMethodContext = ActorMethodContext.CreateForActor(actorMethodName);


            return this.DispatchToActorAsync<IServiceRemotingResponseMessageBody>(
                actorId: actorId,
                actorMethodContext: actorMethodContext,
                createIfRequired: true,
                actorFunc:
                    (actor, innerCancellationToken) =>
                        this.ActorMethodDispatch(methodDispatcher, actor, interfaceId, methodId, requestMsgBody,
                            remotingMessageBodyFactory,
                            innerCancellationToken),
                callContext: callContext,
                timerCall: false,
                cancellationToken: cancellationToken);
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
            return (TEvent)(object)this.eventManager.GetActorEventProxy(actorId, typeof(TEvent));
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
            this.ThrowIfClosed();

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

        public async Task UnregisterReminderAsync(string reminderName, ActorId actorId, bool removeFromStateProvider)
        {
            this.ThrowIfClosed();

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                "Unregistering reminder for actor {0}, reminderName {1}", actorId, reminderName);

            if (removeFromStateProvider)
            {
                await this.StateProvider.DeleteReminderAsync(actorId, reminderName);
            }

            if (this.remindersByActorId.TryGetValue(actorId, out var actorReminders))
            {
                if (actorReminders.TryRemove(reminderName, out var reminder))
                {
                    reminder.Dispose();

                    //
                    // If this was last reminder for this actor, remove entry for actor.
                    // Though space occupied by entry is small, for application that
                    // create and delete actors at high frequency, this will start
                    // piling up redundant memory until the current primary failover happens.
                    //
                    if (actorReminders.Count == 0)
                    {
                        this.remindersByActorId.TryRemove(actorId, out actorReminders);
                    }
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

        public async Task FireReminderAsync(ActorReminder reminder)
        {
            var rearmTimer = true;

            try
            {
                using (var actorScope = this.GetActor(reminder.OwnerActorId, true, false))
                {
                    var actorBase = actorScope.Actor;

                    // if Actor is deleted, reminder should not be fired or armed again.
                    // Its an optimization so that we don't fire the reminder if the actor
                    // is marked for deletion.
                    if (actorBase.MarkedForDeletion)
                    {
                        rearmTimer = false;
                        return;
                    }

                    if (this.actorService.ActorTypeInformation.IsRemindable)
                    {
                        var actor = (IRemindable)actorBase;

                        await this.DispatchToActorAsync<byte[]>(
                            reminder.OwnerActorId,
                            this.reminderMethodContext,
                            false,
                            (async (a, cancellationTkn) =>
                            {
                                await
                                    actor.ReceiveReminderAsync(reminder.Name, reminder.State, reminder.DueTime,
                                        reminder.Period);

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

            // User may delete or update reminder during ReceiveReminderAsync() call.
            // Rearm only if it is still valid.
            if (reminder.IsValid() && rearmTimer)
            {
                if (this.ActorService.Settings.ReminderSettings.AutoDeleteOneTimeReminders &&
                    this.IsOneTimeReminder(reminder))
                {
                    await this.UnregisterOneTimeReminderAsync(reminder);
                }
                else
                {
                    await this.UpdateReminderLastCompletedTimeAsync(reminder);
                    reminder.ArmTimer(reminder.Period);
                }
            }
        }

        #endregion

        #region Actor Query

        public async Task DeleteActorAsync(string callContext, ActorId actorId, CancellationToken cancellationToken)
        {
            ExceptionDispatchInfo exceptionInfo = null;

            if (!this.HasRemindersLoaded)
            {
                throw new ReminderLoadInProgressException(string.Format(CultureInfo.CurrentCulture,
                    SR.DeleteActorConflictWithLoadReminders, actorId));
            }

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                "DeleteActorAsync: Delete call received for actor {0}", actorId);


            // Use ActorConcurrencyLock to synchronize with other actor calls.
            // If the Actor is active, its ActorConcurrencyLock is used for synchronization.
            // If the actor is inactive, a Dummy Actor instance is created and its ActorConcurrencyLock is used for synchronization.
            using (
                var actorUseScope = this.GetActor(actorId: actorId, createIfRequired: true, timerCall: false,
                    createDummyActor: true))
            {
                var actor = actorUseScope.Actor;

                await
                    actor.ConcurrencyLock.Acquire(
                        callContext,
                        (async innerActor => await this.HandleDirtyStateAsync(innerActor)),
                        ActorReentrancyMode.Disallowed,
                        cancellationToken);

                ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                    "DeleteActorAsync: Acquired ReentrancyGuard for actor {0}.", actorId);

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

                        // Remove actor state(and reminders) first and then unregister reminders as RemoveActorState can throw
                        // and in this case reminders should not be unregistered.
                        ActorTrace.Source.WriteInfoWithId(
                            TraceType,
                            this.traceId,
                            "DeleteActorAsync: Removing actor state and reminders for Actor {0}.",
                            actor.Id);

                        await this.StateProvider.RemoveActorAsync(actorId, cancellationToken);

                        ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                            "DeleteActorAsync: Unregistering all reminders for Actor {0}.", actor.Id);

                        if (this.remindersByActorId.TryGetValue(actorId, out var actorReminders))
                        {
                            var reminderNames = actorReminders.Values.Select(r => r.Name).ToList().AsReadOnly();

                            foreach (var reminderName in reminderNames)
                            {
                                await this.UnregisterReminderAsync(reminderName, actor.Id, false);
                            }
                        }

                        ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                            "DeleteActorAsync: Clearing event subscriptions for actor {0}.", actorId);

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
                        // deactivate must happen outside of above try catch to avoid scenarios
                        // in which Remove actor state and reminder from state provider throws.

                        if (this.activeActors.TryRemove(actorId, out var removedActor))
                        {
                            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                                "DeleteActorAsync: Deactivating actor {0}", actorId);

                            await this.DeactivateActorAsync(removedActor);

                            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                                "DeleteActorAsync: Completed Deactivation of actor {0}", actorId);
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
        public async Task<PagedResult<ActorInformation>> GetActorsFromStateProvider(ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            // Get the Actors list from State provider and mark them Active or Inactive
            const int maxCount = PagedResult<ActorInformation>.MaxItemsToReturn;
            var queryResult = await this.StateProvider.GetActorsAsync(maxCount, continuationToken, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var actorInfos =
                queryResult.Items.Select(x => new ActorInformation(x, this.activeActors.ContainsKey(x))).ToList();

            return new PagedResult<ActorInformation>()
            {
                Items = actorInfos,
                ContinuationToken = queryResult.ContinuationToken
            };
        }

        #endregion

        #region Actor Tracing

        public string GetActorTraceId(ActorId actorId)
        {
            return ActorTrace.GetTraceIdForActor(this.actorService.Context.PartitionId,
                this.actorService.Context.ReplicaId, actorId);
        }

        public ActorEventSource TraceSource => ActorTrace.Source;

        #endregion

        #endregion

        #region Actor Method Dispatch Helper Methods

#if !DotNetCoreClr

        private Task<byte[]> ActorMethodDispatch(
            Remoting.V1.Builder.ActorMethodDispatcherBase methodDispatcher,
            ActorBase actor,
            int interfaceId,
            int methodId,
            object requestBody,
            CancellationToken innerCancellationToken)
        {
            var actorInterfaceMethodKey = DiagnosticsEventManager.GetInterfaceMethodKey((uint)interfaceId,
                (uint)methodId);
            this.DiagnosticsEventManager.ActorMethodStart(actorInterfaceMethodKey, actor, RemotingListenerVersion.V1);

            Task<object> dispatchTask;
            try
            {
                dispatchTask = methodDispatcher.DispatchAsync(actor, methodId, requestBody, innerCancellationToken);
            }
            catch (Exception e)
            {
                this.DiagnosticsEventManager.ActorMethodFinish(actorInterfaceMethodKey, actor, e,
                    RemotingListenerVersion.V1);
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
                        this.DiagnosticsEventManager.ActorMethodFinish(actorInterfaceMethodKey, actor, e,
                            RemotingListenerVersion.V1);
                        throw;
                    }
                    this.DiagnosticsEventManager.ActorMethodFinish(actorInterfaceMethodKey, actor, null,
                        RemotingListenerVersion.V1);

                    var serializationStartTime = DateTime.UtcNow;
                    var serializedResponse = methodDispatcher.SerializeResponseMessageBody(responseMsgBody);
                    this.DiagnosticsEventManager.ActorResponseSerializationFinish(serializationStartTime);

                    return serializedResponse;
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }
#endif

        private Task<IServiceRemotingResponseMessageBody> ActorMethodDispatch(
            Remoting.V2.Builder.ActorMethodDispatcherBase methodDispatcher, ActorBase actor, int interfaceId,
            int methodId,
            IServiceRemotingRequestMessageBody requestBody,
            IServiceRemotingMessageBodyFactory remotingMessageBodyFactory, CancellationToken innerCancellationToken)
        {
            var actorInterfaceMethodKey =
                DiagnosticsEventManager.GetInterfaceMethodKey((uint)interfaceId, (uint)methodId);
            this.DiagnosticsEventManager.ActorMethodStart(actorInterfaceMethodKey, actor, RemotingListenerVersion.V2);

            Task<IServiceRemotingResponseMessageBody> dispatchTask;
            try
            {
                dispatchTask = methodDispatcher.DispatchAsync(actor, methodId, requestBody, remotingMessageBodyFactory,
                    innerCancellationToken);
            }
            catch (Exception e)
            {
                this.DiagnosticsEventManager.ActorMethodFinish(actorInterfaceMethodKey, actor, e,
                    RemotingListenerVersion.V2);
                throw;
            }

            return dispatchTask.ContinueWith(
                t =>
                {
                    IServiceRemotingResponseMessageBody responseMsgBody = null;
                    try
                    {
                        responseMsgBody = t.GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        this.DiagnosticsEventManager.ActorMethodFinish(actorInterfaceMethodKey, actor, e,
                            RemotingListenerVersion.V2);
                        throw;
                    }
                    this.DiagnosticsEventManager.ActorMethodFinish(actorInterfaceMethodKey, actor, null,
                        RemotingListenerVersion.V2);


                    return responseMsgBody;
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        private async Task<T> DispatchToActorConcurrencyLockHeldAsync<T>(
            ActorId actorId,
            ActorMethodContext actorMethodContext,
            ActorBase actor,
            Func<ActorBase, CancellationToken, Task<T>> actorFunc,
            string callContext,
            CancellationToken cancellationToken)
        {
            var retval = default(T);

            // if this actor has been deleted or is a dummy actor, then calls must be made on new object.
            if (actor.MarkedForDeletion || actor.IsDummy)
            {
                // Deleted Actor, Method calls will be retried by Actor Proxy.
                throw new ActorDeletedException(string.Format(CultureInfo.CurrentCulture,
                    SR.ActorDeletedExceptionMessage, actorId));
            }

            this.ThrowIfClosed();

            // set the incoming callContext as the new logical call context, before
            // making calls to the actor, so that when the actor makes call this context
            // flows through
            ActorLogicalCallContext.Set(callContext);

            // initialize the actor if needed
            if (ShouldInitialize(actor))
            {
                await this.InitializeAsync(actor);
            }
            try
            {
                // invoke the function of the actor
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

        internal ActorUseScope GetActor(ActorId actorId, bool createIfRequired, bool timerCall,
            bool createDummyActor = false)
        {
            return createIfRequired
                ? this.GetOrCreateActor(actorId, timerCall, createDummyActor)
                : this.GetExistingActor(actorId, timerCall);
        }

        private ActorUseScope GetExistingActor(ActorId actorId, bool timerUse)
        {
            ActorUseScope scope = null;
            if (this.activeActors.TryGetValue(actorId, out var activeActor))
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
                if (!this.activeActors.TryGetValue(actorId, out var actor))
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
            var actor = createDummyActor
                ? this.CreateDummyActor(actorId)
                : this.ActorActivator.Activate(this.actorService, actorId);
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
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                "Reloading state for Actor {0}, since IsDirty state is {1}", actor.Id, actor.IsDirty);

            await ResetStateAsync(actor);
            actor.IsDirty = false;
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
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                "Creating DummyActor to delete inactive actor {0}", actorId);

            return new DummyActor(this.actorService, actorId);
        }

        internal Task OnPreInvokeAsync(ActorBase actor, ActorMethodContext actorMethodContext)
        {
            this.ThrowIfClosed();
            return actor.OnPreActorMethodAsyncInternal(actorMethodContext);
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
                // if the host is closed, mark active actors for early collection rather than waiting till idleTimeout.
                if (this.isClosed)
                {
                    foreach (var activeActor in this.activeActors)
                    {
                        activeActor.Value.GcHandler.MarkForEarlyCollection();
                    }
                }

                if (this.isClosed && (this.activeActors.Count == 0))
                {
                    // all actors are garbaged collected and we are closed
                    // no need to schedule the timer again.
                    this.gcTimer.Dispose();
                    this.gcTimer = null;
                }
                else
                {
                    // Add some randomness to gcTimer firing to handle scenarios in which a Timer firing at exact same interval as
                    // gcTimer can potentially keep the Actor alive forever.
                    double scanIntervalInMilliseconds = 1000 *
                                                        this.actorService.Settings.ActorGarbageCollectionSettings
                                                            .ScanIntervalInSeconds;
                    scanIntervalInMilliseconds += (scanIntervalInMilliseconds * this.random.Next(0, 10)) / 100;
                    this.gcTimer.Change(TimeSpan.FromMilliseconds(scanIntervalInMilliseconds),
                        TimeSpan.FromMilliseconds(-1));
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
                    if (this.activeActors.TryRemove(activeActor.Key, out var deactivatedActor))
                    {
                        deactivatedActors.Add(deactivatedActor);
                    }
                }
            }

            if (deactivatedActors.Count > 0)
            {
#pragma warning disable 4014
                // let the thread continue and deactivate the actors 
                // in asynchronous manner and then reset the GC timer
                this.DeactivateActorsAsync(deactivatedActors);
#pragma warning restore 4014
            }

            this.ArmGcTimer();
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
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

                    ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                        "CleanupRemindersAsync: Waiting for Load reminder task to be finished.");

                    await this.loadRemindersTask;

                    ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                        "CleanupRemindersAsync: Load reminder task has finished.");
                }
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteErrorWithId(TraceType, this.traceId,
                    "CleanupRemindersAsync: Wait for loadRemindersTask failed, exception {0}", e.ToString());
            }

            var remindersForActor = this.remindersByActorId.Values;
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                    "CleanupRemindersAsync: Disposing reminders for {0} actors.", remindersForActor.Count);

            foreach (var reminders in this.remindersByActorId.Values)
            {
                var allReminders = reminders.Values;

                if (allReminders.Count > 0)
                {
                    var actorId = allReminders.First().OwnerActorId;
                    ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                    "CleanupRemindersAsync: Disposing {0} reminders for actor with Id {1}", allReminders.Count, actorId.ToString());

                    foreach (var reminder in allReminders)
                    {
                        reminder.Dispose();
                    }

                    ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                        "CleanupRemindersAsync: Disposed {0} reminders for actor with id {1}", allReminders.Count, actorId.ToString());
                }
            }

            this.remindersByActorId.Clear();

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId,
                    "CleanupRemindersAsync: Disposing of reminders completed for all actors.");
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

            foreach (var actorReminders in reminders)
            {
                var actorId = actorReminders.Key;

                try
                {
                    var remindersToDelete = new List<string>();

                    foreach (var reminderState in actorReminders.Value)
                    {
                        if (this.ActorService.Settings.ReminderSettings.AutoDeleteOneTimeReminders &&
                            reminderState.RemainingDueTime < TimeSpan.Zero &&
                            this.IsOneTimeReminder(reminderState))
                        {
                            remindersToDelete.Add(reminderState.Name);
                            continue;
                        }

                        await this.RegisterOrUpdateReminderAsync(actorId, reminderState, false);
                    }

                    await this.DeleteRemindersSafeAsync(actorId, remindersToDelete, cancellationToken);
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteWarningWithId(
                        TraceType,
                        this.traceId,
                        "Exception encountered while configuring reminder for ActorId={0}. Exception={1}.",
                        actorId.ToString(),
                        ex.ToString());
                }
            }
        }

        private Task RegisterOrUpdateReminderAsync(ActorId actorId, IActorReminderState reminderState,
            bool saveState = true)
        {
            var reminder = new ActorReminder(actorId, this, reminderState);
            return this.RegisterOrUpdateReminderAsync(reminder, reminderState.RemainingDueTime, saveState);
        }

        private async Task RegisterOrUpdateReminderAsync(ActorReminder actorReminder, TimeSpan remainingDueTime,
            bool saveState = true)
        {
            this.ThrowIfClosed();

            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "Registering reminder for actor: ({0}), reminderName: ({1}), remainingDueTime: ({2}),  saveState {3}",
                actorReminder.OwnerActorId,
                actorReminder.Name,
                remainingDueTime,
                saveState);

            var reminderDictionary = this.remindersByActorId.GetOrAdd(actorReminder.OwnerActorId,
                k => new ConcurrentDictionary<string, ActorReminder>());

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
            catch (Exception ex)
            {
                actorReminder.CancelTimer();
                reminderDictionary.TryRemove(actorReminder.Name, out actorReminder);

                if (!(ex is FabricNotPrimaryException))
                {
                    ActorTrace.Source.WriteWarningWithId(
                        TraceType,
                        this.traceId,
                        "Failed to register reminder for actor {0}, reminderName {1}, saveState {2}. Error={3}.",
                        actorReminder.OwnerActorId,
                        actorReminder.Name,
                        saveState,
                        ex.ToString());
                }

                throw;
            }
        }

        private async Task UpdateReminderLastCompletedTimeAsync(ActorReminder reminder)
        {
            try
            {
                this.ThrowIfClosed();

                await
                    this.StateProvider.ReminderCallbackCompletedAsync(reminder.OwnerActorId, reminder,
                        CancellationToken.None);
            }
            catch (FabricNotPrimaryException)
            {
                // Ignore.
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

        private bool IsOneTimeReminder(IActorReminder reminder)
        {
            return (reminder.DueTime > TimeSpan.Zero && reminder.Period < TimeSpan.Zero);
        }

        private async Task UnregisterOneTimeReminderAsync(ActorReminder reminder)
        {
            try
            {
                await this.UnregisterReminderAsync(reminder.Name, reminder.OwnerActorId, true);
            }
            catch (ReminderNotFoundException)
            {
                // User already unregistered the reminder.
            }
            catch (FabricNotPrimaryException)
            {
                // Ignore.
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.traceId,
                    "UnregisterOneTimeReminderAsync(): Failed to unregister reminder for ActorId:[{0}], ReminderName:[{1}]. Exception: {2}",
                    reminder.OwnerActorId,
                    reminder.Name,
                    ex.ToString());
            }
        }

        private async Task DeleteRemindersSafeAsync(ActorId actorId, List<string> remindersToDelete,
            CancellationToken cancellationToken)
        {
            if (remindersToDelete.Count == 0)
            {
                return;
            }

            var deleteIndividually = false;

            try
            {
                var reminderNames = new Dictionary<ActorId, IReadOnlyCollection<string>>
                {
                    { actorId, remindersToDelete }
                };

                await this.StateProvider.DeleteRemindersAsync(reminderNames, cancellationToken);
            }
            catch (FabricMessageTooLargeException)
            {
                deleteIndividually = true;
            }

            if (deleteIndividually)
            {
                var msg = new StringBuilder();
                msg.Append(
                    $"DeleteRemindersSafeAsync(): FabricMessageTooLargeException exception while deleting reminders.");
                msg.Append(
                    $"ActorId:{actorId}, ReminderCount={remindersToDelete.Count}. Switching to individual delete.");

                ActorTrace.Source.WriteWarningWithId(TraceType, this.traceId, msg.ToString());

                foreach (var reminderName in remindersToDelete)
                {
                    await this.StateProvider.DeleteReminderAsync(actorId, reminderName, cancellationToken);
                }
            }
        }

        #endregion

        #region Test Helpers

        internal bool Test_HasAnyReminders()
        {
            return !(this.remindersByActorId.IsEmpty);
        }

        internal bool Test_ReminderDictionaryHasEntry(ActorId actorId)
        {
            return this.remindersByActorId.ContainsKey(actorId);
        }

        #endregion
    }
}
