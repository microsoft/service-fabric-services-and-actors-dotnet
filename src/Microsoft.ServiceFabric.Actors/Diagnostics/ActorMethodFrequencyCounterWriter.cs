// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System.Fabric.Common;

    // This class modifies the value of the performance counter that represents the
    // frequency at which a particular actor method is invoked.
    internal class ActorMethodFrequencyCounterWriter : FabricBaselessPerformanceCounterWriter
    {
        internal ActorMethodFrequencyCounterWriter(FabricPerformanceCounterSetInstance counterSetInstance)
            : base(
                counterSetInstance,
                ActorPerformanceCounters.ActorMethodInvocationsPerSecCounterName)
        {
        }

        internal void UpdateCounterValue()
        {
            this.Counter.Increment();
        }
    }
}
