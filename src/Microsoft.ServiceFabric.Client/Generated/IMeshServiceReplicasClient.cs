// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client.Exceptions;
    using Microsoft.ServiceFabric.Common;
    using Microsoft.ServiceFabric.Common.Exceptions;

    /// <summary>
    /// Interface containing methods for performing MeshServiceReplicasClient operations.
    /// </summary>
    public partial interface IMeshServiceReplicasClient
    {
        /// <summary>
        /// Gets the given replica of the service of an application.
        /// </summary>
        /// <remarks>
        /// Gets the information about the service replica with the given name. The information include the description and
        /// other properties of the service replica.
        /// </remarks>
        /// <param name ="applicationResourceName">The identity of the application.</param>
        /// <param name ="serviceResourceName">The identity of the service.</param>
        /// <param name ="replicaName">Service Fabric replica name.
        /// </param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<ServiceReplicaDescription> GetAsync(
            string applicationResourceName,
            string serviceResourceName,
            string replicaName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Lists all the replicas of a service.
        /// </summary>
        /// <remarks>
        /// Gets the information about all replicas of a service. The information include the description and other properties
        /// of the service replica.
        /// </remarks>
        /// <param name ="applicationResourceName">The identity of the application.</param>
        /// <param name ="serviceResourceName">The identity of the service.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<PagedData<ServiceReplicaDescription>> ListAsync(
            string applicationResourceName,
            string serviceResourceName,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
