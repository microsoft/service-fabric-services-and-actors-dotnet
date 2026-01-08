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
    /// Interface containing methods for performing RepairManagementClient operations.
    /// </summary>
    public partial interface IRepairManagementClient
    {
        /// <summary>
        /// Creates a new repair task.
        /// </summary>
        /// <remarks>
        /// For clusters that have the Repair Manager Service configured,
        /// this API provides a way to create repair tasks that run automatically or manually.
        /// For repair tasks that run automatically, an appropriate repair executor
        /// must be running for each repair action to run automatically.
        /// These are currently only available in specially-configured Azure Cloud Services.
        /// 
        /// To create a manual repair task, provide the set of impacted node names and the
        /// expected impact. When the state of the created repair task changes to approved,
        /// you can safely perform repair actions on those nodes.
        /// 
        /// This API supports the Service Fabric platform; it is not meant to be used directly from your code.
        /// </remarks>
        /// <param name ="repairTask">Describes the repair task to be created or updated.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<RepairTaskUpdateInfo> CreateRepairTaskAsync(
            RepairTask repairTask,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Requests the cancellation of the given repair task.
        /// </summary>
        /// <remarks>
        /// This API supports the Service Fabric platform; it is not meant to be used directly from your code.
        /// </remarks>
        /// <param name ="repairTaskCancelDescription">Describes the repair task to be cancelled.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<RepairTaskUpdateInfo> CancelRepairTaskAsync(
            RepairTaskCancelDescription repairTaskCancelDescription,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a completed repair task.
        /// </summary>
        /// <remarks>
        /// This API supports the Service Fabric platform; it is not meant to be used directly from your code.
        /// </remarks>
        /// <param name ="repairTaskDeleteDescription">Describes the repair task to be deleted.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task DeleteRepairTaskAsync(
            RepairTaskDeleteDescription repairTaskDeleteDescription,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a list of repair tasks matching the given filters.
        /// </summary>
        /// <remarks>
        /// This API supports the Service Fabric platform; it is not meant to be used directly from your code.
        /// </remarks>
        /// <param name ="taskIdFilter">The repair task ID prefix to be matched.</param>
        /// <param name ="stateFilter">A bitwise-OR of the following values, specifying which task states should be included in
        /// the result list.
        /// 
        /// - 1 - Created
        /// - 2 - Claimed
        /// - 4 - Preparing
        /// - 8 - Approved
        /// - 16 - Executing
        /// - 32 - Restoring
        /// - 64 - Completed
        /// </param>
        /// <param name ="executorFilter">The name of the repair executor whose claimed tasks should be included in the
        /// list.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<IEnumerable<RepairTask>> GetRepairTaskListAsync(
            string taskIdFilter = default(string),
            int? stateFilter = default(int?),
            string executorFilter = default(string),
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Forces the approval of the given repair task.
        /// </summary>
        /// <remarks>
        /// This API supports the Service Fabric platform; it is not meant to be used directly from your code.
        /// </remarks>
        /// <param name ="repairTaskApproveDescription">Describes the repair task to be approved.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<RepairTaskUpdateInfo> ForceApproveRepairTaskAsync(
            RepairTaskApproveDescription repairTaskApproveDescription,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the health policy of the given repair task.
        /// </summary>
        /// <remarks>
        /// This API supports the Service Fabric platform; it is not meant to be used directly from your code.
        /// </remarks>
        /// <param name ="repairTaskUpdateHealthPolicyDescription">Describes the repair task healthy policy to be
        /// updated.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<RepairTaskUpdateInfo> UpdateRepairTaskHealthPolicyAsync(
            RepairTaskUpdateHealthPolicyDescription repairTaskUpdateHealthPolicyDescription,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the execution state of a repair task.
        /// </summary>
        /// <remarks>
        /// This API supports the Service Fabric platform; it is not meant to be used directly from your code.
        /// </remarks>
        /// <param name ="repairTask">Describes the repair task to be created or updated.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<RepairTaskUpdateInfo> UpdateRepairExecutionStateAsync(
            RepairTask repairTask,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
