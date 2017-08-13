// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.FabricTransport
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.FabricTransport;

    /// <summary>
    /// Represents a settings that configures the  FabricTransport communication.
    /// </summary>
    public class FabricTransportRemotingSettings
    {
        internal const string DefaultSectionName = "TransportSettings";
        private readonly FabricTransportSettings fabricTransportSettings;

        /// <summary>
        /// Creates a new FabricTransportRemotingSettings with default Values.
        /// </summary>
        public FabricTransportRemotingSettings()
        {
            this.fabricTransportSettings = new FabricTransportSettings();
        }

        internal FabricTransportRemotingSettings(FabricTransportSettings fabricTransportSettings)
        {
            this.fabricTransportSettings = fabricTransportSettings;
        }

        /// <summary>
        /// Gets or sets the operation Timeout  which governs the whole process of sending a message, including receiving a reply message for a request/reply service operation.
        ///  This timeout also applies when sending reply messages from a callback contract method.
        /// </summary>
        /// <value>The OperationTimeout as <see cref="System.TimeSpan"/>.</value>
        /// <remarks>Default Value for Operation Timeout is set as 5 mins.</remarks>
        public TimeSpan OperationTimeout
        {
            get { return this.fabricTransportSettings.OperationTimeout; }
            set { this.fabricTransportSettings.OperationTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the KeepAliveTimeout that provides a way to configure  Tcp keep-alive option.
        /// </summary>
        /// <value>The KeepAliveTimeout as <see cref="System.TimeSpan"/>.</value>
        /// <remarks>Default Value for KeepAliveTimeout Timeout is set as TimeSpan.Zero. which indicates we disable the tcp keepalive option.
        /// If you are using loadbalancer , you may need to configure this in order to avoid  the loadbalancer to close the connection after certain time. </remarks>
        public TimeSpan KeepAliveTimeout
        {
            get { return this.fabricTransportSettings.KeepAliveTimeout; }
            set { this.fabricTransportSettings.KeepAliveTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the maximum time allowed for the connection to be established successfully.
        /// </summary>
        /// <value>The ConnectTimeout as <see cref="System.TimeSpan"/>.</value>
        /// <remarks>Default Value for ConnectTimeout Timeout is set as 5 seconds.</remarks>
        public TimeSpan ConnectTimeout
        {
            get { return this.fabricTransportSettings.ConnectTimeout; }
            set { this.fabricTransportSettings.ConnectTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the maximum size for a message that can be received on a channel configured with this setting.
        /// </summary>
        /// <value>The maximum size of the message in bytes.
        /// </value>
        /// <remarks>
        /// Default Value for MaxMessageSize used is 4194304 bytes
        /// </remarks>
        public long MaxMessageSize
        {
            get { return this.fabricTransportSettings.MaxMessageSize; }
            set { this.fabricTransportSettings.MaxMessageSize = value; }
        }

        /// <summary>
        /// Gets or sets the maximum size, of a queue that stores messages while they are processed for an endpoint configured with this setting. 
        /// </summary>
        /// <value>The maximum size for a Queue that recieves messages from the channel. 
        /// </value>
        /// <remarks>
        /// Default value is 10,000 messages</remarks>
        public long MaxQueueSize
        {
            get { return this.fabricTransportSettings.MaxQueueSize; }
            set { this.fabricTransportSettings.MaxMessageSize = value; }
        }

        ///<summary>
        ///Gets or sets the maximum number of messages actively service processes at one time.
        /// </summary>
        /// <value>
        ///The MaxConcurrentCalls is  the upper limit of active messages in the service.
        /// </value>
        /// <remarks>
        ///    Default  value for the MaxConcurrentCalls is 16*Number of processors.
        /// </remarks>
        public long MaxConcurrentCalls
        {
            get { return this.fabricTransportSettings.MaxConcurrentCalls; }
            set { this.fabricTransportSettings.MaxConcurrentCalls = value; }
        }

        /// <summary>
        /// Gets or sets the security credentials for securing the communication. 
        /// </summary>
        /// <value>The security credentials as  <see cref=" System.Fabric.SecurityCredentials"/>.
        /// </value>
        /// <remarks>
        /// Default Value for SecurityCredentials is None.
        /// SecurityCredential can be of type x509SecurityCredentail <seealso cref="System.Fabric.X509Credentials"/>or
        ///  WindowsCredentials <seealso cref="System.Fabric.WindowsCredentials"/>.
        ///</remarks>
        public SecurityCredentials SecurityCredentials
        {
            get { return this.fabricTransportSettings.SecurityCredentials; }
            set { this.fabricTransportSettings.SecurityCredentials = value; }
        }


        internal FabricTransportSettings GetInternalSettings()
        {
            return this.fabricTransportSettings;
        }

        /// <summary>
        /// Loads the FabricTransport settings from a sectionName specified in the configuration file. 
        /// Configuration File can be specified using the filePath or using the name of the configuration package specified in the service manifest.
        /// It will first try to load config using configPackageName. If configPackageName is not specified then try to load from filePath.
        /// </summary>
        /// <param name="sectionName">The name of the section within the configuration file. If not found section in configuration file, it will throw ArgumentException.</param>
        /// <param name="filepath">The full path of the file where the settings will be loaded from. 
        ///  If not specified , it will first try to load from default Config Package"Config" , if not found then load from Settings "ClientExeName.Settings.xml" present in Client Exe directory. </param>
        ///  <param name="configPackageName"> Name of the configuration package.If its null or empty,it will check for file in filePath.</param>
        /// <returns>The FabricTransportRemotingSettings</returns>
        /// <remarks>
        /// The following are the parameter names that should be provided in the configuration file, to be recognizable by service fabric to load the transport settings.
        ///     
        ///     1. MaxQueueSize - <see cref="MaxQueueSize"/>value in long.
        ///     2. MaxMessageSize - <see cref="MaxMessageSize"/>value in bytes.
        ///     3. MaxConcurrentCalls - <see cref="MaxConcurrentCalls"/>value in long.
        ///     4. SecurityCredentials - <see cref="SecurityCredentials"/> value.
        ///     5. OperationTimeoutInSeconds - <see cref="OperationTimeout"/> value in seconds.
        ///     6. KeepAliveTimeoutInSeconds - <see cref="KeepAliveTimeout"/> value in seconds.
        ///     7. ConnectTimeoutInMilliseconds - <see cref="ConnectTimeout"/> value in milliseconds.
        /// </remarks>
        public static FabricTransportRemotingSettings LoadFrom(string sectionName, string filepath = null,
            string configPackageName = null)
        {
            var fabricTransportSettings = FabricTransportSettings.LoadFrom(sectionName, filepath, configPackageName);
            return new FabricTransportRemotingSettings(fabricTransportSettings);
        }

        /// <summary>
        /// Try to load the FabricTransport settings from a sectionName specified in the configuration file.
        /// Configuration File can be specified using the filePath or using the name of the configuration package specified in the service manifest.
        /// It will first try to load config using configPackageName. If configPackageName is not specified then try to load from filePath.
        /// </summary>
        /// <param name="sectionName">The name of the section within the configuration file. If not found section in configuration file, it return false.</param>
        /// <param name="filepath">The full path of the file where the settings will be loaded from. 
        ///  If not specified , it will first try to load from default Config Package"Config" , if not found then load from Settings "ClientExeName.Settings.xml" present in Client Exe directory. </param>
        ///  <param name="configPackageName">The name of the configuration package. If its null or empty, it will check for file in filePath.</param>
        /// <param name="settings">When this method returns it sets the <see cref="FabricTransportRemotingSettings"/> settings if load from Config succeeded. If fails, its sets settings to null. </param>
        /// <returns><see cref="bool"/> specifies whether the settings get loaded successfully from Config.
        /// It returns true when load from Config succeeded, else return false. </returns>
        /// <remarks>
        /// The following are the parameter names that should be provided in the configuration file,to be recognizable by service fabric to load the transport settings.
        ///     
        ///     1. MaxQueueSize - <see cref="MaxQueueSize"/>value in long.
        ///     2. MaxMessageSize - <see cref="MaxMessageSize"/>value in bytes.
        ///     3. MaxConcurrentCalls - <see cref="MaxConcurrentCalls"/>value in long.
        ///     4. SecurityCredentials - <see cref="SecurityCredentials"/> value.
        ///     5. OperationTimeoutInSeconds - <see cref="OperationTimeout"/> value in seconds.
        ///     6. KeepAliveTimeoutInSeconds - <see cref="KeepAliveTimeout"/> value in seconds.
        ///     7. ConnectTimeoutInMilliseconds - <see cref="ConnectTimeout"/> value in milliseconds.
        /// </remarks>
        public static bool TryLoadFrom(string sectionName, out FabricTransportRemotingSettings settings,
            string filepath = null,
            string configPackageName = null)
        {
            FabricTransportSettings transportSettings;
            var isSucceded = FabricTransportSettings.TryLoadFrom(sectionName, out transportSettings, filepath,
                configPackageName);
            if (isSucceded)
            {
                settings = new FabricTransportRemotingSettings(transportSettings);
                return true;
            }

            settings = null;
            return isSucceded;
        }

        /// <summary>
        /// Returns the default Settings. Loads the configuration file from default Config Package"Config", if not found then try to load from  default config file "ClientExeName.Settings.xml"  from Client Exe directory.
        ///</summary>
        /// <param name="sectionName">The name of the section within the configuration file. If not found section in configuration file, it will return the default Settings.</param>
        /// <returns></returns>
        internal static FabricTransportRemotingSettings GetDefault(string sectionName = DefaultSectionName)
        {
            FabricTransportSettings transportSettings;
            transportSettings = FabricTransportSettings.GetDefault(sectionName);
            return new FabricTransportRemotingSettings(transportSettings);
        }
    }
}
