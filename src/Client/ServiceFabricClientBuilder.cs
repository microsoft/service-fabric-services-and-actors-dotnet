// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client.Resources;
    using Microsoft.ServiceFabric.Common.Security;

    /// <summary>
    /// A Builder for IServiceFabricClient
    /// </summary>
    public class ServiceFabricClientBuilder
    {
        private const string ServiceFabricHttpClientAssemblyName = "Microsoft.ServiceFabric.Client.Http";
        private const string ServiceFabricHttpClientTypeName = "Microsoft.ServiceFabric.Client.Http.ServiceFabricHttpClient,Microsoft.ServiceFabric.Client.Http";
        private bool securitySettingsAlreadySpecified;
        private SecurityType securityType = SecurityType.None;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricClientBuilder"/> class.
        /// </summary>
        public ServiceFabricClientBuilder()
        {
        }

        /// <summary>
        /// Gets delegate to create security settings for connecting to cluster.
        /// </summary>
        public Func<CancellationToken, Task<SecuritySettings>> SecuritySettings { get; private set; }

        /// <summary>
        /// Gets endpoints for the cluster.
        /// </summary>
        public IReadOnlyList<Uri> Endpoints { get; private set; }

        /// <summary>
        /// Gets the client settings for connecting to cluster.
        /// </summary>
        /// <value><see cref="ClientSettings"/> for connecting to cluster.</value>
        public ClientSettings ClientSettings { get; } = new ClientSettings();

        /// <summary>
        /// Gets a container to hold objects needed which are not common to all client implementaions but are needed to Build specific clients.
        /// Using this Container, allows this assembly from not needing the extra references required by client implementations.
        /// eg: ServiceFabricHttpClient may need extra instaces like HjttpClientHandler and DelegatingHandler.
        /// </summary>
        internal Dictionary<Type, object> Container { get; } = new Dictionary<Type, object>();

        /// <summary>
        /// Builds an <see cref="IServiceFabricClient"/> to perform operations against a Service Fabric cluster.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cancel the async operation.</param>
        /// <returns>IMplementation of <see cref="IServiceFabricClient"/>.</returns>
        public Task<IServiceFabricClient> BuildAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this.Endpoints == null || this.Endpoints.Count == 0)
            {
                throw new InvalidOperationException(SR.ErrorClusterEndpointNotProvided);
            }

            // Since this builder can be used to create other implementations of IServiceFabricClient, its generic and
            // should use reflection to create ServiceFabricHttpClient.
            // Make ServiceFabricHttpClient the default implementation.
            // If more client implementaions are added in future, Extension methods on the IServiceFabricClientBuilder (eg. UseTcp()) can be used to indicate the client type (a ClientType enum can be added here) which needs to be created.
            object[] args =
                {
                    this,
                    cancellationToken,
                };

            var type = Type.GetType(ServiceFabricHttpClientTypeName);
            var method = type.GetMethod("CreateAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (Task<IServiceFabricClient>)method.Invoke(null, args);
        }

        /// <summary>
        /// Configures <see cref="IServiceFabricClient"/> to connect to a Service Fabric cluster secured with Azure Active Directory. Use this method if you dont have the metadata to get Token from AAD.
        /// </summary>
        /// <param name="securitySettings">A delegate to get AzureActiveDirectorySecuritySettings.</param>
        /// <returns>Returns an IServiceFabricClientBuilder.</returns>
        public ServiceFabricClientBuilder UseAzureActiveDirectorySecurity(Func<CancellationToken, Task<SecuritySettings>> securitySettings)
        {
            if (this.securitySettingsAlreadySpecified && !this.securityType.Equals(SecurityType.Claims))
            {
                throw new InvalidOperationException($"Security has already been configured with {this.securityType}.");
            }

            this.securityType = SecurityType.Claims;
            this.securitySettingsAlreadySpecified = true;
            this.SecuritySettings = securitySettings;
            return this;
        }

        /// <summary>
        /// Configures <see cref="IServiceFabricClient"/> to connect to a Service Fabric cluster secured with token acquired from a STS (security token service).
        /// </summary>
        /// <param name="securitySettings">A delegate to get ClaimsSecuritySettings.</param>
        /// <returns>Returns an IServiceFabricClientBuilder.</returns>
        /// <remarks>
        /// Security Settings for connecting to a secured cluster are created by calling the delegate <paramref name="securitySettings"/>.
        /// The delegate will be used to refresh security settings if needed by implementaions of <see cref="IServiceFabricClient"/>.
        /// If client request fails because of Authentication, the delegate is invoked once again to get security settings, if the client call fails again because of Authentication after getting 
        /// security settings, the exception is thrown to the user. This allows applications to refresh Claims an X509 security credentials without restarting.
        /// </remarks>
        public ServiceFabricClientBuilder UseClaimsSecurity(Func<CancellationToken, Task<SecuritySettings>> securitySettings)
        {
            if (this.securitySettingsAlreadySpecified && !this.securityType.Equals(SecurityType.Claims))
            {
                throw new InvalidOperationException($"Security has already been configured with {this.securityType}.");
            }

            this.securityType = SecurityType.Claims;
            this.SecuritySettings = securitySettings;
            this.securitySettingsAlreadySpecified = true;
            return this;
        }

        /// <summary>
        /// Configures <see cref="IServiceFabricClient"/> to connect to a Service Fabric cluster secured with Windows credentials.
        /// </summary>
        /// <returns>Returns an IServiceFabricClientBuilder.</returns>
        public ServiceFabricClientBuilder UseWindowsSecurity()
        {
            if (this.securitySettingsAlreadySpecified && !this.securityType.Equals(SecurityType.Windows))
            {
                throw new InvalidOperationException($"Security has already been configured with {this.securityType}.");
            }

            this.securityType = SecurityType.Windows;
            this.SecuritySettings = (ct) => Task.FromResult<SecuritySettings>(new WindowsSecuritySettings());
            this.securitySettingsAlreadySpecified = true;
            return this;
        }

        /// <summary>
        /// Configures <see cref="IServiceFabricClient"/> to connect to a Service Fabric cluster secured with X509 certificates.
        /// </summary>
        /// <param name="securitySettings">A delegate to get X509SecuritySettings.</param>
        /// <returns>Returns an IServiceFabricClientBuilder.</returns>
        public ServiceFabricClientBuilder UseX509Security(Func<CancellationToken, Task<SecuritySettings>> securitySettings)
        {
            if (this.securitySettingsAlreadySpecified && !this.securityType.Equals(SecurityType.X509))
            {
                throw new InvalidOperationException($"Security has already been configured with {this.securityType}.");
            }

            this.securityType = SecurityType.X509;
            this.SecuritySettings = securitySettings;
            this.securitySettingsAlreadySpecified = true;
            return this;
        }

        /// <summary>
        /// Sets endpoints for the Service Fabric cluster to talk to.
        /// </summary>
        /// <param name="clusterEndpoints">Service Fabric cluster endpoints.</param>
        /// <returns>Returns an IServiceFabricClientBuilder.</returns>
        public ServiceFabricClientBuilder UseEndpoints(params Uri[] clusterEndpoints)
        {
            if (clusterEndpoints.Length == 0)
            {
                throw new ArgumentException(SR.ErrorClusterEndpointNotProvided, nameof(clusterEndpoints));
            }

            this.Endpoints = clusterEndpoints.ToList();
            return this;
        }

        /// <summary>
        /// Configures various timeout settings for the IServiceFabricClient.
        /// </summary>
        /// <param name="configureClientSettings">The delegate for configuring the <see cref="ClientSettings"/>.</param>
        /// <returns>The <see cref="ServiceFabricClientBuilder"/></returns>
        public ServiceFabricClientBuilder ConfigureClientSettings(Action<ClientSettings> configureClientSettings)
        {
            configureClientSettings.Invoke(this.ClientSettings);
            return this;
        }
    }
}
