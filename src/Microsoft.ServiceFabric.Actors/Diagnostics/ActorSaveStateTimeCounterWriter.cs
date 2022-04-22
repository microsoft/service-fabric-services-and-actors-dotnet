// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    extern alias Microsoft_ServiceFabric_Internal;

    using System.Fabric.Common;
    using FabricPerformanceCounterSetInstance = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricPerformanceCounterSetInstance;
    using FabricPerformanceCounterWriter = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricPerformanceCounterWriter;

    // This class modifies the value of the performance counter that represents the
    // time taken to save actor state.
    internal class ActorSaveStateTimeCounterWriter : FabricPerformanceCounterWriter
    {
        internal ActorSaveStateTimeCounterWriter(FabricPerformanceCounterSetInstance counterSetInstance)
            : base(
                counterSetInstance,
                ActorPerformanceCounters.ActorSaveStateTimeMillisecCounterName,
                ActorPerformanceCounters.ActorSaveStateTimeMillisecBaseCounterName)
        {
        }

        internal void UpdateCounterValue(ActorStateDiagnosticData stateData)
        {
            if (stateData.OperationTime != null)
            {
                this.Counter.IncrementBy((long)stateData.OperationTime.Value.TotalMilliseconds);
                this.CounterBase.Increment();
            }
        }
    }
}
