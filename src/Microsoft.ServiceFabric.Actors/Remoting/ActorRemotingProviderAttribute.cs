// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

namespace Microsoft.ServiceFabric.Actors.Remoting
{
    /// <summary>
    /// This is a base type for attribute that sets the default remoting provider to use for
    /// remoting the actor interfaces defined or used in the assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     On service side, implementation of this attribute is looked up by
    ///     <see cref="ActorService"/> to create default
    ///     <see cref="IServiceRemotingListener"/> for it.
    ///     </para>
    ///     <para>
    ///     On client side, implementation of this attribute is looked up by
    ///     <see cref="ActorProxyFactory"/> constructor to create a default
    ///     IServiceRemotingClientFactory when it is not specified.
    ///     </para>
    ///     <para>
    ///     Note that on client side when actor proxy is created using the static <see cref="ActorProxy"/>
    ///     class, it uses a default <see cref="ActorProxyFactory"/> once and hence the provider lookup
    ///     happens only for the first time in an assembly, after which the same provider is used.
    ///     </para>
    ///     <para>
    ///     This attribute is looked up in the following order:
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
    public abstract class ActorRemotingProviderAttribute : Attribute
    {
        private static Assembly entryAssembly = Assembly.GetEntryAssembly();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActorRemotingProviderAttribute"/> class.
        /// </summary>
        protected ActorRemotingProviderAttribute()
        {
            this.RemotingListenerVersion = RemotingListenerVersion.V2_1;
            this.RemotingClientVersion = RemotingClientVersion.V2_1;
        }

        /// <summary>
        /// Gets or sets RemotingClientVersion to determine where  V1 or V2 remoting Client is used.
        /// </summary>
        public RemotingClientVersion RemotingClientVersion { get; set; }

        /// <summary>
        ///  Gets or sets RemotingListenerVersion  to determine where listener is in V1, V2 .
        /// </summary>
        public RemotingListenerVersion RemotingListenerVersion { get; set; }

        /// <summary>
        /// Creates a V2 service remoting listener for remoting the service interface.
        /// </summary>
        /// <returns>An <see cref="IServiceRemotingListener"/> for the specified service.</returns>
        public abstract Dictionary<string, Func<ActorService, IServiceRemotingListener>> CreateServiceRemotingListeners();

        /// <summary>
        /// Creates a service remoting client factory that can be used by the
        /// <see cref="ServiceProxyFactory"/> to create a proxy for the remoted interface of the service.
        /// </summary>
        /// <param name="callbackMessageHandler">Client implementation where the callbacks should be dispatched.</param>
        /// <returns>An <see cref="IServiceRemotingClientFactory"/>.</returns>
        public abstract IServiceRemotingClientFactory CreateServiceRemotingClientFactory(IServiceRemotingCallbackMessageHandler callbackMessageHandler);

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

            if (entryAssembly != null)
            {
                var attribute = entryAssembly.GetCustomAttribute<ActorRemotingProviderAttribute>();
                if (attribute != null)
                {
                    return attribute;
                }
            }

            return new FabricTransportActorRemotingProviderAttribute();
        }
    }
}
