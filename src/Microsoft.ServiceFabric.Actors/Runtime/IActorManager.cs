// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Diagnostics;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    /// <summary>
    /// Interface for ACtorManager.
    /// </summary>
    internal interface IActorManager
    {
        /// <summary>
        /// Gets the ACtorService isntance.
        /// </summary>
        ActorService ActorService { get; }

        /// <summary>
        /// Gets ActorEventStore instance.
        /// </summary>
        ActorEventSource TraceSource { get; }

        /// <summary>
        /// Gets a value indicating whether reminders have finished loading for allt he actors in the partition.
        /// </summary>
        bool HasRemindersLoaded { get; }

        /// <summary>
        /// Gets the DiagnosticsEventManager isntance.
        /// </summary>
        DiagnosticsEventManager DiagnosticsEventManager { get; }

        #region Actor Manager Life Cycle

        /// <summary>
        /// Gets a value indicating whether ActorManager is closed.
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// Opens ActorManager when replica becomes primary.
        /// </summary>
        /// <param name="partition">Information service about the partition.</param>
        /// <param name="cancellationToken">Token to cancle the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task OpenAsync(IServicePartition partition, CancellationToken cancellationToken);

        /// <summary>
        /// Closes ActorManager when replica changes role from primary to non-priomary role or is closed
        /// </summary>
        /// <param name="cancellationToken">Token to cancle the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CloseAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Aborts ActorManager.
        /// </summary>
        void Abort();

        #endregion

        #region Actor Method Dispatch

#if !DotNetCoreClr
        /// <summary>
        /// Dispatches method call to actor.
        /// </summary>
        /// <param name="actorId">Id of actor.</param>
        /// <param name="interfaceId">Interface id.</param>
        /// <param name="methodId">Method Id.</param>
        /// <param name="callContext">Call context.</param>
        /// <param name="requestMsgBody">Request Message Body.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<byte[]> InvokeAsync(
            ActorId actorId,
            int interfaceId,
            int methodId,
            string callContext,
            byte[] requestMsgBody,
            CancellationToken cancellationToken);
#endif

        /// <summary>
        /// Dispatches method call to actor.
        /// </summary>
        /// <typeparam name="T">Return type of Actor method.</typeparam>
        /// <param name="actorId">Id of actor.</param>
        /// <param name="actorMethodContext">nformation about the method that is invoked by actor runtime</param>
        /// <param name="createIfRequired">True, if actor instacne to be created.</param>
        /// <param name="actorFunc">Actor method to invole.</param>
        /// <param name="callContext">Call context.</param>
        /// <param name="timerCall">True, if its a timer call.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<T> DispatchToActorAsync<T>(
            ActorId actorId,
            ActorMethodContext actorMethodContext,
            bool createIfRequired,
            Func<ActorBase, CancellationToken, Task<T>> actorFunc,
            string callContext,
            bool timerCall,
            CancellationToken cancellationToken);

        // V2 Stack Apis

        /// <summary>
        /// Dispatches method call to actor.
        /// </summary>
        /// <param name="actorId">Id of actor.</param>
        /// <param name="interfaceId">Interface id.</param>
        /// <param name="methodId">Method Id.</param>
        /// <param name="callContext">Call context.</param>
        /// <param name="requestMsgBody">Request Message Body.</param>
        /// <param name="remotingMessageBodyFactory">Factory for creating remtoing request body and response body objects.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<IServiceRemotingResponseMessageBody> InvokeAsync(
            ActorId actorId,
            int interfaceId,
            int methodId,
            string callContext,
            IServiceRemotingRequestMessageBody requestMsgBody,
            IServiceRemotingMessageBodyFactory remotingMessageBodyFactory,
            CancellationToken cancellationToken);

        #endregion

        #region Actor Events

        /// <summary>
        /// Subscribes to an actor event.
        /// </summary>
        /// <param name="actorId">Id of actor to subscribe.</param>
        /// <param name="eventInterfaceId">Id for the event interface to subscribe to.</param>
        /// <param name="subscriber">Represents information about the subscriber proxy.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SubscribeAsync(ActorId actorId, int eventInterfaceId, IActorEventSubscriberProxy subscriber);

        /// <summary>
        /// Unsubscribes from an actor event.
        /// </summary>
        /// <param name="actorId">Id of the actor to unsubscribe events for.</param>
        /// <param name="eventInterfaceId">Id for the event interface to unsubscribe from.</param>
        /// <param name="subscriberId">Subscriber Id.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UnsubscribeAsync(ActorId actorId, int eventInterfaceId, Guid subscriberId);

        /// <summary>
        /// Gets the event for the specified event interface.
        /// </summary>
        /// <typeparam name="TEvent">The Event interface type.</typeparam>
        /// <param name="actorId">Id of the actor to get event for.</param>
        /// <returns>Returns an Event that represents the specified interface.</returns>
        TEvent GetEvent<TEvent>(ActorId actorId);

        #endregion

        /// <summary>
        /// Registers a new actor reminder or updates an already existing reminder.
        /// </summary>
        /// <param name="actorId">Actor id to register reminder for.</param>
        /// <param name="reminderName">Name of reminder to register.</param>
        /// <param name="state">State to store with the rmeinder.</param>
        /// <param name="dueTime">The amount of time to delay before invoking the reminder for the first time. Specify negative one (-1) milliseconds to disable invocation. Specify zero (0) to invoke the reminder immediately after registration.
        /// </param>
        /// <param name="period">
        /// The time interval between reminder invocations after the first invocation. Specify negative one (-1) milliseconds to disable periodic invocation.
        /// </param>
        /// <param name="saveState">true, if the state needs to be saved.</param>
        /// <returns>A task that represents the asynchronous registration operation.</returns>
        Task<IActorReminder> RegisterOrUpdateReminderAsync(
            ActorId actorId,
            string reminderName,
            byte[] state,
            TimeSpan dueTime,
            TimeSpan period,
            bool saveState = true);

        /// <summary>
        /// Gets the specified reminder for the actor.
        /// </summary>
        /// <param name="reminderName">Name of reminder to get.</param>
        /// <param name="actorId">Actor id to get rmeinder for.</param>
        /// <returns>Actor Reminder.</returns>
        IActorReminder GetReminder(string reminderName, ActorId actorId);

        /// <summary>
        /// Unregisters a reminder previously registered.
        /// </summary>
        /// <param name="reminderName">Name of reminder to unregister.</param>
        /// <param name="actorId">Actor id to unregister reminder for.</param>
        /// <param name="removeFromStateProvider">true, if reminder is to be removed from underlying state provider.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UnregisterReminderAsync(string reminderName, ActorId actorId, bool removeFromStateProvider);

        /// <summary>
        /// Method to start loading reminders from the state provider.
        /// </summary>
        /// <param name="cancellationToken">Token to cancelt he operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StartLoadingRemindersAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Fires the actor reminder.
        /// </summary>
        /// <param name="reminder">ActorReminder to fire.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task FireReminderAsync(ActorReminder reminder);

        #region Actor Query

        /// <summary>
        /// Deletes an actor.
        /// </summary>
        /// <param name="callContext">Call context.</param>
        /// <param name="actorId">Actor id to delete.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteActorAsync(string callContext, ActorId actorId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a paged list of actors from state provider.
        /// </summary>
        /// <param name="continuationToken">Continuation to get next items from the paged list.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<PagedResult<ActorInformation>> GetActorsFromStateProvider(
            ContinuationToken continuationToken,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets the list of reminders.
        /// </summary>
        /// <param name="actorId">ActorId for which reminders to be fetched. A null value indicates all actors in the service.</param>
        /// <param name="continuationToken">
        /// A continuation token to start querying the results from.
        /// A null value of continuation token means start returning values form the beginning.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation of call to server.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task<PagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>> GetRemindersFromStateProviderAsync(
            ActorId actorId,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken);

        #endregion

        /// <summary>
        /// Gets the TraceId for actor.
        /// </summary>
        /// <param name="actorId">Id of the actor.</param>
        /// <returns>Trace Id.</returns>
        string GetActorTraceId(ActorId actorId);
    }
}
