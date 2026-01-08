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
    /// Interface containing methods for performing FaultsClient operations.
    /// </summary>
    public partial interface IFaultsClient
    {
        /// <summary>
        /// This API will induce data loss for the specified partition. It will trigger a call to the OnDataLossAsync API of
        /// the partition.
        /// </summary>
        /// <remarks>
        /// This API will induce data loss for the specified partition. It will trigger a call to the OnDataLoss API of the
        /// partition.
        /// Actual data loss will depend on the specified DataLossMode.
        /// 
        /// - PartialDataLoss - Only a quorum of replicas are removed and OnDataLoss is triggered for the partition but actual
        /// data loss depends on the presence of in-flight replication.
        /// - FullDataLoss - All replicas are removed hence all data is lost and OnDataLoss is triggered.
        /// 
        /// This API should only be called with a stateful service as the target.
        /// 
        /// Calling this API with a system service as the target is not advised.
        /// 
        /// Note:  Once this API has been called, it cannot be reversed. Calling CancelOperation will only stop execution and
        /// clean up internal system state.
        /// It will not restore data if the command has progressed far enough to cause data loss.
        /// 
        /// Call the GetDataLossProgress API with the same OperationId to return information on the operation started with this
        /// API.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="operationId">A GUID that identifies a call of this API.  This is passed into the corresponding
        /// GetProgress API</param>
        /// <param name ="dataLossMode">This enum is passed to the StartDataLoss API to indicate what type of data loss to
        /// induce. Possible values include: 'Invalid', 'PartialDataLoss', 'FullDataLoss'</param>
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
        Task StartDataLossAsync(
            string serviceId,
            PartitionId partitionId,
            Guid? operationId,
            DataLossMode? dataLossMode,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the progress of a partition data loss operation started using the StartDataLoss API.
        /// </summary>
        /// <remarks>
        /// Gets the progress of a data loss operation started with StartDataLoss, using the OperationId.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="operationId">A GUID that identifies a call of this API.  This is passed into the corresponding
        /// GetProgress API</param>
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
        Task<PartitionDataLossProgress> GetDataLossProgressAsync(
            string serviceId,
            PartitionId partitionId,
            Guid? operationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Induces quorum loss for a given stateful service partition.
        /// </summary>
        /// <remarks>
        /// This API is useful for a temporary quorum loss situation on your service.
        /// 
        /// Call the GetQuorumLossProgress API with the same OperationId to return information on the operation started with
        /// this API.
        /// 
        /// This can only be called on stateful persisted (HasPersistedState==true) services.  Do not use this API on stateless
        /// services or stateful in-memory only services.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="operationId">A GUID that identifies a call of this API.  This is passed into the corresponding
        /// GetProgress API</param>
        /// <param name ="quorumLossMode">This enum is passed to the StartQuorumLoss API to indicate what type of quorum loss
        /// to induce. Possible values include: 'Invalid', 'QuorumReplicas', 'AllReplicas'</param>
        /// <param name ="quorumLossDuration">The amount of time for which the partition will be kept in quorum loss.  This
        /// must be specified in seconds.</param>
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
        Task StartQuorumLossAsync(
            string serviceId,
            PartitionId partitionId,
            Guid? operationId,
            QuorumLossMode? quorumLossMode,
            int? quorumLossDuration,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the progress of a quorum loss operation on a partition started using the StartQuorumLoss API.
        /// </summary>
        /// <remarks>
        /// Gets the progress of a quorum loss operation started with StartQuorumLoss, using the provided OperationId.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="operationId">A GUID that identifies a call of this API.  This is passed into the corresponding
        /// GetProgress API</param>
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
        Task<PartitionQuorumLossProgress> GetQuorumLossProgressAsync(
            string serviceId,
            PartitionId partitionId,
            Guid? operationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// This API will restart some or all replicas or instances of the specified partition.
        /// </summary>
        /// <remarks>
        /// This API is useful for testing failover.
        /// 
        /// If used to target a stateless service partition, RestartPartitionMode must be AllReplicasOrInstances.
        /// 
        /// Call the GetPartitionRestartProgress API using the same OperationId to get the progress.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="operationId">A GUID that identifies a call of this API.  This is passed into the corresponding
        /// GetProgress API</param>
        /// <param name ="restartPartitionMode">Describe which partitions to restart. Possible values include: 'Invalid',
        /// 'AllReplicasOrInstances', 'OnlyActiveSecondaries'</param>
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
        Task StartPartitionRestartAsync(
            string serviceId,
            PartitionId partitionId,
            Guid? operationId,
            RestartPartitionMode? restartPartitionMode,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the progress of a PartitionRestart operation started using StartPartitionRestart.
        /// </summary>
        /// <remarks>
        /// Gets the progress of a PartitionRestart started with StartPartitionRestart using the provided OperationId.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="operationId">A GUID that identifies a call of this API.  This is passed into the corresponding
        /// GetProgress API</param>
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
        Task<PartitionRestartProgress> GetPartitionRestartProgressAsync(
            string serviceId,
            PartitionId partitionId,
            Guid? operationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Starts or stops a cluster node.
        /// </summary>
        /// <remarks>
        /// Starts or stops a cluster node.  A cluster node is a process, not the OS instance itself.  To start a node, pass in
        /// "Start" for the NodeTransitionType parameter.
        /// To stop a node, pass in "Stop" for the NodeTransitionType parameter.  This API starts the operation - when the API
        /// returns the node may not have finished transitioning yet.
        /// Call GetNodeTransitionProgress with the same OperationId to get the progress of the operation.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="operationId">A GUID that identifies a call of this API.  This is passed into the corresponding
        /// GetProgress API</param>
        /// <param name ="nodeTransitionType">Indicates the type of transition to perform.  NodeTransitionType.Start will start
        /// a stopped node.  NodeTransitionType.Stop will stop a node that is up. Possible values include: 'Invalid', 'Start',
        /// 'Stop'</param>
        /// <param name ="nodeInstanceId">The node instance ID of the target node.  This can be determined through GetNodeInfo
        /// API.</param>
        /// <param name ="stopDurationInSeconds">The duration, in seconds, to keep the node stopped.  The minimum value is 600,
        /// the maximum is 14400.  After this time expires, the node will automatically come back up.</param>
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
        Task StartNodeTransitionAsync(
            NodeName nodeName,
            Guid? operationId,
            NodeTransitionType? nodeTransitionType,
            string nodeInstanceId,
            int? stopDurationInSeconds,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the progress of an operation started using StartNodeTransition.
        /// </summary>
        /// <remarks>
        /// Gets the progress of an operation started with StartNodeTransition using the provided OperationId.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="operationId">A GUID that identifies a call of this API.  This is passed into the corresponding
        /// GetProgress API</param>
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
        Task<NodeTransitionProgress> GetNodeTransitionProgressAsync(
            NodeName nodeName,
            Guid? operationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a list of user-induced fault operations filtered by provided input.
        /// </summary>
        /// <remarks>
        /// Gets the list of user-induced fault operations filtered by provided input.
        /// </remarks>
        /// <param name ="typeFilter">Used to filter on OperationType for user-induced operations.
        /// 
        /// - 65535 - select all
        /// - 1 - select PartitionDataLoss.
        /// - 2 - select PartitionQuorumLoss.
        /// - 4 - select PartitionRestart.
        /// - 8 - select NodeTransition.
        /// </param>
        /// <param name ="stateFilter">Used to filter on OperationState's for user-induced operations.
        /// 
        /// - 65535 - select All
        /// - 1 - select Running
        /// - 2 - select RollingBack
        /// - 8 - select Completed
        /// - 16 - select Faulted
        /// - 32 - select Cancelled
        /// - 64 - select ForceCancelled
        /// </param>
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
        Task<IEnumerable<OperationStatus>> GetFaultOperationListAsync(
            int? typeFilter,
            int? stateFilter,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Cancels a user-induced fault operation.
        /// </summary>
        /// <remarks>
        /// The following APIs start fault operations that may be cancelled by using CancelOperation: StartDataLoss,
        /// StartQuorumLoss, StartPartitionRestart, StartNodeTransition.
        /// 
        /// If force is false, then the specified user-induced operation will be gracefully stopped and cleaned up.  If force
        /// is true, the command will be aborted, and some internal state
        /// may be left behind.  Specifying force as true should be used with care.  Calling this API with force set to true is
        /// not allowed until this API has already
        /// been called on the same test command with force set to false first, or unless the test command already has an
        /// OperationState of OperationState.RollingBack.
        /// Clarification: OperationState.RollingBack means that the system will be/is cleaning up internal system state caused
        /// by executing the command.  It will not restore data if the
        /// test command was to cause data loss.  For example, if you call StartDataLoss then call this API, the system will
        /// only clean up internal state from running the command.
        /// It will not restore the target partition's data, if the command progressed far enough to cause data loss.
        /// 
        /// Important note:  if this API is invoked with force==true, internal state may be left behind.
        /// </remarks>
        /// <param name ="operationId">A GUID that identifies a call of this API.  This is passed into the corresponding
        /// GetProgress API</param>
        /// <param name ="force">Indicates whether to gracefully roll back and clean up internal system state modified by
        /// executing the user-induced operation.</param>
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
        Task CancelOperationAsync(
            Guid? operationId,
            bool? force,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
