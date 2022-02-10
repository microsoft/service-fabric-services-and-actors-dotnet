// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Data;

    /// <summary>
    /// Represents the interface that an actor state provider needs to implement for
    /// actor runtime to communicate with it.
    /// </summary>
    public interface IActorStateProvider : IStateProviderReplica2
    {
        /// <summary>
        /// Initializes the actor state provider with type information
        /// of the actor type associated with it.
        /// </summary>
        /// <param name="actorTypeInformation">Type information of the actor class</param>
        void Initialize(ActorTypeInformation actorTypeInformation);

        /// <summary>
        /// Invoked as part of the activation process of the actor with the specified actor ID.
        /// </summary>
        /// <param name="actorId">ID of the actor that is activated.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <returns>A task that represents the asynchronous actor activation notification processing.</returns>
        Task ActorActivatedAsync(ActorId actorId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Invoked when a reminder fires and finishes executing its callback
        /// <see cref="IRemindable.ReceiveReminderAsync"/> successfully.
        /// </summary>
        /// <param name="actorId">ID of the actor which own reminder</param>
        /// <param name="reminder">Actor reminder that completed successfully.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous reminder callback completed notification processing.
        /// </returns>
        Task ReminderCallbackCompletedAsync(ActorId actorId, IActorReminder reminder, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Loads the actor state associated with the specified state name for the specified actor ID.
        /// </summary>
        /// <typeparam name="T">Type of value of actor state associated with given state name.</typeparam>
        /// <param name="actorId">ID of the actor for which to load the state.</param>
        /// <param name="stateName">Name of the actor state to load.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="KeyNotFoundException">Actor state associated with specified state name does not exist.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <returns>
        /// A task that represents the asynchronous load operation. The value of TResult
        /// parameter contains value of actor state associated with given state name.
        /// </returns>
        Task<T> LoadStateAsync<T>(ActorId actorId, string stateName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Saves the specified set of actor state changes for the specified actor ID atomically.
        /// </summary>
        /// <param name="actorId">ID of the actor for which to save the state changes.</param>
        /// <param name="stateChanges">Collection of state changes to save.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        /// <remarks>
        /// The collection of state changes should contain only one item for a given state name.
        /// The save operation will fail on trying to add an actor state which already exists
        /// or update/remove an actor state which does not exist.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        /// When <see cref="StateChangeKind"/> is <see cref="StateChangeKind.None"/>
        /// </exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task SaveStateAsync(ActorId actorId, IReadOnlyCollection<ActorStateChange> stateChanges, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Checks whether actor state provider contains an actor state with
        /// specified state name for the specified actor ID.
        /// </summary>
        /// <param name="actorId">ID of the actor for which to check state existence.</param>
        /// <param name="stateName">Name of the actor state to check for existence.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous check operation. The value of TResult
        /// parameter is <c>true</c> if state with specified name exists otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task<bool> ContainsStateAsync(ActorId actorId, string stateName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes all the existing states and reminders associated with specified actor ID atomically.
        /// </summary>
        /// <param name="actorId">ID of the actor for which to remove state.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous remove operation.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task RemoveActorAsync(ActorId actorId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates an enumerable of all the state names associated with specified actor ID.
        /// </summary>
        /// <remarks>
        /// The enumerator returned from actor state provider is safe to use concurrently
        /// with reads and writes to the state provider. It represents a snapshot consistent
        /// view of the state provider.
        /// </remarks>
        /// <param name="actorId">ID of the actor for which to create enumerable.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous enumeration operation. The value of TResult
        /// parameter is an enumerable of all state names associated with specified actor.
        /// </returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task<IEnumerable<string>> EnumerateStateNamesAsync(ActorId actorId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the requested number of ActorID's from the state provider.
        /// </summary>
        /// <param name="numItemsToReturn">Number of items requested to be returned.</param>
        /// <param name="continuationToken">
        /// A continuation token to start querying the results from.
        /// A null value of continuation token means start returning values form the beginning.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation of call to server.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <remarks>
        /// The <paramref name="continuationToken"/> is relative to the state of actor state provider
        /// at the time of invocation of this API. If the state of actor state provider changes (i.e.
        /// new actors are activated or existing actors are deleted) in between calls to this API and
        /// the continuation token from previous call (before the state was modified) is supplied, the
        /// result may contain entries that were already fetched in previous calls.
        /// </remarks>
        Task<PagedResult<ActorId>> GetActorsAsync(int numItemsToReturn, ContinuationToken continuationToken, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the list of reminders from the state provider
        /// </summary>
        /// <param name="numItemsToReturn">Number of items requested to be returned.</param>
        /// <param name="actorId">ActorId for which reminders to be fetched. A null value indicates all actors in the service.</param>
        /// <param name="continuationToken">
        /// A continuation token to start querying the results from.
        /// A null value of continuation token means start returning values form the beginning.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation of call to server.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task<PagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>> GetRemindersAsync(
            int numItemsToReturn,
            ActorId actorId,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken);

        /// <summary>
        /// Saves the specified actor ID reminder. If an actor reminder with
        /// given name does not exist, it adds the actor reminder otherwise
        /// existing actor reminder with same name is updated.
        /// </summary>
        /// <param name="actorId">ID of the actor for which to save the reminder.</param>
        /// <param name="reminder">Actor reminder to save.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task SaveReminderAsync(ActorId actorId, IActorReminder reminder, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the actor reminder with the given reminder name if it exists
        /// </summary>
        /// <param name="actorId">ID of the actor for which to delete the reminder.</param>
        /// <param name="reminderName">Name of the reminder to delete.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task DeleteReminderAsync(ActorId actorId, string reminderName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the specified set of reminders.
        /// </summary>
        /// <param name="reminderNames">The set of reminders to delete.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task DeleteRemindersAsync(IReadOnlyDictionary<ActorId, IReadOnlyCollection<string>> reminderNames, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Loads all the reminders contained in the actor state provider.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for asynchronous load operation.</param>
        /// <returns>
        /// A task that represents the asynchronous load operation. The value of TResult
        /// parameter is a collection of all actor reminders contained in the actor state provider.
        /// </returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        Task<IActorReminderCollection> LoadRemindersAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
