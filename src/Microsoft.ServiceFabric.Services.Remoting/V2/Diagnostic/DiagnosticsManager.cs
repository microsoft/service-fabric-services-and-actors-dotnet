// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class DiagnosticsManager : IDiagnosticsManager
    {
        private readonly DiagnosticsEventManager diagnosticsEventManager;
        private ServiceRemotingPerformanceCounterProvider perfCounterProvider;

        internal DiagnosticsManager(Guid partitionId, long replicaOrInstanceId)
        {
            diagnosticsEventManager = new DiagnosticsEventManager();
            perfCounterProvider = new ServiceRemotingPerformanceCounterProvider(partitionId, replicaOrInstanceId);
            perfCounterProvider.RegisterWithDiagnosticsEventManager(diagnosticsEventManager);
        }

        public void FabricTransportRequestBegin()
        {
            var callbacks = this.diagnosticsEventManager.OnRequestStart;
            if (callbacks != null)
            {
                callbacks();
            }
        }

        public void FabricTransportRequestEnd(Stopwatch stopwatch)
        {
            var callbacks = this.diagnosticsEventManager.OnRequestEnd;
            if (callbacks != null)
            {
                callbacks(stopwatch);
            }
        }

        public void FabricTransportCreateTransportMessage(Stopwatch stopwatch)
        {
            var callbacks = this.diagnosticsEventManager.OnCreateTransportMessage;
            if (callbacks != null)
            {
                callbacks(stopwatch);
            }
        }

        public void FabricTransportCreateRemotingMessage(Stopwatch stopwatch)
        {
            var callbacks = this.diagnosticsEventManager.OnCreateRemotingMessage;
            if (callbacks != null)
            {
                callbacks(stopwatch);
            }
        }

        void IDisposable.Dispose()
        {
            if (perfCounterProvider != null)
            {
                perfCounterProvider.Dispose();
            }
        }
    }
}
