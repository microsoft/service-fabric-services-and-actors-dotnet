// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
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
            if (null != methodData.Exception)
            {
                this.Counter.Increment();
            }
        }
    }

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
