// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Runtime
{
    using System;
    using System.Fabric;

    internal class StatelessServiceInstanceFactory : IStatelessServiceFactory, IDisposable
    {
        private readonly Func<StatelessServiceContext, StatelessService> serviceFactory;
        private readonly RuntimeContext runtimeContext;

        public StatelessServiceInstanceFactory(
            RuntimeContext runtimeContext,
            Func<StatelessServiceContext, StatelessService> serviceFactory)
        {
            this.runtimeContext = runtimeContext;
            this.serviceFactory = serviceFactory;
        }

        IStatelessServiceInstance IStatelessServiceFactory.CreateInstance(
            string serviceTypeName,
            Uri serviceName,
            byte[] initializationData,
            Guid partitionId,
            long instanceId)
        {
            var instanceContext = new StatelessServiceContext(
                this.runtimeContext.NodeContext,
                this.runtimeContext.CodePackageContext,
                serviceTypeName,
                serviceName,
                initializationData,
                partitionId,
                instanceId);

            var service = this.serviceFactory(instanceContext);
            return new StatelessServiceInstanceAdapter(service.Context, service);
        }

        public void Dispose()
        {
            runtimeContext?.Dispose();
        }
    }
}