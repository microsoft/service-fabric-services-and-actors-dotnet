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
    /// Interface containing methods for performing BackupRestoreClient operations.
    /// </summary>
    public partial interface IBackupRestoreClient
    {
        /// <summary>
        /// Creates a backup policy.
        /// </summary>
        /// <remarks>
        /// Creates a backup policy which can be associated later with a Service Fabric application, service or a partition for
        /// periodic backup.
        /// </remarks>
        /// <param name ="backupPolicyDescription">Describes the backup policy.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="validateConnection">Specifies whether to validate the storage connection and credentials before
        /// creating or updating the backup policies.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task CreateBackupPolicyAsync(
            BackupPolicyDescription backupPolicyDescription,
            long? serverTimeout = 60,
            bool? validateConnection = false,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the backup policy.
        /// </summary>
        /// <remarks>
        /// Deletes an existing backup policy. A backup policy must be created before it can be deleted. A currently active
        /// backup policy, associated with any Service Fabric application, service or partition, cannot be deleted without
        /// first deleting the mapping.
        /// </remarks>
        /// <param name ="backupPolicyName">The name of the backup policy.</param>
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
        Task DeleteBackupPolicyAsync(
            string backupPolicyName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all the backup policies configured.
        /// </summary>
        /// <remarks>
        /// Get a list of all the backup policies configured.
        /// </remarks>
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
        Task<PagedData<BackupPolicyDescription>> GetBackupPolicyListAsync(
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a particular backup policy by name.
        /// </summary>
        /// <remarks>
        /// Gets a particular backup policy identified by {backupPolicyName}
        /// </remarks>
        /// <param name ="backupPolicyName">The name of the backup policy.</param>
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
        Task<BackupPolicyDescription> GetBackupPolicyByNameAsync(
            string backupPolicyName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the list of backup entities that are associated with this policy.
        /// </summary>
        /// <remarks>
        /// Returns a list of Service Fabric application, service or partition which are associated with this backup policy.
        /// </remarks>
        /// <param name ="backupPolicyName">The name of the backup policy.</param>
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
        Task<PagedData<BackupEntity>> GetAllEntitiesBackedUpByPolicyAsync(
            string backupPolicyName,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the backup policy.
        /// </summary>
        /// <remarks>
        /// Updates the backup policy identified by {backupPolicyName}
        /// </remarks>
        /// <param name ="backupPolicyDescription">Describes the backup policy.</param>
        /// <param name ="backupPolicyName">The name of the backup policy.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="validateConnection">Specifies whether to validate the storage connection and credentials before
        /// creating or updating the backup policies.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task UpdateBackupPolicyAsync(
            BackupPolicyDescription backupPolicyDescription,
            string backupPolicyName,
            long? serverTimeout = 60,
            bool? validateConnection = false,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Enables periodic backup of stateful partitions under this Service Fabric application.
        /// </summary>
        /// <remarks>
        /// Enables periodic backup of stateful partitions which are part of this Service Fabric application. Each partition is
        /// backed up individually as per the specified backup policy description.
        /// Note only C# based Reliable Actor and Reliable Stateful services are currently supported for periodic backup.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name ="enableBackupDescription">Specifies the parameters for enabling backup.</param>
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
        Task EnableApplicationBackupAsync(
            string applicationId,
            EnableBackupDescription enableBackupDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Disables periodic backup of Service Fabric application.
        /// </summary>
        /// <remarks>
        /// Disables periodic backup of Service Fabric application which was previously enabled.
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
        /// <param name ="disableBackupDescription">Specifies the parameters to disable backup for any backup entity.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task DisableApplicationBackupAsync(
            string applicationId,
            long? serverTimeout = 60,
            DisableBackupDescription disableBackupDescription = default(DisableBackupDescription),
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the Service Fabric application backup configuration information.
        /// </summary>
        /// <remarks>
        /// Gets the Service Fabric backup configuration information for the application and the services and partitions under
        /// this application.
        /// </remarks>
        /// <param name ="applicationId">The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </param>
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
        Task<PagedData<BackupConfigurationInfo>> GetApplicationBackupConfigurationInfoAsync(
            string applicationId,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the list of backups available for every partition in this application.
        /// </summary>
        /// <remarks>
        /// Returns a list of backups available for every partition in this Service Fabric application. The server enumerates
        /// all the backups available at the backup location configured in the backup policy. It also allows filtering of the
        /// result based on start and end datetime or just fetching the latest available backup for every partition.
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
        /// <param name ="latest">Specifies whether to get only the most recent backup available for a partition for the
        /// specified time range.</param>
        /// <param name ="startDateTimeFilter">Specify the start date time from which to enumerate backups, in datetime format.
        /// The date time must be specified in ISO8601 format. This is an optional parameter. If not specified, all backups
        /// from the beginning are enumerated.</param>
        /// <param name ="endDateTimeFilter">Specify the end date time till which to enumerate backups, in datetime format. The
        /// date time must be specified in ISO8601 format. This is an optional parameter. If not specified, enumeration is done
        /// till the most recent backup.</param>
        /// <param name ="continuationToken">The continuation token to obtain next set of results</param>
        /// <param name ="maxResults">The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.</param>
        /// <param name ="metadataFromBlob">Specifies whether to fetch backup metadata from the backup blob in the
        /// response.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<PagedData<BackupInfo>> GetApplicationBackupListAsync(
            string applicationId,
            long? serverTimeout = 60,
            bool? latest = false,
            DateTime? startDateTimeFilter = default(DateTime?),
            DateTime? endDateTimeFilter = default(DateTime?),
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            bool? metadataFromBlob = false,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Suspends periodic backup for the specified Service Fabric application.
        /// </summary>
        /// <remarks>
        /// The application which is configured to take periodic backups, is suspended for taking further backups till it is
        /// resumed again. This operation applies to the entire application's hierarchy. It means all the services and
        /// partitions under this application are now suspended for backup.
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
        Task SuspendApplicationBackupAsync(
            string applicationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Resumes periodic backup of a Service Fabric application which was previously suspended.
        /// </summary>
        /// <remarks>
        /// The previously suspended Service Fabric application resumes taking periodic backup as per the backup policy
        /// currently configured for the same.
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
        Task ResumeApplicationBackupAsync(
            string applicationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Enables periodic backup of stateful partitions under this Service Fabric service.
        /// </summary>
        /// <remarks>
        /// Enables periodic backup of stateful partitions which are part of this Service Fabric service. Each partition is
        /// backed up individually as per the specified backup policy description. In case the application, which the service
        /// is part of, is already enabled for backup then this operation would override the policy being used to take the
        /// periodic backup for this service and its partitions (unless explicitly overridden at the partition level).
        /// Note only C# based Reliable Actor and Reliable Stateful services are currently supported for periodic backup.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name ="enableBackupDescription">Specifies the parameters for enabling backup.</param>
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
        Task EnableServiceBackupAsync(
            string serviceId,
            EnableBackupDescription enableBackupDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Disables periodic backup of Service Fabric service which was previously enabled.
        /// </summary>
        /// <remarks>
        /// Disables periodic backup of Service Fabric service which was previously enabled. Backup must be explicitly enabled
        /// before it can be disabled.
        /// In case the backup is enabled for the Service Fabric application, which this service is part of, this service would
        /// continue to be periodically backed up as per the policy mapped at the application level.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name ="disableBackupDescription">Specifies the parameters to disable backup for any backup entity.</param>
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
        Task DisableServiceBackupAsync(
            string serviceId,
            DisableBackupDescription disableBackupDescription = default(DisableBackupDescription),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the Service Fabric service backup configuration information.
        /// </summary>
        /// <remarks>
        /// Gets the Service Fabric backup configuration information for the service and the partitions under this service.
        /// </remarks>
        /// <param name ="serviceId">The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
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
        Task<PagedData<BackupConfigurationInfo>> GetServiceBackupConfigurationInfoAsync(
            string serviceId,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the list of backups available for every partition in this service.
        /// </summary>
        /// <remarks>
        /// Returns a list of backups available for every partition in this Service Fabric service. The server enumerates all
        /// the backups available in the backup store configured in the backup policy. It also allows filtering of the result
        /// based on start and end datetime or just fetching the latest available backup for every partition.
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
        /// <param name ="latest">Specifies whether to get only the most recent backup available for a partition for the
        /// specified time range.</param>
        /// <param name ="startDateTimeFilter">Specify the start date time from which to enumerate backups, in datetime format.
        /// The date time must be specified in ISO8601 format. This is an optional parameter. If not specified, all backups
        /// from the beginning are enumerated.</param>
        /// <param name ="endDateTimeFilter">Specify the end date time till which to enumerate backups, in datetime format. The
        /// date time must be specified in ISO8601 format. This is an optional parameter. If not specified, enumeration is done
        /// till the most recent backup.</param>
        /// <param name ="continuationToken">The continuation token to obtain next set of results</param>
        /// <param name ="maxResults">The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.</param>
        /// <param name ="metadataFromBlob">Specifies whether to fetch backup metadata from the backup blob in the
        /// response.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<PagedData<BackupInfo>> GetServiceBackupListAsync(
            string serviceId,
            long? serverTimeout = 60,
            bool? latest = false,
            DateTime? startDateTimeFilter = default(DateTime?),
            DateTime? endDateTimeFilter = default(DateTime?),
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            bool? metadataFromBlob = false,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Suspends periodic backup for the specified Service Fabric service.
        /// </summary>
        /// <remarks>
        /// The service which is configured to take periodic backups, is suspended for taking further backups till it is
        /// resumed again. This operation applies to the entire service's hierarchy. It means all the partitions under this
        /// service are now suspended for backup.
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
        Task SuspendServiceBackupAsync(
            string serviceId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Resumes periodic backup of a Service Fabric service which was previously suspended.
        /// </summary>
        /// <remarks>
        /// The previously suspended Service Fabric service resumes taking periodic backup as per the backup policy currently
        /// configured for the same.
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
        Task ResumeServiceBackupAsync(
            string serviceId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Enables periodic backup of the stateful persisted partition.
        /// </summary>
        /// <remarks>
        /// Enables periodic backup of stateful persisted partition. Each partition is backed up as per the specified backup
        /// policy description. In case the application or service, which is partition is part of, is already enabled for
        /// backup then this operation would override the policy being used to take the periodic backup of this partition.
        /// Note only C# based Reliable Actor and Reliable Stateful services are currently supported for periodic backup.
        /// </remarks>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="enableBackupDescription">Specifies the parameters for enabling backup.</param>
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
        Task EnablePartitionBackupAsync(
            PartitionId partitionId,
            EnableBackupDescription enableBackupDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Disables periodic backup of Service Fabric partition which was previously enabled.
        /// </summary>
        /// <remarks>
        /// Disables periodic backup of partition which was previously enabled. Backup must be explicitly enabled before it can
        /// be disabled.
        /// In case the backup is enabled for the Service Fabric application or service, which this partition is part of, this
        /// partition would continue to be periodically backed up as per the policy mapped at the higher level entity.
        /// </remarks>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="disableBackupDescription">Specifies the parameters to disable backup for any backup entity.</param>
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
        Task DisablePartitionBackupAsync(
            PartitionId partitionId,
            DisableBackupDescription disableBackupDescription = default(DisableBackupDescription),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the partition backup configuration information
        /// </summary>
        /// <remarks>
        /// Gets the Service Fabric Backup configuration information for the specified partition.
        /// </remarks>
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
        Task<PartitionBackupConfigurationInfo> GetPartitionBackupConfigurationInfoAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the list of backups available for the specified partition.
        /// </summary>
        /// <remarks>
        /// Returns a list of backups available for the specified partition. The server enumerates all the backups available in
        /// the backup store configured in the backup policy. It also allows filtering of the result based on start and end
        /// datetime or just fetching the latest available backup for the partition.
        /// </remarks>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="latest">Specifies whether to get only the most recent backup available for a partition for the
        /// specified time range.</param>
        /// <param name ="startDateTimeFilter">Specify the start date time from which to enumerate backups, in datetime format.
        /// The date time must be specified in ISO8601 format. This is an optional parameter. If not specified, all backups
        /// from the beginning are enumerated.</param>
        /// <param name ="endDateTimeFilter">Specify the end date time till which to enumerate backups, in datetime format. The
        /// date time must be specified in ISO8601 format. This is an optional parameter. If not specified, enumeration is done
        /// till the most recent backup.</param>
        /// <param name ="metadataFromBlob">Specifies whether to fetch backup metadata from the backup blob in the
        /// response.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<PagedData<BackupInfo>> GetPartitionBackupListAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            bool? latest = false,
            DateTime? startDateTimeFilter = default(DateTime?),
            DateTime? endDateTimeFilter = default(DateTime?),
            bool? metadataFromBlob = false,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Suspends periodic backup for the specified partition.
        /// </summary>
        /// <remarks>
        /// The partition which is configured to take periodic backups, is suspended for taking further backups till it is
        /// resumed again.
        /// </remarks>
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
        Task SuspendPartitionBackupAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Resumes periodic backup of partition which was previously suspended.
        /// </summary>
        /// <remarks>
        /// The previously suspended partition resumes taking periodic backup as per the backup policy currently configured for
        /// the same.
        /// </remarks>
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
        Task ResumePartitionBackupAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Triggers backup of the partition's state.
        /// </summary>
        /// <remarks>
        /// Creates a backup of the stateful persisted partition's state. In case the partition is already being periodically
        /// backed up, then by default the new backup is created at the same backup storage. One can also override the same by
        /// specifying the backup storage details as part of the request body. Once the backup is initiated, its progress can
        /// be tracked using the GetBackupProgress operation.
        /// In case, the operation times out, specify a greater backup timeout value in the query parameter.
        /// </remarks>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="backupPartitionDescription">Describes the parameters to backup the partition now. If not present,
        /// backup operation uses default parameters from the backup policy current associated with this partition.</param>
        /// <param name ="backupTimeout">Specifies the maximum amount of time, in minutes, to wait for the backup operation to
        /// complete. Post that, the operation completes with timeout error. However, in certain corner cases it could be that
        /// though the operation returns back timeout, the backup actually goes through. In case of timeout error, its
        /// recommended to invoke this operation again with a greater timeout value. The default value for the same is 10
        /// minutes.</param>
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
        Task BackupPartitionAsync(
            PartitionId partitionId,
            BackupPartitionDescription backupPartitionDescription = default(BackupPartitionDescription),
            int? backupTimeout = 10,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets details for the latest backup triggered for this partition.
        /// </summary>
        /// <remarks>
        /// Returns information about the state of the latest backup along with details or failure reason in case of
        /// completion.
        /// </remarks>
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
        Task<BackupProgressInfo> GetPartitionBackupProgressAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Triggers restore of the state of the partition using the specified restore partition description.
        /// </summary>
        /// <remarks>
        /// Restores the state of a of the stateful persisted partition using the specified backup point. In case the partition
        /// is already being periodically backed up, then by default the backup point is looked for in the storage specified in
        /// backup policy. One can also override the same by specifying the backup storage details as part of the restore
        /// partition description in body. Once the restore is initiated, its progress can be tracked using the
        /// GetRestoreProgress operation.
        /// In case, the operation times out, specify a greater restore timeout value in the query parameter.
        /// </remarks>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="restorePartitionDescription">Describes the parameters to restore the partition.</param>
        /// <param name ="latest">Specifies whether BackupRestore Service would automatically determine the latest backup
        /// available and Restore using that. Set to false by default, but user can pass True and BackupRestore service will
        /// automatically fetch the latest backup and Restore the partition using that.</param>
        /// <param name ="restoreTimeout">Specifies the maximum amount of time to wait, in minutes, for the restore operation
        /// to complete. Post that, the operation returns back with timeout error. However, in certain corner cases it could be
        /// that the restore operation goes through even though it completes with timeout. In case of timeout error, its
        /// recommended to invoke this operation again with a greater timeout value. the default value for the same is 10
        /// minutes.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="metadataFromBlob">Specifies whether to fetch backup metadata from the backup blob in the
        /// response.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task RestorePartitionAsync(
            PartitionId partitionId,
            RestorePartitionDescription restorePartitionDescription = default(RestorePartitionDescription),
            bool? latest = false,
            int? restoreTimeout = 10,
            long? serverTimeout = 60,
            bool? metadataFromBlob = false,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Triggers Skipping of Restore of  partition if partition is stuck on trying to Restore and user wants to get out of
        /// OnDataLossAsync and continue with the replica data instead of restoring.
        /// </summary>
        /// <remarks>
        /// SkipRestore API initiates Skipping of ongoing Restore attempt by Backup Restore service on a partition, if
        /// AutoRestoreOnDataLoss is enabled on the partition's Backup Policy. This helps in skipping LSN check and Restoring
        /// directly from the replica instead of Backup.
        /// </remarks>
        /// <param name ="partitionId">The identity of the partition.</param>
        /// <param name ="restoreTimeout">Specifies the maximum amount of time to wait, in minutes, for the Skip restore
        /// operation to complete. Post that, the operation returns back with timeout error. However, in certain corner cases
        /// it could be that the restore operation goes through even though it completes with timeout. In case of timeout
        /// error, its recommended to invoke this operation again with a greater timeout value. the default value for the same
        /// is 10 minutes.</param>
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
        Task SkipRestorePartitionAsync(
            PartitionId partitionId,
            int? restoreTimeout = 10,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets details for the latest restore operation triggered for this partition.
        /// </summary>
        /// <remarks>
        /// Returns information about the state of the latest restore operation along with details or failure reason in case of
        /// completion.
        /// </remarks>
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
        Task<RestoreProgressInfo> GetPartitionRestoreProgressAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the list of backups available for the specified backed up entity at the specified backup location.
        /// </summary>
        /// <remarks>
        /// Gets the list of backups available for the specified backed up entity (Application, Service or Partition) at the
        /// specified backup location (FileShare or Azure Blob Storage).
        /// </remarks>
        /// <param name ="getBackupByStorageQueryDescription">Describes the filters and backup storage details to be used for
        /// enumerating backups.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
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
        Task<PagedData<BackupInfo>> GetBackupsFromBackupLocationAsync(
            GetBackupByStorageQueryDescription getBackupByStorageQueryDescription,
            long? serverTimeout = 60,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
