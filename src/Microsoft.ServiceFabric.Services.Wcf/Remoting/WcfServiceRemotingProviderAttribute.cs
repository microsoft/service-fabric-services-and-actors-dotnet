// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.Wcf
{
    /// <summary>
    /// This attributes allows to set WCF transport as the default service remoting transport provider in the assembly and customization for it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class WcfServiceRemotingProviderAttribute : ServiceRemotingProviderAttribute
    {
        private const long DefaultMaxMessageSize = 4 * 1024 * 1024;
        private static readonly TimeSpan DefaultOpenCloseTimeout = TimeSpan.FromMilliseconds(5000);

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceRemotingProviderAttribute"/> class.
        /// </summary>
        public WcfServiceRemotingProviderAttribute()
        {
        }

        /// <summary>
        ///     Gets or Sets the maximum message size that can be transferred over remoting.
        /// </summary>
        /// <value>
        ///     The maximum message size that can be transferred over remoting.
        /// </value>
        public long MaxMessageSize
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or Sets the timeout in milliseconds for opening the connection from client side and waiting for the listener to open on the service side.
        /// </summary>
        /// <value>
        ///     The timeout in milliseconds for opening the connection from client side and waiting for the listener to open on the service side.
        /// </value>
        public long OpenTimeoutInMilliSeconds
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or Sets the timeout in milliseconds to wait before closing the connection to let existing messages drain.
        /// </summary>
        /// <value>
        ///     The timeout in milliseconds to wait before closing the connection to let existing messages drain.
        /// </value>
        public long CloseTimeoutInMilliSeconds
        {
            get;
            set;
        }

        /// <summary>
        ///     Creates a service remoting client factory that can be used by the
        ///     <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.ServiceProxyFactory"/>
        ///     to create a proxy for the remoted interface of the service.
        /// </summary>
        /// <param name="callbackMessageHandler">
        ///     Client implementation where the callbacks should be dispatched.
        /// </param>
        /// <returns>
        ///     A <see cref="WcfServiceRemotingClientFactory"/>.
        /// </returns>
        public override IServiceRemotingClientFactory CreateServiceRemotingClientFactoryV2(
            IServiceRemotingCallbackMessageHandler callbackMessageHandler)
        {
            return new WcfServiceRemotingClientFactory(
                WcfUtility.CreateTcpClientBinding(
                    this.GetMaxMessageSize(),
                    this.GetOpenTimeout(),
                    this.GetCloseTimeout()),
                callbackMessageHandler);
        }

        /// <summary>
        /// Returns the func method that creates the remoting listeners.
        /// </summary>
        /// <returns>Func to create Remoting Listener</returns>
        public override Dictionary<string, Func<ServiceContext, IService, IServiceRemotingListener>>
            CreateServiceRemotingListeners()
        {
            var dic = new Dictionary<string, Func<ServiceContext, IService, IServiceRemotingListener>>();

            if (Helper.IsRemotingV2(this.RemotingListenerVersion))
            {
                dic.Add(DefaultV2listenerName, 
                    (serviceContext, serviceImplementation) =>
                    {
                        var bindings = WcfUtility.CreateTcpListenerBinding(
                            this.GetMaxMessageSize(),
                            this.GetOpenTimeout(),
                            this.GetCloseTimeout());
                        return new WcfServiceRemotingListener(serviceContext, serviceImplementation, bindings);
                    });
            }

            if (Helper.IsRemotingV2_1(this.RemotingListenerVersion))
            {
                dic.Add(DefaultWrappedMessageStackListenerName, 
                    (serviceContext, serviceImplementation) =>
                    {
                        var bindings = WcfUtility.CreateTcpListenerBinding(
                            this.GetMaxMessageSize(),
                            this.GetOpenTimeout(),
                            this.GetCloseTimeout());
                        return new WcfServiceRemotingListener(
                                serviceContext,
                                serviceImplementation,
                                bindings,
                                useWrappedMessage: true);
                    });
            }

            return dic;
        }

        private long GetMaxMessageSize()
        {
            return (this.MaxMessageSize > 0) ? this.MaxMessageSize : DefaultMaxMessageSize;
        }

        private TimeSpan GetCloseTimeout()
        {
            return (this.CloseTimeoutInMilliSeconds > 0) ? TimeSpan.FromMilliseconds(this.CloseTimeoutInMilliSeconds) : DefaultOpenCloseTimeout;
        }

        private TimeSpan GetOpenTimeout()
        {
            return (this.OpenTimeoutInMilliSeconds > 0) ? TimeSpan.FromMilliseconds(this.OpenTimeoutInMilliSeconds) : DefaultOpenCloseTimeout;
        }
    }
}
