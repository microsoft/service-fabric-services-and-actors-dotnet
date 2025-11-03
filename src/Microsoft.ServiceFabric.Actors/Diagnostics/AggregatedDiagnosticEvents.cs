// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    sealed class AggregatedDiagnosticEvents : IDiagnosticEvents
    {
        readonly IEnumerable<IDiagnosticEvents> diagnosticEvents;

        internal AggregatedDiagnosticEvents(IEnumerable<IDiagnosticEvents> diagnosticEvents)
        {
            _ = diagnosticEvents ?? throw new ArgumentNullException(nameof(diagnosticEvents));
            if (diagnosticEvents.Any(d => d == null))
                throw new ArgumentException(nameof(diagnosticEvents));
            this.diagnosticEvents = diagnosticEvents;
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
    }
}
