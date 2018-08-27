// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Mesh
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.FabricTransport;

    /// <summary>
    /// Represents a settings that configures the  FabricTransport communication.
    /// </summary>
    public class FabricTransportRemotingMeshClientSettings
    {
        internal const string DefaultSectionName = "TransportSettings";

        private readonly FabricTransportSettings fabricTransportSettings;

        private int headerBufferSize;
        private int headerMaxBufferCount;
        private bool useWrappedMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportRemotingMeshClientSettings"/> class with default values.
        /// </summary>
        public FabricTransportRemotingMeshClientSettings()
        {
            this.fabricTransportSettings = new FabricTransportSettings();
            this.headerBufferSize = Base.V2.Constants.DefaultHeaderBufferSize;
            this.headerMaxBufferCount = Base.V2.Constants.DefaultHeaderMaxBufferCount;
            this.useWrappedMessage = false;
        }

        internal FabricTransportRemotingMeshClientSettings(FabricTransportSettings fabricTransportSettings)
            : this()
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
        /// <value>The maximum size for a Queue that receives messages from the channel.
        /// </value>
        /// <remarks>
        /// Default value is 10,000 messages</remarks>
        public long MaxQueueSize
        {
            get { return this.fabricTransportSettings.MaxQueueSize; }
            set { this.fabricTransportSettings.MaxQueueSize = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of messages actively service processes at one time.
        /// </summary>
        /// <remarks>
        /// The MaxConcurrentCalls is the upper limit of active messages in the service. The default value for the MaxConcurrentCalls is 16*Number of processors.
        /// </remarks>
        public long MaxConcurrentCalls
        {
            get { return this.fabricTransportSettings.MaxConcurrentCalls; }
            set { this.fabricTransportSettings.MaxConcurrentCalls = value; }
        }

        /// <summary>
        /// Gets or sets the size of the each header buffer.
        /// </summary>
        /// <remarks>
        /// The default value for the HeaderBufferSize is 1024 bytes.
        /// </remarks>
        public int HeaderBufferSize
        {
            get { return this.headerBufferSize; }
            set { this.headerBufferSize = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of header buffers assigned to the BufferPool.
        /// </summary>
        /// <remarks>
        /// The default value for the HeaderMaxBufferCount is 1000.
        /// </remarks>
        public int HeaderMaxBufferCount
        {
            get { return this.headerMaxBufferCount; }
            set { this.headerMaxBufferCount = value; }
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
        /// </remarks>
        public SecurityCredentials SecurityCredentials
        {
            get { return this.fabricTransportSettings.SecurityCredentials; }
            set { this.fabricTransportSettings.SecurityCredentials = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the remoting method parameters should be wrapped or not before sending it over the wire. When UseWrappedMessage is set to false, parameters  will not be wrapped. When this value is set to true, the parameters will be wrapped.Default value is false.
        /// </summary>
        public bool UseWrappedMessage
        {
            get { return this.useWrappedMessage; }
            set { this.useWrappedMessage = value; }
        }

        internal FabricTransportSettings GetInternalSettings()
        {
            return this.fabricTransportSettings;
        }
    }
}
