// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Threading;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal class DiagnosticsContext
    {
        long pendingActorMethodCalls;
        long lastReportedPendingActorMethodCalls;

        internal virtual void IncremenetPendingActorMethodCalls() { Interlocked.Increment(ref pendingActorMethodCalls); }

        internal virtual void DecremenetPendingActorMethodCalls() { Interlocked.Decrement(ref pendingActorMethodCalls); }

        internal virtual long UpdateLastReportedActorMethodCalls()
        {
            Interlocked.Decrement(ref pendingActorMethodCalls);

            var delta = pendingActorMethodCalls - lastReportedPendingActorMethodCalls;
            lastReportedPendingActorMethodCalls = pendingActorMethodCalls;

            return delta;
        }

        internal virtual long PendingActorMethodCalls => pendingActorMethodCalls;
        internal long LastReportedPendingActorMethodCalls => lastReportedPendingActorMethodCalls;
    }
}
