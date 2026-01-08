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
    /// Interface containing methods for performing ServiceTypeClient operations.
    /// </summary>
    public partial interface IServiceTypeClient
    {
        /// <summary>
        /// Gets the list containing the information about service types that are supported by a provisioned application type
        /// in a Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Gets the list containing the information about service types that are supported by a provisioned application type
        /// in a Service Fabric cluster. The provided application type must exist. Otherwise, a 404 status is returned.
        /// </remarks>
        /// <param name ="applicationTypeName">The name of the application type.</param>
        /// <param name ="applicationTypeVersion">The version of the application type.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<IEnumerable<ServiceTypeInfo>> GetServiceTypeInfoListAsync(
            string applicationTypeName,
            string applicationTypeVersion,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the information about a specific service type that is supported by a provisioned application type in a Service
        /// Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Gets the information about a specific service type that is supported by a provisioned application type in a Service
        /// Fabric cluster. The provided application type must exist. Otherwise, a 404 status is returned. A 204 response is
        /// returned if the specified service type is not found in the cluster.
        /// </remarks>
        /// <param name ="applicationTypeName">The name of the application type.</param>
        /// <param name ="applicationTypeVersion">The version of the application type.</param>
        /// <param name ="serviceTypeName">Specifies the name of a Service Fabric service type.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<ServiceTypeInfo> GetServiceTypeInfoByNameAsync(
            string applicationTypeName,
            string applicationTypeVersion,
            string serviceTypeName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the manifest describing a service type.
        /// </summary>
        /// <remarks>
        /// Gets the manifest describing a service type. The response contains the service manifest XML as a string.
        /// </remarks>
        /// <param name ="applicationTypeName">The name of the application type.</param>
        /// <param name ="applicationTypeVersion">The version of the application type.</param>
        /// <param name ="serviceManifestName">The name of a service manifest registered as part of an application type in a
        /// Service Fabric cluster.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<ServiceTypeManifest> GetServiceManifestAsync(
            string applicationTypeName,
            string applicationTypeVersion,
            string serviceManifestName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the list containing the information about service types from the applications deployed on a node in a Service
        /// Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Gets the list containing the information about service types from the applications deployed on a node in a Service
        /// Fabric cluster. The response includes the name of the service type, its registration status, the code package that
        /// registered it and activation ID of the service package.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="serviceManifestName">The name of the service manifest to filter the list of deployed service type
        /// information. If specified, the response will only contain the information about service types that are defined in
        /// this service manifest.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<IEnumerable<DeployedServiceTypeInfo>> GetDeployedServiceTypeInfoListAsync(
            NodeName nodeName,
            string applicationId,
            string serviceManifestName = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the information about a specified service type of the application deployed on a node in a Service Fabric
        /// cluster.
        /// </summary>
        /// <remarks>
        /// Gets the list containing the information about a specific service type from the applications deployed on a node in
        /// a Service Fabric cluster. The response includes the name of the service type, its registration status, the code
        /// package that registered it and activation ID of the service package. Each entry represents one activation of a
        /// service type, differentiated by the activation ID.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="serviceTypeName">Specifies the name of a Service Fabric service type.</param>
        /// <param name ="serviceManifestName">The name of the service manifest to filter the list of deployed service type
        /// information. If specified, the response will only contain the information about service types that are defined in
        /// this service manifest.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<IEnumerable<DeployedServiceTypeInfo>> GetDeployedServiceTypeInfoByNameAsync(
            NodeName nodeName,
            string applicationId,
            string serviceTypeName,
            string serviceManifestName = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
