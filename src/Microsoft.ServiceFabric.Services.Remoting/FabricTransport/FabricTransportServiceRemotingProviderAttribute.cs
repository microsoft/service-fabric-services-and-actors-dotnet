// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.FabricTransport
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.Base;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

#if !DotNetCoreClr
    using Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime;

#endif

    /// <summary>
    /// This attributes allows to set Fabric TCP transport as the default service remoting transport provider in the assembly and customization of it.
    /// </summary>
    public class FabricTransportServiceRemotingProviderAttribute : ServiceRemotingProviderAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportServiceRemotingProviderAttribute"/> class.
        /// </summary>
        public FabricTransportServiceRemotingProviderAttribute()
        {
        }

        /// <summary>
        ///     Gets or sets the maximum size of the remoting message in bytes.
        ///     If value for this property is not specified or it is less than or equals to zero,
        ///     a default value of 4,194,304 bytes (4 MB) is used.
        /// </summary>
        /// <value>
        ///     The maximum size of the remoting message in bytes. If this value is not specified
        ///     or it is less than or equals to zero, a default value of 4,194,304 bytes (4 MB) is used.
        /// </value>
        public long MaxMessageSize { get; set; }

        /// <summary>
        ///     Gets or Sets the operation timeout in seconds. If the operation is not completed in the specified
        ///     time, it will be timed out. By default, exception handler of
        ///     <see cref="V2.FabricTransport.Client.FabricTransportServiceRemotingClientFactory"/>
        ///     retries the timed out exception. It is recommended to not change the operation timeout from it's default value.
        /// </summary>
        /// <value>
        ///     The operation timeout in seconds. If not specified or less than zero, default operation timeout
        ///     of maximum value is used.
        /// </value>
        public long OperationTimeoutInSeconds { get; set; }

        /// <summary>
        ///     Gets or Sets the keep alive timeout in seconds. This settings is useful in the scenario when the client
        ///     and service are connected via load balancer that closes the connection if it is idle for some time.
        ///     If keep alive timeout is configured, the connection will be kept alive by sending ping messages at
        ///     that interval.
        /// </summary>
        /// <value>
        ///     The keep alive timeout in seconds.
        /// </value>
        public long KeepAliveTimeoutInSeconds { get; set; }

        /// <summary>
        ///     Gets or Sets the connect timeout in milliseconds. This settings specifies the maximum time allowed for the connection
        ///     to be established.
        /// </summary>
        /// <value>
        ///     The connect timeout in Milliseconds.
        /// </value>
        /// <remarks>Default Value for ConnectTimeout Timeout is 5 seconds.</remarks>
        public long ConnectTimeoutInMilliseconds { get; set; }

#if !DotNetCoreClr

        /// <summary>
        ///     Creates a service remoting listener for remoting the service interface.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="serviceImplementation">
        ///     The service implementation object.
        /// </param>
        /// <returns>
        ///     A <see cref="FabricTransportServiceRemotingListener"/>
        ///     as <see cref="IServiceRemotingListener"/>
        ///     for the specified service implementation.
        /// </returns>
        public override IServiceRemotingListener CreateServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation)
        {
            var settings = FabricTransportRemotingListenerSettings.GetDefault();
            settings.MaxMessageSize = this.GetAndValidateMaxMessageSize(settings.MaxMessageSize);
            settings.OperationTimeout = this.GetAndValidateOperationTimeout(settings.OperationTimeout);
            settings.KeepAliveTimeout = this.GetKeepAliveTimeout(settings.KeepAliveTimeout);
            return new FabricTransportServiceRemotingListener(serviceContext, serviceImplementation, settings);
        }

        /// <summary>
        ///     Creates a  V1 service remoting client factory for connecting to the service over remoted service interfaces.
        /// </summary>
        /// <param name="callbackClient">
        ///    The client implementation where the callbacks should be dispatched.
        /// </param>
        /// <returns>
        ///     A <see cref="FabricTransportServiceRemotingClientFactory"/>
        ///     as <see cref="V1.Client.IServiceRemotingClientFactory"/>
        ///     that can be used with <see cref="Remoting.Client.ServiceProxyFactory"/> to
        ///     generate service proxy to talk to a stateless or stateful service over remoted actor interface.
        /// </returns>
        public override V1.Client.IServiceRemotingClientFactory CreateServiceRemotingClientFactory(
            V1.IServiceRemotingCallbackClient callbackClient)
        {
            var settings = FabricTransportRemotingSettings.GetDefault();
            settings.MaxMessageSize = this.GetAndValidateMaxMessageSize(settings.MaxMessageSize);
            settings.OperationTimeout = this.GetAndValidateOperationTimeout(settings.OperationTimeout);
            settings.KeepAliveTimeout = this.GetKeepAliveTimeout(settings.KeepAliveTimeout);
            settings.ConnectTimeout = this.GetConnectTimeout(settings.ConnectTimeout);
            return new FabricTransportServiceRemotingClientFactory(settings, callbackClient);
        }

#endif

        /// <summary>
        ///     Creates a V2 service remoting listener for remoting the service interface.
        /// </summary>
        /// <returns>
        ///     A <see cref="V2.FabricTransport.Runtime.FabricTransportServiceRemotingListener"/>
        ///     as <see cref="IServiceRemotingListener"/>
        ///     for the specified service implementation.
        /// </returns>
        public override Dictionary<string, Func<ServiceContext, IService, IServiceRemotingListener>>
            CreateServiceRemotingListeners()
        {
            var dic = new Dictionary<string, Func<ServiceContext, IService, IServiceRemotingListener>>();

            if ((RemotingHelper.IsRemotingV2(this.RemotingListenerVersion)))
            {
                dic.Add(ServiceRemotingProviderAttribute.DefaultV2listenerName, (serviceContext, serviceImplementation)
                    =>
                {
                    var listenerSettings = this.GetListenerSettings(serviceContext);
                    return new V2.FabricTransport.Runtime.FabricTransportServiceRemotingListener(
                        serviceContext: serviceContext,
                        serviceImplementation: serviceImplementation,
                        remotingListenerSettings: listenerSettings);
                });
            }

            if (RemotingHelper.IsRemotingV2_1(this.RemotingListenerVersion))
            {
                dic.Add(ServiceRemotingProviderAttribute.DefaultWrappedMessageStackListenerName, (
                    serviceContext, serviceImplementation) =>
                {
                    var listenerSettings = this.GetListenerSettings(serviceContext);
                    listenerSettings.UseWrappedMessage = true;
                    return new V2.FabricTransport.Runtime.FabricTransportServiceRemotingListener(
                        serviceContext,
                        serviceImplementation,
                        listenerSettings);
                });
            }

            return dic;
        }

        /// <summary>
        ///     Creates a  V2 service remoting client factory for connecting to the service over remoted service interfaces.
        /// </summary>
        /// <param name="callbackMessageHandler">
        ///    The client implementation where the callbacks should be dispatched.
        /// </param>
        /// <returns>
        ///     A <see cref="V2.FabricTransport.Client.FabricTransportServiceRemotingClientFactory"/>
        ///     as <see cref="IServiceRemotingClientFactory"/>
        ///     that can be used with <see cref="Remoting.Client.ServiceProxyFactory"/> to
        ///     generate service proxy to talk to a stateless or stateful service over remoted actor interface.
        /// </returns>
        public override IServiceRemotingClientFactory CreateServiceRemotingClientFactoryV2(
            IServiceRemotingCallbackMessageHandler callbackMessageHandler)
        {
            var settings = FabricTransportRemotingSettings.GetDefault();
            settings.MaxMessageSize = this.GetAndValidateMaxMessageSize(settings.MaxMessageSize);
            settings.OperationTimeout = this.GetAndValidateOperationTimeout(settings.OperationTimeout);
            settings.KeepAliveTimeout = this.GetKeepAliveTimeout(settings.KeepAliveTimeout);
            settings.ConnectTimeout = this.GetConnectTimeout(settings.ConnectTimeout);
            if (RemotingHelper.IsRemotingV2_1(this.RemotingClientVersion))
            {
                settings.UseWrappedMessage = true;
            }

            return new V2.FabricTransport.Client.FabricTransportServiceRemotingClientFactory(
                remotingSettings: settings,
                remotingCallbackMessageHandler: callbackMessageHandler);
        }

        internal FabricTransportRemotingListenerSettings GetListenerSettings(ServiceContext serviceContext)
        {
            var settings = FabricTransportRemotingListenerSettings.GetDefault();
            settings.MaxMessageSize = this.GetAndValidateMaxMessageSize(settings.MaxMessageSize);
            settings.OperationTimeout = this.GetAndValidateOperationTimeout(settings.OperationTimeout);
            settings.KeepAliveTimeout = this.GetKeepAliveTimeout(settings.KeepAliveTimeout);
            return settings;
        }

        private long GetAndValidateMaxMessageSize(long maxMessageSize)
        {
            return (this.MaxMessageSize > 0) ? this.MaxMessageSize : maxMessageSize;
        }

        private TimeSpan GetAndValidateOperationTimeout(TimeSpan operationTimeout)
        {
            return (this.OperationTimeoutInSeconds > 0)
                ? TimeSpan.FromSeconds(this.OperationTimeoutInSeconds)
                : operationTimeout;
        }

        private TimeSpan GetKeepAliveTimeout(TimeSpan keepAliveTimeout)
        {
            return (this.KeepAliveTimeoutInSeconds > 0)
                ? TimeSpan.FromSeconds(this.KeepAliveTimeoutInSeconds)
                : keepAliveTimeout;
        }

        private TimeSpan GetConnectTimeout(TimeSpan connectTimeout)
        {
            return (this.ConnectTimeoutInMilliseconds > 0)
                ? TimeSpan.FromMilliseconds(this.ConnectTimeoutInMilliseconds)
                : connectTimeout;
        }
    }
}
