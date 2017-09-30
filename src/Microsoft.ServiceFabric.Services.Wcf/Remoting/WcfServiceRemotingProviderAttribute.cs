﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Wcf
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Communication.Wcf;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

    /// <summary>
    ///     Sets WCF as the default service remoting transport provider in the assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class WcfServiceRemotingProviderAttribute : ServiceRemotingProviderAttribute
    {
        private const long DefaultMaxMessageSize = 4*1024*1024;
        private static readonly TimeSpan DefaultOpenCloseTimeout = TimeSpan.FromMilliseconds(5000);

        /// <summary>
        /// Constructs a <see cref="WcfServiceRemotingProviderAttribute"/> which can be used
        /// to set WCF transport as the default service remoting transport provider in the assembly.
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
        ///     Creates a V1 service remoting listener for remoting the service interface.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="serviceImplementation">
        ///     The service implementation object.
        /// </param>
        /// <returns>
        ///     A <see cref=" V1.Wcf.Runtime.WcfServiceRemotingListener"/> for the specified service.
        /// </returns>
        public override IServiceRemotingListener CreateServiceRemotingListener(
            ServiceContext serviceContext, 
            IService serviceImplementation)
        {
            return new V1.Wcf.Runtime.WcfServiceRemotingListener(
                serviceContext,
                serviceImplementation,
                WcfUtility.CreateTcpListenerBinding(
                    this.GetMaxMessageSize(),
                    this.GetOpenTimeout(),
                    this.GetCloseTimeout()));
        }


        /// <summary>
        ///     Creates a V1 service remoting client factory that can be used by the 
        ///     <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.ServiceProxyFactory"/> 
        ///     to create a proxy for the remoted interface of the service.
        /// </summary>
        /// <param name="callbackClient">
        ///     Client implementation where the callbacks should be dispatched.
        /// </param>
        /// <returns>
        ///     A <see cref="Microsoft.ServiceFabric.Services.Remoting.V1.Wcf.Client.WcfServiceRemotingClientFactory"/>.
        /// </returns>
        public override V1.Client.IServiceRemotingClientFactory CreateServiceRemotingClientFactory(
            V1.IServiceRemotingCallbackClient callbackClient)
        {
            return new V1.Wcf.Client.WcfServiceRemotingClientFactory(
                WcfUtility.CreateTcpClientBinding(
                    this.GetMaxMessageSize(),
                    this.GetOpenTimeout(),
                    this.GetCloseTimeout()),
                    callbackClient);
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
        ///     A <see cref="Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Client.WcfServiceRemotingClientFactory"/>.
        /// </returns>
        public override V2.Client.IServiceRemotingClientFactory CreateServiceRemotingClientFactoryV2(
            IServiceRemotingCallbackMessageHandler callbackMessageHandler)
        {
            return new Services.Remoting.V2.Wcf.Client.WcfServiceRemotingClientFactory(
                WcfUtility.CreateTcpClientBinding(
                    this.GetMaxMessageSize(),
                    this.GetOpenTimeout(),
                    this.GetCloseTimeout()),
                callbackMessageHandler);
        }

        /// <summary>
        ///     Creates a V2 service remoting listener for remoting the service interface.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="serviceImplementation">
        ///     The service implementation object.
        /// </param>
        /// <returns>
        ///     A <see cref=" V2.Wcf.Runtime.WcfServiceRemotingListener"/> for the specified service.
        /// </returns>
        public override IServiceRemotingListener CreateServiceRemotingListenerV2(
            ServiceContext serviceContext, IService serviceImplementation)
        {
            return new V2.Wcf.Runtime.WcfServiceRemotingListener(
                serviceContext,
                serviceImplementation,
                WcfUtility.CreateTcpListenerBinding(
                    this.GetMaxMessageSize(),
                    this.GetOpenTimeout(),
                    this.GetCloseTimeout()));
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