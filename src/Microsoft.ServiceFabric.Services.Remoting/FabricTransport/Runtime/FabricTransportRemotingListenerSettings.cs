// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime
{
    using System;
    using System.Fabric;
    using System.Fabric.Common;
    using Microsoft.ServiceFabric.FabricTransport;
    using Microsoft.ServiceFabric.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Constants = Microsoft.ServiceFabric.Services.Remoting.V2.Constants;

    /// <summary>
    /// Settings that configures the  FabricTransport Listener.
    /// </summary>
    public class FabricTransportRemotingListenerSettings
    {
        private static readonly string Tracetype = "FabricTransportRemotingListenerSettings";
        private static readonly int DefaultRemotingExceptionDepth = 2;
        private readonly FabricTransportListenerSettings listenerSettings;
        private int headerBufferSize;
        private int headerMaxBufferCount;
        private bool useWrappedMessage;
        private int remotingExceptionDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportRemotingListenerSettings"/> class with default values.
        /// </summary>
        public FabricTransportRemotingListenerSettings()
        {
            this.listenerSettings = new FabricTransportListenerSettings();
            this.headerBufferSize = Constants.DefaultHeaderBufferSize;
            this.headerMaxBufferCount = Constants.DefaultHeaderMaxBufferCount;
            this.useWrappedMessage = false;
            this.remotingExceptionDepth = DefaultRemotingExceptionDepth;
            this.ExceptionSerializationTechnique = ExceptionSerialization.BinaryFormatter;
        }

        private FabricTransportRemotingListenerSettings(FabricTransportListenerSettings listenerSettings)
            : this()
        {
            this.listenerSettings = listenerSettings;
        }

        /// <summary>
        /// Exception serialization option to use(applicable only to V2 Remoting).
        /// </summary>
        public enum ExceptionSerialization
        {
            /// <summary>
            /// Uses DCS to serialize exception details in service remoting message.
            /// </summary>
            Default,

            /// <summary>
            /// Uses binary formatter to serialize exception details in service remoting message.
            /// To be used in compat scenarios. This option will be deprecated in future.
            /// </summary>
            BinaryFormatter,
        }

        /// <summary>
        /// Gets or sets the exception serialization technique.
        /// </summary>
        /// <remarks>Applies only to V2 Remoting.</remarks>
        public ExceptionSerialization ExceptionSerializationTechnique { get; set; }

        /// <summary>
        /// Gets or sets the name of the endpoint resource specified in ServiceManifest.
        /// This is used to obtain the port number on which to service will listen.
        /// </summary>
        /// <value>
        /// EndpointResourceName is  name of the  endpoint resource defined in the service manifest.
        /// </value>
        /// <remarks>
        /// Default value of EndpointResourceName  is "ServiceEndpoint" </remarks>
        public string EndpointResourceName
        {
            get { return this.listenerSettings.EndpointResourceName; }
            set { this.listenerSettings.EndpointResourceName = value; }
        }

        /// <summary>
        /// Gets or sets operation Timeout  which governs the whole process of sending a message, including receiving a reply message for a request/reply service operation.
        ///  This timeout also applies when sending reply messages from a callback contract method.
        /// </summary>
        /// <value>OperationTimeout as <see cref="System.TimeSpan"/></value>
        /// <remarks>Default Value for Operation Timeout is set as 5 mins</remarks>
        public TimeSpan OperationTimeout
        {
            get { return this.listenerSettings.OperationTimeout; }
            set { this.listenerSettings.OperationTimeout = value; }
        }

        /// <summary>
        /// Gets or sets keepAliveTimeout which provides a way to configure  Tcp keep-alive option.
        /// </summary>
        /// <value>KeepAliveTimeout as <see cref="System.TimeSpan"/></value>
        /// <remarks>Default Value for KeepAliveTimeout Timeout is set as TimeSpan.Zero. which indicates we disable the tcp keepalive option.
        /// If you are using loadbalancer , you may need to configure this in order to avoid  the loadbalancer to close the connection after certain time </remarks>
        public TimeSpan KeepAliveTimeout
        {
            get { return this.listenerSettings.KeepAliveTimeout; }
            set { this.listenerSettings.KeepAliveTimeout = value; }
        }

        /// <summary>
        /// Gets or sets Max MessageSize  for a message that can be received on a channel configured with this setting.
        /// </summary>
        /// <value>Maximum size of the message in bytes.
        /// </value>
        /// <remarks>
        /// Default Value for MaxMessageSize used is 4194304 bytes
        /// </remarks>
        public long MaxMessageSize
        {
            get { return this.listenerSettings.MaxMessageSize; }
            set { this.listenerSettings.MaxMessageSize = value; }
        }

        /// <summary>
        /// Gets or sets the maximum size, of a queue that stores messages while they are processed for an endpoint configured with this setting.
        /// </summary>
        /// <value> Max Size for a Queue that receives messages from the channel
        /// </value>
        /// <remarks>
        /// Default value is 10,000 messages</remarks>
        public long MaxQueueSize
        {
            get { return this.listenerSettings.MaxQueueSize; }
            set { this.listenerSettings.MaxQueueSize = value; }
        }

        /// <summary>
        /// Gets or sets the maxConcurrentCalls which represents maximum number of messages actively service processes at one time.
        /// </summary>
        /// <value>
        /// MaxConcurrentCalls is  the upper limit of active messages in the service.
        /// </value>
        /// <remarks>
        /// Default value for the MaxConcurrentCalls is 0 which indicates that the setting is not enabled. This implies that all the messages are active and are processed simultaneously.
        /// </remarks>
        public long MaxConcurrentCalls
        {
            get { return this.listenerSettings.MaxConcurrentCalls; }
            set { this.listenerSettings.MaxConcurrentCalls = value; }
        }

        /// <summary>
        ///  Gets or sets headerBufferSize which represents size of each header buffer in the bufferPool .
        /// Default Remoting Serialization is using BufferPooling to avoid allocation every time.
        /// So If you are adding any header in <see cref="ServiceRemotingRequestMessageHeader"/> , which can increase the headerSize to be
        /// larger than BufferSize, it is recomended then to change this value to higher value .If bufferSize is less than serialized header bytes,
        /// we copy the header to larger buffer.
        /// </summary>
        /// <remarks>
        ///    Defaults  value for the HeaderBufferSize is 1024 bytes.
        /// </remarks>
        public int HeaderBufferSize
        {
            get { return this.headerBufferSize; }
            set { this.headerBufferSize = value; }
        }

        /// <summary>
        ///  Gets or sets headerMaxBufferCount which represents the maximum number of header buffers assigned  to the BufferPool.
        /// </summary>
        /// <remarks>
        ///    Defaults  value for the HeaderMaxBufferCount is 1000.
        /// </remarks>
        public int HeaderMaxBufferCount
        {
            get { return this.headerMaxBufferCount; }
            set { this.headerMaxBufferCount = value; }
        }

        /// <summary>
        /// Gets or sets security credentials for securing the communication
        /// </summary>
        /// <value>SecurityCredentials as  <see cref=" System.Fabric.SecurityCredentials"/>
        /// </value>
        /// <remarks>
        /// Default Value for SecurityCredentials is None
        /// SecurityCredential can be of type x509SecurityCredentail <seealso cref="System.Fabric.X509Credentials"/>or
        /// WindowsCredentials <seealso cref="System.Fabric.WindowsCredentials"/> </remarks>
        public SecurityCredentials SecurityCredentials
        {
            get { return this.listenerSettings.SecurityCredentials; }
            set { this.listenerSettings.SecurityCredentials = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the remoting method parameters should be wrapped or not before sending it over the wire. When UseWrappedMessage is set to false, parameters  will not be wrapped. When this value is set to true, the parameters will be wrapped.Default value is false.
        /// </summary>
        public bool UseWrappedMessage
        {
            get { return this.useWrappedMessage; }
            set { this.useWrappedMessage = value; }
        }

        /// <summary>
        /// Gets or sets the depth of exceptions to be sent to the client incase of remoting call failing with exception.
        /// </summary>
        /// <remarks>The allowed values are greater than or equal to 1. If the supplied value is less than 1, then the field is set to int.MaxValue.</remarks>
        public int RemotingExceptionDepth
        {
            get
            {
                return this.remotingExceptionDepth;
            }

            set
            {
                if (value <= 0)
                {
                    this.remotingExceptionDepth = int.MaxValue;
                }
                else
                {
                    this.remotingExceptionDepth = value;
                }
            }
        }

        internal static object DefaultEndpointResourceName
        {
            get { return FabricTransportListenerSettings.DefaultEndpointResourceName; }
        }

        /// <summary>
        /// Loads the FabricTransport settings from a section specified in the service settings configuration file - settings.xml
        /// </summary>
        /// <param name="sectionName">Name of the section within the configuration file. if not found , it throws ArgumentException.</param>
        /// <param name="configPackageName"> Name of the configuration package. if not found Settings.xml in the configuration package path, it throws ArgumentException.
        /// If not specified, default name is "Config"</param>
        /// <returns>FabricTransportRemotingListenerSettings</returns>
        /// <remarks>
        /// The following are the parameter names that should be provided in the configuration file,to be recognizable by service fabric to load the transport settings.
        ///
        ///     1. MaxQueueSize - <see cref="FabricTransportRemotingSettings.MaxQueueSize"/>value in long.
        ///     2. MaxMessageSize - <see cref="FabricTransportRemotingSettings.MaxMessageSize"/>value in bytes.
        ///     3. MaxConcurrentCalls - <see cref="FabricTransportRemotingSettings.MaxConcurrentCalls"/>value in long.
        ///     4. SecurityCredentials - <see cref="FabricTransportRemotingSettings.SecurityCredentials"/> value.
        ///     5. OperationTimeoutInSeconds - <see cref="FabricTransportRemotingSettings.OperationTimeout"/> value in seconds.
        ///     6. KeepAliveTimeoutInSeconds - <see cref="FabricTransportRemotingSettings.KeepAliveTimeout"/> value in seconds.
        /// </remarks>
        public static FabricTransportRemotingListenerSettings LoadFrom(
            string sectionName,
            string configPackageName = null)
        {
            var listenerSettings = FabricTransportListenerSettings.LoadFrom(sectionName, configPackageName);
            return new FabricTransportRemotingListenerSettings(listenerSettings);
        }

        /// <summary>
        /// Try to load the FabricTransport settings from a section specified in the service settings configuration file - settings.xml
        /// </summary>
        /// <param name="sectionName">Name of the section within the configuration file. if not found , it return false</param>
        /// <param name="remotingListenerSettings">When this method returns it sets the <see cref="FabricTransportRemotingListenerSettings"/> listenersettings if load from Config succeeded. If fails ,its sets listenerSettings to null/> </param>
        /// <param name="configPackageName"> Name of the configuration package. if not found Settings.xml in the configuration package path, it return false.
        /// If not specified, default name is "Config"</param>
        /// <returns> <see cref="bool"/> specifies whether the settings get loaded successfully from Config.
        /// It returns true when load from Config succeeded, else return false.</returns>
        /// <remarks>
        /// The following are the parameter names that should be provided in the configuration file,to be recognizable by service fabric to load the transport settings.
        ///
        ///     1. MaxQueueSize - <see cref="FabricTransportRemotingSettings.MaxQueueSize"/>value in long.
        ///     2. MaxMessageSize - <see cref="FabricTransportRemotingSettings.MaxMessageSize"/>value in bytes.
        ///     3. MaxConcurrentCalls - <see cref="FabricTransportRemotingSettings.MaxConcurrentCalls"/>value in long.
        ///     4. SecurityCredentials - <see cref="FabricTransportRemotingSettings.SecurityCredentials"/> value.
        ///     5. OperationTimeoutInSeconds - <see cref="FabricTransportRemotingSettings.OperationTimeout"/> value in seconds.
        ///     6. KeepAliveTimeoutInSeconds - <see cref="FabricTransportRemotingSettings.KeepAliveTimeout"/> value in seconds.
        /// </remarks>
        public static bool TryLoadFrom(
            string sectionName,
            out FabricTransportRemotingListenerSettings remotingListenerSettings,
            string configPackageName = null)
        {
            var isSucceded =
                FabricTransportListenerSettings.TryLoadFrom(sectionName, out var listenerSettings, configPackageName);
            if (isSucceded)
            {
                remotingListenerSettings = new FabricTransportRemotingListenerSettings(listenerSettings);
                return true;
            }

            remotingListenerSettings = null;
            return false;
        }

        internal static FabricTransportRemotingListenerSettings GetDefault(
            string sectionName = FabricTransportSettings.DefaultSectionName)
        {
            var listenerinternalSettings = FabricTransportListenerSettings.GetDefault(sectionName);

            var settings = new FabricTransportRemotingListenerSettings(listenerinternalSettings);

            AppTrace.TraceSource.WriteInfo(
                Tracetype,
                "MaxMessageSize: {0} , MaxConcurrentCalls: {1} , MaxQueueSize: {2} , OperationTimeoutInSeconds: {3} KeepAliveTimeoutInSeconds : {4} , SecurityCredentials {5} , HeaderBufferSize {6}," + "HeaderBufferCount {7} , ExceptionSerializationTechinique {8} , RemotingExceptionDepth {9}",
                settings.MaxMessageSize,
                settings.MaxConcurrentCalls,
                settings.MaxQueueSize,
                settings.OperationTimeout.TotalSeconds,
                settings.KeepAliveTimeout.TotalSeconds,
                settings.SecurityCredentials.CredentialType,
                settings.HeaderBufferSize,
                settings.HeaderMaxBufferCount,
                settings.ExceptionSerializationTechnique.ToString(),
                settings.RemotingExceptionDepth);

            return settings;
        }

        internal FabricTransportListenerSettings GetInternalSettings()
        {
            return this.listenerSettings;
        }
    }
}
