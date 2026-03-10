// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics.Metrics;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    sealed class MetricDiagnostics : IDiagnostics
    {
        private const string ActorMetricsNamespace = "Actor";
        private const string NoneException = "None";

        readonly IClock clock;

        readonly IMeter<long> pendingMethodCalls;
        readonly IMeter<TimeSpan> acquireLockDuration;
        readonly IMeter<TimeSpan> releaseLockDuration;
        readonly IMeter3D<TimeSpan> methodExecutionDuration;
        readonly IMeter<TimeSpan> onActivateAsyncDuration;
        readonly IMeter<TimeSpan> requestProcessingDuration;
        readonly IMeter<TimeSpan> loadStateDuration;
        readonly IMeter<TimeSpan> saveStateDuration;

        readonly IReadOnlyDictionary<long, ActorMethodInfo> actorMethodInfo;

        public MetricDiagnostics(IMeterProvider<long> longMeterProvider, IMeterProvider<TimeSpan> timeSpanProvider, IClock clock, ActorMethodFriendlyNameBuilder nameBuilder, ActorTypeInformation typeInfo)
        {
            _ = longMeterProvider ?? throw new ArgumentNullException(nameof(longMeterProvider));
            _ = timeSpanProvider ?? throw new ArgumentNullException(nameof(timeSpanProvider));
            _ = nameBuilder ?? throw new ArgumentNullException(nameof(nameBuilder));
            _ = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));

            this.pendingMethodCalls = longMeterProvider.CreateMeter(ActorMetricsNamespace, "PendingMethodCalls");
            this.acquireLockDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "AcquireLockDuration");
            this.releaseLockDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "ReleaseLockDuration");
            this.methodExecutionDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "MethodExecutionDuration", "MethodName", "MethodSigniture", "Exception");
            this.onActivateAsyncDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "OnActivateAsyncDuration");
            this.requestProcessingDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "RequestProcessingDuration");
            this.loadStateDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "LoadStateDuration");
            this.saveStateDuration = timeSpanProvider.CreateMeter(ActorMetricsNamespace, "SaveStateDuration");

            this.actorMethodInfo = ActorMethodInfoUtil.BuildActorMethodInfo(nameBuilder, typeInfo);
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
            var methodInfo = actorMethodInfo[actorMethodDiagnosticData.InterfaceMethodKey];
            methodExecutionDuration.Record(clock.UtcNow - startTime, methodInfo.methodName, methodInfo.methodSignature, actorMethodDiagnosticData.Exception != null ? actorMethodDiagnosticData.Exception.GetType().Name : NoneException);
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

        public void Dispose()
        {
            pendingMethodCalls.Dispose();
            acquireLockDuration.Dispose();
            releaseLockDuration.Dispose();
            methodExecutionDuration.Dispose();
            onActivateAsyncDuration.Dispose();
            requestProcessingDuration.Dispose();
            loadStateDuration.Dispose();
            saveStateDuration.Dispose();
        }
    }
}
