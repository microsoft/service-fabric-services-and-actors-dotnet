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
    /// Interface containing methods for performing InfrastructureClient operations.
    /// </summary>
    public partial interface IInfrastructureClient
    {
        /// <summary>
        /// Invokes an administrative command on the given Infrastructure Service instance.
        /// </summary>
        /// <remarks>
        /// For clusters that have one or more instances of the Infrastructure Service configured,
        /// this API provides a way to send infrastructure-specific commands to a particular
        /// instance of the Infrastructure Service.
        /// 
        /// Available commands and their corresponding response formats vary depending upon
        /// the infrastructure on which the cluster is running.
        /// 
        /// This API supports the Service Fabric platform; it is not meant to be used directly from your code.
        /// </remarks>
        /// <param name ="command">The text of the command to be invoked. The content of the command is
        /// infrastructure-specific.</param>
        /// <param name ="serviceId">The identity of the infrastructure service. This is the full name of the infrastructure
        /// service without the 'fabric:' URI scheme. This parameter required only for the cluster that has more than one
        /// instance of infrastructure service running.</param>
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
        Task<string> InvokeInfrastructureCommandAsync(
            string command,
            string serviceId = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Invokes a read-only query on the given infrastructure service instance.
        /// </summary>
        /// <remarks>
        /// For clusters that have one or more instances of the Infrastructure Service configured,
        /// this API provides a way to send infrastructure-specific queries to a particular
        /// instance of the Infrastructure Service.
        /// 
        /// Available commands and their corresponding response formats vary depending upon
        /// the infrastructure on which the cluster is running.
        /// 
        /// This API supports the Service Fabric platform; it is not meant to be used directly from your code.
        /// </remarks>
        /// <param name ="command">The text of the command to be invoked. The content of the command is
        /// infrastructure-specific.</param>
        /// <param name ="serviceId">The identity of the infrastructure service. This is the full name of the infrastructure
        /// service without the 'fabric:' URI scheme. This parameter required only for the cluster that has more than one
        /// instance of infrastructure service running.</param>
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
        Task<string> InvokeInfrastructureQueryAsync(
            string command,
            string serviceId = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
