// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Diagnostics;
    using Microsoft.ServiceFabric.Services.Common;
    using System.Globalization;

    /// <summary>
    /// The base class for actors.
    /// </summary>
    /// <remarks>
    /// The base type for actors, that provides the common functionality
    /// for actors that derive from <see cref="Actor"/>.
    /// The state is preserved across actor garbage collections and fail-overs.
    /// The storage and retrieval of the state is provided by the actor state provider. See 
    /// <see cref="IActorStateProvider"/> for more information.
    /// </remarks>
    /// <seealso cref="Actor"/>
    public abstract class ActorBase
    {
        private readonly IActorManager actorManager;
        private readonly ActorId actorId;
        private readonly DiagnosticsManagerActorContext diagnosticsContext;

        private const string TraceType = "ActorBase";
        private readonly string traceId;
        private List<IActorTimer> timers;
        private volatile bool markedForDeletion;

        internal ActorBase(ActorService actorService, ActorId actorId)
        {
            this.actorManager = actorService.ActorManager;
            this.actorId = actorId;

            this.timers = null;
            this.IsDirty = false;
            this.IsInitialized = false;
            this.IsDummy = false;
            this.diagnosticsContext = new DiagnosticsManagerActorContext();

            this.traceId = this.Manager.GetActorTraceId(actorId);
            this.ConcurrencyLock = new ActorConcurrencyLock(this, this.ActorService.Settings.ActorConcurrencySettings);

            var gcSettings = this.actorManager.ActorService.Settings.ActorGarbageCollectionSettings;
            var maxIdleTicks = gcSettings.IdleTimeoutInSeconds / gcSettings.ScanIntervalInSeconds;
            this.GcHandler = new IdleObjectGcHandle(maxIdleTicks);
        }

        /// <summary>
        /// Gets the identity of this actor with the actor service.
        /// </summary>
        /// <value>The <see cref="ActorId"/> for the actor.</value>
        public ActorId Id
        {
            get { return this.actorId; }
        }

        /// <summary>
        /// Gets the name of the application that contains the actor service that is hosting this actor.
        /// </summary>
        /// <value>The name of application that contains the actor service that is hosting this actor.</value>
        public string ApplicationName
        {
            get { return this.actorManager.ActorService.Context.CodePackageActivationContext.ApplicationName; }
        }

        /// <summary>
        /// Gets the URI of the actor service that is hosting this actor.
        /// </summary>
        /// <value>The <see cref="System.Uri"/> of the actor service that is hosting this actor.</value>
        public Uri ServiceUri
        {
            get { return this.actorManager.ActorService.Context.ServiceName; }
        }

        /// <summary>
        /// Gets the stateful service replica that is hosting the actor.
        /// </summary>
        /// <value>
        /// The <see cref="Runtime.ActorService"/> that represents the stateful service replica hosting the actor.
        /// </value>
        public ActorService ActorService
        {
            get { return this.actorManager.ActorService; }
        }

        internal IdleObjectGcHandle GcHandler { get; private set; }

        internal ActorConcurrencyLock ConcurrencyLock { get; private set; }

        internal bool IsDirty { get; set; }

        internal bool IsInitialized { get; set; }

        internal DiagnosticsManagerActorContext DiagnosticsContext
        {
            get { return this.diagnosticsContext; }
        }
        
        internal bool MarkedForDeletion
        {
            get { return this.markedForDeletion; }

            set { this.markedForDeletion = value; }
        }

        internal bool IsDummy { get; set; }

        internal IActorManager Manager
        {
            get { return this.actorManager; }
        }

        /// <summary>
        /// Override this method to initialize the members, initialize state or register timers. This method is called right after the actor is activated
        /// and before any method call or reminders are dispatched on it.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding OnActivateAsync operation.</returns>
        protected virtual Task OnActivateAsync()
        {
            return TaskDone.Done;
        }

        /// <summary>
        ///  Override this method to release any resources. This method is called when actor is deactivated (garbage collected by Actor Runtime).
        ///  Actor operations like state changes should not be called from this method.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding OnDeactivateAsync operation.</returns>
        protected virtual Task OnDeactivateAsync()
        {
            return TaskDone.Done;
        }

        /// <summary>
        /// This method is invoked by actor runtime just before invoking an actor method. Override this method
        /// for performing any actions prior to an actor method is invoked.
        /// </summary>
        /// <param name="actorMethodContext">
        /// An <see cref="ActorMethodContext"/> describing the method that will be invoked by actor runtime after this method finishes.
        /// </param>
        /// <returns>
        /// A <see cref="Task">Task</see> representing pre-actor-method operation.
        /// </returns>
        /// <remarks>
        /// This method is invoked by actor runtime prior to:
        /// <list type="bullet">
        /// <item><description>Invoking an actor interface method when a client request comes.</description></item>
        /// <item><description>Invoking a method on <see cref="IRemindable"/> interface when a reminder fires.</description></item>
        /// <item><description>Invoking a timer callback when timer fires.</description></item>
        /// </list>
        /// </remarks>
        protected virtual Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            return TaskDone.Done;
        }

        /// <summary>
        /// This method is invoked by actor runtime an actor method has finished execution. Override this method
        /// for performing any actions after an actor method has finished execution. 
        /// </summary>
        /// <param name="actorMethodContext">
        /// An <see cref="ActorMethodContext"/> describing the method that was invoked by actor runtime prior to this method.
        /// </param>
        /// <returns>
        /// A <see cref="Task">Task</see> representing post-actor-method operation.
        /// </returns>
        /// /// <remarks>
        /// This method is invoked by actor runtime prior to:
        /// <list type="bullet">
        /// <item><description>Invoking an actor interface method when a client request comes.</description></item>
        /// <item><description>Invoking a method on <see cref="IRemindable"/> interface when a reminder fires.</description></item>
        /// <item><description>Invoking a timer callback when timer fires.</description></item>
        /// </list>
        /// </remarks>
        protected virtual Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            return TaskDone.Done;
        }

        /// <summary>
        /// Unregisters a Timer previously set on this actor.
        /// </summary>
        /// <param name="timer">IActorTimer representing timer that needs to be unregistered..</param>
        protected void UnregisterTimer(IActorTimer timer)
        {
            if ((timer != null) && (this.timers != null))
            {
                if (this.timers.Remove(timer))
                {
                    timer.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets the event for the specified event interface.
        /// </summary>
        /// <typeparam name="TEvent">Event interface type.</typeparam>
        /// <returns>Returns Event that represents the specified interface.</returns>
        protected TEvent GetEvent<TEvent>()
        {
            return this.Manager.GetEvent<TEvent>(this.Id);
        }

        /// <summary>
        /// Gets the actor reminder with specified reminder name.
        /// </summary>
        /// <param name="reminderName">Name of the reminder to get.</param>
        /// <returns>
        /// An <see cref="IActorReminder"/> that represents an actor reminder.
        /// </returns>
        /// <exception cref="ReminderNotFoundException">Reminder not found for the actor.</exception>
        protected IActorReminder GetReminder(string reminderName)
        {
            this.CheckIfReminderOperationIsPossible(reminderName);
            return this.Manager.GetReminder(reminderName, this.Id);
        }

        /// <summary>
        /// Unregisters a reminder previously registered using <see cref="Microsoft.ServiceFabric.Actors.Runtime.ActorBase.RegisterReminderAsync" />.
        /// </summary>
        /// <param name="reminder">The actor reminder to unregister.</param>
        /// <returns>
        /// A task that represents the asynchronous unregistration operation.
        /// </returns>
        /// <exception cref="System.Fabric.FabricException">
        /// The specified reminder is not registered.
        /// </exception>
        protected async Task UnregisterReminderAsync(IActorReminder reminder)
        {
            this.CheckIfReminderOperationIsPossible(reminder.Name);
            await this.Manager.UnregisterReminderAsync(reminder.Name, this.Id, removeFromStateProvider: true);
        }

        /// <summary>
        /// Registers a Timer for the actor.
        /// </summary>
        /// <param name="asyncCallback">
        /// A delegate that specifies a method to be called when the timer fires.
        /// It has one parameter: the state object passed to RegisterTimer.
        /// It returns a <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.
        /// </param>
        /// <param name="state">An object containing information to be used by the callback method, or null.</param>
        /// <param name="dueTime">The amount of time to delay before the async callback is first invoked. 
        /// Specify negative one (-1) milliseconds to prevent the timer from starting. 
        /// Specify zero (0) to start the timer immediately.
        /// </param>
        /// <param name="period">
        /// The time interval between invocations of the async callback. 
        /// Specify negative one (-1) milliseconds to disable periodic signaling.</param>
        /// <returns>Returns IActorTimer object.</returns>
        protected IActorTimer RegisterTimer(
            Func<object, Task> asyncCallback,
            object state,
            TimeSpan dueTime,
            TimeSpan period)
        {   
            if (this.GcHandler.IsGarbageCollected)
            {
                throw new ObjectDisposedException("actor");
            }

            if (this.timers == null)
            {
                this.timers = new List<IActorTimer>();
            }

            var timer = new ActorTimer(this, asyncCallback, state, dueTime, period);
            this.timers.Add(timer);
            return timer;
        }

        /// <summary>
        /// Registers a reminder with the actor.
        /// </summary>
        /// <param name="reminderName">Name of the reminder to register. The name must be unique per actor.</param>
        /// <param name="state">User state passed to the reminder invocation.</param>
        /// <param name="dueTime">The amount of time to delay before invoking the reminder for the first time. Specify negative one (-1) milliseconds to disable invocation. Specify zero (0) to invoke the reminder immediately after registration.
        /// </param>
        /// <param name="period">
        /// The time interval between reminder invocations after the first invocation. Specify negative one (-1) milliseconds to disable periodic invocation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous registration operation. The result of the task provides information about the registered reminder and is used to unregister the reminder using <see cref="Microsoft.ServiceFabric.Actors.Runtime.ActorBase.UnregisterReminderAsync" />.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The class deriving from <see cref="Microsoft.ServiceFabric.Actors.Runtime.ActorBase" /> must implement <see cref="Microsoft.ServiceFabric.Actors.Runtime.IRemindable" /> to consume reminder invocations. Multiple reminders can be registered at any time, uniquely identified by <paramref name="reminderName" />. Existing reminders can also be updated by calling this method again. Reminder invocations are synchronized both with other reminders and other actor method callbacks.
        /// </para>
        /// </remarks>
        protected async Task<IActorReminder> RegisterReminderAsync(
            string reminderName,
            byte[] state,
            TimeSpan dueTime,
            TimeSpan period)
        {
            this.CheckIfReminderOperationIsPossible(reminderName);
            return await this.Manager.RegisterOrUpdateReminderAsync(this.Id, reminderName, state, dueTime, period);
        }

        private void CheckIfReminderOperationIsPossible(string reminderName)
        {
            if (!(this is IRemindable))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, SR.ActorNotIRemindable, reminderName, this.Id));
            }

            if (!this.Manager.HasRemindersLoaded)
            {
                throw new ReminderLoadInProgressException(String.Format(CultureInfo.CurrentCulture, SR.UnregisterReminderConflict, reminderName, this.Id));
            }
        }
        
        internal async Task OnActivateInternalAsync()
        {
            this.Manager.DiagnosticsEventManager.ActorOnActivateAsyncStart(this);
            await this.OnActivateAsync();
            this.Manager.DiagnosticsEventManager.ActorOnActivateAsyncFinish(this);

            this.Manager.TraceSource.WriteInfoWithId(TraceType, this.traceId, "Activated");
        }

        internal virtual async Task OnDeactivateInternalAsync()
        {
            this.Manager.TraceSource.WriteInfoWithId(TraceType, this.traceId, "Deactivating ...");
            if (this.timers != null)
            {
                var toDispose = this.timers.ToArray();
                this.timers.Clear();

                foreach (var t in toDispose)
                {
                    t.Dispose();
                }
            }

            await this.OnDeactivateAsync();
            this.Manager.TraceSource.WriteInfoWithId(TraceType, this.traceId, "Deactivated");
        }

        internal void OnInvokeFailedInternal()
        {
            this.IsDirty = true;
        }
        
        internal Task ResetStateAsyncInternal()
        {
            return this.OnResetStateAsyncInternal();
        }

        internal Task SaveStateAsyncInternal()
        {
            return this.OnSaveStateAsyncInternal();
        }

        internal Task OnPreActorMethodAsyncInternal(ActorMethodContext actorMethodContext)
        {
            return this.OnPreActorMethodAsync(actorMethodContext);
        }

        internal Task OnPostActorMethodAsyncInternal(ActorMethodContext actorMethodContext)
        {
            return this.OnPostActorMethodAsync(actorMethodContext);
        }

        internal abstract Task OnResetStateAsyncInternal();

        internal abstract Task OnSaveStateAsyncInternal();

        internal abstract Task OnPostActivateAsync();
    }
}
