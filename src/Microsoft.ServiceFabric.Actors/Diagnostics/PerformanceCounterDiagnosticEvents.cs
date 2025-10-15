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

        public void AcquireActorLockFinish(DiagnosticsManagerActorContext diagnosticContext, DateTime startTime, ActorId actorId)
        {
            throw new NotImplementedException();
        }

        public void AcquireActorLockStart(DiagnosticsManagerActorContext diagnosticContext)
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorActivated(ActorId actorId)
        {
            throw new NotImplementedException();
        }

        public void ActorChangeRole(ReplicaRole currentRole, ReplicaRole newRole)
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorDeactivated(ActorId actorId)
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorMethodFinish(DiagnosticsManagerActorContext diagnosticContext, DateTime startTime, ActorId actorId, long interfaceMethodKey, Exception e, RemotingListenerVersion remotingListener)
        {
            throw new NotImplementedException();
        }

        public void ActorMethodStart(DiagnosticsManagerActorContext diagnosticContext, ActorId actorId, long interfaceMethodKey, RemotingListenerVersion remotingListener)
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorOnActivateAsyncFinish(DateTime startTime)
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorOnActivateAsyncStart()
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorRequestProcessingFinish(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public void ActorRequestProcessingStart()
        {
            throw new NotImplementedException();
        }

        public void LoadActorStateFinish(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public void LoadActorStateStart()
        {
            // Intentionally left blank, since we don't track
        }

        public void ReleaseActorLock(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public void SaveActorStateFinish(ActorId actorId, DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public void SaveActorStateStart(ActorId actorId)
        {
            // Intentionally left blank, since we don't track
        }
    }
}
