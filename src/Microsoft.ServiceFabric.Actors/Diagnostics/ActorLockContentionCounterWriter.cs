// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System.Fabric.Common;

    // This class modifies the value of the performance counter that represents the
    // number of pending actor calls that are waiting for the actor lock.
    internal class ActorLockContentionCounterWriter : FabricBaselessPerformanceCounterWriter
    {
        internal ActorLockContentionCounterWriter(FabricPerformanceCounterSetInstance counterSetInstance)
            : base(
                counterSetInstance,
                ActorPerformanceCounters.ActorCallsWaitingForLockCounterName)
        {
        }

        internal void UpdateCounterValue(PendingActorMethodDiagnosticData pendingMethodData)
        {
            var delta = pendingMethodData.PendingActorMethodCallsDelta;
            if (delta != 0)
            {
                this.Counter.IncrementBy(delta);
            }
        }
    }
}
