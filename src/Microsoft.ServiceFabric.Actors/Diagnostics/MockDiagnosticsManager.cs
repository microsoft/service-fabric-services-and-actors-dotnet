// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal sealed class MockDiagnosticsManager : IDiagnosticsManager
    {
        private readonly DiagnosticsEventManager diagnosticsEventManager;

        DiagnosticsEventManager IDiagnosticsManager.DiagnosticsEventManager
        {
            get
            {
                return this.diagnosticsEventManager;
            }
        }

        internal MockDiagnosticsManager(ActorService actorService)
        {
            this.diagnosticsEventManager = new DiagnosticsEventManager(actorService.MethodFriendlyNameBuilder);
        }

        void IDisposable.Dispose()
        {
            // Nothing to clean.
        }
    }
}
