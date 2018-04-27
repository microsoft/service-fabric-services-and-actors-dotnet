using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Web.Runtime;
using Microsoft.ServiceFabric.Services.Wcf;

namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Web.Client
{


    /// <summary>
    /// An <see cref="Microsoft.ServiceFabric.Services.Communication.Client.ICommunicationClientFactory{TCommunicationClient}"/> that uses
    /// Windows Communication Foundation to create <see cref="WcfWebCommunicationClient{TServiceContract}"/>
    /// to communicate with stateless and stateful services that are using 
    /// <see cref="WcfWebCommunicationListener{TServiceContract}"/>.
    /// </summary>
    /// <typeparam name="TServiceContract">WCF REST based service contract</typeparam>
    public class WcfWebCommunicationClientFactory<TServiceContract> :
        CommunicationClientFactoryBase<WcfWebCommunicationClient<TServiceContract>>
        where TServiceContract : class
    {

        private readonly Binding clientBinding;

        /// <summary>
        /// Constructs a factory to create clients using WCF REST to communicate with the services.
        /// </summary>
        /// <param name="clientBinding">
        ///     WCF binding to use for the client. If the client binding is not specified or null,
        ///     a default <see cref="WebHttpBinding"/> is created.
        /// </param>
        /// <param name="exceptionHandlers">
        ///     Exception handlers to handle the exceptions encountered in communicating with the service.
        /// </param>
        /// <param name="servicePartitionResolver">
        ///     Service partition resolver to resolve the service endpoints. If not specified, a default 
        ///     service partition resolver returned by <see cref="ServicePartitionResolver.GetDefault"/> is used.
        /// </param>
        /// <param name="traceId">
        ///     Id to use in diagnostics traces from this component.
        /// </param>
        public WcfWebCommunicationClientFactory(
            Binding clientBinding = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            IServicePartitionResolver servicePartitionResolver = null,
            string traceId = null)
            : base(servicePartitionResolver, GetExceptionHandlers(exceptionHandlers), traceId)
        {
            if (clientBinding == null)
                clientBinding = new WebHttpBinding();

            this.clientBinding = clientBinding;
        }

        /// <summary>
        /// Creates WCF communication clients to communicate over the given channel.
        /// </summary>
        /// <param name="channel">Service contract based WCF channel.</param>
        /// <returns>The communication client that was created</returns>
        protected virtual WcfWebCommunicationClient<TServiceContract> CreateWcfCommunicationClient(TServiceContract channel)
        {
            return new WcfWebCommunicationClient<TServiceContract>(channel);
        }

        /// <summary>
        /// Creates a communication client for the given endpoint address.
        /// </summary>
        /// <param name="endpoint">Endpoint address where the service is listening</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The communication client that was created</returns>
        protected override async Task<WcfWebCommunicationClient<TServiceContract>> CreateClientAsync(
            string endpoint,
            CancellationToken cancellationToken)
        {
            var endpointAddress = new EndpointAddress(endpoint);
            var channel = ChannelFactory<TServiceContract>.CreateChannel(this.clientBinding, endpointAddress);

            var clientChannel = ((IClientChannel)channel);
            Exception connectionTimeoutException = null;
            try
            {
                var openTask = Task.Factory.FromAsync(
                    clientChannel.BeginOpen(this.clientBinding.OpenTimeout, null, null),
                    clientChannel.EndOpen);
                if (await Task.WhenAny(openTask, Task.Delay(this.clientBinding.OpenTimeout, cancellationToken)) == openTask)
                {
                    if (openTask.Exception != null)
                    {
                        throw openTask.Exception;
                    }
                }
                else
                {
                    clientChannel.Abort();
                    throw new TimeoutException(string.Format(CultureInfo.CurrentCulture, SR.ErrorCommunicationClientOpenTimeout, this.clientBinding.OpenTimeout));
                }
            }
            catch (AggregateException ae)
            {
                ae.Handle(x => x is TimeoutException);
                connectionTimeoutException = ae;
            }
            catch (TimeoutException te)
            {
                connectionTimeoutException = te;
            }
            if (connectionTimeoutException != null)
            {
                throw new EndpointNotFoundException(
                    connectionTimeoutException.Message,
                    connectionTimeoutException);
            }

            clientChannel.OperationTimeout = this.clientBinding.ReceiveTimeout;
            return this.CreateWcfCommunicationClient(channel);
        }

        /// <summary>
        /// Returns true if the client is still valid. Connection oriented transports can use this method to indicate that the client is no longer
        /// connected to the service.
        /// </summary>
        /// <param name="client">WCF communication client</param>
        /// <returns>true if the client is valid, false otherwise</returns>
        protected override bool ValidateClient(WcfWebCommunicationClient<TServiceContract> client)
        {
            return (client.ClientChannel.State == CommunicationState.Opened);
        }

        /// <summary>
        /// Returns true if the client is still valid and connected to the endpoint specified in the parameter.
        /// </summary>
        /// <param name="endpoint">endpoint string</param>
        /// <param name="client">WCF communication client</param>
        /// <returns>true if the client is valid, false otherwise</returns>
        protected override bool ValidateClient(string endpoint, WcfWebCommunicationClient<TServiceContract> client)
        {
            var clientChannel = client.ClientChannel;
            return ((clientChannel.State == CommunicationState.Opened) &&
                    (clientChannel.RemoteAddress.Uri.Equals(new Uri(endpoint))));
        }

        /// <summary>
        /// Aborts the given client
        /// </summary>
        /// <param name="client">Communication client</param>
        protected override void AbortClient(WcfWebCommunicationClient<TServiceContract> client)
        {
            client.ClientChannel.Abort();
        }

        private static IEnumerable<IExceptionHandler> GetExceptionHandlers(
            IEnumerable<IExceptionHandler> exceptionHandlers)
        {
            var handlers = new List<IExceptionHandler>();
            if (exceptionHandlers != null)
            {
                handlers.AddRange(exceptionHandlers);
            }
            handlers.Add(new WcfExceptionHandler());
            return handlers;
        }

    }

}
