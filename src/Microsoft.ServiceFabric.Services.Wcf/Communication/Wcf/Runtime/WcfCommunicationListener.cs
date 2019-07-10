// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime
{
    using System;
    using System.Fabric;
    using System.Globalization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    /// <summary>
    /// A Windows Communication Foundation based listener for Service Fabric based stateless or stateful service.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the WCF service contract.</typeparam>
    public class WcfCommunicationListener<TServiceContract> : ICommunicationListener
    {
        private readonly ServiceEndpoint endpoint;
        private readonly ServiceHost host;
        private readonly string listenAddress;
        private readonly string publishAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfCommunicationListener{TServiceContract}"/> class
        /// that uses default binding and default endpoint address.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which this communication listener is being constructed.
        /// </param>
        /// <param name="wcfServiceObject">
        ///     WCF service implementing the specified WCF service contract.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         The default listener binding is created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method.
        ///     </para>
        ///     <para>
        ///         The default endpoint address is created using the endpoint resource defined in the service manifest. The name of the endpoint resource is
        ///         derived from the WCF service contract type using <see cref="Microsoft.ServiceFabric.Services.ServiceNameFormat.GetEndpointName"/> method.
        ///         If matching endpoint resource is not found in the service manifest, a default endpoint resource definition with port zero is used.
        ///     </para>
        /// </remarks>
        public WcfCommunicationListener(
            ServiceContext serviceContext,
            TServiceContract wcfServiceObject)
            : this(serviceContext, wcfServiceObject, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfCommunicationListener{TServiceContract}"/> class
        /// that uses specified listener binding and endpoint address derived from the specified endpoint resource name.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which this communication listener is being constructed.
        /// </param>
        /// <param name="wcfServiceObject">
        ///     WCF service implementing the specified WCF service contract.
        /// </param>
        /// <param name="listenerBinding">
        ///     The binding to use for the WCF endpoint. If the listenerBinding is not specified or it is null, a default listener binding is
        ///     created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method.
        /// </param>
        /// <param name="endpointResourceName">
        ///     The name of the endpoint resource defined in the service manifest that should be used to create the address for the listener.
        ///     If the endpointResourceName is not specified or it is null, its name is derived from the WCF service contract type using
        ///     <see cref="Microsoft.ServiceFabric.Services.ServiceNameFormat.GetEndpointName"/> method.
        ///     If matching endpoint resource is not found in the service manifest, a default endpoint resource definition with port zero is used.
        /// </param>
        public WcfCommunicationListener(
            ServiceContext serviceContext,
            TServiceContract wcfServiceObject,
            Binding listenerBinding = null,
            string endpointResourceName = null)
            : this(serviceContext, wcfServiceObject, listenerBinding, null, endpointResourceName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfCommunicationListener{TServiceContract}"/> class that uses specified listener binding and
        ///     endpoint address derived from the specified endpoint resource name.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which this communication listener is being constructed.
        /// </param>
        /// <param name="wcfServiceType">
        ///     Type of WCF service implementing the specified WCF service contract.
        /// </param>
        /// <param name="listenerBinding">
        ///     The binding to use for the WCF endpoint. If the listenerBinding is not specified or it is null, a default listener binding is
        ///     created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method.
        /// </param>
        /// <param name="endpointResourceName">
        ///     The name of the endpoint resource defined in the service manifest that should be used to create the address for the listener.
        ///     If the endpointResourceName is not specified or it is null, its name is derived from the WCF service contract type using
        ///     <see cref="Microsoft.ServiceFabric.Services.ServiceNameFormat.GetEndpointName"/> method.
        ///     If matching endpoint resource is not found in the service manifest, a default endpoint resource definition with port zero is used.
        /// </param>
        public WcfCommunicationListener(
            ServiceContext serviceContext,
            Type wcfServiceType,
            Binding listenerBinding = null,
            string endpointResourceName = null)
            : this(serviceContext, wcfServiceType, listenerBinding, null, endpointResourceName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfCommunicationListener{TServiceContract}"/> class
        ///  that uses specified listener binding and endpoint address derived from the specified endpoint address.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which this communication listener is being constructed.
        /// </param>
        /// <param name="wcfServiceObject">
        ///     WCF service implementing the specified WCF service contract.
        /// </param>
        /// <param name="listenerBinding">
        ///     The binding to use for the WCF endpoint. If the listenerBinding is not specified or it is null, a default listener binding is
        ///     created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method.
        /// </param>
        /// <param name="address">
        ///     The listen address for the WCF endpoint. If the address is not specified or it is null, a default address is created by
        ///     looking up the endpoint resource from the service manifest. The endpoint resource name is derived from the WCF
        ///     service contract type using <see cref="Microsoft.ServiceFabric.Services.ServiceNameFormat.GetEndpointName"/> method.
        ///     If matching endpoint resource is not found in the service manifest, a default endpoint resource definition with port zero is used.
        /// </param>
        public WcfCommunicationListener(
            ServiceContext serviceContext,
            TServiceContract wcfServiceObject,
            Binding listenerBinding = null,
            EndpointAddress address = null)
            : this(serviceContext, wcfServiceObject, listenerBinding, address, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfCommunicationListener{TServiceContract}"/> class
        /// that uses specified listener binding and endpoint address derived from the specified endpoint address.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which this communication listener is being constructed.
        /// </param>
        /// <param name="wcfServiceType">
        ///     Type of WCF service implementing the specified WCF service contract.
        /// </param>
        /// <param name="listenerBinding">
        ///     The binding to use for the WCF endpoint. If the listenerBinding is not specified or it is null, a default listener binding is
        ///     created using <see cref="WcfUtility.CreateTcpListenerBinding"/> method.
        /// </param>
        /// <param name="address">
        ///     The listen address for the WCF endpoint. If the address is not specified or it is null, a default address is created by
        ///     looking up the endpoint resource from the service manifest. The endpoint resource name is derived from the WCF
        ///     service contract type using <see cref="Microsoft.ServiceFabric.Services.ServiceNameFormat.GetEndpointName"/> method.
        ///     If matching endpoint resource is not found in the service manifest, a default endpoint resource definition with port zero is used.
        /// </param>
        public WcfCommunicationListener(
            ServiceContext serviceContext,
            Type wcfServiceType,
            Binding listenerBinding = null,
            EndpointAddress address = null)
            : this(serviceContext, wcfServiceType, listenerBinding, address, null)
        {
        }

        private WcfCommunicationListener(
            ServiceContext serviceContext,
            TServiceContract wcfServiceObject,
            Binding listenerBinding,
            EndpointAddress address,
            string endpointResourceName)
        {
            if (listenerBinding == null)
            {
                listenerBinding = WcfUtility.DefaultTcpListenerBinding;
            }

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

            this.endpoint = CreateServiceEndpoint(typeof(TServiceContract), wcfServiceObject, listenerBinding, address);
            this.host = CreateServiceHost(wcfServiceObject, this.endpoint);
            ServiceTelemetry.CommunicationListenerUsageEvent(serviceContext, TelemetryConstants.WCFCommunicationListener);
        }

        private WcfCommunicationListener(
            ServiceContext serviceContext,
            Type wcfServiceType,
            Binding listenerBinding,
            EndpointAddress address,
            string endpointResourceName)
        {
            if (listenerBinding == null)
            {
                listenerBinding = WcfUtility.DefaultTcpListenerBinding;
            }

            if (address == null)
            {
                address = GetEndpointAddress(
                    serviceContext,
                    listenerBinding,
                    endpointResourceName);
            }

            this.endpoint = CreateServiceEndpoint(typeof(TServiceContract), wcfServiceType, listenerBinding, address);
            this.host = CreateServiceHost(wcfServiceType, this.endpoint);
            ServiceTelemetry.CommunicationListenerUsageEvent(serviceContext, TelemetryConstants.WCFCommunicationListener);
        }

        /// <summary>
        ///     Gets the <see cref="System.ServiceModel.ServiceHost"/> used by this listener to host the
        ///     WCF service implementation.
        /// </summary>
        /// <value>
        ///     A <see cref="System.ServiceModel.ServiceHost"/> used by this listener to host the
        ///     WCF service implementation.
        /// </value>
        /// <remarks>
        ///     The service host is created by the listener in its constructor. Before this communication
        ///     listener is opened by the runtime via <see cref="ICommunicationListener.OpenAsync(CancellationToken)"/> method,
        ///     the service host can be customized by accessing it via this property.
        /// </remarks>
        public ServiceHost ServiceHost
        {
            get { return this.host; }
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
        Task<string> ICommunicationListener.OpenAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.FromAsync(
              this.host.BeginOpen(this.endpoint.Binding.OpenTimeout, null, null),
              ar =>
              {
                  this.host.EndOpen(ar);
                  var listenUri = this.endpoint.Behaviors.Find<ListenUriEndpointBehavior>().ListenUri.ToString();
                  var publishUri = string.IsNullOrWhiteSpace(this.listenAddress) && string.IsNullOrWhiteSpace(this.publishAddress) ? listenUri : listenUri.Replace(this.listenAddress, this.publishAddress);

                  System.Fabric.Common.AppTrace.TraceSource.WriteInfo("WcfCommunicationListener.OpenAsync", "ListenURI = {0} PublishURI = {1}", listenUri, publishUri);

                  return publishUri;
              });
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

        private static ServiceEndpoint CreateServiceEndpoint(Type contractType, Type serviceType, Binding binding, EndpointAddress address)
        {
            var endpoint = new ServiceEndpoint(
                ContractDescription.GetContract(contractType, serviceType),
                binding,
                address);
            endpoint.Behaviors.Add(new ListenUriEndpointBehavior());

            return endpoint;
        }

        private static ServiceEndpoint CreateServiceEndpoint(Type contractType, object serviceImplementation, Binding binding, EndpointAddress address)
        {
            var endpoint = new ServiceEndpoint(
                ContractDescription.GetContract(contractType, serviceImplementation),
                binding,
                address);
            endpoint.Behaviors.Add(new ListenUriEndpointBehavior());

            return endpoint;
        }

        private static ServiceHost CreateServiceHost(
            object wcfServiceObject,
            ServiceEndpoint endpoint)
        {
            return ConfigureHost(new ServiceHost(wcfServiceObject), endpoint, true);
        }

        private static ServiceHost CreateServiceHost(
            Type wcfServiceType,
            ServiceEndpoint endpoint)
        {
            return ConfigureHost(new ServiceHost(wcfServiceType), endpoint, false);
        }

        private static ServiceHost ConfigureHost(ServiceHost host, ServiceEndpoint endpoint, bool singleton)
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

            // set global error handler behavior
            var globalErrorHandlerBehavior = new WcfGlobalErrorHandlerBehaviorAttribute();
            host.Description.Behaviors.Add(globalErrorHandlerBehavior);

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

            host.AddServiceEndpoint(endpoint);

            // Exception Handler for Background Threads
            ExceptionHandler.AsynchronousThreadExceptionHandler = new WcfAsyncThreadExceptionHandler();
            return host;
        }

        private static EndpointAddress GetEndpointAddress(
            ServiceContext serviceContext,
            Binding binding,
            string endpointResourceName)
        {
            var listenAddress = GetListenAddress(
                serviceContext,
                binding.Scheme,
                GetEndpointPort(
                    serviceContext.CodePackageActivationContext,
                    typeof(TServiceContract),
                    endpointResourceName));

            return new EndpointAddress(listenAddress);
        }

        private static int GetEndpointPort(
           ICodePackageActivationContext codePackageActivationContext,
           Type serviceContactType,
           string endpointResourceName)
        {
            var port = 0;

            var endpointName = endpointResourceName;
            if (string.IsNullOrEmpty(endpointName) && (serviceContactType != null))
            {
                endpointName = ServiceNameFormat.GetEndpointName(serviceContactType);
            }

            var endpoints = codePackageActivationContext.GetEndpoints();
            foreach (var endpoint in endpoints)
            {
                if (string.Compare(endpoint.Name, endpointName, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    port = endpoint.Port;
                    break;
                }
            }

            return port;
        }

        private static Uri GetListenAddress(
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
