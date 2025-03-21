// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Reflection;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

namespace Microsoft.ServiceFabric.Services.Remoting
{
    /// <summary>
    /// This is a base type for attribute that sets the default service remoting provider to use for
    /// remoting the service interfaces defined and used in the assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     On service side, implementation of this attribute is looked up by
    ///     <see cref="ServiceRemotingExtensions.CreateServiceRemotingReplicaListeners{TStatefulService}(TStatefulService)"/> and
    ///     <see cref="ServiceRemotingExtensions.CreateServiceRemotingInstanceListeners{TStatelessService}(TStatelessService)"/>
    ///     methods on the runtime to create a default <see cref="IServiceRemotingListener"/> for the stateful and stateless services.
    ///     </para>
    ///     <para>
    ///     On client side, implementation of this attribute is looked up by
    ///     <see cref="ServiceProxyFactory"/> constructor to create a default
    ///     <see cref="IServiceRemotingClientFactory"/> when it is not specified.
    ///     </para>
    ///     <para>
    ///     Note that on client side
    ///     <see cref="Client.ServiceProxy.Create{TServiceInterface}(Uri, Services.Client.ServicePartitionKey, Communication.Client.TargetReplicaSelector, string)"/>
    ///     method create a default <see cref="ServiceProxyFactory"/> once and hence the provider lookup happens
    ///     only for the first time, after which the same provider is used.
    ///     </para>
    ///     <para>
    ///     The order in which this attribute is looked up is as follows:
    ///     <list type="number">
    ///         <item>
    ///             In the entry <see cref="Assembly"/> obtained by calling method <see cref="Assembly.GetEntryAssembly"/>
    ///         </item>
    ///         <item>
    ///             In the <see cref="Assembly"/> that defines the remote interface for which listener or proxy is being created.
    ///         </item>
    ///     </list>
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly)]
    public abstract class ServiceRemotingProviderAttribute : Attribute
    {
        private static Assembly entryAssembly = Assembly.GetEntryAssembly();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRemotingProviderAttribute"/> class.
        /// </summary>
        public ServiceRemotingProviderAttribute()
        {
            this.RemotingListenerVersion = RemotingListenerVersion.V2_1;
            this.RemotingClientVersion = RemotingClientVersion.V2_1;
        }

        /// <summary>
        /// Gets or sets the version of the remoting client to use.
        /// </summary>
        public RemotingClientVersion RemotingClientVersion { get; set; }

        /// <summary>
        /// Gets or sets the version that the remoting listener to use.
        /// </summary>
        public RemotingListenerVersion RemotingListenerVersion { get; set; }

        internal static string DefaultV2listenerName
        {
            get { return "V2Listener"; }
        }

        internal static string DefaultWrappedMessageStackListenerName
        {
            get { return "V2_1Listener"; }
        }

        /// <summary>
        /// Returns the func method that creates the remoting listeners.
        /// </summary>
        /// <returns>Func</returns>
        public abstract Dictionary<string, Func<ServiceContext, IService, IServiceRemotingListener>>
            CreateServiceRemotingListeners();

        /// <summary>
        /// Creates a service remoting client factory that can be used by the
        /// <see cref="ServiceProxyFactory"/> to create a proxy for the remoted interface of the service.
        /// </summary>
        /// <param name="callbackMessageHandler">Client implementation where the callbacks should be dispatched.</param>
        public abstract IServiceRemotingClientFactory CreateServiceRemotingClientFactoryV2(
            IServiceRemotingCallbackMessageHandler callbackMessageHandler);

        internal static ServiceRemotingProviderAttribute GetProvider(IEnumerable<Type> types = null)
        {
            if (types != null)
            {
                foreach (var t in types)
                {
                    var attribute = t.GetTypeInfo().Assembly.GetCustomAttribute<ServiceRemotingProviderAttribute>();
                    if (attribute != null)
                    {
                        return attribute;
                    }
                }
            }

            if (entryAssembly != null)
            {
                var attribute = entryAssembly.GetCustomAttribute<ServiceRemotingProviderAttribute>();
                if (attribute != null)
                {
                    return attribute;
                }
            }

            return new FabricTransportServiceRemotingProviderAttribute();
        }
    }
}
