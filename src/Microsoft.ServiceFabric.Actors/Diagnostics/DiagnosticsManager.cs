// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal sealed class DiagnosticsManager : IDiagnosticsManager
    {
        private readonly DiagnosticsEventManager diagnosticsEventManager;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        // to allow following references easily in the dumps
        private PerformanceCounterProviderV2 perfCounterProviderV2;
        private EventSourceProvider eventSourceProviderV2;

        internal DiagnosticsManager(ActorService actorService)
        {
            this.diagnosticsEventManager = new DiagnosticsEventManager(actorService.MethodFriendlyNameBuilder);

            // V2 providers are compatible with V1 provider
            this.perfCounterProviderV2 = new PerformanceCounterProviderV2(actorService.Context.PartitionId, actorService.ActorTypeInformation);
            this.eventSourceProviderV2 = new EventSourceProviderV2(actorService.Context, actorService.ActorTypeInformation);
            this.perfCounterProviderV2.RegisterWithDiagnosticsEventManager(this.diagnosticsEventManager);
            this.eventSourceProviderV2.RegisterWithDiagnosticsEventManager(this.diagnosticsEventManager);
        }

        DiagnosticsEventManager IDiagnosticsManager.DiagnosticsEventManager
        {
            get
            {
                return this.diagnosticsEventManager;
            }
        }

        void IDisposable.Dispose()
        {
            if (this.perfCounterProviderV2 != null)
            {
                this.perfCounterProviderV2.Dispose();
            }
        }
    }
}
