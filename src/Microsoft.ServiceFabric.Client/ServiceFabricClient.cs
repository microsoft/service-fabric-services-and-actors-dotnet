// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client.Resources;
    using Microsoft.ServiceFabric.Common.Exceptions;
    using Microsoft.ServiceFabric.Common.Security;

    /// <summary>
    /// Represents the base class for the Service Fabric client.
    /// </summary>
    public abstract class ServiceFabricClient : IServiceFabricClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricClient"/> class.
        /// </summary>
        /// <param name="clusterEndpoints">Uris for Service Cluster management endpoint.</param>
        /// <param name="securitySettings">A delegate to get ClaimsSecuritySettings.</param>
        /// <param name="clientSettings">Client settings for connecting to cluster. Default value is null which means connecting to unsecured cluster.</param>
        protected ServiceFabricClient(
            IReadOnlyList<Uri> clusterEndpoints,
            Func<CancellationToken, Task<SecuritySettings>> securitySettings = null,
            ClientSettings clientSettings = null)
        {
            if (clusterEndpoints == null)
            {
                throw new ArgumentNullException(nameof(clusterEndpoints));
            }

            if (clusterEndpoints.Count == 0)
            {
                throw new ArgumentException(SR.ErrorClusterEndpointNotProvided, nameof(clusterEndpoints));
            }

            if (clusterEndpoints.Any(url => url == null))
            {
                throw new ArgumentException(SR.ErrorUrlCannotBeNull, nameof(clusterEndpoints));
            }

            this.ClusterEndpoints = clusterEndpoints;

            // setup security settings
            this.SecuritySettingsFunc = securitySettings;
            this.ClientSettings = clientSettings;
        }

        /// <inheritdoc/>
        public IApplicationClient Applications { get; protected set; }

        /// <inheritdoc/>
        public IApplicationTypeClient ApplicationTypes { get; protected set; }

        /// <inheritdoc/>
        public IBackupRestoreClient BackupRestore { get; protected set; }

        /// <inheritdoc/>
        public IChaosClient ChaosClient { get; protected set; }

        /// <inheritdoc/>
        public IClusterClient Cluster { get; protected set; }

        /// <inheritdoc/>
        public ICodePackageClient CodePackages { get; protected set; }

        /// <inheritdoc/>
        public IComposeDeploymentClient ComposeDeployments { get; protected set; }

        /// <inheritdoc/>
        public IFaultsClient Faults { get; protected set; }

        /// <inheritdoc/>
        public IImageStoreClient ImageStore { get; protected set; }

        /// <inheritdoc/>
        public IInfrastructureClient Infrastructure { get; protected set; }

        /// <inheritdoc/>
        public INodeClient Nodes { get; protected set; }

        /// <inheritdoc/>
        public IPartitionClient Partitions { get; protected set; }

        /// <inheritdoc/>
        public IPropertyManagementClient Properties { get; protected set; }

        /// <inheritdoc/>
        public IReplicaClient Replicas { get; protected set; }

        /// <inheritdoc/>
        public IRepairManagementClient Repairs { get; protected set; }

        /// <inheritdoc/>
        public IServiceClient Services { get; protected set; }

        /// <inheritdoc/>
        public IServicePackageClient ServicePackages { get; protected set; }

        /// <inheritdoc/>
        public IServiceTypeClient ServiceTypes { get; protected set; }

        /// <inheritdoc/>
        public IEventsStoreClient EventsStore { get; protected set; }

        /// <inheritdoc/>
        public IMeshApplicationsClient MeshApplications { get; protected set; }

        /// <inheritdoc/>
        public IMeshVolumesClient MeshVolumes { get; protected set; }

        /// <inheritdoc/>
        public IMeshSecretsClient MeshSecrets { get; protected set; }

        /// <inheritdoc/>
        public IMeshSecretValuesClient MeshSecretValues { get; protected set; }

        /// <inheritdoc/>
        public IMeshNetworksClient MeshNetworks { get; protected set; }

        /// <inheritdoc/>
        public IMeshGatewaysClient MeshGateways { get; protected set; }

        /// <inheritdoc/>
        public IMeshServicesClient MeshServices { get; protected set; }

        /// <inheritdoc/>
        public IMeshCodePackagesClient MeshCodePackages { get; protected set; }

        /// <inheritdoc/>
        public IMeshServiceReplicasClient MeshServiceReplicas { get; protected set; }

        /// <summary>
        /// Gets the client settings for connecting to cluster.
        /// </summary>
        /// <value><see cref="ClientSettings"/> for connecting to cluster.</value>
        protected ClientSettings ClientSettings { get; }

        /// <summary>
        /// Gets the delegate to create security settings for connecting to cluster.
        /// </summary>
        /// <value><see cref="SecuritySettings"/> for connecting to cluster.</value>
        protected Func<CancellationToken, Task<SecuritySettings>> SecuritySettingsFunc { get; }

        /// <summary>
        /// Gets the Uri for Service Cluster management endpoint.
        /// </summary>
        /// <value>Cluster endpoint <see cref="System.Uri"/> .</value>
        protected IReadOnlyList<Uri> ClusterEndpoints { get; }

        /// <summary>
        /// Sends an HTTP get request to cluster http gateway.
        /// </summary>
        /// <param name="requestFunc">Func to create HttpRequest to send.</param>
        /// <param name="relativeUri">Relative request Uri.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The payload of the GET response.</returns>
        /// <exception cref="ServiceFabricException">When the response is not a success.</exception>
        public abstract Task<HttpResponseMessage> SendAsync(
            Func<HttpRequestMessage> requestFunc,
            string relativeUri,
            CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract void Dispose();
    }
}
