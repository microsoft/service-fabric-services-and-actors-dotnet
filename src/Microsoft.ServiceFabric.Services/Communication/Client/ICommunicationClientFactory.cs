// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Client;

    /// <summary>
    /// Defines the interface that must be implemented to provide a factory for communication clients to talk to a service fabric service.
    /// </summary>
    /// <typeparam name="TCommunicationClient">Type of communication client.</typeparam>
    public interface ICommunicationClientFactory<TCommunicationClient>
        where TCommunicationClient : ICommunicationClient
    {
        /// <summary>
        /// Event handler that is fired when the Communication client connects to the service endpoint.
        /// </summary>
        event EventHandler<CommunicationClientEventArgs<TCommunicationClient>> ClientConnected;

        /// <summary>
        /// Event handler that is fired when the Communication client disconnects from the service endpoint.
        /// </summary>
        event EventHandler<CommunicationClientEventArgs<TCommunicationClient>> ClientDisconnected;

        /// <summary>
        /// Resolves a partition of the specified service containing one or more communication listeners and returns a client to communicate
        /// to the endpoint corresponding to the given listenerName.
        ///
        /// The endpoint of the service is of the form - {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}.
        /// </summary>
        /// <param name="serviceUri">Uri of the service to resolve.</param>
        /// <param name="partitionKey">Key that identifies the partition to resolve.</param>
        /// <param name="targetReplicaSelector">Specifies which replica in the partition identified by the partition key, the client should connect to.</param>
        /// <param name="listenerName">Specifies which listener in the endpoint of the chosen replica, to which the client should connect to.</param>
        /// <param name="retrySettings">Specifies the retry policy that should be used for exceptions that occur when creating the client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result of the Task is
        /// the CommunicationClient(<see cref="ICommunicationClient" />) object.
        /// </returns>
        Task<TCommunicationClient> GetClientAsync(
            Uri serviceUri,
            ServicePartitionKey partitionKey,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken);

        /// <summary>
        /// Re-resolves a partition of the specified service containing one or more communication listeners and returns a client to communicate
        /// to the endpoint corresponding to the given listenerName.
        ///
        /// The endpoint of the service is of the form - {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}.
        /// </summary>
        /// <param name="previousRsp">Previous ResolvedServicePartition value.</param>
        /// <param name="targetReplicaSelector">Specifies which replica in the partition identified by the partition key, the client should connect to.</param>
        /// <param name="listenerName">Specifies which listener in the endpoint of the chosen replica, to which the client should connect to.</param>
        /// <param name="retrySettings">Specifies the retry policy that should be used for exceptions that occur when creating the client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result of the Task is
        /// the CommunicationClient(<see cref="ICommunicationClient" />) object.
        /// </returns>
        Task<TCommunicationClient> GetClientAsync(
            ResolvedServicePartition previousRsp,
            TargetReplicaSelector targetReplicaSelector,
            string listenerName,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken);

        /// <summary>
        /// Handles the exceptions that occur in the CommunicationClient when sending a message to the Service.
        /// </summary>
        /// <param name="client">Communication client.</param>
        /// <param name="exceptionInformation">Information about exception that happened while communicating with the service.</param>
        /// <param name="retrySettings">Specifies the retry policy that should be used for handling the reported exception.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result of the Task is
        /// a <see cref="OperationRetryControl" /> object that provides information on retry policy for this exception.
        /// </returns>
        Task<OperationRetryControl> ReportOperationExceptionAsync(
            TCommunicationClient client,
            ExceptionInformation exceptionInformation,
            OperationRetrySettings retrySettings,
            CancellationToken cancellationToken);
    }
}
