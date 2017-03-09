// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Services.Remoting;


    /// <summary>
    /// Defines the interface containing methods which can be called at Actor Service level.
    /// </summary>
    public interface IActorService : IService
    {
        /// <summary>
        /// Gets the list of Actors by querying the actor service.
        /// </summary>
        /// <param name="continuationToken">A continuation token to start querying the results from.
        /// A null value of continuation token means start returning values form the beginning.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task that represents the asynchronous operation of call to server.</returns>
        Task<PagedResult<ActorInformation>> GetActorsAsync(ContinuationToken continuationToken,
            CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an Actor from the Actor service.
        /// </summary>
        /// <param name="actorId"><see cref="ActorId"/> of the actor to be deleted.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task that represents the asynchronous operation of call to server.</returns>
        /// <remarks>
        /// <para>An active actor, will be deactivated and its state will also be deleted from state provider.</para>
        /// <para>An in-active actor's state will be deleted from state provider.</para>
        /// <para>If this method is called for a non-existent actor id in the system, it will be a no-op.</para>
        /// </remarks>
        Task DeleteActorAsync(ActorId actorId, CancellationToken cancellationToken);
    }
}
