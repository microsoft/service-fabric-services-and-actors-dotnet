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
    /// Interface containing methods for performing MeshNetworksClient operations.
    /// </summary>
    public partial interface IMeshNetworksClient
    {
        /// <summary>
        /// Creates or updates a Network resource.
        /// </summary>
        /// <remarks>
        /// Creates a Network resource with the specified name, description and properties. If Network resource with the same
        /// name exists, then it is updated with the specified description and properties. Network resource provides
        /// connectivity between application services.
        /// </remarks>
        /// <param name ="networkResourceName">The identity of the network.</param>
        /// <param name="jsonDescription">String representing the JSON description of the network resource to be created or updated.</param>
        /// <param name="apiVersion">Api version for the server.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<NetworkResourceDescription> CreateOrUpdateAsync(
            string networkResourceName,
            string jsonDescription,
            string apiVersion = Constants.DefaultApiVersionForResources,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
