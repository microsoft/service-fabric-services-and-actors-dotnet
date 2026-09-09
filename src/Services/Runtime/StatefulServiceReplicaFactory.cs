// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;

namespace Microsoft.ServiceFabric.Services.Runtime;

class StatefulServiceReplicaFactory : IStatefulServiceFactory, IDisposable
{
    readonly Func<StatefulServiceContext, StatefulServiceBase> serviceFactory;
    readonly RuntimeContext runtimeContext;

    public StatefulServiceReplicaFactory(
        RuntimeContext runtimeContext,
        Func<StatefulServiceContext, StatefulServiceBase> serviceFactory)
    {
        this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        this.runtimeContext = runtimeContext ?? throw new ArgumentNullException(nameof(runtimeContext));
    }

    IStatefulServiceReplica IStatefulServiceFactory.CreateReplica(
        string serviceTypeName,
        Uri serviceName,
        byte[] initializationData,
        Guid partitionId,
        long replicaId)
    {
        var serviceContext = new StatefulServiceContext(
            runtimeContext.NodeContext,
            runtimeContext.CodePackageContext,
            serviceTypeName,
            serviceName,
            initializationData,
            partitionId,
            replicaId);

        StatefulServiceBase service = serviceFactory(serviceContext) ?? throw new InvalidOperationException($"{nameof(serviceFactory)} returned null");
        return new StatefulServiceReplicaAdapter(service.Context, service);
    }

    public void Dispose() => runtimeContext.Dispose();
}
