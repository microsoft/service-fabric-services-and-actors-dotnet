// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal sealed class DiagnosticsManager : IDiagnosticsManager
    {
        private readonly DiagnosticsEventManager diagnosticsEventManager;
        private readonly PerformanceCounterProvider perfCounterProvider;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        // to allow following references easily in the dumps.
        private readonly EventSourceProvider eventSourceProvider;

        DiagnosticsEventManager IDiagnosticsManager.DiagnosticsEventManager
        {
            get
            {
                return this.diagnosticsEventManager;
            } 
        }

        internal DiagnosticsManager(ActorService actorService)
        {
            this.diagnosticsEventManager = new DiagnosticsEventManager(actorService.MethodFriendlyNameBuilder);
            this.perfCounterProvider = new PerformanceCounterProvider(actorService.Context.PartitionId, actorService.ActorTypeInformation);
            this.perfCounterProvider.RegisterWithDiagnosticsEventManager(this.diagnosticsEventManager);
            this.eventSourceProvider = new EventSourceProvider(actorService.Context, actorService.ActorTypeInformation);
            this.eventSourceProvider.RegisterWithDiagnosticsEventManager(this.diagnosticsEventManager);
        }

        void IDisposable.Dispose()
        {
            if (null != this.perfCounterProvider)
            {
                this.perfCounterProvider.Dispose();
            }
        }
    }
}
