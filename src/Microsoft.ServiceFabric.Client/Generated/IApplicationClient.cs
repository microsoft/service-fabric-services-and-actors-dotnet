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
    /// Interface containing methods for performing ApplicationClient operations.
    /// </summary>
    public partial interface IApplicationClient
    {
        /// <summary>
        /// Creates a Service Fabric application.
        /// </summary>
        /// <remarks>
        /// Creates a Service Fabric application using the specified description.
        /// </remarks>
        /// <param name ="applicationDescription">Description for creating an application.</param>
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
        Task CreateApplicationAsync(
            ApplicationDescription applicationDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes an existing Service Fabric application.
        /// </summary>
        /// <remarks>
        /// An application must be created before it can be deleted. Deleting an application will delete all services that are
        /// part of that application. By default, Service Fabric will try to close service replicas in a graceful manner and
        /// then delete the service. However, if a service is having issues closing the replica gracefully, the delete
        /// operation may take a long time or get stuck. Use the optional ForceRemove flag to skip the graceful close sequence
        /// and forcefully delete the application and all of its services.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
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
        Task DeleteApplicationAsync(
            string applicationId,
            bool? forceRemove = default(bool?),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets load information about a Service Fabric application.
        /// </summary>
        /// <remarks>
        /// Returns the load information about the application that was created or in the process of being created in the
        /// Service Fabric cluster and whose name matches the one specified as the parameter. The response includes the name,
        /// minimum nodes, maximum nodes, the number of nodes the application is occupying currently, and application load
        /// metric information about the application.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
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
        Task<ApplicationLoadInfo> GetApplicationLoadInfoAsync(
            string applicationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the list of applications created in the Service Fabric cluster that match the specified filters.
        /// </summary>
        /// <remarks>
        /// Gets the information about the applications that were created or in the process of being created in the Service
        /// Fabric cluster and match the specified filters. The response includes the name, type, status, parameters, and other
        /// details about the application. If the applications do not fit in a page, one page of results is returned as well as
        /// a continuation token, which can be used to get the next page. Filters ApplicationTypeName and
        /// ApplicationDefinitionKindFilter cannot be specified at the same time.
        /// </remarks>
        /// <param name ="applicationDefinitionKindFilter">Used to filter on ApplicationDefinitionKind, which is the mechanism
        /// used to define a Service Fabric application.
        /// - Default - Default value, which performs the same function as selecting "All". The value is 0.
        /// - All - Filter that matches input with any ApplicationDefinitionKind value. The value is 65535.
        /// - ServiceFabricApplicationDescription - Filter that matches input with ApplicationDefinitionKind value
        /// ServiceFabricApplicationDescription. The value is 1.
        /// - Compose - Filter that matches input with ApplicationDefinitionKind value Compose. The value is 2.
        /// </param>
        /// <param name ="applicationTypeName">The application type name used to filter the applications to query for. This
        /// value should not contain the application type version.</param>
        /// <param name ="excludeApplicationParameters">The flag that specifies whether application parameters will be excluded
        /// from the result.</param>
        /// <param name ="continuationToken">The continuation token to obtain next set of results</param>
        /// <param name ="maxResults">The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.</param>
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
        Task<PagedData<ApplicationInfo>> GetApplicationInfoListAsync(
            int? applicationDefinitionKindFilter = 0,
            string applicationTypeName = default(string),
            bool? excludeApplicationParameters = false,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets information about a Service Fabric application.
        /// </summary>
        /// <remarks>
        /// Returns the information about the application that was created or in the process of being created in the Service
        /// Fabric cluster and whose name matches the one specified as the parameter. The response includes the name, type,
        /// status, parameters, and other details about the application.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="excludeApplicationParameters">The flag that specifies whether application parameters will be excluded
        /// from the result.</param>
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
        Task<ApplicationInfo> GetApplicationInfoAsync(
            string applicationId,
            bool? excludeApplicationParameters = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the health of the service fabric application.
        /// </summary>
        /// <remarks>
        /// Returns the heath state of the service fabric application. The response reports either Ok, Error or Warning health
        /// state. If the entity is not found in the health store, it will return Error.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
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
        /// <param name ="deployedApplicationsHealthStateFilter">Allows filtering of the deployed applications health state
        /// objects returned in the result of application health query based on their health state.
        /// The possible values for this parameter include integer value of one of the following health states. Only deployed
        /// applications that match the filter will be returned.
        /// All deployed applications are used to evaluate the aggregated health state. If not specified, all entries are
        /// returned.
        /// The state values are flag-based enumeration, so the value could be a combination of these values, obtained using
        /// bitwise 'OR' operator.
        /// For example, if the provided value is 6 then health state of deployed applications with HealthState value of OK (2)
        /// and Warning (4) are returned.
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
        /// </param>
        /// <param name ="servicesHealthStateFilter">Allows filtering of the services health state objects returned in the
        /// result of services health query based on their health state.
        /// The possible values for this parameter include integer value of one of the following health states.
        /// Only services that match the filter are returned. All services are used to evaluate the aggregated health state.
        /// If not specified, all entries are returned. The state values are flag-based enumeration, so the value could be a
        /// combination of these values,
        /// obtained using bitwise 'OR' operator. For example, if the provided value is 6 then health state of services with
        /// HealthState value of OK (2) and Warning (4) will be returned.
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
        Task<ApplicationHealth> GetApplicationHealthAsync(
            string applicationId,
            int? eventsHealthStateFilter = 0,
            int? deployedApplicationsHealthStateFilter = 0,
            int? servicesHealthStateFilter = 0,
            bool? excludeHealthStatistics = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the health of a Service Fabric application using the specified policy.
        /// </summary>
        /// <remarks>
        /// Gets the health of a Service Fabric application. Use EventsHealthStateFilter to filter the collection of health
        /// events reported on the node based on the health state. Use ClusterHealthPolicies to override the health policies
        /// used to evaluate the health.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
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
        /// <param name ="deployedApplicationsHealthStateFilter">Allows filtering of the deployed applications health state
        /// objects returned in the result of application health query based on their health state.
        /// The possible values for this parameter include integer value of one of the following health states. Only deployed
        /// applications that match the filter will be returned.
        /// All deployed applications are used to evaluate the aggregated health state. If not specified, all entries are
        /// returned.
        /// The state values are flag-based enumeration, so the value could be a combination of these values, obtained using
        /// bitwise 'OR' operator.
        /// For example, if the provided value is 6 then health state of deployed applications with HealthState value of OK (2)
        /// and Warning (4) are returned.
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
        /// </param>
        /// <param name ="servicesHealthStateFilter">Allows filtering of the services health state objects returned in the
        /// result of services health query based on their health state.
        /// The possible values for this parameter include integer value of one of the following health states.
        /// Only services that match the filter are returned. All services are used to evaluate the aggregated health state.
        /// If not specified, all entries are returned. The state values are flag-based enumeration, so the value could be a
        /// combination of these values,
        /// obtained using bitwise 'OR' operator. For example, if the provided value is 6 then health state of services with
        /// HealthState value of OK (2) and Warning (4) will be returned.
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
        Task<ApplicationHealth> GetApplicationHealthUsingPolicyAsync(
            string applicationId,
            int? eventsHealthStateFilter = 0,
            int? deployedApplicationsHealthStateFilter = 0,
            int? servicesHealthStateFilter = 0,
            bool? excludeHealthStatistics = false,
            ApplicationHealthPolicy applicationHealthPolicy = default(ApplicationHealthPolicy),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a health report on the Service Fabric application.
        /// </summary>
        /// <remarks>
        /// Reports health state of the specified Service Fabric application. The report must contain the information about the
        /// source of the health report and property on which it is reported.
        /// The report is sent to a Service Fabric gateway Application, which forwards to the health store.
        /// The report may be accepted by the gateway, but rejected by the health store after extra validation.
        /// For example, the health store may reject the report because of an invalid parameter, like a stale sequence number.
        /// To see whether the report was applied in the health store, get application health and check that the report appears
        /// in the HealthEvents section.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
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
        Task ReportApplicationHealthAsync(
            string applicationId,
            HealthInformation healthInformation,
            bool? immediate = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Starts upgrading an application in the Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Validates the supplied application upgrade parameters and starts upgrading the application if the parameters are
        /// valid.
        /// Note,
        /// [ApplicationParameter](https://docs.microsoft.com/dotnet/api/system.fabric.description.applicationdescription.applicationparameters)s
        /// are not preserved across an application upgrade.
        /// In order to preserve current application parameters, the user should get the parameters using
        /// [GetApplicationInfo](./GetApplicationInfo.md) operation first and pass them into the upgrade API call as shown in
        /// the example.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="applicationUpgradeDescription">Parameters for an application upgrade.</param>
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
        Task StartApplicationUpgradeAsync(
            string applicationId,
            ApplicationUpgradeDescription applicationUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets details for the latest upgrade performed on this application.
        /// </summary>
        /// <remarks>
        /// Returns information about the state of the latest application upgrade along with details to aid debugging
        /// application health issues.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
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
        Task<ApplicationUpgradeProgressInfo> GetApplicationUpgradeAsync(
            string applicationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates an ongoing application upgrade in the Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Updates the parameters of an ongoing application upgrade from the ones specified at the time of starting the
        /// application upgrade. This may be required to mitigate stuck application upgrades due to incorrect parameters or
        /// issues in the application to make progress.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="applicationUpgradeUpdateDescription">Parameters for updating an existing application upgrade.</param>
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
        Task UpdateApplicationUpgradeAsync(
            string applicationId,
            ApplicationUpgradeUpdateDescription applicationUpgradeUpdateDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates a Service Fabric application.
        /// </summary>
        /// <remarks>
        /// Updates a Service Fabric application instance. The set of properties that can be updated are a subset of the
        /// properties that were specified at the time of creating the application.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="applicationUpdateDescription">Parameters for updating an existing application instance.</param>
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
        Task UpdateApplicationAsync(
            string applicationId,
            ApplicationUpdateDescription applicationUpdateDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Resumes upgrading an application in the Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Resumes an unmonitored manual Service Fabric application upgrade. Service Fabric upgrades one upgrade domain at a
        /// time. For unmonitored manual upgrades, after Service Fabric finishes an upgrade domain, it waits for you to call
        /// this API before proceeding to the next upgrade domain.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="resumeApplicationUpgradeDescription">Describes the parameters for resuming an application
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
        Task ResumeApplicationUpgradeAsync(
            string applicationId,
            ResumeApplicationUpgradeDescription resumeApplicationUpgradeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Starts rolling back the currently on-going upgrade of an application in the Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Starts rolling back the current application upgrade to the previous version. This API can only be used to roll back
        /// the current in-progress upgrade that is rolling forward to new version. If the application is not currently being
        /// upgraded use StartApplicationUpgrade API to upgrade it to desired version, including rolling back to a previous
        /// version.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
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
        Task RollbackApplicationUpgradeAsync(
            string applicationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the list of applications deployed on a Service Fabric node.
        /// </summary>
        /// <remarks>
        /// Gets the list of applications deployed on a Service Fabric node. The results do not include information about
        /// deployed system applications unless explicitly queried for by ID. Results encompass deployed applications in
        /// active, activating, and downloading states. This query requires that the node name corresponds to a node on the
        /// cluster. The query fails if the provided node name does not point to any active Service Fabric nodes on the
        /// cluster.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="includeHealthState">Include the health state of an entity.
        /// If this parameter is false or not specified, then the health state returned is "Unknown".
        /// When set to true, the query goes in parallel to the node and the health system service before the results are
        /// merged.
        /// As a result, the query is more expensive and may take a longer time.
        /// </param>
        /// <param name ="continuationToken">The continuation token to obtain next set of results</param>
        /// <param name ="maxResults">The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<PagedData<DeployedApplicationInfo>> GetDeployedApplicationInfoListAsync(
            NodeName nodeName,
            long? serverTimeout = 60,
            bool? includeHealthState = false,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the information about an application deployed on a Service Fabric node.
        /// </summary>
        /// <remarks>
        /// This query returns system application information if the application ID provided is for system application. Results
        /// encompass deployed applications in active, activating, and downloading states. This query requires that the node
        /// name corresponds to a node on the cluster. The query fails if the provided node name does not point to any active
        /// Service Fabric nodes on the cluster.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="includeHealthState">Include the health state of an entity.
        /// If this parameter is false or not specified, then the health state returned is "Unknown".
        /// When set to true, the query goes in parallel to the node and the health system service before the results are
        /// merged.
        /// As a result, the query is more expensive and may take a longer time.
        /// </param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<DeployedApplicationInfo> GetDeployedApplicationInfoAsync(
            NodeName nodeName,
            string applicationId,
            long? serverTimeout = 60,
            bool? includeHealthState = false,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the information about health of an application deployed on a Service Fabric node.
        /// </summary>
        /// <remarks>
        /// Gets the information about health of an application deployed on a Service Fabric node. Use EventsHealthStateFilter
        /// to optionally filter for the collection of HealthEvent objects reported on the deployed application based on health
        /// state. Use DeployedServicePackagesHealthStateFilter to optionally filter for DeployedServicePackageHealth children
        /// based on health state.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
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
        /// <param name ="deployedServicePackagesHealthStateFilter">Allows filtering of the deployed service package health
        /// state objects returned in the result of deployed application health query based on their health state.
        /// The possible values for this parameter include integer value of one of the following health states.
        /// Only deployed service packages that match the filter are returned. All deployed service packages are used to
        /// evaluate the aggregated health state of the deployed application.
        /// If not specified, all entries are returned.
        /// The state values are flag-based enumeration, so the value can be a combination of these values, obtained using the
        /// bitwise 'OR' operator.
        /// For example, if the provided value is 6 then health state of service packages with HealthState value of OK (2) and
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
        /// <param name ="excludeHealthStatistics">Indicates whether the health statistics should be returned as part of the
        /// query result. False by default.
        /// The statistics show the number of children entities in health state Ok, Warning, and Error.
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
        Task<DeployedApplicationHealth> GetDeployedApplicationHealthAsync(
            NodeName nodeName,
            string applicationId,
            int? eventsHealthStateFilter = 0,
            int? deployedServicePackagesHealthStateFilter = 0,
            bool? excludeHealthStatistics = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the information about health of an application deployed on a Service Fabric node. using the specified policy.
        /// </summary>
        /// <remarks>
        /// Gets the information about health of an application deployed on a Service Fabric node using the specified policy.
        /// Use EventsHealthStateFilter to optionally filter for the collection of HealthEvent objects reported on the deployed
        /// application based on health state. Use DeployedServicePackagesHealthStateFilter to optionally filter for
        /// DeployedServicePackageHealth children based on health state. Use ApplicationHealthPolicy to optionally override the
        /// health policies used to evaluate the health. This API only uses 'ConsiderWarningAsError' field of the
        /// ApplicationHealthPolicy. The rest of the fields are ignored while evaluating the health of the deployed
        /// application.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
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
        /// <param name ="deployedServicePackagesHealthStateFilter">Allows filtering of the deployed service package health
        /// state objects returned in the result of deployed application health query based on their health state.
        /// The possible values for this parameter include integer value of one of the following health states.
        /// Only deployed service packages that match the filter are returned. All deployed service packages are used to
        /// evaluate the aggregated health state of the deployed application.
        /// If not specified, all entries are returned.
        /// The state values are flag-based enumeration, so the value can be a combination of these values, obtained using the
        /// bitwise 'OR' operator.
        /// For example, if the provided value is 6 then health state of service packages with HealthState value of OK (2) and
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
        /// <param name ="applicationHealthPolicy">Describes the health policies used to evaluate the health of an application
        /// or one of its children.
        /// If not present, the health evaluation uses the health policy from application manifest or the default health
        /// policy.
        /// </param>
        /// <param name ="excludeHealthStatistics">Indicates whether the health statistics should be returned as part of the
        /// query result. False by default.
        /// The statistics show the number of children entities in health state Ok, Warning, and Error.
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
        Task<DeployedApplicationHealth> GetDeployedApplicationHealthUsingPolicyAsync(
            NodeName nodeName,
            string applicationId,
            int? eventsHealthStateFilter = 0,
            int? deployedServicePackagesHealthStateFilter = 0,
            ApplicationHealthPolicy applicationHealthPolicy = default(ApplicationHealthPolicy),
            bool? excludeHealthStatistics = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a health report on the Service Fabric application deployed on a Service Fabric node.
        /// </summary>
        /// <remarks>
        /// Reports health state of the application deployed on a Service Fabric node. The report must contain the information
        /// about the source of the health report and property on which it is reported.
        /// The report is sent to a Service Fabric gateway Service, which forwards to the health store.
        /// The report may be accepted by the gateway, but rejected by the health store after extra validation.
        /// For example, the health store may reject the report because of an invalid parameter, like a stale sequence number.
        /// To see whether the report was applied in the health store, get deployed application health and check that the
        /// report appears in the HealthEvents section.
        /// </remarks>
        /// <param name ="nodeName">The name of the node.</param>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
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
        Task ReportDeployedApplicationHealthAsync(
            NodeName nodeName,
            string applicationId,
            HealthInformation healthInformation,
            bool? immediate = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the Arm Metadata for a specific Application.
        /// </summary>
        /// <remarks>
        /// Updates the Arm Metadata for a specific Application. Is able to be called immediately after the create app API is
        /// called.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="applicationArmMetadataUpdateDescription">The Arm metadata to be associated with a specific
        /// application</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="force">Force parameter used to prevent accidental Arm metadata update.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task UpdateApplicationArmMetadataAsync(
            string applicationId,
            ApplicationArmMetadataUpdateDescription applicationArmMetadataUpdateDescription,
            long? serverTimeout = 60,
            bool? force = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
