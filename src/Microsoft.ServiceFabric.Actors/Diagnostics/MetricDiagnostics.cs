// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics.Metrics;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    sealed class MetricDiagnostics : IDiagnostics
    {
        private const string ActorMetricsNamespace = "Actor";

        readonly IClock clock;

        readonly IMeter<long> pendingMethodCalls;
        readonly IMeter<TimeSpan> acquireLockDuration;
        readonly IMeter<TimeSpan> releaseLockDuration;
        readonly IMeter1D<long> methodExceptionCount;
        readonly IMeter1D<TimeSpan> methodExecutionDuration;
        readonly IMeter<TimeSpan> onActivateAsyncDuration;
        readonly IMeter<TimeSpan> requestProcessingDuration;
        readonly IMeter<TimeSpan> loadStateDuration;
        readonly IMeter<TimeSpan> saveStateDuration;

        public MetricDiagnostics(IMeterProvider<long> longMeterProvider, IMeterProvider<TimeSpan> timeSpanProvider, IClock clock)
        {
            _ = longMeterProvider ?? throw new ArgumentNullException(nameof(longMeterProvider));
            _ = timeSpanProvider ?? throw new ArgumentNullException(nameof(timeSpanProvider));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));

            this.pendingMethodCalls = longMeterProvider.CreateMeter(ActorMetricsNamespace, "PendingMethodCalls");
            this.acquireLockDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "AcquireLockDuration");
            this.releaseLockDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "ReleaseLockDuration");
            this.methodExceptionCount = longMeterProvider.CreateMeter(ActorMetricsNamespace, "MethodExceptionCount", "MethodId");
            this.methodExecutionDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "MethodExecutionDuration", "MethodId");
            this.onActivateAsyncDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "OnActivateAsyncDuration");
            this.requestProcessingDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "RequestProcessingDuration");
            this.loadStateDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "LoadStateDuration");
            this.saveStateDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "SaveStateDuration");
        }

        public void AcquireActorLockFinish(PendingActorMethodDiagnosticData diagnosticData, DateTime startTime)
        {
            pendingMethodCalls.Record(diagnosticData.PendingActorMethodCalls);
            acquireLockDuration.Record(clock.UtcNow - startTime);
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
            methodExecutionDuration.Record(clock.UtcNow - startTime, actorMethodDiagnosticData.MethodId.ToString());

            if (actorMethodDiagnosticData.Exception != null)
            {
                methodExceptionCount.Record(1, actorMethodDiagnosticData.MethodId.ToString());
            }
        }

        public void ActorMethodStart(ActorId actorId, long interfaceMethodKey)
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorOnActivateAsyncFinish(DateTime startTime)
        {
            onActivateAsyncDuration.Record(clock.UtcNow - startTime);
        }

        public void ActorOnActivateAsyncStart()
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorRequestProcessingFinish(DateTime startTime)
        {
            requestProcessingDuration.Record(clock.UtcNow - startTime);
        }

        public void ActorRequestProcessingStart()
        {
            // Intentionally left blank, since we don't track
        }

        public void LoadActorStateFinish(DateTime startTime)
        {
            loadStateDuration.Record(clock.UtcNow - startTime);
        }

        public void LoadActorStateStart()
        {
            // Intentionally left blank, since we don't track
        }

        public void ReleaseActorLock(DateTime startTime)
        {
            releaseLockDuration.Record(clock.UtcNow - startTime);
        }

        public void SaveActorStateFinish(ActorId actorId, DateTime startTime)
        {
            saveStateDuration.Record(clock.UtcNow - startTime);
        }

        public void SaveActorStateStart(ActorId actorId)
        {
            // Intentionally left blank, since we don't track
        }
    }
}
