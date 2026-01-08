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
    /// Interface containing methods for performing MeshServicesClient operations.
    /// </summary>
    public partial interface IMeshServicesClient
    {
        /// <summary>
        /// Gets the Service resource with the given name.
        /// </summary>
        /// <remarks>
        /// Gets the information about the Service resource with the given name. The information include the description and
        /// other properties of the Service.
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
        Task<ServiceResourceDescription> GetAsync(
            string applicationResourceName,
            string serviceResourceName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Lists all the service resources.
        /// </summary>
        /// <remarks>
        /// Gets the information about all services of an application resource. The information include the description and
        /// other properties of the Service.
        /// </remarks>
        /// <param name ="applicationResourceName">The identity of the application.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<PagedData<ServiceResourceDescription>> ListAsync(
            string applicationResourceName,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
