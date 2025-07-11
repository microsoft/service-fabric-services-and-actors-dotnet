// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal sealed class DiagnosticsManager : IDiagnosticsManager
    {
        private readonly DiagnosticsEventManager diagnosticsEventManager;
        private ServiceRemotingPerformanceCounterProvider perfCounterProvider;

        internal DiagnosticsManager(ServiceContext serviceContext)
        {
            diagnosticsEventManager = new DiagnosticsEventManager();
            perfCounterProvider = new ServiceRemotingPerformanceCounterProvider(serviceContext.PartitionId, serviceContext.ReplicaOrInstanceId);
            perfCounterProvider.RegisterWithDiagnosticsEventManager(diagnosticsEventManager);
        }

        DiagnosticsEventManager IDiagnosticsManager.DiagnosticsEventManager => diagnosticsEventManager;

        void IDisposable.Dispose()
        {
            if (perfCounterProvider != null)
            {
                perfCounterProvider.Dispose();
            }
        }
    }
}
