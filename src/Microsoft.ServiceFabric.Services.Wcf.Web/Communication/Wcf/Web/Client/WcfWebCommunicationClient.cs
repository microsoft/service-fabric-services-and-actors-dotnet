using System.Fabric;
using System.ServiceModel;

using Microsoft.ServiceFabric.Services.Communication.Client;

namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Web.Client
{

    /// <summary>
    /// A WCF client created by <see cref="WcfWebCommunicationClientFactory{TServiceContract}"/> to communicate 
    /// with a Service Fabric service using <see cref="Runtime.WcfWebCommunicationListener{TServiceContract}"/>.
    /// </summary>
    /// <typeparam name="TServiceContract">WCF service contract</typeparam>
    public class WcfWebCommunicationClient<TServiceContract> : ICommunicationClient
        where TServiceContract : class
    {

        private readonly TServiceContract channel;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="channel"></param>
        internal WcfWebCommunicationClient(TServiceContract channel)
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
        ~WcfWebCommunicationClient()
        {
            this.ClientChannel.Abort();
        }

    }

}
