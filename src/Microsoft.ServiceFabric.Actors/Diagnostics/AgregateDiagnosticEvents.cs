using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal class AgregateDiagnosticEvents : IDiagnosticEvents
    {
        readonly IEnumerable<IDiagnosticEvents> diagnosticEvents;
        public AgregateDiagnosticEvents(IEnumerable<IDiagnosticEvents> diagnosticEvents)
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
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.AcquireActorLockFailed(diagnosticContext);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void AcquireActorLockFinish(DiagnosticsManagerActorContext diagnosticContext, DateTime startTime, ActorId actorId)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.AcquireActorLockFinish(diagnosticContext, startTime, actorId);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void AcquireActorLockStart(DiagnosticsManagerActorContext diagnosticContext)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.AcquireActorLockStart(diagnosticContext);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void ActorActivated(ActorId actorId)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.ActorActivated(actorId);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void ActorChangeRole(ReplicaRole currentRole, ReplicaRole newRole)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.ActorChangeRole(currentRole, newRole);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void ActorDeactivated(ActorId actorId)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.ActorDeactivated(actorId);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void ActorMethodFinish(DiagnosticsManagerActorContext diagnosticContext, DateTime startTime, ActorId actorId, long interfaceMethodKey, Exception e, RemotingListenerVersion remotingListener)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.ActorMethodFinish(diagnosticContext, startTime, actorId, interfaceMethodKey, e, remotingListener);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void ActorMethodStart(DiagnosticsManagerActorContext diagnosticContext, ActorId actorId, long interfaceMethodKey, RemotingListenerVersion remotingListener)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.ActorMethodStart(diagnosticContext, actorId, interfaceMethodKey, remotingListener);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void ActorOnActivateAsyncFinish(DateTime startTime)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.ActorOnActivateAsyncFinish(startTime);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void ActorOnActivateAsyncStart()
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.ActorOnActivateAsyncStart();
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void ActorRequestProcessingFinish(DateTime startTime)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.ActorRequestProcessingFinish(startTime);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void ActorRequestProcessingStart()
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.ActorRequestProcessingStart();
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void LoadActorStateFinish(DateTime startTime)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.LoadActorStateFinish(startTime);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void LoadActorStateStart()
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.LoadActorStateStart();
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void ReleaseActorLock(DateTime startTime)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.ReleaseActorLock(startTime);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void SaveActorStateFinish(ActorId actorId, DateTime startTime)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.SaveActorStateFinish(actorId, startTime);
                }
            }
            catch
            {
                HandleException();
            }
        }

        public void SaveActorStateStart(ActorId actorId)
        {
            try
            {
                foreach (IDiagnosticEvents d in diagnosticEvents)
                {
                    d.SaveActorStateStart(actorId);
                }
            }
            catch
            {
                HandleException();
            }
        }

        private void HandleException()
        {
            throw new NotImplementedException();
        }
    }
}
