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
    /// Interface containing methods for performing ApplicationTypeClient operations.
    /// </summary>
    public partial interface IApplicationTypeClient
    {
        /// <summary>
        /// Gets the list of application types in the Service Fabric cluster.
        /// </summary>
        /// <remarks>
        /// Returns the information about the application types that are provisioned or in the process of being provisioned in
        /// the Service Fabric cluster. Each version of an application type is returned as one application type. The response
        /// includes the name, version, status, and other details about the application type. This is a paged query, meaning
        /// that if not all of the application types fit in a page, one page of results is returned as well as a continuation
        /// token, which can be used to get the next page. For example, if there are 10 application types but a page only fits
        /// the first three application types, or if max results is set to 3, then three is returned. To access the rest of the
        /// results, retrieve subsequent pages by using the returned continuation token in the next query. An empty
        /// continuation token is returned if there are no subsequent pages.
        /// </remarks>
        /// <param name ="applicationTypeDefinitionKindFilter">Used to filter on ApplicationTypeDefinitionKind which is the
        /// mechanism used to define a Service Fabric application type.
        /// - Default - Default value, which performs the same function as selecting "All". The value is 0.
        /// - All - Filter that matches input with any ApplicationTypeDefinitionKind value. The value is 65535.
        /// - ServiceFabricApplicationPackage - Filter that matches input with ApplicationTypeDefinitionKind value
        /// ServiceFabricApplicationPackage. The value is 1.
        /// - Compose - Filter that matches input with ApplicationTypeDefinitionKind value Compose. The value is 2.
        /// </param>
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
        Task<PagedData<ApplicationTypeInfo>> GetApplicationTypeInfoListAsync(
            int? applicationTypeDefinitionKindFilter = 0,
            bool? excludeApplicationParameters = false,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the list of application types in the Service Fabric cluster matching exactly the specified name.
        /// </summary>
        /// <remarks>
        /// Returns the information about the application types that are provisioned or in the process of being provisioned in
        /// the Service Fabric cluster. These results are of application types whose name match exactly the one specified as
        /// the parameter, and which comply with the given query parameters. All versions of the application type matching the
        /// application type name are returned, with each version returned as one application type. The response includes the
        /// name, version, status, and other details about the application type. This is a paged query, meaning that if not all
        /// of the application types fit in a page, one page of results is returned as well as a continuation token, which can
        /// be used to get the next page. For example, if there are 10 application types but a page only fits the first three
        /// application types, or if max results is set to 3, then three is returned. To access the rest of the results,
        /// retrieve subsequent pages by using the returned continuation token in the next query. An empty continuation token
        /// is returned if there are no subsequent pages.
        /// </remarks>
        /// <param name ="applicationTypeName">The name of the application type.</param>
        /// <param name ="applicationTypeVersion">The version of the application type.</param>
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
        Task<PagedData<ApplicationTypeInfo>> GetApplicationTypeInfoListByNameAsync(
            string applicationTypeName,
            string applicationTypeVersion = default(string),
            bool? excludeApplicationParameters = false,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Provisions or registers a Service Fabric application type with the cluster using the '.sfpkg' package in the
        /// external store or using the application package in the image store.
        /// </summary>
        /// <remarks>
        /// Provisions a Service Fabric application type with the cluster. The provision is required before any new
        /// applications can be instantiated.
        /// The provision operation can be performed either on the application package specified by the
        /// relativePathInImageStore, or by using the URI of the external '.sfpkg'.
        /// </remarks>
        /// <param name ="provisionApplicationTypeDescription">The base type of provision application type description which
        /// supports either image store-based provision or external store-based provision.</param>
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
        Task ProvisionApplicationTypeAsync(
            ProvisionApplicationTypeDescriptionBase provisionApplicationTypeDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes or unregisters a Service Fabric application type from the cluster.
        /// </summary>
        /// <remarks>
        /// This operation can only be performed if all application instances of the application type have been deleted. Once
        /// the application type is unregistered, no new application instances can be created for this particular application
        /// type.
        /// </remarks>
        /// <param name ="applicationTypeName">The name of the application type.</param>
        /// <param name ="unprovisionApplicationTypeDescriptionInfo">The relative path for the application package in the image
        /// store specified during the prior copy operation.</param>
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
        Task UnprovisionApplicationTypeAsync(
            string applicationTypeName,
            UnprovisionApplicationTypeDescriptionInfo unprovisionApplicationTypeDescriptionInfo,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the Arm Metadata for a specific Application Type.
        /// </summary>
        /// <remarks>
        /// Updates the Arm Metadata for a specific Application Type. Is able to be called immediately after the provision app
        /// type API is called.
        /// </remarks>
        /// <param name ="applicationTypeName">The name of the application type.</param>
        /// <param name ="applicationTypeVersion">The version of the application type.</param>
        /// <param name ="applicationTypeArmMetadataUpdateDescription">The Arm metadata to be associated with a specific
        /// application type</param>
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
        Task UpdateApplicationTypeArmMetadataAsync(
            string applicationTypeName,
            string applicationTypeVersion,
            ApplicationTypeArmMetadataUpdateDescription applicationTypeArmMetadataUpdateDescription,
            long? serverTimeout = 60,
            bool? force = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the manifest describing an application type.
        /// </summary>
        /// <remarks>
        /// The response contains the application manifest XML as a string.
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
        Task<ApplicationTypeManifest> GetApplicationManifestAsync(
            string applicationTypeName,
            string applicationTypeVersion,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
