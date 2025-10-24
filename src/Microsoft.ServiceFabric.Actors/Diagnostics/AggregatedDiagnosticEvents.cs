// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal class AggregatedDiagnosticEvents : IDiagnosticEvents
    {
        readonly IEnumerable<IDiagnosticEvents> diagnosticEvents;
        public AggregatedDiagnosticEvents(IEnumerable<IDiagnosticEvents> diagnosticEvents)
        {
            _ = diagnosticEvents ?? throw new ArgumentNullException(nameof(diagnosticEvents));
            if (diagnosticEvents.Any(d => d == null))
            {
                throw new ArgumentException(nameof(diagnosticEvents));
            }

            this.diagnosticEvents = diagnosticEvents;
        }
        public void AcquireActorLockFailed(DiagnosticsManagerActorContext diagnosticContext)
        {
            Interlocked.Decrement(ref diagnosticContext.PendingActorMethodCalls);
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.AcquireActorLockFailed(diagnosticContext);
            }
        }

        public void AcquireActorLockStart(DiagnosticsManagerActorContext diagnosticContext)
        {
            Interlocked.Increment(ref diagnosticContext.PendingActorMethodCalls);
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.AcquireActorLockStart(diagnosticContext);
            }
        }

        public void ActorActivated(ActorId actorId)
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.ActorActivated(actorId);
            }
        }

        public void ActorChangeRole(ReplicaRole currentRole, ReplicaRole newRole)
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.ActorChangeRole(currentRole, newRole);
            }
        }

        public void ActorDeactivated(ActorId actorId)
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.ActorDeactivated(actorId);
            }
        }

        public void ActorMethodFinish(DateTime startTime, ActorId actorId, long interfaceMethodKey, Exception e, RemotingListenerVersion remotingListener)
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.ActorMethodFinish(startTime, actorId, interfaceMethodKey, e, remotingListener);
            }
        }

        public void ActorMethodStart(ActorId actorId, long interfaceMethodKey, RemotingListenerVersion remotingListener)
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.ActorMethodStart(actorId, interfaceMethodKey, remotingListener);
            }
        }

        public void ActorOnActivateAsyncFinish(DateTime startTime)
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.ActorOnActivateAsyncFinish(startTime);
            }
        }

        public void ActorOnActivateAsyncStart()
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.ActorOnActivateAsyncStart();
            }
        }

        public void ActorRequestProcessingFinish(DateTime startTime)
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.ActorRequestProcessingFinish(startTime);
            }
        }

        public void ActorRequestProcessingStart()
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.ActorRequestProcessingStart();
            }
        }

        public void LoadActorStateFinish(DateTime startTime)
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.LoadActorStateFinish(startTime);
            }
        }

        public void LoadActorStateStart()
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.LoadActorStateStart();
            }
        }

        public void ReleaseActorLock(DateTime startTime)
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.ReleaseActorLock(startTime);
            }
        }

        public void SaveActorStateFinish(ActorId actorId, DateTime startTime)
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.SaveActorStateFinish(actorId, startTime);
            }
        }

        public void SaveActorStateStart(ActorId actorId)
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.SaveActorStateStart(actorId);
            }
        }

        public void AcquireActorLockFinish(PendingActorMethodDiagnosticData diagnosticData, DateTime startTime)
        {
            foreach (IDiagnosticEvents d in diagnosticEvents)
            {
                d.AcquireActorLockFinish(diagnosticData, startTime);
            }
        }

        public void AcquireActorLockFinishPreProcess(DiagnosticsManagerActorContext diagnosticContext, DateTime startTime, ActorId actorId)
        {
            var pendingActorMethodCalls = Interlocked.Decrement(ref diagnosticContext.PendingActorMethodCalls);
            var delta = pendingActorMethodCalls - diagnosticContext.LastReportedPendingActorMethodCalls;
            diagnosticContext.LastReportedPendingActorMethodCalls = diagnosticContext.PendingActorMethodCalls;

            var diagnosticData = new PendingActorMethodDiagnosticData() { ActorId = actorId, PendingActorMethodCalls = pendingActorMethodCalls, PendingActorMethodCallsDelta = delta };
            AcquireActorLockFinish(diagnosticData, startTime);
        }
    }
}
