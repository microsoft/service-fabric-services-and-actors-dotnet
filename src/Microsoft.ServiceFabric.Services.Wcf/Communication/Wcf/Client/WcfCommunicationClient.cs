// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Client
{
    using System.Fabric;
    using System.ServiceModel;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// A WCF client created by <see cref="WcfCommunicationClientFactory{TServiceContract}"/> to communicate 
    /// with a Service Fabric service using <see cref="Runtime.WcfCommunicationListener{TServiceContract}"/>.
    /// </summary>
    /// <typeparam name="TServiceContract">WCF service contract</typeparam>
    public class WcfCommunicationClient<TServiceContract> : ICommunicationClient
        where TServiceContract : class
    {
        private readonly TServiceContract channel;

        internal WcfCommunicationClient(TServiceContract channel)
        {
            this.channel = channel;
        }

        /// <summary>
        /// Gets or sets the resolved service partition which contains information about the partition
        /// and the endpoints that can be used to communication with the service replica or instance.
        /// </summary>
        /// <value>The resolved service partition.</value>
        public ResolvedServicePartition ResolvedServicePartition { get; set; }

        /// <summary>
        /// Gets or sets the name of the listener in the service replica or instance to which the client is
        /// connected to.
        /// </summary>
        /// <value>The name of the listener in the service replica or instance to which the client is
        /// connected to.</value>
        public string ListenerName { get; set; }

        /// <summary>
        /// Gets or sets the service endpoint to which the client is connected to.
        /// </summary>
        /// <value>The service endpoint to which the client is connected to.</value>
        public ResolvedServiceEndpoint Endpoint { get; set; }

        /// <summary>
        /// Gets the WCF channel for the specified contract that this communication client uses.
        /// </summary>
        /// <value>The WCF channel for the specified contract that this communication client uses.</value>
        public TServiceContract Channel
        {
            get { return this.channel; }
        }

        internal IClientChannel ClientChannel
        {
            get { return (IClientChannel)this.Channel; }
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~WcfCommunicationClient()
        {
            this.ClientChannel.Abort();
        }
    }
}
