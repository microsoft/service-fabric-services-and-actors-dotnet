// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Runtime
{
    using System;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The static class that provides methods to register reliable services with Service Fabric runtime.
    /// </summary>
    public static class ServiceRuntime
    {
        /// <summary>
        /// Registers a reliable stateless service with Service Fabric runtime.
        /// </summary>
        /// <param name="serviceTypeName">The service type name as provied in service manifest.</param>
        /// <param name="serviceFactory">A factory method to create stateless service objects.</param>
        /// <param name="timeout">The timeout for the register operation.</param>
        /// <para>The default timeout for this operation is taken from ServiceFactoryRegistrationTimeout in Hosting section of the cluster manifest. Default value for ServiceFactoryRegistrationTimeout is 120 seconds.</para>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous register operation.
        /// </returns>
        public static async Task RegisterServiceAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceFactory.ThrowIfNull("serviceFactory");
            serviceTypeName.ThrowIfNullOrWhiteSpace("serviceTypeName");

            var runtimeContext = await RuntimeContext.GetOrCreateAsync(timeout, cancellationToken);
            await runtimeContext.Runtime.RegisterStatelessServiceFactoryAsync(
                serviceTypeName,
                new StatelessServiceInstanceFactory(runtimeContext, serviceFactory),
                timeout,
                cancellationToken);
        }
        /// <summary>
        /// Registers a reliable stateful service with Service Fabric runtime.
        /// </summary>
        /// <param name="serviceTypeName">The service type name as provied in service manifest.</param>
        /// <param name="serviceFactory">A factory method to create stateful service objects.</param>
        /// <param name="timeout">The timeout for the register operation.</param>
        /// <para>The default timeout for this operation is taken from ServiceFactoryRegistrationTimeout in Hosting section of the cluster manifest. Default value for ServiceFactoryRegistrationTimeout is 120 seconds.</para>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous register operation.
        /// </returns>
        public static async Task RegisterServiceAsync(
            string serviceTypeName,
            Func<StatefulServiceContext, StatefulServiceBase> serviceFactory,
            TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceFactory.ThrowIfNull("serviceFactory");
            serviceTypeName.ThrowIfNullOrWhiteSpace("serviceTypeName");

            var runtimeContext = await RuntimeContext.GetOrCreateAsync(timeout, cancellationToken);
            await runtimeContext.Runtime.RegisterStatefulServiceFactoryAsync(
                serviceTypeName,
                new StatefulServiceReplicaFactory(runtimeContext, serviceFactory),
                timeout,
                cancellationToken);
        }
    }
}
