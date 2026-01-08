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
    /// Interface containing methods for performing MeshVolumesClient operations.
    /// </summary>
    public partial interface IMeshVolumesClient
    {
        /// <summary>
        /// Creates or updates a Volume resource.
        /// </summary>
        /// <remarks>
        /// Creates a Volume resource with the specified name, description and properties. If Volume resource with the same
        /// name exists, then it is updated with the specified description and properties.
        /// </remarks>
        /// <param name ="volumeResourceName">The identity of the volume.</param>
        /// <param name ="volumeResourceDescription">Description for creating a Volume resource.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<VolumeResourceDescription> CreateOrUpdateAsync(
            string volumeResourceName,
            VolumeResourceDescription volumeResourceDescription,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the Volume resource with the given name.
        /// </summary>
        /// <remarks>
        /// Gets the information about the Volume resource with the given name. The information include the description and
        /// other properties of the Volume.
        /// </remarks>
        /// <param name ="volumeResourceName">The identity of the volume.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<VolumeResourceDescription> GetAsync(
            string volumeResourceName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the Volume resource.
        /// </summary>
        /// <remarks>
        /// Deletes the Volume resource identified by the name.
        /// </remarks>
        /// <param name ="volumeResourceName">The identity of the volume.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task DeleteAsync(
            string volumeResourceName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Lists all the volume resources.
        /// </summary>
        /// <remarks>
        /// Gets the information about all volume resources in a given resource group. The information include the description
        /// and other properties of the Volume.
        /// </remarks>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<PagedData<VolumeResourceDescription>> ListAsync(
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
