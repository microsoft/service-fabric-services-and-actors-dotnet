// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System.Fabric.Common;

    // This class modifies the value of the performance counter that represents the
    // time taken to execute a particular actor method.
    internal class ActorMethodExecTimeCounterWriter : FabricPerformanceCounterWriter
    {
        internal ActorMethodExecTimeCounterWriter(FabricPerformanceCounterSetInstance counterSetInstance)
            : base(
                counterSetInstance,
                ActorPerformanceCounters.ActorMethodExecTimeMillisecCounterName,
                ActorPerformanceCounters.ActorMethodExecTimeMillisecBaseCounterName)
        {
        }

        internal void UpdateCounterValue(ActorMethodDiagnosticData methodData)
        {
            if (methodData.MethodExecutionTime != null)
            {
                this.Counter.IncrementBy((long)methodData.MethodExecutionTime.Value.TotalMilliseconds);
                this.CounterBase.Increment();
            }
        }
    }
}
