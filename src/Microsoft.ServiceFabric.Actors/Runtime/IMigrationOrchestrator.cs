// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    /// <summary>
    /// Interface definition for migraiton operations./>.
    /// </summary>
    public interface IMigrationOrchestrator
    {
        /// <summary>
        /// Gets the Migration ready actor state provider.
        /// </summary>
        /// <returns>Migration ready actor state provider.</returns>
        public IActorStateProvider GetMigrationActorStateProvider();

        /// <summary>
        /// Gets the communication listener to opne migration endpoint.
        /// </summary>
        /// <returns>Migration specific communication listener.</returns>
        public ICommunicationListener GetMigrationCommunicationListener();

        /// <summary>
        /// Starts the migration operation.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on long running taks.</param>
        /// <returns>A task that represents the asynchronous migration operation.</returns>
        public Task StartMigrationAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Starts the downtime phase.
        /// For source service, downtime phase indicates unavailability for any actor operations.
        /// For target service, downtime phase indicates catching up final sequence numbers from source service
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on long running taks.</param>
        /// <returns>A task that represents the asynchronous migration operation.</returns>
        public Task StartDowntimeAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Aborts the migration workflow.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on long running taks.</param>
        /// <returns>A task that represents the asynchronous migration operation.</returns>
        public Task AbortMigrationAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets the actor service state.
        /// A true value indicates state provider is in a state to accept read/write operation.
        /// A false value indicates state provider is either not ready yet or in reject state post migration.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on long running taks.</param>
        /// <returns>
        /// A task that represents the asynchronous migration operation.
        /// A task with true result indicates actor calls are allowed on the actor service, false otherwise
        /// </returns>
        public Task<bool> AreActorCallsAllowedAsync(CancellationToken cancellationToken);
    }
}
