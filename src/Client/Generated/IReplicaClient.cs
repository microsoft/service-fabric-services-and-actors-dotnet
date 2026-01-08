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
    /// Interface containing methods for performing ReplicaClient operations.
    /// </summary>
    public partial interface IReplicaClient
    {
        /// <summary>
        /// Gets the information about replicas of a Service Fabric service partition.
        /// </summary>
        /// <remarks>
        /// The GetReplicas endpoint returns information about the replicas of the specified partition. The response includes
        /// the ID, role, status, health, node name, uptime, and other details about the replica.
        /// </remarks>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="continuationToken">The continuation token to obtain next set of results</param>
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
        Task<PagedData<ReplicaInfo>> GetReplicaInfoListAsync(
            PartitionId partitionId,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the information about a replica of a Service Fabric partition.
        /// </summary>
        /// <remarks>
        /// The response includes the ID, role, status, health, node name, uptime, and other details about the replica.
        /// </remarks>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="replicaId">The identifier of the replica.</param>
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
        Task<ReplicaInfo> GetReplicaInfoAsync(
            PartitionId partitionId,
            ReplicaId replicaId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the health of a Service Fabric stateful service replica or stateless service instance.
        /// </summary>
        /// <remarks>
        /// Gets the health of a Service Fabric replica.
        /// Use EventsHealthStateFilter to filter the collection of health events reported on the replica based on the health
        /// state.
        /// </remarks>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="replicaId">The identifier of the replica.</param>
        /// <param name ="eventsHealthStateFilter">Allows filtering the collection of HealthEvent objects returned based on
        /// health state.
        /// The possible values for this parameter include integer value of one of the following health states.
        /// Only events that match the filter are returned. All events are used to evaluate the aggregated health state.
        /// If not specified, all entries are returned. The state values are flag-based enumeration, so the value could be a
        /// combination of these values, obtained using the bitwise 'OR' operator. For example, If the provided value is 6 then
        /// all of the events with HealthState value of OK (2) and Warning (4) are returned.
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
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
        Task<ReplicaHealth> GetReplicaHealthAsync(
            PartitionId partitionId,
            ReplicaId replicaId,
            int? eventsHealthStateFilter = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the health of a Service Fabric stateful service replica or stateless service instance using the specified
        /// policy.
        /// </summary>
        /// <remarks>
        /// Gets the health of a Service Fabric stateful service replica or stateless service instance.
        /// Use EventsHealthStateFilter to filter the collection of health events reported on the cluster based on the health
        /// state.
        /// Use ApplicationHealthPolicy to optionally override the health policies used to evaluate the health. This API only
        /// uses 'ConsiderWarningAsError' field of the ApplicationHealthPolicy. The rest of the fields are ignored while
        /// evaluating the health of the replica.
        /// </remarks>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="replicaId">The identifier of the replica.</param>
        /// <param name ="eventsHealthStateFilter">Allows filtering the collection of HealthEvent objects returned based on
        /// health state.
        /// The possible values for this parameter include integer value of one of the following health states.
        /// Only events that match the filter are returned. All events are used to evaluate the aggregated health state.
        /// If not specified, all entries are returned. The state values are flag-based enumeration, so the value could be a
        /// combination of these values, obtained using the bitwise 'OR' operator. For example, If the provided value is 6 then
        /// all of the events with HealthState value of OK (2) and Warning (4) are returned.
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
        /// </param>
        /// <param name ="applicationHealthPolicy">Describes the health policies used to evaluate the health of an application
        /// or one of its children.
        /// If not present, the health evaluation uses the health policy from application manifest or the default health
        /// policy.
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
        Task<ReplicaHealth> GetReplicaHealthUsingPolicyAsync(
            PartitionId partitionId,
            ReplicaId replicaId,
            int? eventsHealthStateFilter = 0,
            ApplicationHealthPolicy applicationHealthPolicy = default(ApplicationHealthPolicy),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a health report on the Service Fabric replica.
        /// </summary>
        /// <remarks>
        /// Reports health state of the specified Service Fabric replica. The report must contain the information about the
        /// source of the health report and property on which it is reported.
        /// The report is sent to a Service Fabric gateway Replica, which forwards to the health store.
        /// The report may be accepted by the gateway, but rejected by the health store after extra validation.
        /// For example, the health store may reject the report because of an invalid parameter, like a stale sequence number.
        /// To see whether the report was applied in the health store, run GetReplicaHealth and check that the report appears
        /// in the HealthEvents section.
        /// </remarks>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="replicaId">The identifier of the replica.</param>
        /// <param name ="serviceKind">The kind of service replica (Stateless or Stateful) for which the health is being
        /// reported. Following are the possible values. Possible values include: 'Stateless', 'Stateful'</param>
        /// <param name ="healthInformation">Describes the health information for the health report. This information needs to
        /// be present in all of the health reports sent to the health manager.</param>
        /// <param name ="immediate">A flag that indicates whether the report should be sent immediately.
        /// A health report is sent to a Service Fabric gateway Application, which forwards to the health store.
        /// If Immediate is set to true, the report is sent immediately from HTTP Gateway to the health store, regardless of
        /// the fabric client settings that the HTTP Gateway Application is using.
        /// This is useful for critical reports that should be sent as soon as possible.
        /// Depending on timing and other conditions, sending the report may still fail, for example if the HTTP Gateway is
        /// closed or the message doesn't reach the Gateway.
        /// If Immediate is set to false, the report is sent based on the health client settings from the HTTP Gateway.
        /// Therefore, it will be batched according to the HealthReportSendInterval configuration.
        /// This is the recommended setting because it allows the health client to optimize health reporting messages to health
        /// store as well as health report processing.
        /// By default, reports are not sent immediately.
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
        Task ReportReplicaHealthAsync(
            PartitionId partitionId,
            ReplicaId replicaId,
            ReplicaHealthReportServiceKind? serviceKind,
            HealthInformation healthInformation,
            bool? immediate = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the list of replicas deployed on a Service Fabric node.
        /// </summary>
        /// <remarks>
        /// Gets the list containing the information about replicas deployed on a Service Fabric node. The information include
        /// partition ID, replica ID, status of the replica, name of the service, name of the service type, and other
        /// information. Use PartitionId or ServiceManifestName query parameters to return information about the deployed
        /// replicas matching the specified values for those parameters.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="partitionId">The identity of the partition.</param>
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
        Task<IEnumerable<DeployedServiceReplicaInfo>> GetDeployedServiceReplicaInfoListAsync(
            NodeName nodeName,
            string applicationId,
            PartitionId partitionId = default(PartitionId),
            string serviceManifestName = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the details of replica deployed on a Service Fabric node.
        /// </summary>
        /// <remarks>
        /// Gets the details of the replica deployed on a Service Fabric node. The information includes service kind, service
        /// name, current service operation, current service operation start date time, partition ID, replica/instance ID,
        /// reported load, and other information.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="replicaId">The identifier of the replica.</param>
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
        Task<DeployedServiceReplicaDetailInfo> GetDeployedServiceReplicaDetailInfoAsync(
            NodeName nodeName,
            PartitionId partitionId,
            ReplicaId replicaId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the details of replica deployed on a Service Fabric node.
        /// </summary>
        /// <remarks>
        /// Gets the details of the replica deployed on a Service Fabric node. The information includes service kind, service
        /// name, current service operation, current service operation start date time, partition ID, replica/instance ID,
        /// reported load, and other information.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="partitionId">The identity of the partition.</param>
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
        Task<DeployedServiceReplicaDetailInfo> GetDeployedServiceReplicaDetailInfoByPartitionIdAsync(
            NodeName nodeName,
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Restarts a service replica of a persisted service running on a node.
        /// </summary>
        /// <remarks>
        /// Restarts a service replica of a persisted service running on a node. Warning - There are no safety checks performed
        /// when this API is used. Incorrect use of this API can lead to availability loss for stateful services.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="replicaId">The identifier of the replica.</param>
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
        Task RestartReplicaAsync(
            NodeName nodeName,
            PartitionId partitionId,
            ReplicaId replicaId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes a service replica running on a node.
        /// </summary>
        /// <remarks>
        /// This API simulates a Service Fabric replica failure by removing a replica from a Service Fabric cluster. The
        /// removal closes the replica, transitions the replica to the role None, and then removes all of the state information
        /// of the replica from the cluster. This API tests the replica state removal path, and simulates the report fault
        /// permanent path through client APIs. Warning - There are no safety checks performed when this API is used. Incorrect
        /// use of this API can lead to data loss for stateful services. In addition, the forceRemove flag impacts all other
        /// replicas hosted in the same process. If the cluster has the replica soft delete feature enabled, the flow is
        /// updated to no longer remove all of the state information of the replica from the cluster. This state is instead
        /// persisted on the disk to be restorable from in the case of unexpected quorum loss. If the partition is otherwise
        /// healthy, Service Fabric will permanently delete the replica state periodically.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="replicaId">The identifier of the replica.</param>
        /// <param name ="forceRemove">Remove a Service Fabric application or service forcefully without going through the
        /// graceful shutdown sequence. This parameter can be used to forcefully delete an application or service for which
        /// delete is timing out due to issues in the service code that prevents graceful close of replicas.</param>
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
        Task RemoveReplicaAsync(
            NodeName nodeName,
            PartitionId partitionId,
            ReplicaId replicaId,
            bool? forceRemove = default(bool?),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Recovers a ToBeRemoved replica by reopening the stateful service replica.
        /// </summary>
        /// <remarks>
        /// Recovers a ToBeRemoved replica by reopening the stateful service replica. Use this if the partition is stuck in
        /// quorum loss in order to prevent data loss. In the event of Quorum Loss, customers should restore all soft deleted
        /// (ToBeRemoved) replicas using this API. Service Fabric automatically determines which replicas to retain to restore
        /// quorum. This command has no effect on replicas which are not in the ToBeRemoved state.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="replicaId">The identifier of the replica.</param>
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
        Task RestoreReplicaAsync(
            NodeName nodeName,
            PartitionId partitionId,
            ReplicaId replicaId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
