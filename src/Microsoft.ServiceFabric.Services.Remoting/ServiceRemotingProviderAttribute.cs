// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Reflection;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
#if !DotNetCoreClr
    using Microsoft.ServiceFabric.Services.Remoting.V1.Client;
#endif
    /// <summary>
    /// This is a base type for attribute that sets the default service remoting provider to use for 
    /// remoting the service interfaces defined and used in the assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     On service side, implementation of this attribute is looked up by
    ///     <see cref="ServiceRemotingExtensions.CreateServiceRemotingListener{TStatefulService}(TStatefulService,System.Fabric.StatefulServiceContext)"/> and
    ///     <see cref="ServiceRemotingExtensions.CreateServiceRemotingListener{TStatelessService}(TStatelessService,System.Fabric.StatelessServiceContext)"/> 
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
    ///             In the entry <see cref="System.Reflection.Assembly"/> obtained by calling method <see cref="System.Reflection.Assembly.GetEntryAssembly"/> 
    ///         </item>
    ///         <item>
    ///             In the <see cref="System.Reflection.Assembly"/> that defines the remote interface for which listener or proxy is being created. 
    ///         </item>
    ///     </list>
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly)]
    public abstract class ServiceRemotingProviderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the ServiceRemotingProviderAttribute class.
        /// </summary>
        public ServiceRemotingProviderAttribute()
        {
        }
        /// <summary>
        /// RemotingClient is used to determine where  V1 or V2 remoting Client is used.
        /// </summary>
        public RemotingClient RemotingClient { get; set; }

        /// <summary>
        /// RemotingListener is used to determine where listener is in V1, V2 or Compact Mode.
        /// </summary>
        public RemotingListener RemotingListener { get; set; }

        internal static string DefaultV2listenerName
        {
            get { return "V2Listener"; }
        }

#if !DotNetCoreClr

        /// <summary>
        /// Creates a V1 service remoting listener for remoting the service interface.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="serviceImplementation">The service implementation object.</param>
        /// <returns>An <see cref="IServiceRemotingListener"/> for the specified service.</returns>
        public abstract IServiceRemotingListener CreateServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation);

        /// <summary>
        /// Creates a service remoting client factory that can be used by the 
        /// <see cref="ServiceProxyFactory"/> to create a proxy for the remoted interface of the service.
        /// </summary>
        /// <param name="callbackClient">Client implementation where the callbacks should be dispatched.</param>
        /// <returns>An <see cref="IServiceRemotingClientFactory"/>.</returns>
        public abstract IServiceRemotingClientFactory CreateServiceRemotingClientFactory(
            Remoting.V1.IServiceRemotingCallbackClient callbackClient);

#endif
        /// <summary>
        /// Creates a V2 service remoting listener for remoting the service interface.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="serviceImplementation">The service implementation object.</param>
        /// <returns>An <see cref="IServiceRemotingListener"/> for the specified service.</returns>
        public abstract IServiceRemotingListener CreateServiceRemotingListenerV2(
            ServiceContext serviceContext,
            IService serviceImplementation);

        /// <summary>
        /// Creates a V2 service remoting client factory that can be used by the 
        /// <see cref="ServiceProxyFactory"/> to create a proxy for the remoted interface of the service.
        /// </summary>
        /// <param name="callbackMessageHandler">Client implementation where the callbacks should be dispatched.</param>
        /// <returns>An <see cref="Microsoft.ServiceFabric.Services.Remoting.V2.Client.IServiceRemotingClientFactory"/>.</returns>

        public abstract Remoting.V2.Client.IServiceRemotingClientFactory CreateServiceRemotingClientFactoryV2(
            Remoting.V2.Client.IServiceRemotingCallbackMessageHandler callbackMessageHandler);

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

            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                var attribute = assembly.GetCustomAttribute<ServiceRemotingProviderAttribute>();
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return new FabricTransportServiceRemotingProviderAttribute();
        }
    }

    /// <summary>
    /// Determines the remoting stack for client
    /// </summary>
    public enum RemotingClient
    {
#if !DotNetCoreClr
        /// <summary>
        /// This is selected to create V1 Client. V1 is an old(soon to be deprecated) Remoting Stack.
        /// </summary>
        V1Client = 0,
#endif

        /// <summary>
        /// This is selected to create V2 Client. V2 is a new Remoting Stack.
        /// </summary>
        V2Client = 1,
    }

    /// <summary>
    /// Determines the remoting stack for server/listener when using remoting provider attribuite to determine the remoting client.
    /// </summary>
    public enum RemotingListener
    {
#if !DotNetCoreClr
        /// <summary>
        /// This is selected to create V1 Listener.V1 is an old (soon to be deprecated) Remoting Stack.
        /// </summary>
        V1Listener = 0,

        /// <summary>
        /// This is selected to create Listener which creates both V1 and V2 Listener to support both V1 and V2 Clients.
        /// This is useful in case of upgrade from V1 to V2 Listener.
        /// </summary>
        CompatListener = 1,
#endif

        /// <summary>
        /// This is selected to create V2 Listener.V2 is a new Remoting Stack.
        /// </summary>
        V2Listener = 2,
    }
}
