// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    sealed class AggregatedDiagnostics : IDiagnostics, IDisposable
    {
        readonly IEnumerable<IDiagnostics> diagnosticEvents;

        internal AggregatedDiagnostics(IEnumerable<IDiagnostics> diagnosticEvents)
        {
            _ = diagnosticEvents ?? throw new ArgumentNullException(nameof(diagnosticEvents));
            if (diagnosticEvents.Any(d => d == null))
                throw new ArgumentException(nameof(diagnosticEvents));
            this.diagnosticEvents = diagnosticEvents;
        }

        public void ActorActivated(ActorId actorId)
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.ActorActivated(actorId);
            }
        }

        public void ActorChangeRole(ReplicaRole currentRole, ReplicaRole newRole)
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.ActorChangeRole(currentRole, newRole);
            }
        }

        public void ActorDeactivated(ActorId actorId)
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.ActorDeactivated(actorId);
            }
        }

        public void ActorMethodFinish(ActorMethodDiagnosticData actorMethodDiagnosticData, DateTime startTime)
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.ActorMethodFinish(actorMethodDiagnosticData, startTime);
            }
        }

        public void ActorMethodStart(ActorId actorId, long interfaceMethodKey)
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.ActorMethodStart(actorId, interfaceMethodKey);
            }
        }

        public void ActorOnActivateAsyncFinish(DateTime startTime)
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.ActorOnActivateAsyncFinish(startTime);
            }
        }

        public void ActorOnActivateAsyncStart()
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.ActorOnActivateAsyncStart();
            }
        }

        public void ActorRequestProcessingFinish(DateTime startTime)
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.ActorRequestProcessingFinish(startTime);
            }
        }

        public void ActorRequestProcessingStart()
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.ActorRequestProcessingStart();
            }
        }

        public void LoadActorStateFinish(DateTime startTime)
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.LoadActorStateFinish(startTime);
            }
        }

        public void LoadActorStateStart()
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.LoadActorStateStart();
            }
        }

        public void ReleaseActorLock(DateTime startTime)
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.ReleaseActorLock(startTime);
            }
        }

        public void SaveActorStateFinish(ActorId actorId, DateTime startTime)
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.SaveActorStateFinish(actorId, startTime);
            }
        }

        public void SaveActorStateStart(ActorId actorId)
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.SaveActorStateStart(actorId);
            }
        }

        public void AcquireActorLockFinish(PendingActorMethodDiagnosticData diagnosticData, DateTime startTime)
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                d.AcquireActorLockFinish(diagnosticData, startTime);
            }
        }

        public void Dispose()
        {
            foreach (IDiagnostics d in diagnosticEvents)
            {
                if (d is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
