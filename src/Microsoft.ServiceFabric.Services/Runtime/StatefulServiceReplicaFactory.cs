// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Runtime
{
    using System;
    using System.Fabric;

    internal class StatefulServiceReplicaFactory : IStatefulServiceFactory, IDisposable
    {
        private readonly Func<StatefulServiceContext, StatefulServiceBase> serviceFactory;
        private readonly RuntimeContext runtimeContext;

        public StatefulServiceReplicaFactory(
            RuntimeContext runtimeContext,
            Func<StatefulServiceContext, StatefulServiceBase> serviceFactory)
        {
            this.serviceFactory = serviceFactory;
            this.runtimeContext = runtimeContext;
        }

        IStatefulServiceReplica IStatefulServiceFactory.CreateReplica(
            string serviceTypeName,
            Uri serviceName,
            byte[] initializationData,
            Guid partitionId,
            long replicaId)
        {
            var serviceContext = new StatefulServiceContext(
                this.runtimeContext.NodeContext,
                this.runtimeContext.CodePackageContext,
                serviceTypeName,
                serviceName,
                initializationData,
                partitionId,
                replicaId);

            var service = this.serviceFactory(serviceContext);
            return new StatefulServiceReplicaAdapter(service.Context, service);
        }

        public void Dispose()
        {
            runtimeContext?.Dispose();
        }
    }
}