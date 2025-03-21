// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.Runtime
{
    /// <summary>
    /// This class adds extensions methods to create <see cref="IServiceRemotingListener"/>
    /// for remoting methods of the service interfaces that are derived from
    /// <see cref="IService"/> interface.
    /// </summary>
    public static class ServiceRemotingExtensions
    {
        /// <summary>
        ///  An extension method that creates an <see cref="IServiceRemotingListener"/>
        /// for a stateful service implementation.
        /// </summary>
        /// <typeparam name="TStatefulService">Type constraint on the service implementation. The service implementation must
        /// derive from <see cref="StatefulServiceBase"/> and implement one or more
        /// interfaces that derive from <see cref="IService"/> interface.</typeparam>
        /// <param name="serviceImplementation">A stateful service implementation.</param>
        /// <returns>A <see cref="IServiceRemotingListener"/> communication
        /// listener that remotes the interfaces deriving from <see cref="IService"/> interface.</returns>
        public static IEnumerable<ServiceReplicaListener> CreateServiceRemotingReplicaListeners<TStatefulService>(
            this TStatefulService serviceImplementation)
            where TStatefulService : StatefulServiceBase, IService
        {
            var serviceTypeInformation = ServiceTypeInformation.Get(serviceImplementation.GetType());
            var interfaceTypes = serviceTypeInformation.InterfaceTypes;
            var impl = (IService)serviceImplementation;
            var provider = ServiceRemotingProviderAttribute.GetProvider(interfaceTypes);
            var serviceReplicaListeners = new List<ServiceReplicaListener>();

            var listeners = provider.CreateServiceRemotingListeners();
            foreach (var kvp in listeners)
            {
                serviceReplicaListeners.Add(new ServiceReplicaListener(
                    t => kvp.Value(serviceImplementation.Context, impl),
                    kvp.Key));
            }

            return serviceReplicaListeners;
        }

        /// <summary>
        /// An extension method that creates an <see cref="IServiceRemotingListener"/>
        /// for a stateless service implementation.
        /// </summary>
        /// <typeparam name="TStatelessService">Type constraint on the service implementation. The service implementation must
        /// derive from <see cref="System.Fabric.Query.StatelessService"/> and implement one or more
        /// interfaces that derive from <see cref="IService"/> interface.</typeparam>
        /// <param name="serviceImplementation">A stateless service implementation.</param>
        /// <returns>A <see cref="IServiceRemotingListener"/> communication
        /// listener that remotes the interfaces deriving from <see cref="IService"/> interface.</returns>
        public static IEnumerable<ServiceInstanceListener> CreateServiceRemotingInstanceListeners<TStatelessService>(
            this TStatelessService serviceImplementation)
            where TStatelessService : StatelessService, IService
        {
            var serviceTypeInformation = ServiceTypeInformation.Get(serviceImplementation.GetType());
            var interfaceTypes = serviceTypeInformation.InterfaceTypes;
            var impl = (IService)serviceImplementation;
            var provider = ServiceRemotingProviderAttribute.GetProvider(interfaceTypes);
            var serviceInstanceListeners = new List<ServiceInstanceListener>();

            var listeners = provider.CreateServiceRemotingListeners();
            foreach (var kvp in listeners)
            {
                serviceInstanceListeners.Add(new ServiceInstanceListener(
                    t => kvp.Value(serviceImplementation.Context, impl),
                    kvp.Key));
            }

            return serviceInstanceListeners;
        }
    }
}
