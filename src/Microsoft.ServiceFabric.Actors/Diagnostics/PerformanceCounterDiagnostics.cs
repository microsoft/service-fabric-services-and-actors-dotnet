// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Microsoft.ServiceFabric.Diagnostics;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    sealed class PerformanceCounterDiagnostics : IDiagnostics
    {
        readonly PerformanceCounterProviderV2 performanceCounterProvider;
        readonly IClock clock;

        internal PerformanceCounterDiagnostics(PerformanceCounterProviderV2 performanceCounterProvider, IClock clock)
        {
            this.performanceCounterProvider = performanceCounterProvider ?? throw new ArgumentNullException(nameof(performanceCounterProvider));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public void AcquireActorLockFinish(PendingActorMethodDiagnosticData diagnosticData, DateTime startTime)
        {
            if (performanceCounterProvider.actorLockContentionCounterWriter != null)
            {
                performanceCounterProvider.actorLockContentionCounterWriter.UpdateCounterValue(diagnosticData);
            }
            if (performanceCounterProvider.actorLockAcquireWaitTimeCounterWriter != null)
            {
                performanceCounterProvider.actorLockAcquireWaitTimeCounterWriter.UpdateCounterValue(LongMillisecondsSinceStart(startTime));
            }
        }

        public void ActorActivated(ActorId actorId)
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorChangeRole(ReplicaRole currentRole, ReplicaRole newRole)
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorDeactivated(ActorId actorId)
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorMethodFinish(ActorMethodDiagnosticData actorMethodDiagnosticData, DateTime startTime)
        {
            var counterWriters = performanceCounterProvider.GetMethodSpecificCounterWriters(actorMethodDiagnosticData.InterfaceMethodKey, actorMethodDiagnosticData.RemotingListener);

            actorMethodDiagnosticData.MethodExecutionTime = TimeSpan.FromMilliseconds(LongMillisecondsSinceStart(startTime));

            if (counterWriters.ActorMethodFrequencyCounterWriter != null)
            {
                counterWriters.ActorMethodFrequencyCounterWriter.UpdateCounterValue();
            }

            if (counterWriters.ActorMethodExceptionFrequencyCounterWriter != null)
            {
                counterWriters.ActorMethodExceptionFrequencyCounterWriter.UpdateCounterValue(actorMethodDiagnosticData);
            }

            if (counterWriters.ActorMethodExecTimeCounterWriter != null)
            {
                counterWriters.ActorMethodExecTimeCounterWriter.UpdateCounterValue(actorMethodDiagnosticData);
            }
        }

        public void ActorMethodStart(ActorId actorId, long interfaceMethodKey)
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorOnActivateAsyncFinish(DateTime startTime)
        {
            if (performanceCounterProvider.actorOnActivateAsyncTimeCounterWriter != null)
            {
                performanceCounterProvider.actorOnActivateAsyncTimeCounterWriter.UpdateCounterValue(LongMillisecondsSinceStart(startTime));
            }
        }

        public void ActorOnActivateAsyncStart()
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorRequestProcessingFinish(DateTime startTime)
        {
            if (performanceCounterProvider.actorOutstandingRequestsCounterWriter != null)
            {
                performanceCounterProvider.actorOutstandingRequestsCounterWriter.UpdateCounterValue(-1);
            }
            if (performanceCounterProvider.actorRequestProcessingTimeCounterWriter != null)
            {
                performanceCounterProvider.actorRequestProcessingTimeCounterWriter.UpdateCounterValue(LongMillisecondsSinceStart(startTime));
            }
        }

        public void ActorRequestProcessingStart()
        {
            if (performanceCounterProvider.actorOutstandingRequestsCounterWriter != null)
            {
                performanceCounterProvider.actorOutstandingRequestsCounterWriter.UpdateCounterValue(1);
            }
        }

        public void LoadActorStateFinish(DateTime startTime)
        {
            if (performanceCounterProvider.actorLoadStateTimeCounterWriter != null)
            {
                performanceCounterProvider.actorLoadStateTimeCounterWriter.UpdateCounterValue(LongMillisecondsSinceStart(startTime));
            }
        }

        public void LoadActorStateStart()
        {
            // Intentionally left blank, since we don't track
        }

        public void ReleaseActorLock(DateTime startTime)
        {
            if (performanceCounterProvider.actorLockHoldTimeCounterWriter != null)
            {
                performanceCounterProvider.actorLockHoldTimeCounterWriter.UpdateCounterValue(LongMillisecondsSinceStart(startTime));
            }
        }

        public void SaveActorStateFinish(ActorId actorId, DateTime startTime)
        {
            if (performanceCounterProvider.actorSaveStateTimeCounterWriter != null)
            {
                performanceCounterProvider.actorSaveStateTimeCounterWriter.UpdateCounterValue(new ActorStateDiagnosticData() { ActorId = actorId, OperationTime = clock.UtcNow - startTime });
            }
        }

        public void SaveActorStateStart(ActorId actorId)
        {
            // Intentionally left blank, since we don't track
        }

        private long LongMillisecondsSinceStart(DateTime startTime)
        {
            return (long)(clock.UtcNow - startTime).TotalMilliseconds;
        }
    }
}
