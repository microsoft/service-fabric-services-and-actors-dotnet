// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client.Exceptions;
    using Microsoft.ServiceFabric.Common;
    using Microsoft.ServiceFabric.Common.Exceptions;    

    /// <summary>
    /// Interface containing methods for performing VolumeResourceClient operations.
    /// </summary>
    public partial interface IMeshVolumesClient
    {
        /// <summary>
        /// Creates or updates a volume resource.
        /// </summary>
        /// <remarks>
        /// Creates a volume resource with the specified name and description. If a volume with the same name already exists,
        /// then its description is updated to the one indicated in this request.
        /// Use volume resources to create private volume and configure public connectivity for services within your
        /// application.
        /// </remarks>        
        /// <param name ="volumeResourceName">Service Fabric Volume resource name.</param>        
        /// <param name="jsonDescription">String representing the JSON description of the volume resource to be created or updated.</param>
        /// <param name="apiVersion">Api version for the server.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as volume connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<VolumeResourceDescription> CreateOrUpdateAsync(
            string volumeResourceName,
            string jsonDescription,
            string apiVersion = Constants.DefaultApiVersionForResources,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
