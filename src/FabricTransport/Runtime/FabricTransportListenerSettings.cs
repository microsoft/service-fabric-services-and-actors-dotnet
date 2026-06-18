// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using System.Fabric.Common;
using System.Globalization;

namespace Microsoft.ServiceFabric.FabricTransport.Runtime
{
    /// <summary>
    /// Configures the FabricTransport listener.
    /// </summary>
    internal class FabricTransportListenerSettings : FabricTransportSettings
    {
        /// <summary>
        /// Specifies the default name of the endpoint resource, used when <see cref="EndpointResourceName"/> is not specified.
        /// </summary>
        internal const string DefaultEndpointResourceName = "ServiceEndpoint";
        private static readonly string Tracetype = "FabricTransportListenerSettings";
        private static readonly string DefaultPackageName = "Config";

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportListenerSettings"/> class.
        /// </summary>
        public FabricTransportListenerSettings()
        {
            this.EndpointResourceName = DefaultEndpointResourceName;
        }

        /// <summary>
        /// Gets or sets the name of the endpoint resource specified in the service manifest, used to obtain the port on which the service listens.
        /// </summary>
        /// <value>The default is <c>ServiceEndpoint</c>.</value>
        public string EndpointResourceName { get; set; }

        /// <summary>
        /// Returns the <see cref="FabricTransportListenerSettings"/> loaded from the section named <paramref name="sectionName"/> specified in the configuration package.
        /// </summary>
        /// <param name="sectionName">The name of the section within the configuration file.</param>
        /// <param name="configPackageName">The name of the configuration package. If not specified, the default name <c>Config</c> is used.</param>
        /// <remarks>
        /// The following are the parameter names that should be provided in the configuration file, to be recognizable by Service Fabric to load the transport settings.
        /// <list type="number">
        ///     <item><c>MaxQueueSize</c> - <see cref="FabricTransportSettings.MaxQueueSize"/> as a <see langword="long"/> value.</item>
        ///     <item><c>MaxMessageSize</c> - <see cref="FabricTransportSettings.MaxMessageSize"/> value in bytes.</item>
        ///     <item><c>MaxConcurrentCalls</c> - <see cref="FabricTransportSettings.MaxConcurrentCalls"/> as a <see langword="long"/> value.</item>
        ///     <item><c>SecurityCredentialsType</c> - One of <c>None</c>, <c>X509</c>, or <c>Windows</c> that selects the <see cref="SecurityCredentials"/> type.
        ///         <c>Windows</c> credentials additionally read <c>RemoteSecurityPrincipalName</c>. <c>X509</c> credentials additionally read <c>CertificateFindType</c>,
        ///         <c>CertificateFindValue</c>, <c>CertificateProtectionLevel</c>, <c>CertificateStoreLocation</c>, <c>CertificateStoreName</c>, <c>CertificateRemoteCommonNames</c>,
        ///         <c>CertificateRemoteThumbprints</c>, <c>CertificateIssuerThumbprints</c>, <c>CertificateFindValuebySecondary</c>, and <c>CertificateApplicationIssuerStore/</c> entries.</item>
        ///     <item><c>OperationTimeoutInSeconds</c> - <see cref="FabricTransportSettings.OperationTimeout"/> value in seconds.</item>
        ///     <item><c>KeepAliveTimeoutInSeconds</c> - <see cref="FabricTransportSettings.KeepAliveTimeout"/> value in seconds.</item>
        ///     <item><c>ConnectTimeoutInMilliseconds</c> - <see cref="FabricTransportSettings.ConnectTimeout"/> value in milliseconds.</item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The configuration package is not found, the section named <paramref name="sectionName"/> is not found in the configuration, or
        /// a configuration value cannot be parsed into its target enumeration type.
        /// </exception>
        /// <exception cref="FormatException">
        /// A numeric configuration value is not in a format recognized by its target type.
        /// </exception>
        /// <exception cref="OverflowException">
        /// A numeric configuration value is outside the range of its target type.
        /// </exception>
        public static FabricTransportListenerSettings LoadFrom(string sectionName, string configPackageName = null)
        {
            var settings = new FabricTransportListenerSettings();
            var packageName = configPackageName ?? DefaultPackageName;
            var isInitialized = settings.InitializeConfigFileFromConfigPackage(packageName);

            if (!isInitialized)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrorConfigPackageNotFound,
                    configPackageName));
            }

            isInitialized = settings.InitializeSettingsFromConfig(sectionName);

            if (!isInitialized)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrorSectionNameNotFound,
                    sectionName));
            }

            AppTrace.TraceSource.WriteInfo(
                Tracetype,
                "MaxMessageSize: {0} , MaxConcurrentCalls: {1} , MaxQueueSize: {2} , OperationTimeoutInSeconds: {3} KeepAliveTimeoutInSeconds : {4} , SecurityCredentials {5}",
                settings.MaxMessageSize,
                settings.MaxConcurrentCalls,
                settings.MaxQueueSize,
                settings.OperationTimeout.TotalSeconds,
                settings.KeepAliveTimeout.TotalSeconds,
                settings.SecurityCredentials.CredentialType);

            return settings;
        }


        /// <summary>
        /// Tries to load the <see cref="FabricTransportListenerSettings"/> from the section named <paramref name="sectionName"/> specified in the configuration package into <paramref name="listenerSettings"/> and returns
        /// <see langword="true"/> if it was successfully loaded; otherwise returns <see langword="false"/>.
        /// </summary>
        /// <param name="sectionName">The name of the section within the configuration file.</param>
        /// <param name="listenerSettings">When this method returns, contains the <see cref="FabricTransportListenerSettings"/> loaded from configuration if the load succeeded, or <see langword="null"/> if it failed. This parameter is treated as uninitialized.</param>
        /// <param name="configPackageName">The name of the configuration package. If not specified, the default name <c>Config</c> is used.</param>
        /// <inheritdoc path="/remarks" cref="LoadFrom(string, string)"/>
        public static bool TryLoadFrom(string sectionName, out FabricTransportListenerSettings listenerSettings,
            string configPackageName = null)
        {
            try
            {
                var settings = new FabricTransportListenerSettings();
                var packageName = configPackageName ?? DefaultPackageName;
                var isInitialized = settings.InitializeConfigFileFromConfigPackage(packageName);

                if (!isInitialized)
                {
                    listenerSettings = null;
                    return false;
                }

                isInitialized = settings.InitializeSettingsFromConfig(sectionName);

                if (!isInitialized)
                {
                    listenerSettings = null;
                    return false;
                }

                listenerSettings = settings;

                AppTrace.TraceSource.WriteWarning(
                    Tracetype,
                    "MaxMessageSize: {0} , MaxConcurrentCalls: {1} , MaxQueueSize: {2} , OperationTimeoutInSeconds: {3} KeepAliveTimeoutInSeconds : {4} , SecurityCredentials {5}",
                    settings.MaxMessageSize,
                    settings.MaxConcurrentCalls,
                    settings.MaxQueueSize,
                    settings.OperationTimeout.TotalSeconds,
                    settings.KeepAliveTimeout.TotalSeconds,
                    settings.SecurityCredentials.CredentialType);

                return true;
            }
            catch (Exception ex)
            {
                AppTrace.TraceSource.WriteInfo(Tracetype, "Exception thrown while loading from Config {0}", ex);
                listenerSettings = null;
                return false;
            }
        }

        /// <summary>
        /// Returns the <see cref="FabricTransportListenerSettings"/> loaded from the section named <paramref name="sectionName"/> specified in the configuration package, or the default settings if the section cannot be loaded.
        /// </summary>
        /// <param name="sectionName">The name of the section within the configuration file. If not specified, the default <c>TransportSettings</c> section is used.</param>
        internal new static FabricTransportListenerSettings GetDefault(string sectionName = DefaultSectionName)
        {
            FabricTransportListenerSettings listenerSettings = null;
            if (!TryLoadFrom(sectionName, out listenerSettings))
            {
                listenerSettings = new FabricTransportListenerSettings();

                AppTrace.TraceSource.WriteInfo(
                    Tracetype,
                    "Loading Default Settings , MaxMessageSize: {0} , MaxConcurrentCalls: {1} , MaxQueueSize: {2} , OperationTimeoutInSeconds: {3} KeepAliveTimeoutInSeconds : {4} , SecurityCredentials {5}",
                    listenerSettings.MaxMessageSize,
                    listenerSettings.MaxConcurrentCalls,
                    listenerSettings.MaxQueueSize,
                    listenerSettings.OperationTimeout.TotalSeconds,
                    listenerSettings.KeepAliveTimeout.TotalSeconds,
                    listenerSettings.SecurityCredentials.CredentialType);
            }
            return listenerSettings;
        }

        /// <summary>
        /// Returns the <see cref="FabricTransportListenerAddress"/> for the listener of the service replica or instance described by <paramref name="serviceContext"/>.
        /// </summary>
        /// <param name="serviceContext">The context of the service replica or instance that hosts the listener.</param>
        /// <returns>An address unique to the replica or instance, combining its listen address with the port of the configured endpoint resource.</returns>
        internal FabricTransportListenerAddress GetListenerAddress(ServiceContext serviceContext)
        {
            var replicaId = serviceContext.ReplicaOrInstanceId;
            var partitionId = serviceContext.PartitionId;
            var path = string.Format(CultureInfo.InvariantCulture, "{0}-{1}-{2}", partitionId, replicaId, Guid.NewGuid());
            var port = Helper.GetEndpointPort(serviceContext.CodePackageActivationContext, this.EndpointResourceName);
            return new FabricTransportListenerAddress(serviceContext.ListenAddress, port, path);
        }
    }
}
