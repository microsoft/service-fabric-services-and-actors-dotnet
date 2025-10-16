// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal class PerformanceCounterDiagnosticEvents : IDiagnosticEvents
    {
        readonly PerformanceCounterProviderV2 performanceCounterProvider;
        readonly IClock clock;

        internal PerformanceCounterDiagnosticEvents(PerformanceCounterProviderV2 performanceCounterProvider, IClock clock)
        {
            this.performanceCounterProvider = performanceCounterProvider ?? throw new ArgumentNullException(nameof(performanceCounterProvider));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public void AcquireActorLockFailed(DiagnosticsManagerActorContext diagnosticContext)
        {
            // Intentionally left blank, since we don't track
        }

        public void AcquireActorLockFinish(PendingActorMethodDiagnosticData diagnosticData, DateTime startTime)
        {
            if (this.performanceCounterProvider.actorLockContentionCounterWriter != null)
            {
                this.performanceCounterProvider.actorLockContentionCounterWriter.UpdateCounterValue(diagnosticData);
            }
            if (this.performanceCounterProvider.actorLockAcquireWaitTimeCounterWriter != null)
            {
                this.performanceCounterProvider.actorLockAcquireWaitTimeCounterWriter.UpdateCounterValue(LongMillisecondsSinceStart(startTime));
            }
        }

        public void AcquireActorLockFinishPreProcess(DiagnosticsManagerActorContext diagnosticContext, DateTime startTime, ActorId actorId)
        {
            // Intentionally left blank, since we don't track
        }

        public void AcquireActorLockStart(DiagnosticsManagerActorContext diagnosticContext)
        {
            // Intentionally left blank, since we don't track
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

        public void ActorMethodFinish(DateTime startTime, ActorId actorId, long interfaceMethodKey, Exception e, RemotingListenerVersion remotingListener)
        {
            var counterWriters = this.performanceCounterProvider.GetMethodSpecificCounterWriters(interfaceMethodKey, remotingListener);

            ActorMethodDiagnosticData methodData = new ActorMethodDiagnosticData()
            {
                ActorId = actorId,
                Exception = e,
                InterfaceMethodKey = interfaceMethodKey,
                RemotingListener = remotingListener,
                MethodExecutionTime = TimeSpan.FromMilliseconds(LongMillisecondsSinceStart(startTime))
            };

            if (counterWriters.ActorMethodFrequencyCounterWriter != null)
            {
                counterWriters.ActorMethodFrequencyCounterWriter.UpdateCounterValue();
            }

            if (counterWriters.ActorMethodExceptionFrequencyCounterWriter != null)
            {
                counterWriters.ActorMethodExceptionFrequencyCounterWriter.UpdateCounterValue(methodData);
            }

            if (counterWriters.ActorMethodExecTimeCounterWriter != null)
            {
                counterWriters.ActorMethodExecTimeCounterWriter.UpdateCounterValue(methodData);
            }
        }

        public void ActorMethodStart(ActorId actorId, long interfaceMethodKey, RemotingListenerVersion remotingListener)
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorOnActivateAsyncFinish(DateTime startTime)
        {
            if (this.performanceCounterProvider.actorOnActivateAsyncTimeCounterWriter != null)
            {
                this.performanceCounterProvider.actorOnActivateAsyncTimeCounterWriter.UpdateCounterValue(LongMillisecondsSinceStart(startTime));
            }
        }

        public void ActorOnActivateAsyncStart()
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorRequestProcessingFinish(DateTime startTime)
        {
            if (this.performanceCounterProvider.actorOutstandingRequestsCounterWriter != null)
            {
                this.performanceCounterProvider.actorOutstandingRequestsCounterWriter.UpdateCounterValue(-1);
            }
            if (this.performanceCounterProvider.actorRequestProcessingTimeCounterWriter != null)
            {
                this.performanceCounterProvider.actorRequestProcessingTimeCounterWriter.UpdateCounterValue(LongMillisecondsSinceStart(startTime));
            }
        }

        public void ActorRequestProcessingStart()
        {
            if (this.performanceCounterProvider.actorOutstandingRequestsCounterWriter != null)
            {
                this.performanceCounterProvider.actorOutstandingRequestsCounterWriter.UpdateCounterValue(1);
            }
        }

        public void LoadActorStateFinish(DateTime startTime)
        {
            if (this.performanceCounterProvider.actorLoadStateTimeCounterWriter != null)
            {
                this.performanceCounterProvider.actorLoadStateTimeCounterWriter.UpdateCounterValue(LongMillisecondsSinceStart(startTime));
            }
        }

        public void LoadActorStateStart()
        {
            // Intentionally left blank, since we don't track
        }

        public void ReleaseActorLock(DateTime startTime)
        {
            if (this.performanceCounterProvider.actorLockHoldTimeCounterWriter != null)
            {
                this.performanceCounterProvider.actorLockHoldTimeCounterWriter.UpdateCounterValue(LongMillisecondsSinceStart(startTime));
            }
        }

        public void SaveActorStateFinish(ActorId actorId, DateTime startTime)
        {
            if (this.performanceCounterProvider.actorSaveStateTimeCounterWriter != null)
            {
                this.performanceCounterProvider.actorSaveStateTimeCounterWriter.UpdateCounterValue(new ActorStateDiagnosticData() { ActorId = actorId, OperationTime = clock.UtcNow - startTime });
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
