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
    /// Interface containing methods for performing CodePackageClient operations.
    /// </summary>
    public partial interface ICodePackageClient
    {
        /// <summary>
        /// Gets the list of code packages deployed on a Service Fabric node.
        /// </summary>
        /// <remarks>
        /// Gets the list of code packages deployed on a Service Fabric node for the given application.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="serviceManifestName">The name of a service manifest registered as part of an application type in a
        /// Service Fabric cluster.</param>
        /// <param name ="codePackageName">The name of code package specified in service manifest registered as part of an
        /// application type in a Service Fabric cluster.</param>
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
        Task<IEnumerable<DeployedCodePackageInfo>> GetDeployedCodePackageInfoListAsync(
            NodeName nodeName,
            string applicationId,
            string serviceManifestName = default(string),
            string codePackageName = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Restarts a code package deployed on a Service Fabric node in a cluster.
        /// </summary>
        /// <remarks>
        /// Restarts a code package deployed on a Service Fabric node in a cluster. This aborts the code package process, which
        /// will restart all the user service replicas hosted in that process.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="restartDeployedCodePackageDescription">Describes the deployed code package on Service Fabric node to
        /// restart.</param>
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
        Task RestartDeployedCodePackageAsync(
            NodeName nodeName,
            string applicationId,
            RestartDeployedCodePackageDescription restartDeployedCodePackageDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the container logs for container deployed on a Service Fabric node.
        /// </summary>
        /// <remarks>
        /// Gets the container logs for container deployed on a Service Fabric node for the given code package.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="serviceManifestName">The name of a service manifest registered as part of an application type in a
        /// Service Fabric cluster.</param>
        /// <param name ="codePackageName">The name of code package specified in service manifest registered as part of an
        /// application type in a Service Fabric cluster.</param>
        /// <param name ="tail">Number of lines to show from the end of the logs. Default is 100. 'all' to show the complete
        /// logs.</param>
        /// <param name ="previous">Specifies whether to get container logs from exited/dead containers of the code package
        /// instance.</param>
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
        Task<ContainerLogs> GetContainerLogsDeployedOnNodeAsync(
            NodeName nodeName,
            string applicationId,
            string serviceManifestName,
            string codePackageName,
            string tail = default(string),
            bool? previous = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Invoke container API on a container deployed on a Service Fabric node.
        /// </summary>
        /// <remarks>
        /// Invoke container API on a container deployed on a Service Fabric node for the given code package.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="serviceManifestName">The name of a service manifest registered as part of an application type in a
        /// Service Fabric cluster.</param>
        /// <param name ="codePackageName">The name of code package specified in service manifest registered as part of an
        /// application type in a Service Fabric cluster.</param>
        /// <param name ="codePackageInstanceId">ID that uniquely identifies a code package instance deployed on a service
        /// fabric node.</param>
        /// <param name ="containerApiRequestBody">Parameters for making container API call</param>
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
        Task<ContainerApiResponse> InvokeContainerApiAsync(
            NodeName nodeName,
            string applicationId,
            string serviceManifestName,
            string codePackageName,
            string codePackageInstanceId,
            ContainerApiRequestBody containerApiRequestBody,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
