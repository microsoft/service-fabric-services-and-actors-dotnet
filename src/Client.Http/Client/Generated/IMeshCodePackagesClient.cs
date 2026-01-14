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
    /// Interface containing methods for performing MeshCodePackagesClient operations.
    /// </summary>
    public partial interface IMeshCodePackagesClient
    {
        /// <summary>
        /// Gets the logs from the container.
        /// </summary>
        /// <remarks>
        /// Gets the logs for the container of the specified code package of the service replica.
        /// </remarks>
        /// <param name ="applicationResourceName">The identity of the application.</param>
        /// <param name ="serviceResourceName">The identity of the service.</param>
        /// <param name ="replicaName">Service Fabric replica name.
        /// </param>
        /// <param name ="codePackageName">The name of code package of the service.</param>
        /// <param name ="tail">Number of lines to show from the end of the logs. Default is 100. 'all' to show the complete
        /// logs.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<ContainerLogs> GetContainerLogsAsync(
            string applicationResourceName,
            string serviceResourceName,
            string replicaName,
            string codePackageName,
            string tail = default(string),
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
