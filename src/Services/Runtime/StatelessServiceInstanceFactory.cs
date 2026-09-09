// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;

namespace Microsoft.ServiceFabric.Services.Runtime;

class StatelessServiceInstanceFactory : IStatelessServiceFactory, IDisposable
{
    readonly Func<StatelessServiceContext, StatelessService> serviceFactory;
    readonly RuntimeContext runtimeContext;

    public StatelessServiceInstanceFactory(
        RuntimeContext runtimeContext,
        Func<StatelessServiceContext, StatelessService> serviceFactory)
    {
        this.runtimeContext = runtimeContext ?? throw new ArgumentNullException(nameof(runtimeContext));
        this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
    }

    IStatelessServiceInstance IStatelessServiceFactory.CreateInstance(
        string serviceTypeName,
        Uri serviceName,
        byte[] initializationData,
        Guid partitionId,
        long instanceId)
    {
        var instanceContext = new StatelessServiceContext(
            runtimeContext.NodeContext,
            runtimeContext.CodePackageContext,
            serviceTypeName,
            serviceName,
            initializationData,
            partitionId,
            instanceId);

        StatelessService service = serviceFactory(instanceContext) ?? throw new InvalidOperationException($"{nameof(serviceFactory)} returned null");
        return new StatelessServiceInstanceAdapter(service.Context, service);
    }

    public void Dispose() => runtimeContext.Dispose();
}
