// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Mesh.FabricTransport.Runtime
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;

    /// <summary>
    /// Settings that configures the  FabricTransport Listener.
    /// </summary>
    public class FabricTransportRemotingMeshListenerSettings
    {
        private readonly FabricTransportListenerSettings listenerSettings;
        private int headerBufferSize;
        private int headerMaxBufferCount;
        private bool useWrappedMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportRemotingMeshListenerSettings"/> class with default values.
        /// </summary>
        public FabricTransportRemotingMeshListenerSettings()
        {
            this.listenerSettings = new FabricTransportListenerSettings();
            this.headerBufferSize = Constants.DefaultHeaderBufferSize;
            this.headerMaxBufferCount = Constants.DefaultHeaderMaxBufferCount;
            this.useWrappedMessage = false;
        }

        private FabricTransportRemotingMeshListenerSettings(FabricTransportListenerSettings listenerSettings)
            : this()
        {
            this.listenerSettings = listenerSettings;
        }

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
        ///    Defaults  value for the MaxConcurrentCalls is to the Number of processors.
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

        internal static object DefaultEndpointResourceName
        {
            get { return FabricTransportListenerSettings.DefaultEndpointResourceName; }
        }

        internal FabricTransportListenerSettings GetInternalSettings()
        {
            return this.listenerSettings;
        }
    }
}
