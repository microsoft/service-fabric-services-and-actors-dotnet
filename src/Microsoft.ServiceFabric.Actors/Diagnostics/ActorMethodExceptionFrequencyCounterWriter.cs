// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    extern alias Microsoft_ServiceFabric_Internal;

    using FabricBaselessPerformanceCounterWriter = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricBaselessPerformanceCounterWriter;
    using FabricPerformanceCounterSetInstance = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricPerformanceCounterSetInstance;

    // This class modifies the value of the performance counter that represents the
    // frequency at which a particular actor method throws exceptions.
    internal class ActorMethodExceptionFrequencyCounterWriter : FabricBaselessPerformanceCounterWriter
    {
        internal ActorMethodExceptionFrequencyCounterWriter(FabricPerformanceCounterSetInstance counterSetInstance)
            : base(
                counterSetInstance,
                ActorPerformanceCounters.ActorMethodExceptionsPerSecCounterName)
        {
        }

        internal void UpdateCounterValue(ActorMethodDiagnosticData methodData)
        {
            if (methodData.Exception != null)
            {
                this.Counter.Increment();
            }
        }
    }
}
