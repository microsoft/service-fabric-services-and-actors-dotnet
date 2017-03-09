// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Runtime
{
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    /// This class adds extensions methods to create <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.IServiceRemotingListener"/>
    /// for remoting methods of the service interfaces that are derived from 
    /// <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.
    /// </summary>
    public static class ServiceRemotingExtensions
    {
        /// <summary>
        /// An extension method that creates an <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.IServiceRemotingListener"/>
        /// for a stateful service implementation.
        /// </summary>
        /// <typeparam name="TStatefulService">Type constraint on the service implementation. The service implementation must
        /// derive from <see cref="Microsoft.ServiceFabric.Services.Runtime.StatefulServiceBase"/> and implement one or more
        /// interfaces that derive from <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.</typeparam>
        /// <param name="serviceImplementation">A stateful service implementation.</param>
        /// <param name="serviceContext">The context under which the service is operating.</param>
        /// <returns>A <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.IServiceRemotingListener"/> communication
        /// listener that remotes the interfaces deriving from <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.</returns>
        public static IServiceRemotingListener CreateServiceRemotingListener<TStatefulService>(
            this TStatefulService serviceImplementation,
            StatefulServiceContext serviceContext) where TStatefulService : StatefulServiceBase, IService
        {
            return CreateServiceRemotingListener(serviceContext, serviceImplementation);
        }

        /// <summary>
        /// An extension method that creates an <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.IServiceRemotingListener"/>
        /// for a stateless service implementation.
        /// </summary>
        /// <typeparam name="TStatelessService">Type constraint on the service implementation. The service implementation must
        /// derive from <see cref="Microsoft.ServiceFabric.Services.Runtime.StatelessService"/> and implement one or more
        /// interfaces that derive from <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.</typeparam>
        /// <param name="serviceImplementation">A stateless service implementation.</param>
        /// <param name="serviceContext">The context under which the service is operating.</param>
        /// <returns>A <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.IServiceRemotingListener"/> communication
        /// listener that remotes the interfaces deriving from <see cref="Microsoft.ServiceFabric.Services.Remoting.IService"/> interface.</returns>
        public static IServiceRemotingListener CreateServiceRemotingListener<TStatelessService>(
            this TStatelessService serviceImplementation,
            StatelessServiceContext serviceContext) where TStatelessService : StatelessService, IService
        {
            return CreateServiceRemotingListener(serviceContext, serviceImplementation);
        }

        private static IServiceRemotingListener CreateServiceRemotingListener(
            ServiceContext serviceContext,
            object serviceImplementation)
        {
            var serviceTypeInformation = ServiceTypeInformation.Get(serviceImplementation.GetType());
            var interfaceTypes = serviceTypeInformation.InterfaceTypes;

            var provider = ServiceRemotingProviderAttribute.GetProvider(interfaceTypes);
            return provider.CreateServiceRemotingListener(serviceContext, (IService)serviceImplementation);
        }
    }
}