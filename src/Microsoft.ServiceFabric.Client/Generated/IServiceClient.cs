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
    /// Interface containing methods for performing ServiceClient operations.
    /// </summary>
    public partial interface IServiceClient
    {
        /// <summary>
        /// Gets the information about all services belonging to the application specified by the application ID.
        /// </summary>
        /// <remarks>
        /// Returns the information about all services belonging to the application specified by the application ID.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="serviceTypeName">The service type name used to filter the services to query for.</param>
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
        Task<PagedData<ServiceInfo>> GetServiceInfoListAsync(
            string applicationId,
            string serviceTypeName = default(string),
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the information about the specific service belonging to the Service Fabric application.
        /// </summary>
        /// <remarks>
        /// Returns the information about the specified service belonging to the specified Service Fabric application.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
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
        Task<ServiceInfo> GetServiceInfoAsync(
            string applicationId,
            string serviceId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the name of the Service Fabric application for a service.
        /// </summary>
        /// <remarks>
        /// Gets the name of the application for the specified service. A 404 FABRIC_E_SERVICE_DOES_NOT_EXIST error is returned
        /// if a service with the provided service ID does not exist.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
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
        Task<ApplicationNameInfo> GetApplicationNameInfoAsync(
            string serviceId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates the specified Service Fabric service.
        /// </summary>
        /// <remarks>
        /// This api allows creating a new Service Fabric stateless or stateful service under a specified Service Fabric
        /// application. The description for creating the service includes partitioning information and optional properties for
        /// placement and load balancing. Some of the properties can later be modified using `UpdateService` API.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="serviceDescription">The information necessary to create a service.</param>
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
        Task CreateServiceAsync(
            string applicationId,
            ServiceDescription serviceDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a Service Fabric service from the service template.
        /// </summary>
        /// <remarks>
        /// Creates a Service Fabric service from the service template defined in the application manifest. A service template
        /// contains the properties that will be same for the service instance of the same type. The API allows overriding the
        /// properties that are usually different for different services of the same service type.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="serviceFromTemplateDescription">Describes the service that needs to be created from the template
        /// defined in the application manifest.</param>
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
        Task CreateServiceFromTemplateAsync(
            string applicationId,
            ServiceFromTemplateDescription serviceFromTemplateDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes an existing Service Fabric service.
        /// </summary>
        /// <remarks>
        /// A service must be created before it can be deleted. By default, Service Fabric will try to close service replicas
        /// in a graceful manner and then delete the service. However, if the service is having issues closing the replica
        /// gracefully, the delete operation may take a long time or get stuck. Use the optional ForceRemove flag to skip the
        /// graceful close sequence and forcefully delete the service.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
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
        Task DeleteServiceAsync(
            string serviceId,
            bool? forceRemove = default(bool?),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates a Service Fabric service using the specified update description.
        /// </summary>
        /// <remarks>
        /// This API allows updating properties of a running Service Fabric service. The set of properties that can be updated
        /// are a subset of the properties that were specified at the time of creating the service. The current set of
        /// properties can be obtained using `GetServiceDescription` API. Note that updating the properties of a running
        /// service is different than upgrading your application using `StartApplicationUpgrade` API. The upgrade is a long
        /// running background operation that involves moving the application from one version to another, one upgrade domain
        /// at a time, whereas update applies the new properties immediately to the service.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name ="serviceUpdateDescription">The information necessary to update a service.</param>
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
        Task UpdateServiceAsync(
            string serviceId,
            ServiceUpdateDescription serviceUpdateDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the description of an existing Service Fabric service.
        /// </summary>
        /// <remarks>
        /// Gets the description of an existing Service Fabric service. A service must be created before its description can be
        /// obtained.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
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
        Task<ServiceDescription> GetServiceDescriptionAsync(
            string serviceId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the health of the specified Service Fabric service.
        /// </summary>
        /// <remarks>
        /// Gets the health information of the specified service.
        /// Use EventsHealthStateFilter to filter the collection of health events reported on the service based on the health
        /// state.
        /// Use PartitionsHealthStateFilter to filter the collection of partitions returned.
        /// If you specify a service that does not exist in the health store, this request returns an error.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
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
        /// <param name ="partitionsHealthStateFilter">Allows filtering of the partitions health state objects returned in the
        /// result of service health query based on their health state.
        /// The possible values for this parameter include integer value of one of the following health states.
        /// Only partitions that match the filter are returned. All partitions are used to evaluate the aggregated health
        /// state.
        /// If not specified, all entries are returned. The state values are flag-based enumeration, so the value could be a
        /// combination of these value
        /// obtained using bitwise 'OR' operator. For example, if the provided value is 6 then health state of partitions with
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
        Task<ServiceHealth> GetServiceHealthAsync(
            string serviceId,
            int? eventsHealthStateFilter = 0,
            int? partitionsHealthStateFilter = 0,
            bool? excludeHealthStatistics = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the health of the specified Service Fabric service, by using the specified health policy.
        /// </summary>
        /// <remarks>
        /// Gets the health information of the specified service.
        /// If the application health policy is specified, the health evaluation uses it to get the aggregated health state.
        /// If the policy is not specified, the health evaluation uses the application health policy defined in the application
        /// manifest, or the default health policy, if no policy is defined in the manifest.
        /// Use EventsHealthStateFilter to filter the collection of health events reported on the service based on the health
        /// state.
        /// Use PartitionsHealthStateFilter to filter the collection of partitions returned.
        /// If you specify a service that does not exist in the health store, this request returns an error.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
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
        /// <param name ="partitionsHealthStateFilter">Allows filtering of the partitions health state objects returned in the
        /// result of service health query based on their health state.
        /// The possible values for this parameter include integer value of one of the following health states.
        /// Only partitions that match the filter are returned. All partitions are used to evaluate the aggregated health
        /// state.
        /// If not specified, all entries are returned. The state values are flag-based enumeration, so the value could be a
        /// combination of these value
        /// obtained using bitwise 'OR' operator. For example, if the provided value is 6 then health state of partitions with
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
        Task<ServiceHealth> GetServiceHealthUsingPolicyAsync(
            string serviceId,
            int? eventsHealthStateFilter = 0,
            int? partitionsHealthStateFilter = 0,
            ApplicationHealthPolicy applicationHealthPolicy = default(ApplicationHealthPolicy),
            bool? excludeHealthStatistics = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a health report on the Service Fabric service.
        /// </summary>
        /// <remarks>
        /// Reports health state of the specified Service Fabric service. The report must contain the information about the
        /// source of the health report and property on which it is reported.
        /// The report is sent to a Service Fabric gateway Service, which forwards to the health store.
        /// The report may be accepted by the gateway, but rejected by the health store after extra validation.
        /// For example, the health store may reject the report because of an invalid parameter, like a stale sequence number.
        /// To see whether the report was applied in the health store, run GetServiceHealth and check that the report appears
        /// in the HealthEvents section.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
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
        Task ReportServiceHealthAsync(
            string serviceId,
            HealthInformation healthInformation,
            bool? immediate = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Resolve a Service Fabric partition.
        /// </summary>
        /// <remarks>
        /// Resolve a Service Fabric service partition to get the endpoints of the service replicas.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name ="partitionKeyType">Key type for the partition. This parameter is required if the partition scheme for
        /// the service is Int64Range or Named. The possible values are following.
        /// - None (1) - Indicates that the PartitionKeyValue parameter is not specified. This is valid for the partitions with
        /// partitioning scheme as Singleton. This is the default value. The value is 1.
        /// - Int64Range (2) - Indicates that the PartitionKeyValue parameter is an int64 partition key. This is valid for the
        /// partitions with partitioning scheme as Int64Range. The value is 2.
        /// - Named (3) - Indicates that the PartitionKeyValue parameter is a name of the partition. This is valid for the
        /// partitions with partitioning scheme as Named. The value is 3.
        /// </param>
        /// <param name ="partitionKeyValue">Partition key. This is required if the partition scheme for the service is
        /// Int64Range or Named.
        /// This is not the partition ID, but rather, either the integer key value, or the name of the partition ID.
        /// For example, if your service is using ranged partitions from 0 to 10, then they PartitionKeyValue would be an
        /// integer in that range. Query service description to see the range or name.
        /// </param>
        /// <param name ="previousRspVersion">The value in the Version field of the response that was received previously. This
        /// is required if the user knows that the result that was gotten previously is stale.</param>
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
        Task<ResolvedServicePartition> ResolveServiceAsync(
            string serviceId,
            int? partitionKeyType = default(int?),
            string partitionKeyValue = default(string),
            string previousRspVersion = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the information about unplaced replica of the service.
        /// </summary>
        /// <remarks>
        /// Returns the information about the unplaced replicas of the service.
        /// If PartitionId is specified, then result will contain information only about unplaced replicas for that partition.
        /// If PartitionId is not specified, then result will contain information about unplaced replicas for all partitions of
        /// that service.
        /// If OnlyQueryPrimaries is set to true, then result will contain information only about primary replicas, and will
        /// ignore unplaced secondary replicas.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="onlyQueryPrimaries">Indicates that unplaced replica information will be queries only for primary
        /// replicas.</param>
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
        Task<UnplacedReplicaInformation> GetUnplacedReplicaInformationAsync(
            string serviceId,
            PartitionId partitionId = default(PartitionId),
            bool? onlyQueryPrimaries = false,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the Arm Metadata for a specific service.
        /// </summary>
        /// <remarks>
        /// Updates the Arm Metadata for a specific service.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name ="serviceArmMetadataUpdateDescription">The Arm metadata to be associated with a specific
        /// service</param>
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
        Task UpdateServiceArmMetadataAsync(
            string serviceId,
            ServiceArmMetadataUpdateDescription serviceArmMetadataUpdateDescription,
            long? serverTimeout = 60,
            bool? force = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
