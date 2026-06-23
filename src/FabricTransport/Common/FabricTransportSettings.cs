// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Common;
using System.Fabric.Interop;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.ServiceFabric.FabricTransport
{
    /// <summary>
    /// Represents the settings that configure FabricTransport communication.
    /// </summary>
    internal class FabricTransportSettings
    {
        internal const string DefaultSectionName = "TransportSettings";
        private const uint DefaultQueueSize = 10000;
        //Got these values from experimenting 
        private static readonly int DefaultConcurrentCalls = 0;
        private const string TraceType = "FabricTransportSettings";
        private const string RemoteSecurityPrincipalName = "RemoteSecurityPrincipalName";
        private const string CertificateFindType = "CertificateFindType";
        private const string CertificateFindValue = "CertificateFindValue";
        private const string CertificateStoreLocation = "CertificateStoreLocation";
        private const string CertificateStoreName = "CertificateStoreName";
        private const string CertificateRemoteCommonNames = "CertificateRemoteCommonNames";
        private const string CertificateRemoteThumbprints = "CertificateRemoteThumbprints";
        private const string CertificateIssuerThumbprints = "CertificateIssuerThumbprints";
        private const string CertificateFindValuebySecondary = "CertificateFindValuebySecondary";
        private const string CertificateProtectionLevel = "CertificateProtectionLevel";
        private const string CertificateApplicationIssuerStorePrefix = "CertificateApplicationIssuerStore/";
        private const string SecurityCredentialsType = "SecurityCredentialsType ";
        private const string MaxQueueSizeSettingName = "MaxQueueSize";
        private const string MaxMessageSizeSettingName = "MaxMessageSize";
        private const string MaxConcurrentCallsSettingName = "MaxConcurrentCalls";
        private const string OperationTimeoutInSecondsSettingName = "OperationTimeoutInSeconds";
        private const string KeepAliveTimeoutInSecondsSettingName = "KeepAliveTimeoutInSeconds";
        private const string ConnectTimeoutInMillisecondsSettingName = "ConnectTimeoutInMilliseconds";
        internal static readonly uint DefaultMaxReceivedMessageSize = 4*1024*1024;
        internal static readonly TimeSpan DefaultConnectTimeout = TimeSpan.FromSeconds(5);
        internal static readonly TimeSpan DefaultOperationTimeout = TimeSpan.FromMinutes(5);
        internal static readonly TimeSpan DefaultKeepAliveTimeout = TimeSpan.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportSettings"/> class.
        /// </summary>
        public FabricTransportSettings()
        {
            this.SecurityCredentials = new NoneSecurityCredentials();
            this.OperationTimeout = DefaultOperationTimeout;
            this.KeepAliveTimeout = DefaultKeepAliveTimeout;
            this.MaxMessageSize = DefaultMaxReceivedMessageSize;
            this.MaxQueueSize = DefaultQueueSize;
            this.MaxConcurrentCalls = DefaultConcurrentCalls;
            this.ConnectTimeout = DefaultConnectTimeout;
        }

        /// <summary>
        /// Gets or sets the operation timeout that governs the whole process of sending a message, including receiving a reply message for a request/reply service operation.
        /// </summary>
        /// <value>The default is 5 minutes.</value>
        /// <remarks>This timeout also applies when sending reply messages from a callback contract method.</remarks>
        public TimeSpan OperationTimeout { get; set; }

        /// <summary>
        /// Gets or sets the keep-alive timeout that configures the TCP keep-alive option.
        /// </summary>
        /// <value>The default is <see cref="TimeSpan.Zero"/>, which disables the TCP keep-alive option.</value>
        /// <remarks>When using a load balancer, you may need to configure this to prevent the load balancer from closing the connection after a period of inactivity.</remarks>
        public TimeSpan KeepAliveTimeout { get; set; }

        /// <summary>
        /// Gets or sets the connect timeout that specifies the maximum time allowed for the connection to be established successfully.
        /// </summary>
        /// <value>The default is 5 seconds.</value>
        public TimeSpan ConnectTimeout { get; set; }

        /// <summary>
        /// Gets or sets the maximum size for a message that can be received on a channel configured with this setting.
        /// </summary>
        /// <value>The default is 4,194,304 bytes.</value>
        public long MaxMessageSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of a queue that stores messages while they are processed for an endpoint configured with this setting.
        /// </summary>
        /// <value>The default is 10,000 messages.</value>
        public long MaxQueueSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of messages actively serviced at one time.
        /// </summary>
        /// <value>The default is 0, which processes all messages simultaneously.</value>
        public long MaxConcurrentCalls { get; set; }

        /// <summary>
        /// Gets or sets the security credentials for securing the communication.
        /// </summary>
        /// <value>The default is <see cref="NoneSecurityCredentials"/>.</value>
        /// <remarks>
        /// The credentials can be of type <see cref="X509Credentials"/>, <see cref="WindowsCredentials"/>, or, to disable security, <see cref="NoneSecurityCredentials"/> (the default).
        /// </remarks>
        public SecurityCredentials SecurityCredentials { get; set; }

        internal FabricServiceConfigSection ConfigSection { get; private set; }

        /// <summary>
        /// Returns the <see cref="FabricTransportSettings"/> loaded from the section named <paramref name="sectionName"/> specified in the configuration file.
        /// </summary>
        /// <param name="sectionName">The name of the section within the configuration file. If <see langword="null"/>, the default <c>TransportSettings</c> section is used.</param>
        /// <param name="filepath">The full path of the file where the settings will be loaded from.
        /// If not specified, it will first try to load from the default configuration package <c>Config</c>, and if not found, from the <c>ClientExeName.Settings.xml</c> settings file in the client executable directory.</param>
        /// <param name="configPackageName">The name of the configuration package. If it's <see langword="null"/> or empty, it will check for the file in <paramref name="filepath"/>.</param>
        /// <remarks>
        /// The configuration file can be specified using <paramref name="filepath"/> or the name of the configuration package specified in the service manifest.
        /// It first tries to load the configuration using <paramref name="configPackageName"/>. If <paramref name="configPackageName"/> is not specified, it then tries to load from <paramref name="filepath"/>.
        /// <para>
        /// The following are the parameter names that should be provided in the configuration file, to be recognizable by Service Fabric to load the transport settings.
        /// <list type="number">
        ///     <item><c>MaxQueueSize</c> - <see cref="MaxQueueSize"/> as a <see langword="long"/> value.</item>
        ///     <item><c>MaxMessageSize</c> - <see cref="MaxMessageSize"/> value in bytes.</item>
        ///     <item><c>MaxConcurrentCalls</c> - <see cref="MaxConcurrentCalls"/> as a <see langword="long"/> value.</item>
        ///     <item><c>SecurityCredentialsType</c> - One of <c>None</c>, <c>X509</c>, or <c>Windows</c> that selects the <see cref="SecurityCredentials"/> type.
        ///         <c>Windows</c> credentials additionally read <c>RemoteSecurityPrincipalName</c>. <c>X509</c> credentials additionally read <c>CertificateFindType</c>,
        ///         <c>CertificateFindValue</c>, <c>CertificateProtectionLevel</c>, <c>CertificateStoreLocation</c>, <c>CertificateStoreName</c>, <c>CertificateRemoteCommonNames</c>,
        ///         <c>CertificateRemoteThumbprints</c>, <c>CertificateIssuerThumbprints</c>, <c>CertificateFindValuebySecondary</c>, and <c>CertificateApplicationIssuerStore/</c> entries.</item>
        ///     <item><c>OperationTimeoutInSeconds</c> - <see cref="OperationTimeout"/> value in seconds.</item>
        ///     <item><c>KeepAliveTimeoutInSeconds</c> - <see cref="KeepAliveTimeout"/> value in seconds.</item>
        ///     <item><c>ConnectTimeoutInMilliseconds</c> - <see cref="ConnectTimeout"/> value in milliseconds.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// <paramref name="configPackageName"/> is specified but the configuration package is not found,
        /// <paramref name="filepath"/> is specified but the configuration file is not found,
        /// the section named <paramref name="sectionName"/> is not found in the configuration, or
        /// a configuration value cannot be parsed into its target enumeration type.
        /// </exception>
        /// <exception cref="FormatException">
        /// A numeric configuration value is not in a format recognized by its target type.
        /// </exception>
        /// <exception cref="OverflowException">
        /// A numeric configuration value is outside the range of its target type.
        /// </exception>
        public static FabricTransportSettings LoadFrom(
            string sectionName,
            string filepath = null,
            string configPackageName = null)
        {
            bool isInitialized;
            var settings = new FabricTransportSettings();

            if (!string.IsNullOrEmpty(configPackageName))
            {
                isInitialized = settings.InitializeConfigFileFromConfigPackage(configPackageName);

                if (!isInitialized)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            SR.ErrorConfigPackageNotFound,
                            configPackageName));
                }
            }
            else if (!string.IsNullOrEmpty(filepath))
            {
                isInitialized = settings.InitializeConfigFile(filepath);

                if (!isInitialized)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            SR.ErrorConfigFileNotFound,
                            filepath));
                }
            }

            isInitialized = settings.InitializeSettingsFromConfig(sectionName);
            if (!isInitialized)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ErrorSectionNameNotFound,
                        sectionName));
            }

            AppTrace.TraceSource.WriteInfo(
                TraceType,
                "MaxMessageSize: {0} , MaxConcurrentCalls: {1} , MaxQueueSize: {2} , OperationTimeoutInSeconds: {3} KeepAliveTimeoutInSeconds : {4} , SecurityCredentials {5} , ConnectTimeoutInMilliseconds {6}",
                settings.MaxMessageSize,
                settings.MaxConcurrentCalls,
                settings.MaxQueueSize,
                settings.OperationTimeout.TotalSeconds,
                settings.KeepAliveTimeout.TotalSeconds,
                settings.SecurityCredentials.CredentialType,
                settings.ConnectTimeout.TotalMilliseconds);

            return settings;
        }

        /// <summary>
        /// Tries to load the <see cref="FabricTransportSettings"/> from the section named <paramref name="sectionName"/> specified in the configuration file into <paramref name="settings"/> and returns
        /// <see langword="true"/> if it was successfully loaded; otherwise returns <see langword="false"/>.
        /// </summary>
        /// <param name="sectionName">The name of the section within the configuration file. If <see langword="null"/>, the default <c>TransportSettings</c> section is used. Returns <see langword="false"/> if the section is not found.</param>
        /// <param name="settings">When this method returns, contains the <see cref="FabricTransportSettings"/> loaded from configuration if the load succeeded, or <see langword="null"/> if it failed. This parameter is treated as uninitialized.</param>
        /// <param name="filepath">The full path of the file where the settings will be loaded from.
        /// If not specified, it will first try to load from the default configuration package <c>Config</c>, and if not found, from the <c>ClientExeName.Settings.xml</c> settings file in the client executable directory.</param>
        /// <param name="configPackageName">The name of the configuration package. If it's <see langword="null"/> or empty, it will check for the file in <paramref name="filepath"/>.</param>
        /// <returns><see langword="true"/> if the settings were loaded successfully from configuration; otherwise, <see langword="false"/>.</returns>
        /// <inheritdoc path="/remarks" cref="LoadFrom(string, string, string)"/>
        public static bool TryLoadFrom(string sectionName, out FabricTransportSettings settings, string filepath = null,
            string configPackageName = null)
        {
            try
            {
                bool isInitialized;
                var fabricTransportSettings = new FabricTransportSettings();

                if (!string.IsNullOrEmpty(configPackageName))
                {
                    isInitialized = fabricTransportSettings.InitializeConfigFileFromConfigPackage(configPackageName);

                    if (!isInitialized)
                    {
                        settings = null;
                        return false;
                    }
                }
                else if (!string.IsNullOrEmpty(filepath))
                {
                    isInitialized = fabricTransportSettings.InitializeConfigFile(filepath);

                    if (!isInitialized)
                    {
                        settings = null;
                        return false;
                    }
                }

                isInitialized = fabricTransportSettings.InitializeSettingsFromConfig(sectionName);
                if (!isInitialized)
                {
                    settings = null;
                    return false;
                }

                settings = fabricTransportSettings;

                AppTrace.TraceSource.WriteInfo(
                    TraceType,
                    "MaxMessageSize: {0} , MaxConcurrentCalls: {1} , MaxQueueSize: {2} , OperationTimeoutInSeconds: {3} KeepAliveTimeoutInSeconds : {4} , SecurityCredentials {5} , ConnectTimeoutInMilliseconds {6}",
                    settings.MaxMessageSize,
                    settings.MaxConcurrentCalls,
                    settings.MaxQueueSize,
                    settings.OperationTimeout.TotalSeconds,
                    settings.KeepAliveTimeout.TotalSeconds,
                    settings.SecurityCredentials.CredentialType,
                    settings.ConnectTimeout.TotalMilliseconds);


                return true;
            }
            catch (Exception ex)
            {
                // return false if load from fails
                AppTrace.TraceSource.WriteWarning(TraceType, "Exception thrown while loading from Config {0}", ex);
                settings = null;
                return false;
            }
        }

        internal static FabricTransportSettings GetDefault(string sectionName = DefaultSectionName)
        {
            FabricTransportSettings settings = null;
            if (!TryLoadFrom(sectionName, out settings))
            {
                settings = new FabricTransportSettings();
                AppTrace.TraceSource.WriteInfo(
                    TraceType,
                    "Loading Default Settings , MaxMessageSize: {0} , MaxConcurrentCalls: {1} , MaxQueueSize: {2} , OperationTimeoutInSeconds: {3} KeepAliveTimeoutInSeconds : {4} , SecurityCredentials {5}",
                    settings.MaxMessageSize,
                    settings.MaxConcurrentCalls,
                    settings.MaxQueueSize,
                    settings.OperationTimeout.TotalSeconds,
                    settings.KeepAliveTimeout.TotalSeconds,
                    settings.SecurityCredentials.CredentialType);
            }
            return settings;
        }

        internal bool InitializeConfigFileFromConfigPackage(string configPackageName)
        {
            return FabricServiceConfig.InitializeFromConfigPackage(configPackageName);
        }

        internal bool InitializeConfigFile(string filePath)
        {
            return FabricServiceConfig.Initialize(filePath);
        }

        internal bool InitializeSettingsFromConfig(string sectionName)
        {
            this.ConfigSection = new FabricServiceConfigSection((sectionName ?? DefaultSectionName), this.OnInitialize);
            return this.ConfigSection.Initialize();
        }

        internal virtual void OnInitialize()
        {
            this.MaxConcurrentCalls = this.ConfigSection.GetSetting<long>(MaxConcurrentCallsSettingName,
                DefaultConcurrentCalls);
            this.MaxMessageSize = this.ConfigSection.GetSetting<long>(MaxMessageSizeSettingName,
                DefaultMaxReceivedMessageSize);
            this.MaxQueueSize = this.ConfigSection.GetSetting<long>(MaxQueueSizeSettingName, DefaultQueueSize);
            var operationTimeoutInSeconds = this.ConfigSection.GetSetting<double>(OperationTimeoutInSecondsSettingName,
                0);
            if (operationTimeoutInSeconds == 0)
            {
                this.OperationTimeout = DefaultOperationTimeout;
            }
            else
            {
                this.OperationTimeout = TimeSpan.FromSeconds(operationTimeoutInSeconds);
            }
            var keepAliveTimeoutInSeconds = this.ConfigSection.GetSetting<double>(KeepAliveTimeoutInSecondsSettingName,
                0);

            if (keepAliveTimeoutInSeconds == 0)
            {
                this.KeepAliveTimeout = DefaultKeepAliveTimeout;
            }
            else
            {
                this.KeepAliveTimeout = TimeSpan.FromSeconds(keepAliveTimeoutInSeconds);
            }

            var ConnectTimeoutInMilliSeconds =
                this.ConfigSection.GetSetting<double>(ConnectTimeoutInMillisecondsSettingName, 0);

            if (ConnectTimeoutInMilliSeconds == 0)
            {
                this.ConnectTimeout = DefaultConnectTimeout;
            }
            else
            {
                this.ConnectTimeout = TimeSpan.FromMilliseconds(ConnectTimeoutInMilliSeconds);
            }

            this.SecurityCredentials = this.LoadSecurityCredential();
        }


        internal IntPtr ToNative(PinCollection pin)
        {
            var nativeObj = new NativeTypes.FABRIC_SERVICE_TRANSPORT_SETTINGS();
            nativeObj.Reserved = IntPtr.Zero;

            if (this.SecurityCredentials != null)
            {
                nativeObj.SecurityCredentials = this.SecurityCredentials.ToNative(pin);
            }
            else
            {
                nativeObj.SecurityCredentials = IntPtr.Zero;
            }

            if (this.OperationTimeout.TotalSeconds < 0)
            {
                nativeObj.OperationTimeoutInSeconds = 0;
            }
            else
            {
                nativeObj.OperationTimeoutInSeconds = (uint) this.OperationTimeout.TotalSeconds;
            }

            if (this.KeepAliveTimeout.TotalSeconds < 0)
            {
                nativeObj.KeepAliveTimeoutInSeconds = 0;
            }
            else
            {
                nativeObj.KeepAliveTimeoutInSeconds = (uint) this.KeepAliveTimeout.TotalSeconds;
            }

            Helper.ThrowIfValueOutOfBounds(this.MaxMessageSize, MaxMessageSizeSettingName);

            nativeObj.MaxMessageSize = (uint) this.MaxMessageSize;

            Helper.ThrowIfValueOutOfBounds(this.MaxConcurrentCalls, MaxConcurrentCallsSettingName);
            nativeObj.MaxConcurrentCalls = (uint) this.MaxConcurrentCalls;

            Helper.ThrowIfValueOutOfBounds(this.MaxQueueSize, MaxQueueSizeSettingName);

            nativeObj.MaxQueueSize = (uint) this.MaxQueueSize;

            var ex1settings = new NativeTypes.FABRIC_SERVICE_TRANSPORT_SETTINGS_EX1();

            if (this.ConnectTimeout.TotalMilliseconds < 0)
            {
                ex1settings.ConnectTimeoutInMilliseconds = (uint) DefaultConnectTimeout.TotalMilliseconds;
            }
            else
            {
                ex1settings.ConnectTimeoutInMilliseconds = (uint) this.ConnectTimeout.TotalMilliseconds;
            }

            var ex2settings = new NativeTypes.FABRIC_SERVICE_TRANSPORT_SETTINGS_EX2();
            ex2settings.EnableMaxConcurrentCalls = NativeTypes.ToBOOLEAN(this.MaxConcurrentCalls > 0);

            ex1settings.Reserved = pin.AddBlittable(ex2settings);

            nativeObj.Reserved = pin.AddBlittable(ex1settings);

            return pin.AddBlittable(nativeObj);
        }

        private SecurityCredentials LoadSecurityCredential()
        {
            var credentialType = this.ConfigSection.GetSetting<CredentialType>(SecurityCredentialsType,
                CredentialType.None);
            switch (credentialType)
            {
                case CredentialType.X509:
                    return this.X509SecurityCredentialsBuilder();
                case CredentialType.Windows:
                    return this.WindowsSecurityCredentialsBuilder();
            }

            return new NoneSecurityCredentials();
        }

        private SecurityCredentials WindowsSecurityCredentialsBuilder()
        {
            var windowsCredentials = new WindowsCredentials();
            windowsCredentials.RemoteSpn = this.ConfigSection.GetSetting<string>(RemoteSecurityPrincipalName, null);
            return windowsCredentials;
        }

        private SecurityCredentials X509SecurityCredentialsBuilder()
        {
            var x509SecurityCredential = new X509Credentials();
            x509SecurityCredential.FindType = this.ConfigSection.GetSetting<X509FindType>(CertificateFindType,
                x509SecurityCredential.FindType);
            x509SecurityCredential.ProtectionLevel = this.ConfigSection.GetSetting<ProtectionLevel>(
                CertificateProtectionLevel,
                x509SecurityCredential.ProtectionLevel);
            x509SecurityCredential.FindValue = this.ConfigSection.GetSetting<object>(CertificateFindValue,
                x509SecurityCredential.FindValue);

            x509SecurityCredential.StoreLocation =
                this.ConfigSection.GetSetting<StoreLocation>(CertificateStoreLocation,
                    x509SecurityCredential.StoreLocation);

            x509SecurityCredential.StoreName = this.ConfigSection.GetSetting<string>(CertificateStoreName,
                x509SecurityCredential.StoreName);
            var remoteCommonNames = this.ConfigSection.GetSettingsList<string>(CertificateRemoteCommonNames);

            foreach (var name in remoteCommonNames)
            {
                x509SecurityCredential.RemoteCommonNames.Add(name);
            }

            var remoteCertThumbPrints = this.ConfigSection.GetSettingsList<string>(CertificateRemoteThumbprints);
            foreach (var name in remoteCertThumbPrints)
            {
                x509SecurityCredential.RemoteCertThumbprints.Add(name);
            }

            var issuerCertThumbPrints = this.ConfigSection.GetSettingsList<string>(CertificateIssuerThumbprints);
            foreach (var name in issuerCertThumbPrints)
            {
                x509SecurityCredential.IssuerThumbprints.Add(name);
            }

            x509SecurityCredential.FindValueSecondary =
                this.ConfigSection.GetSetting<object>(CertificateFindValuebySecondary,
                    x509SecurityCredential.FindValueSecondary);

            var issuerCertStores = this.ConfigSection.GetSettingsMapFromPrefix(CertificateApplicationIssuerStorePrefix);
            var remoteCertIssuers = new List<X509IssuerStore>();
            foreach (var issuerCertStore in issuerCertStores)
            {
                var issuerStoreLocations = issuerCertStore.Value.Split(',').ToList();
                remoteCertIssuers.Add(new X509IssuerStore(issuerCertStore.Key, issuerStoreLocations));
            }
            if (remoteCertIssuers.Count !=0)
            {
                x509SecurityCredential.RemoteCertIssuers = remoteCertIssuers;
            }

            return x509SecurityCredential;
        }
    }
}
