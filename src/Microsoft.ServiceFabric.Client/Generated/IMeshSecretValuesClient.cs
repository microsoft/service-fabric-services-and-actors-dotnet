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
    /// Interface containing methods for performing MeshSecretValuesClient operations.
    /// </summary>
    public partial interface IMeshSecretValuesClient
    {
        /// <summary>
        /// Adds the specified value as a new version of the specified secret resource.
        /// </summary>
        /// <remarks>
        /// Creates a new value of the specified secret resource. The name of the value is typically the version identifier.
        /// Once created the value cannot be changed.
        /// </remarks>
        /// <param name ="secretResourceName">The name of the secret resource.</param>
        /// <param name ="secretValueResourceName">The name of the secret resource value which is typically the version
        /// identifier for the value.</param>
        /// <param name ="secretValueResourceDescription">Description for creating a value of a secret resource.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<SecretValueResourceDescription> AddValueAsync(
            string secretResourceName,
            string secretValueResourceName,
            SecretValueResourceDescription secretValueResourceDescription,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the specified secret value resource.
        /// </summary>
        /// <remarks>
        /// Get the information about the specified named secret value resources. The information does not include the actual
        /// value of the secret.
        /// </remarks>
        /// <param name ="secretResourceName">The name of the secret resource.</param>
        /// <param name ="secretValueResourceName">The name of the secret resource value which is typically the version
        /// identifier for the value.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<SecretValueResourceDescription> GetAsync(
            string secretResourceName,
            string secretValueResourceName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the specified  value of the named secret resource.
        /// </summary>
        /// <remarks>
        /// Deletes the secret value resource identified by the name. The name of the resource is typically the version
        /// associated with that value. Deletion will fail if the specified value is in use.
        /// </remarks>
        /// <param name ="secretResourceName">The name of the secret resource.</param>
        /// <param name ="secretValueResourceName">The name of the secret resource value which is typically the version
        /// identifier for the value.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task DeleteAsync(
            string secretResourceName,
            string secretValueResourceName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// List names of all values of the specified secret resource.
        /// </summary>
        /// <remarks>
        /// Gets information about all secret value resources of the specified secret resource. The information includes the
        /// names of the secret value resources, but not the actual values.
        /// </remarks>
        /// <param name ="secretResourceName">The name of the secret resource.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<PagedData<SecretValueResourceDescription>> ListAsync(
            string secretResourceName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Lists the specified value of the secret resource.
        /// </summary>
        /// <remarks>
        /// Lists the decrypted value of the specified named value of the secret resource. This is a privileged operation.
        /// </remarks>
        /// <param name ="secretResourceName">The name of the secret resource.</param>
        /// <param name ="secretValueResourceName">The name of the secret resource value which is typically the version
        /// identifier for the value.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<SecretValue> ShowAsync(
            string secretResourceName,
            string secretValueResourceName,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
