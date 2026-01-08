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
    /// Interface containing methods for performing ClusterClient operations.
    /// </summary>
    public partial interface IClusterClient
    {
        /// <summary>
        /// Get the Service Fabric cluster manifest.
        /// </summary>
        /// <remarks>
        /// Get the Service Fabric cluster manifest. The cluster manifest contains properties of the cluster that include
        /// different node types on the cluster,
        /// security configurations, fault, and upgrade domain topologies, etc.
        /// 
        /// These properties are specified as part of the ClusterConfig.JSON file while deploying a stand-alone cluster.
        /// However, most of the information in the cluster manifest
        /// is generated internally by service fabric during cluster deployment in other deployment scenarios (e.g. when using
        /// Azure portal).
        /// 
        /// The contents of the cluster manifest are for informational purposes only and users are not expected to take a
        /// dependency on the format of the file contents or its interpretation.
        /// </remarks>
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
        Task<ClusterManifest> GetClusterManifestAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the health of a Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Use EventsHealthStateFilter to filter the collection of health events reported on the cluster based on the health
        /// state.
        /// Similarly, use NodesHealthStateFilter and ApplicationsHealthStateFilter to filter the collection of nodes and
        /// applications returned based on their aggregated health state.
        /// </remarks>
        /// <param name ="nodesHealthStateFilter">Allows filtering of the node health state objects returned in the result of
        /// cluster health query
        /// based on their health state. The possible values for this parameter include integer value of one of the
        /// following health states. Only nodes that match the filter are returned. All nodes are used to evaluate the
        /// aggregated health state.
        /// If not specified, all entries are returned.
        /// The state values are flag-based enumeration, so the value could be a combination of these values obtained using
        /// bitwise 'OR' operator.
        /// For example, if the provided value is 6 then health state of nodes with HealthState value of OK (2) and Warning (4)
        /// are returned.
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
        /// </param>
        /// <param name ="applicationsHealthStateFilter">Allows filtering of the application health state objects returned in
        /// the result of cluster health
        /// query based on their health state.
        /// The possible values for this parameter include integer value obtained from members or bitwise operations
        /// on members of HealthStateFilter enumeration. Only applications that match the filter are returned.
        /// All applications are used to evaluate the aggregated health state. If not specified, all entries are returned.
        /// The state values are flag-based enumeration, so the value could be a combination of these values obtained using
        /// bitwise 'OR' operator.
        /// For example, if the provided value is 6 then health state of applications with HealthState value of OK (2) and
        /// Warning (4) are returned.
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
        /// </param>
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
        /// <param name ="excludeHealthStatistics">Indicates whether the health statistics should be returned as part of the
        /// query result. False by default.
        /// The statistics show the number of children entities in health state Ok, Warning, and Error.
        /// </param>
        /// <param name ="includeSystemApplicationHealthStatistics">Indicates whether the health statistics should include the
        /// fabric:/System application health statistics. False by default.
        /// If IncludeSystemApplicationHealthStatistics is set to true, the health statistics include the entities that belong
        /// to the fabric:/System application.
        /// Otherwise, the query result includes health statistics only for user applications.
        /// The health statistics must be included in the query result for this parameter to be applied.
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
        Task<ClusterHealth> GetClusterHealthAsync(
            int? nodesHealthStateFilter = 0,
            int? applicationsHealthStateFilter = 0,
            int? eventsHealthStateFilter = 0,
            bool? excludeHealthStatistics = false,
            bool? includeSystemApplicationHealthStatistics = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the health of a Service Fabric cluster using the specified policy.
        /// </summary>
        /// <remarks>
        /// Use EventsHealthStateFilter to filter the collection of health events reported on the cluster based on the health
        /// state.
        /// Similarly, use NodesHealthStateFilter and ApplicationsHealthStateFilter to filter the collection of nodes and
        /// applications returned based on their aggregated health state.
        /// Use ClusterHealthPolicies to override the health policies used to evaluate the health.
        /// </remarks>
        /// <param name ="nodesHealthStateFilter">Allows filtering of the node health state objects returned in the result of
        /// cluster health query
        /// based on their health state. The possible values for this parameter include integer value of one of the
        /// following health states. Only nodes that match the filter are returned. All nodes are used to evaluate the
        /// aggregated health state.
        /// If not specified, all entries are returned.
        /// The state values are flag-based enumeration, so the value could be a combination of these values obtained using
        /// bitwise 'OR' operator.
        /// For example, if the provided value is 6 then health state of nodes with HealthState value of OK (2) and Warning (4)
        /// are returned.
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
        /// </param>
        /// <param name ="applicationsHealthStateFilter">Allows filtering of the application health state objects returned in
        /// the result of cluster health
        /// query based on their health state.
        /// The possible values for this parameter include integer value obtained from members or bitwise operations
        /// on members of HealthStateFilter enumeration. Only applications that match the filter are returned.
        /// All applications are used to evaluate the aggregated health state. If not specified, all entries are returned.
        /// The state values are flag-based enumeration, so the value could be a combination of these values obtained using
        /// bitwise 'OR' operator.
        /// For example, if the provided value is 6 then health state of applications with HealthState value of OK (2) and
        /// Warning (4) are returned.
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
        /// </param>
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
        /// <param name ="excludeHealthStatistics">Indicates whether the health statistics should be returned as part of the
        /// query result. False by default.
        /// The statistics show the number of children entities in health state Ok, Warning, and Error.
        /// </param>
        /// <param name ="includeSystemApplicationHealthStatistics">Indicates whether the health statistics should include the
        /// fabric:/System application health statistics. False by default.
        /// If IncludeSystemApplicationHealthStatistics is set to true, the health statistics include the entities that belong
        /// to the fabric:/System application.
        /// Otherwise, the query result includes health statistics only for user applications.
        /// The health statistics must be included in the query result for this parameter to be applied.
        /// </param>
        /// <param name ="clusterHealthPolicies">Describes the health policies used to evaluate the cluster health.
        /// If not present, the health evaluation uses the cluster health policy defined in the cluster manifest or the default
        /// cluster health policy.
        /// By default, each application is evaluated using its specific application health policy, defined in the application
        /// manifest, or the default health policy, if no policy is defined in manifest.
        /// If the application health policy map is specified, and it has an entry for an application, the specified
        /// application health policy
        /// is used to evaluate the application health.
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
        Task<ClusterHealth> GetClusterHealthUsingPolicyAsync(
            int? nodesHealthStateFilter = 0,
            int? applicationsHealthStateFilter = 0,
            int? eventsHealthStateFilter = 0,
            bool? excludeHealthStatistics = false,
            bool? includeSystemApplicationHealthStatistics = false,
            ClusterHealthPolicies clusterHealthPolicies = default(ClusterHealthPolicies),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the health of a Service Fabric cluster using health chunks.
        /// </summary>
        /// <remarks>
        /// Gets the health of a Service Fabric cluster using health chunks. Includes the aggregated health state of the
        /// cluster, but none of the cluster entities.
        /// To expand the cluster health and get the health state of all or some of the entities, use the POST URI and specify
        /// the cluster health chunk query description.
        /// </remarks>
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
        Task<ClusterHealthChunk> GetClusterHealthChunkAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the health of a Service Fabric cluster using health chunks.
        /// </summary>
        /// <remarks>
        /// Gets the health of a Service Fabric cluster using health chunks. The health evaluation is done based on the input
        /// cluster health chunk query description.
        /// The query description allows users to specify health policies for evaluating the cluster and its children.
        /// Users can specify very flexible filters to select which cluster entities to return. The selection can be done based
        /// on the entities health state and based on the hierarchy.
        /// The query can return multi-level children of the entities based on the specified filters. For example, it can
        /// return one application with a specified name, and for this application, return
        /// only services that are in Error or Warning, and all partitions and replicas for one of these services.
        /// </remarks>
        /// <param name ="clusterHealthChunkQueryDescription">Describes the cluster and application health policies used to
        /// evaluate the cluster health and the filters to select which cluster entities to be returned.
        /// If the cluster health policy is present, it is used to evaluate the cluster events and the cluster nodes. If not
        /// present, the health evaluation uses the cluster health policy defined in the cluster manifest or the default
        /// cluster health policy.
        /// By default, each application is evaluated using its specific application health policy, defined in the application
        /// manifest, or the default health policy, if no policy is defined in manifest.
        /// If the application health policy map is specified, and it has an entry for an application, the specified
        /// application health policy
        /// is used to evaluate the application health.
        /// Users can specify very flexible filters to select which cluster entities to include in response. The selection can
        /// be done based on the entities health state and based on the hierarchy.
        /// The query can return multi-level children of the entities based on the specified filters. For example, it can
        /// return one application with a specified name, and for this application, return
        /// only services that are in Error or Warning, and all partitions and replicas for one of these services.
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
        Task<ClusterHealthChunk> GetClusterHealthChunkUsingPolicyAndAdvancedFiltersAsync(
            ClusterHealthChunkQueryDescription clusterHealthChunkQueryDescription = default(ClusterHealthChunkQueryDescription),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a health report on the Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Sends a health report on a Service Fabric cluster. The report must contain the information about the source of the
        /// health report and property on which it is reported.
        /// The report is sent to a Service Fabric gateway node, which forwards to the health store.
        /// The report may be accepted by the gateway, but rejected by the health store after extra validation.
        /// For example, the health store may reject the report because of an invalid parameter, like a stale sequence number.
        /// To see whether the report was applied in the health store, run GetClusterHealth and check that the report appears
        /// in the HealthEvents section.
        /// </remarks>
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
        Task ReportClusterHealthAsync(
            HealthInformation healthInformation,
            bool? immediate = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a list of fabric code versions that are provisioned in a Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Gets a list of information about fabric code versions that are provisioned in the cluster. The parameter
        /// CodeVersion can be used to optionally filter the output to only that particular version.
        /// </remarks>
        /// <param name ="codeVersion">The product version of Service Fabric.</param>
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
        Task<IEnumerable<FabricCodeVersionInfo>> GetProvisionedFabricCodeVersionInfoListAsync(
            string codeVersion = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a list of fabric config versions that are provisioned in a Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Gets a list of information about fabric config versions that are provisioned in the cluster. The parameter
        /// ConfigVersion can be used to optionally filter the output to only that particular version.
        /// </remarks>
        /// <param name ="configVersion">The config version of Service Fabric.</param>
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
        Task<IEnumerable<FabricConfigVersionInfo>> GetProvisionedFabricConfigVersionInfoListAsync(
            string configVersion = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the progress of the current cluster upgrade.
        /// </summary>
        /// <remarks>
        /// Gets the current progress of the ongoing cluster upgrade. If no upgrade is currently in progress, get the last
        /// state of the previous cluster upgrade.
        /// </remarks>
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
        Task<ClusterUpgradeProgressObject> GetClusterUpgradeProgressAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get the Service Fabric standalone cluster configuration.
        /// </summary>
        /// <remarks>
        /// The cluster configuration contains properties of the cluster that include different node types on the cluster,
        /// security configurations, fault, and upgrade domain topologies, etc.
        /// </remarks>
        /// <param name ="configurationApiVersion">The API version of the Standalone cluster json configuration.</param>
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
        Task<ClusterConfiguration> GetClusterConfigurationAsync(
            string configurationApiVersion,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get the cluster configuration upgrade status of a Service Fabric standalone cluster.
        /// </summary>
        /// <remarks>
        /// Get the cluster configuration upgrade status details of a Service Fabric standalone cluster.
        /// </remarks>
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
        Task<ClusterConfigurationUpgradeStatusInfo> GetClusterConfigurationUpgradeStatusAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get the service state of Service Fabric Upgrade Orchestration Service.
        /// </summary>
        /// <remarks>
        /// Get the service state of Service Fabric Upgrade Orchestration Service. This API is internally used for support
        /// purposes.
        /// </remarks>
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
        Task<UpgradeOrchestrationServiceState> GetUpgradeOrchestrationServiceStateAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Update the service state of Service Fabric Upgrade Orchestration Service.
        /// </summary>
        /// <remarks>
        /// Update the service state of Service Fabric Upgrade Orchestration Service. This API is internally used for support
        /// purposes.
        /// </remarks>
        /// <param name ="upgradeOrchestrationServiceState">Service state of Service Fabric Upgrade Orchestration
        /// Service.</param>
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
        Task<UpgradeOrchestrationServiceStateSummary> SetUpgradeOrchestrationServiceStateAsync(
            UpgradeOrchestrationServiceState upgradeOrchestrationServiceState,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Provision the code or configuration packages of a Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Validate and provision the code or configuration packages of a Service Fabric cluster.
        /// </remarks>
        /// <param name ="provisionFabricDescription">Describes the parameters for provisioning a cluster.</param>
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
        Task ProvisionClusterAsync(
            ProvisionFabricDescription provisionFabricDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Unprovision the code or configuration packages of a Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// It is supported to unprovision code and configuration separately.
        /// </remarks>
        /// <param name ="unprovisionFabricDescription">Describes the parameters for unprovisioning a cluster.</param>
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
        Task UnprovisionClusterAsync(
            UnprovisionFabricDescription unprovisionFabricDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Roll back the upgrade of a Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Roll back the code or configuration upgrade of a Service Fabric cluster.
        /// </remarks>
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
        Task RollbackClusterUpgradeAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Make the cluster upgrade move on to the next upgrade domain.
        /// </summary>
        /// <remarks>
        /// Make the cluster code or configuration upgrade move on to the next upgrade domain if appropriate.
        /// </remarks>
        /// <param name ="resumeClusterUpgradeDescription">Describes the parameters for resuming a cluster upgrade.</param>
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
        Task ResumeClusterUpgradeAsync(
            ResumeClusterUpgradeDescription resumeClusterUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Start upgrading the code or configuration version of a Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Validate the supplied upgrade parameters and start upgrading the code or configuration version of a Service Fabric
        /// cluster if the parameters are valid.
        /// </remarks>
        /// <param name ="startClusterUpgradeDescription">Describes the parameters for starting a cluster upgrade.</param>
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
        Task StartClusterUpgradeAsync(
            StartClusterUpgradeDescription startClusterUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Start upgrading the configuration of a Service Fabric standalone cluster.
        /// </summary>
        /// <remarks>
        /// Validate the supplied configuration upgrade parameters and start upgrading the cluster configuration if the
        /// parameters are valid.
        /// </remarks>
        /// <param name ="clusterConfigurationUpgradeDescription">Parameters for a standalone cluster configuration
        /// upgrade.</param>
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
        Task StartClusterConfigurationUpgradeAsync(
            ClusterConfigurationUpgradeDescription clusterConfigurationUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Update the upgrade parameters of a Service Fabric cluster upgrade.
        /// </summary>
        /// <remarks>
        /// Update the upgrade parameters used during a Service Fabric cluster upgrade.
        /// </remarks>
        /// <param name ="updateClusterUpgradeDescription">Parameters for updating a cluster upgrade.</param>
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
        Task UpdateClusterUpgradeAsync(
            UpdateClusterUpgradeDescription updateClusterUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the Azure Active Directory metadata used for secured connection to cluster.
        /// </summary>
        /// <remarks>
        /// Gets the Azure Active Directory metadata used for secured connection to cluster.
        /// This API is not supposed to be called separately. It provides information needed to set up an Azure Active
        /// Directory secured connection with a Service Fabric cluster.
        /// </remarks>
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
        Task<AadMetadataObject> GetAadMetadataAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get the current Service Fabric cluster version.
        /// </summary>
        /// <remarks>
        /// If a cluster upgrade is happening, then this API will return the lowest (older) version of the current and target
        /// cluster runtime versions.
        /// </remarks>
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
        Task<ClusterVersion> GetClusterVersionAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the load of a Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Retrieves the load information of a Service Fabric cluster for all the metrics that have load or capacity defined.
        /// </remarks>
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
        Task<ClusterLoadInfo> GetClusterLoadAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Changes the verbosity of service placement health reporting.
        /// </summary>
        /// <remarks>
        /// If verbosity is set to true, then detailed health reports will be generated when replicas cannot be placed or
        /// dropped.
        /// If verbosity is set to false, then no health reports will be generated when replicas cannot be placed or dropped.
        /// </remarks>
        /// <param name ="enabled">The verbosity of service placement health reporting.</param>
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
        Task ToggleVerboseServicePlacementHealthReportingAsync(
            bool? enabled,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Validate and assess the impact of a code or configuration version update of a Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Validate the supplied upgrade parameters and assess the expected impact of a code or configuration version upgrade
        /// of a Service Fabric cluster. The upgrade will not be initiated.
        /// </remarks>
        /// <param name ="startClusterUpgradeDescription">Describes the parameters for starting a cluster upgrade.</param>
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
        Task<ValidateClusterUpgradeResult> ValidateClusterUpgradeAsync(
            StartClusterUpgradeDescription startClusterUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
