// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Remoting
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;

    /// <summary>
    /// This is a base type for attribute that sets the default remoting provider to use for 
    /// remoting the actor interfaces defined or used in the assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     On service side, implementation of this attribute is looked up by
    ///     <see cref="Microsoft.ServiceFabric.Actors.Runtime.ActorService"/> to create default 
    ///     <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.IServiceRemotingListener"/> for it. 
    ///     </para>
    ///     <para>
    ///     On client side, implementation of this attribute is looked up by 
    ///     <see cref="Microsoft.ServiceFabric.Actors.Client.ActorProxyFactory"/> constructor to create a default
    ///     <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.IServiceRemotingClientFactory"/> when it is not specified.
    ///     </para>
    ///     <para>
    ///     Note that on client side when actor proxy is created using the static <see cref="Microsoft.ServiceFabric.Actors.Client.ActorProxy"/>
    ///     class, it uses a default <see cref="Microsoft.ServiceFabric.Actors.Client.ActorProxyFactory"/> once and hence the provider lookup 
    ///     happens only for the first time in an assembly, after which the same provider is used.
    ///     </para>
    ///     <para>
    ///     This attribute is looked up in the following order:
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
    public abstract class ActorRemotingProviderAttribute : Attribute
    {
        /// <summary>
        ///     Instantiates a new <see cref="ActorRemotingProviderAttribute"/>.
        /// </summary>
        protected ActorRemotingProviderAttribute()
        {
        }

        /// <summary>
        ///     Creates a service remoting listener for remoting the actor interfaces.
        /// </summary>
        /// <param name="actorService">
        ///     The implementation of the actor service that hosts the actors whose interfaces
        ///     needs to be remoted.
        /// </param>
        /// <returns>
        ///     An <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.IServiceRemotingListener"/> 
        ///     for the specified actor service.
        /// </returns>
        public abstract IServiceRemotingListener CreateServiceRemotingListener(
            ActorService actorService);

        /// <summary>
        ///     Creates a service remoting client factory to connected to the remoted actor interfaces.
        /// </summary>
        /// <param name="callbackClient">
        ///     Client implementation where the callbacks should be dispatched.
        /// </param>
        /// <returns>
        ///     An <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.IServiceRemotingClientFactory"/>
        ///     that can be used with <see cref="Microsoft.ServiceFabric.Actors.Client.ActorProxyFactory"/> to 
        ///     generate actor proxy to talk to the actor over remoted actor interface.
        /// </returns>
        public abstract IServiceRemotingClientFactory CreateServiceRemotingClientFactory(
            IServiceRemotingCallbackClient callbackClient);

        internal static ActorRemotingProviderAttribute GetProvider(IEnumerable<Type> types = null)
        {
            if (types != null)
            {
                foreach (var t in types)
                {
                    var attribute = t.GetTypeInfo().Assembly.GetCustomAttribute<ActorRemotingProviderAttribute>();
                    if (attribute != null)
                    {
                        return attribute;
                    }
                }
            }
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                var attribute = assembly.GetCustomAttribute<ActorRemotingProviderAttribute>();
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return new FabricTransport.FabricTransportActorRemotingProviderAttribute();
        }
    }
}