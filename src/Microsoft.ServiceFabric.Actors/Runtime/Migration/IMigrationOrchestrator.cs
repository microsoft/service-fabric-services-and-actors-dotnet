// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    /// <summary>
    /// Interface definition for migraiton operations./>.
    /// </summary>
    internal interface IMigrationOrchestrator
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
        /// Gets the message handler that forwards requests when the current service is unable to service the request.
        /// </summary>
        /// <param name="actorService">Actor service.</param>
        /// <param name="messageHandler">Original message handler.</param>
        /// <param name="requestForwarderFactory">Request forwarder factory.</param>
        /// <returns>Request forwardable message handler.</returns>
        public IServiceRemotingMessageHandler GetMessageHandler(ActorService actorService, IServiceRemotingMessageHandler messageHandler, Func<RequestForwarderContext, IRequestForwarder> requestForwarderFactory);

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
        /// Attempts to resume migration in the event of failover.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation on long running taks.</param>
        /// <returns>True indicates migration resumed post failover, false incase the migration workflow has not previously started.</returns>
        public Task<bool> TryResumeMigrationAsync(CancellationToken cancellationToken);

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
        /// <returns>
        /// A True result indicates actor calls are allowed on the actor service, false otherwise.
        /// </returns>
        public bool AreActorCallsAllowed();

        /// <summary>
        /// Is actor call to be forwarded if the current actor service cannot service the request.
        /// </summary>
        /// <returns>True if the request needs to be forwarded, false otherwise.</returns>
        public bool IsActorCallToBeForwarded();

        /// <summary>
        /// Throws migration exception if actor calls are not allowed.
        /// </summary>
        public void ThrowIfActorCallsDisallowed();

        /// <summary>
        /// Gets the migration start mode.
        /// </summary>
        /// <returns>Return true if the MigrationMode is Auto, false otherwise.</returns>
        public bool IsAutoStartMigration();

        /// <summary>
        /// Register Migration completion callback.
        /// </summary>
        /// <param name="completionCallback">Completion callback</param>
        public void RegisterCompletionCallback(Func<bool, CancellationToken, Task> completionCallback);
    }
}
