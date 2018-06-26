using System;
using System.Fabric;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;

namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Web.Runtime
{

    /// <summary>
    /// A Windows Communication Foundation based listener for Service Fabric based stateless or stateful service
    /// using the <see cref="WebServiceHost"/>.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the WCF service contract.</typeparam>
    public class WcfWebCommunicationListener<TServiceContract> :
        ICommunicationListener
    {

        readonly ServiceEndpoint endpoint;
        readonly WebServiceHost host;
        readonly string listenAddress;
        readonly string publishAddress;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="serviceContext"></param>
        /// <param name="wcfServiceObject"></param>
        public WcfWebCommunicationListener(
            ServiceContext serviceContext,
            TServiceContract wcfServiceObject) :
            this(serviceContext, wcfServiceObject, null, null, null)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="serviceContext"></param>
        /// <param name="wcfServiceObject"></param>
        /// <param name="listenerBinding"></param>
        /// <param name="endpointResourceName"></param>
        public WcfWebCommunicationListener(
            ServiceContext serviceContext,
            TServiceContract wcfServiceObject,
            Binding listenerBinding = null,
            string endpointResourceName = null) :
            this(serviceContext, wcfServiceObject, listenerBinding, null, endpointResourceName)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="serviceContext"></param>
        /// <param name="wcfServiceType"></param>
        /// <param name="listenerBinding"></param>
        /// <param name="endpointResourceName"></param>
        public WcfWebCommunicationListener(
            ServiceContext serviceContext,
            Type wcfServiceType,
            Binding listenerBinding = null,
            string endpointResourceName = null) :
            this(serviceContext, wcfServiceType, listenerBinding, null, endpointResourceName)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="serviceContext"></param>
        /// <param name="wcfServiceObject"></param>
        /// <param name="listenerBinding"></param>
        /// <param name="address"></param>
        public WcfWebCommunicationListener(
            ServiceContext serviceContext,
            TServiceContract wcfServiceObject,
            Binding listenerBinding = null,
            EndpointAddress address = null) :
            this(serviceContext, wcfServiceObject, listenerBinding, address, null)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="serviceContext"></param>
        /// <param name="wcfServiceType"></param>
        /// <param name="listenerBinding"></param>
        /// <param name="address"></param>
        public WcfWebCommunicationListener(
            ServiceContext serviceContext,
            Type wcfServiceType,
            Binding listenerBinding = null,
            EndpointAddress address = null) :
            this(serviceContext, wcfServiceType, listenerBinding, address, null)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="serviceContext"></param>
        /// <param name="wcfServiceObject"></param>
        /// <param name="listenerBinding"></param>
        /// <param name="address"></param>
        /// <param name="endpointResourceName"></param>
        WcfWebCommunicationListener(
            ServiceContext serviceContext,
            TServiceContract wcfServiceObject,
            Binding listenerBinding,
            EndpointAddress address,
            string endpointResourceName)
        {
            if (listenerBinding == null)
                listenerBinding = new WebHttpBinding();

            this.listenAddress = string.Empty;
            this.publishAddress = string.Empty;

            if (address == null)
            {
                address = GetEndpointAddress(
                    serviceContext,
                    listenerBinding,
                    endpointResourceName);

                this.listenAddress = serviceContext.ListenAddress;
                this.publishAddress = serviceContext.PublishAddress;
            }

            this.endpoint = CreateServiceEndpoint(typeof(TServiceContract), listenerBinding, address);
            this.host = CreateServiceHost(wcfServiceObject, this.endpoint);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="serviceContext"></param>
        /// <param name="wcfServiceType"></param>
        /// <param name="listenerBinding"></param>
        /// <param name="address"></param>
        /// <param name="endpointResourceName"></param>
        WcfWebCommunicationListener(
            ServiceContext serviceContext,
            Type wcfServiceType,
            Binding listenerBinding,
            EndpointAddress address,
            string endpointResourceName)
        {
            if (listenerBinding == null)
                listenerBinding = new WebHttpBinding();

            if (address == null)
                address = GetEndpointAddress(
                    serviceContext,
                    listenerBinding,
                    endpointResourceName);

            this.endpoint = CreateServiceEndpoint(typeof(TServiceContract), listenerBinding, address);
            this.host = CreateServiceHost(wcfServiceType, this.endpoint);
        }

        /// <summary>
        /// Gets the <see cref="WebServiceHost"/> used by this listener to host the WCF service implementation.
        /// </summary>
        public WebServiceHost ServiceHost => this.host;

        /// <summary>
        /// This method causes the communication listener to be opened. Once the Open completes, the communication
        /// listener becomes usable - accepts and sends messages.
        /// </summary>
        Task<string> ICommunicationListener.OpenAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.FromAsync(
                this.host.BeginOpen(this.endpoint.Binding.OpenTimeout, null, null),
                ar =>
                {
                    this.host.EndOpen(ar);
                    var listenUri = this.endpoint.Behaviors.Find<ListenUriEndpointBehavior>().ListenUri.ToString();
                    var publishUri = string.IsNullOrWhiteSpace(this.listenAddress) && string.IsNullOrWhiteSpace(this.publishAddress) ? listenUri : listenUri.Replace(this.listenAddress, this.publishAddress);
                    return publishUri;
                });
        }

        /// <summary>
        /// This method causes the communication listener to close. Close is a terminal state and 
        /// this method allows the communication listener to transition to this state in a
        /// graceful manner.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async Task ICommunicationListener.CloseAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Factory.FromAsync(
                    this.host.BeginClose(this.endpoint.Binding.CloseTimeout, null, null),
                    ar => this.host.EndClose(ar));
            }
            catch
            {
                this.host.Abort();
            }
        }

        /// <summary>
        /// This method causes the communication listener to close. Close is a terminal state and
        /// this method causes the transition to close ungracefully. Any outstanding operations
        /// (including close) should be canceled when this method is called.
        /// </summary>
        void ICommunicationListener.Abort()
        {
            this.host.Abort();
        }

        static ServiceEndpoint CreateServiceEndpoint(Type contractType, Binding binding, EndpointAddress address)
        {
            return new ServiceEndpoint(
                ContractDescription.GetContract(contractType),
                binding,
                address)
            {
                Behaviors =
                {
                    new ListenUriEndpointBehavior()
                }
            };
        }

        static WebServiceHost CreateServiceHost(
           object wcfServiceObject,
           ServiceEndpoint endpoint)
        {
            return ConfigureHost(new WebServiceHost(wcfServiceObject), endpoint, true);
        }

        static WebServiceHost CreateServiceHost(
           Type wcfServiceType,
           ServiceEndpoint endpoint)
        {
            return ConfigureHost(new WebServiceHost(wcfServiceType), endpoint, false);
        }

        static WebServiceHost ConfigureHost(WebServiceHost host, ServiceEndpoint endpoint, bool singleton)
        {
            var serviceBehavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            if (serviceBehavior == null)
            {
                serviceBehavior = new ServiceBehaviorAttribute();
                host.Description.Behaviors.Add(serviceBehavior);
            }

            if (singleton)
            {
                serviceBehavior.InstanceContextMode = InstanceContextMode.Single;
                serviceBehavior.ConcurrencyMode = ConcurrencyMode.Multiple;
            }

            // set debug behavior
            var serviceDebug = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (serviceDebug == null)
            {
                serviceDebug = new ServiceDebugBehavior();
                host.Description.Behaviors.Add(serviceDebug);
            }

            serviceDebug.IncludeExceptionDetailInFaults = true;

            // set throttling behavior
            var serviceThrottle = host.Description.Behaviors.Find<ServiceThrottlingBehavior>();
            if (serviceThrottle == null)
            {
                serviceThrottle = new ServiceThrottlingBehavior();
                host.Description.Behaviors.Add(serviceThrottle);
            }

            serviceThrottle.MaxConcurrentCalls = int.MaxValue;
            serviceThrottle.MaxConcurrentInstances = int.MaxValue;
            serviceThrottle.MaxConcurrentSessions = int.MaxValue;

            if (endpoint.ListenUri.Port == 0)
            {
                endpoint.ListenUriMode = ListenUriMode.Unique;
                serviceBehavior.AddressFilterMode = AddressFilterMode.Any;
            }

            var webBehavior = endpoint.Behaviors.Find<WebHttpBehavior>();
            if (webBehavior == null)
            {
                webBehavior = new WebHttpBehavior();
                endpoint.Behaviors.Add(webBehavior);
            }

            host.AddServiceEndpoint(endpoint);

            return host;
        }

        static EndpointAddress GetEndpointAddress(
            ServiceContext serviceContext,
            Binding binding,
            string endpointResourceName)
        {
            return new EndpointAddress(GetListenAddress(
                serviceContext,
                binding.Scheme,
                GetEndpointPort(
                    serviceContext.CodePackageActivationContext,
                    typeof(TServiceContract),
                    endpointResourceName)));
        }

        static int GetEndpointPort(
           ICodePackageActivationContext codePackageActivationContext,
           Type serviceContactType,
           string endpointResourceName)
        {
            var port = 0;

            var endpointName = endpointResourceName;
            if (string.IsNullOrEmpty(endpointName) && serviceContactType != null)
                endpointName = ServiceNameFormat.GetEndpointName(serviceContactType);

            foreach (var endpoint in codePackageActivationContext.GetEndpoints())
            {
                if (string.Compare(endpoint.Name, endpointName, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    port = endpoint.Port;
                    break;
                }
            }

            return port;
        }

        static Uri GetListenAddress(
            ServiceContext serviceContext,
            string scheme,
            int port)
        {
            return new Uri(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}://{1}:{2}/{5}/{3}-{4}",
                    scheme,
                    serviceContext.ListenAddress,
                    port,
                    serviceContext.PartitionId,
                    serviceContext.ReplicaOrInstanceId,
                    Guid.NewGuid()));
        }

    }

}
