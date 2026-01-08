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
    /// Interface containing methods for performing MeshSecretsClient operations.
    /// </summary>
    public partial interface IMeshSecretsClient
    {
        /// <summary>
        /// Creates or updates a secret resource.
        /// </summary>
        /// <remarks>
        /// Creates a secret resource with the specified name, description and metadata describing its value(s). If a secret
        /// resource with the same name does not exist, a new resource is created with specified properties.
        /// If a secret resource with the same name exists, its description is updated. Updating an existing secret resource
        /// may not change its kind, name or content type.
        /// </remarks>
        /// <param name ="secretResourceName">Service Fabric gateway resource name.</param>        
        /// <param name="jsonDescription">String representing the JSON description of the secret resource to be created or updated.</param>
        /// <param name="apiVersion">Api version for the server.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<SecretResourceDescription> CreateOrUpdateAsync(
            string secretResourceName,
            string jsonDescription,
            string apiVersion = Constants.DefaultApiVersionForResources,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
