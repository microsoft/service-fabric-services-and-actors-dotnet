// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    /// This class adds extensions methods to create <see cref="IServiceRemotingListener"/>
    /// for remoting methods of the service interfaces that are derived from 
    /// <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.
    /// </summary>
    public static class ServiceRemotingExtensions
    {
#if !DotNetCoreClr
        /// <summary>
        /// An extension method that creates an <see cref="IServiceRemotingListener"/>
        /// for a stateful service implementation. This is deprecated implementation. Use this Api CreateServiceRemotingReplicaListeners instead
        /// </summary>
        /// <typeparam name="TStatefulService">Type constraint on the service implementation. The service implementation must
        /// derive from <see cref="Microsoft.ServiceFabric.Services.Runtime.StatefulServiceBase"/> and implement one or more
        /// interfaces that derive from <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.</typeparam>
        /// <param name="serviceImplementation">A stateful service implementation.</param>
        /// <param name="serviceContext">The context under which the service is operating.</param>
        /// <returns>A <see cref="IServiceRemotingListener"/> communication
        /// listener that remotes the interfaces deriving from <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.</returns>
        public static IServiceRemotingListener CreateServiceRemotingListener<TStatefulService>(
            this TStatefulService serviceImplementation,
            StatefulServiceContext serviceContext) where TStatefulService : StatefulServiceBase, IService
        {
            return CreateServiceRemotingListener(serviceContext, serviceImplementation);
        }

        /// <summary>
        /// An extension method that creates an <see cref="IServiceRemotingListener"/>
        /// for a stateless service implementation. This is deprecated implementation. Use CreateServiceRemotingInstanceListeners Api instead.
        /// </summary>
        /// <typeparam name="TStatelessService">Type constraint on the service implementation. The service implementation must
        /// derive from <see cref="System.Fabric.Query.StatelessService"/> and implement one or more
        /// interfaces that derive from <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.</typeparam>
        /// <param name="serviceImplementation">A stateless service implementation.</param>
        /// <param name="serviceContext">The context under which the service is operating.</param>
        /// <returns>A <see cref="IServiceRemotingListener"/> communication
        /// listener that remotes the interfaces deriving from <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.</returns>
        public static IServiceRemotingListener CreateServiceRemotingListener<TStatelessService>(
            this TStatelessService serviceImplementation,
            StatelessServiceContext serviceContext) where TStatelessService : StatelessService, IService
        {
            return CreateServiceRemotingListener(serviceContext, serviceImplementation);
        }

#endif
        /// <summary>
        /// An extension method that creates an <see cref="IServiceRemotingListener"/>
        /// for a stateful service implementation.
        /// <typeparam name="TStatefulService">Type constraint on the service implementation. The service implementation must
        /// derive from <see cref="Microsoft.ServiceFabric.Services.Runtime.StatefulServiceBase"/> and implement one or more
        /// interfaces that derive from <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.</typeparam>
        ///</summary>
        /// <param name="serviceImplementation">A stateful service implementation.</param>
        /// <returns>A <see cref="IServiceRemotingListener"/> communication
        /// listener that remotes the interfaces deriving from <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.</returns>
        public static IEnumerable<ServiceReplicaListener> CreateServiceRemotingReplicaListeners<TStatefulService>(
            this TStatefulService serviceImplementation)
            where TStatefulService : StatefulServiceBase, IService

        {
            var serviceTypeInformation = ServiceTypeInformation.Get(serviceImplementation.GetType());
            var interfaceTypes = serviceTypeInformation.InterfaceTypes;
            var impl = (IService)serviceImplementation;
            var provider = ServiceRemotingProviderAttribute.GetProvider(interfaceTypes);
#if !DotNetCoreClr
            if (provider.RemotingListenerVersion.Equals(RemotingListenerVersion.V2))
            {
                return new[]
                {
                    new ServiceReplicaListener(
                        (t) =>
                        {
                            return provider.CreateServiceRemotingListenerV2(serviceImplementation.Context, impl);
                        }, ServiceRemotingProviderAttribute.DefaultV2listenerName
                    )
                };
            }

            if (provider.RemotingListenerVersion.Equals(RemotingListenerVersion.Compat))
            {
                return new[]
                {
                    new ServiceReplicaListener((t) =>
                        {
                            return provider.CreateServiceRemotingListener(serviceImplementation.Context, impl);
                        }
                    ),
                    new ServiceReplicaListener(
                        (t) =>
                        {
                            return provider.CreateServiceRemotingListenerV2(serviceImplementation.Context, impl);
                        }, ServiceRemotingProviderAttribute.DefaultV2listenerName
                    )
                };
            }
            else
            {
                return new[]
                {
                    new ServiceReplicaListener((t) =>
                        {
                            return provider.CreateServiceRemotingListener(serviceImplementation.Context, impl);
                        }
                    )
                };
            }

#else
            return new[]
               {
                    new ServiceReplicaListener(
                        (t) =>
                        {
                            return provider.CreateServiceRemotingListenerV2(serviceImplementation.Context, impl);
                        }, ServiceRemotingProviderAttribute.DefaultV2listenerName
                    )
                };
#endif
        }

        /// <summary>
        /// An extension method that creates an <see cref="IServiceRemotingListener"/>
        /// for a stateless service implementation.
        /// </summary>
        /// <typeparam name="TStatelessService">Type constraint on the service implementation. The service implementation must
        /// derive from <see cref="System.Fabric.Query.StatelessService"/> and implement one or more
        /// interfaces that derive from <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.</typeparam>
        /// <param name="serviceImplementation">A stateless service implementation.</param>
        /// <returns>A <see cref="IServiceRemotingListener"/> communication
        /// listener that remotes the interfaces deriving from <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.</returns>
        public static IEnumerable<ServiceInstanceListener> CreateServiceRemotingInstanceListeners<TStatelessService>(
            this TStatelessService serviceImplementation)
            where TStatelessService : StatelessService, IService

        {
            var serviceTypeInformation = ServiceTypeInformation.Get(serviceImplementation.GetType());
            var interfaceTypes = serviceTypeInformation.InterfaceTypes;
            var impl = (IService)serviceImplementation;
            var provider = ServiceRemotingProviderAttribute.GetProvider(interfaceTypes);
#if !DotNetCoreClr
            if (provider.RemotingListenerVersion.Equals(RemotingListenerVersion.V2))
            {
                return new[]
                {
                    new ServiceInstanceListener(
                        (t) =>
                        {
                            return provider.CreateServiceRemotingListenerV2(serviceImplementation.Context, impl);
                        }, ServiceRemotingProviderAttribute.DefaultV2listenerName
                    ),
                };
            }
            if (provider.RemotingListenerVersion.Equals(RemotingListenerVersion.Compat))
            {
                return new[]
                {
                    new ServiceInstanceListener((t) =>
                        {
                            return provider.CreateServiceRemotingListener(serviceImplementation.Context, impl);
                        }
                    ),
                    new ServiceInstanceListener(
                        (t) =>
                        {
                            return provider.CreateServiceRemotingListenerV2(serviceImplementation.Context, impl);
                        }, ServiceRemotingProviderAttribute.DefaultV2listenerName
                    )
                };
            }
            else
            {
                return new[]
                {
                    new ServiceInstanceListener((t) =>
                        {
                            return provider.CreateServiceRemotingListener(serviceImplementation.Context, impl);
                        }
                    )
                };
            }
#else
            return new[]
                {
                    new ServiceInstanceListener(
                        (t) =>
                        {
                            return provider.CreateServiceRemotingListenerV2(serviceImplementation.Context, impl);
                        }, ServiceRemotingProviderAttribute.DefaultV2listenerName
                    ),
                };
#endif
        }
#if !DotNetCoreClr
        private static IServiceRemotingListener CreateServiceRemotingListener(
            ServiceContext serviceContext,
            object serviceImplementation)
        {
            var serviceTypeInformation = ServiceTypeInformation.Get(serviceImplementation.GetType());
            var interfaceTypes = serviceTypeInformation.InterfaceTypes;

            var provider = ServiceRemotingProviderAttribute.GetProvider(interfaceTypes);
            if (provider.RemotingListenerVersion.Equals(RemotingListenerVersion.V2))
            {
                throw new NotSupportedException(
                    "This extension method doesnt support V2Listener or CompatListener. Use CreateServiceRemotingReplicaListeners for using V2Stack ");
            }
            return provider.CreateServiceRemotingListener(serviceContext, (IService)serviceImplementation);
        }
#endif
    }
}
