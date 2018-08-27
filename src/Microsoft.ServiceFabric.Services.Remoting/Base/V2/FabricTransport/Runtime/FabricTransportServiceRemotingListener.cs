// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2.FabricTransport.Runtime
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport;
    using Microsoft.ServiceFabric.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Runtime;

    /// <summary>
    ///     An <see cref="IServiceRemotingListener"/> that uses
    ///     fabric TCP transport to provide interface remoting for stateless and stateful services.
    /// </summary>
    internal class FabricTransportServiceRemotingListener : IServiceRemotingListener
    {
        private readonly FabricTransportMessageHandler transportMessageHandler;
        private readonly string publishAddress;
        private readonly string listenAddress;
        private Microsoft.ServiceFabric.FabricTransport.V2.Runtime.FabricTransportListener fabricTransportlistener;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingListener"/> class.
        /// TODO - Fix the documenetation
        /// </summary>
        /// <param name="publishAddress">TODO</param>
        /// <param name="listenAddress">listen  address</param>
        /// <param name="partitionId">partitionId </param>
        /// <param name="replicaorInstanceId">replics Id</param>
        /// <param name="serviceRemotingMessageHandler">messge handler</param>
        /// <param name="listenerSettings">listener settings</param>
        /// <param name="address">listener address</param>
        /// <param name="serializersManager">serializersManager</param>
        public FabricTransportServiceRemotingListener(
            string publishAddress,
            string listenAddress,
            Guid partitionId,
            long replicaorInstanceId,
            IServiceRemotingMessageHandler serviceRemotingMessageHandler,
            FabricTransportListenerAddress address,
            FabricTransportSettings listenerSettings,
            ServiceRemotingMessageSerializationManager serializersManager)
        {
            this.publishAddress = publishAddress;
            this.listenAddress = listenAddress;
            this.transportMessageHandler = new FabricTransportMessageHandler(
                serviceRemotingMessageHandler,
                serializersManager,
                partitionId,
                replicaorInstanceId);

            this.fabricTransportlistener = new Microsoft.ServiceFabric.FabricTransport.V2.Runtime.FabricTransportListener(
                listenerSettings,
                address,
                this.transportMessageHandler,
                new FabricTransportRemotingConnectionHandler());
        }

        /// <summary>
        /// This method causes the communication listener to be opened. Once the Open
        /// completes, the communication listener becomes usable - accepts and sends messages.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result of the Task is
        /// the endpoint string.
        /// </returns>
        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            return Task.Run(
(Func<Task<string>>)(async () =>
                {
                    var listenUri = await this.fabricTransportlistener.OpenAsync(cancellationToken);
                    var publishUri = listenUri.Replace(this.listenAddress, (string)this.publishAddress);

                    System.Fabric.Common.AppTrace.TraceSource.WriteInfo(
                        "FabricTransportServiceRemotingListenerV2.OpenAsync",
                        "ListenURI = {0} PublishURI = {1}",
                        listenUri,
                        publishUri);

                    return publishUri;
                }), cancellationToken);
        }

        /// <summary>
        /// This method causes the communication listener to close. Close is a terminal state and
        /// this method allows the communication listener to transition to this state in a
        /// graceful manner.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            await this.fabricTransportlistener.CloseAsync(
                cancellationToken);
            this.Dispose();
        }

        /// <summary>
        /// This method causes the communication listener to close. Close is a terminal state and
        /// this method causes the transition to close ungracefully. Any outstanding operations
        /// (including close) should be canceled when this method is called.
        /// </summary>
        public void Abort()
        {
            this.fabricTransportlistener.Abort();
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.fabricTransportlistener != null)
            {
                this.fabricTransportlistener.Dispose();
                this.fabricTransportlistener = null;
            }

            this.transportMessageHandler.Dispose();
        }

        private int GetPublishPort(string endpointName)
        {
            string portEnv = Environment.GetEnvironmentVariable(endpointName);
            if (!string.IsNullOrEmpty(portEnv))
            {
                Console.WriteLine("{0} Port is ={1}", endpointName, portEnv);
                int port;

                if (int.TryParse(portEnv, out port))
                {
                    return port;
                }
            }

            // Otherwise use the same listening port
            return 0;
        }
    }
}
