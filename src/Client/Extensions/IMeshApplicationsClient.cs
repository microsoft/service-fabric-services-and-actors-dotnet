// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client
{    
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Interface containing methods for performing ApplicationResourceClient operations.
    /// </summary>
    public partial interface IMeshApplicationsClient
    {
        /// <summary>
        /// Creates or updates an application with the given name and description.
        /// </summary>
        /// <remarks>
        /// Creates an application with the specified name and description. If an application with the same name already
        /// exists, then its description are updated to the one indicated in this request.
        /// </remarks>        
        /// <param name="applicationResourceName">Name of Service Fabric Mesh Application.</param>
        /// <param name="jsonDescription">String representing the JSON description of the application to be created or updated.</param>
        /// <param name="apiVersion">Api version for the server.</param>
        /// <param name="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="T:InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="T:ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="T:ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="P:Microsoft.ServiceFabric.Common.FabricError.ErrorCode" />, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="T:System.OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<ApplicationResourceDescription> CreateOrUpdateAsync(
            string applicationResourceName,
            string jsonDescription,
            string apiVersion = Constants.DefaultApiVersionForResources,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
