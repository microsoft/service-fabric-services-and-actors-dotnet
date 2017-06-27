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
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;

    /// <summary>
    /// Represents a base type for attribute that sets the default service remoting provider to use for 
    /// remoting the service interfaces defined and used in the assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     On service side, implementation of this attribute is looked up by
    ///     <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.ServiceRemotingExtensions.CreateServiceRemotingListener{TStatefulService}(TStatefulService, StatefulServiceContext)"/> and
    ///     <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.ServiceRemotingExtensions.CreateServiceRemotingListener{TStatelessService}(TStatelessService, StatelessServiceContext)"/> 
    ///     methods on the runtime to create a default <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.IServiceRemotingListener"/> for the stateful and stateless services. 
    ///     </para>
    ///     <para>
    ///     On client side, implementation of this attribute is looked up by 
    ///     <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.ServiceProxyFactory"/> constructor to create a default
    ///     <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.IServiceRemotingClientFactory"/> when it is not specified.
    ///     </para>
    ///     <para>
    ///     Note that on client side
    ///     <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.ServiceProxy.Create{TServiceInterface}(Uri, Services.Client.ServicePartitionKey, Communication.Client.TargetReplicaSelector, string)"/> 
    ///     method create a default <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.ServiceProxyFactory"/> once and hence the provider lookup happens
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
        /// Creates a service remoting listener for remoting the service interface.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="serviceImplementation">The service implementation object.</param>
        /// <returns>An <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.IServiceRemotingListener"/> for the specified service.</returns>
        public abstract IServiceRemotingListener CreateServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation);

        /// <summary>
        /// Creates a service remoting client factory that can be used by the 
        /// <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.ServiceProxyFactory"/> to create a proxy for the remoted interface of the service.
        /// </summary>
        /// <param name="callbackClient">The client implementation where the callbacks should be dispatched.</param>
        /// <returns>An <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.IServiceRemotingClientFactory"/>.</returns>
        public abstract IServiceRemotingClientFactory CreateServiceRemotingClientFactory(
            IServiceRemotingCallbackClient callbackClient);

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
            return new FabricTransport.FabricTransportServiceRemotingProviderAttribute();
        }
    }
}
