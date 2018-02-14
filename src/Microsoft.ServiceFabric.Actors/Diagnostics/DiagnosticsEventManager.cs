// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;
    using System.Fabric;
    using System.Threading;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting;

    internal class DiagnosticsEventManager
    {
        internal delegate void OnDiagnosticEvent();
        internal delegate void OnDiagnosticEvent<T>(T eventData);

        internal OnDiagnosticEvent<ChangeRoleDiagnosticData> onActorChangeRole;
        internal OnDiagnosticEvent<ActivationDiagnosticData> onActorActivated;
        internal OnDiagnosticEvent<ActivationDiagnosticData> onActorDeactivated;
        internal OnDiagnosticEvent<PendingActorMethodDiagnosticData> onPendingActorMethodCallsUpdated;
        internal OnDiagnosticEvent<ActorMethodDiagnosticData> onActorMethodStart;
        internal OnDiagnosticEvent<ActorMethodDiagnosticData> onActorMethodFinish;
        internal OnDiagnosticEvent<ActorStateDiagnosticData> onSaveActorStateFinish;
        internal OnDiagnosticEvent<ActorStateDiagnosticData> onSaveActorStateStart;
        internal OnDiagnosticEvent onActorRequestProcessingStart;
        internal OnDiagnosticEvent<TimeSpan> onActorRequestProcessingFinish;
        internal OnDiagnosticEvent<TimeSpan> onActorLockAcquired;
        internal OnDiagnosticEvent<TimeSpan> onActorLockReleased;
        internal OnDiagnosticEvent<TimeSpan> onActorRequestDeserializationFinish;
        internal OnDiagnosticEvent<TimeSpan> onActorResponseSerializationFinish;
        internal OnDiagnosticEvent<TimeSpan> onActorOnActivateAsyncFinish;
        internal OnDiagnosticEvent<TimeSpan> onLoadActorStateFinish;

        private ChangeRoleDiagnosticData changeRoleDiagnosticData;
        internal ActorMethodFriendlyNameBuilder ActorMethodFriendlyNameBuilder { get; private set; }

        internal DiagnosticsEventManager(ActorMethodFriendlyNameBuilder methodFriendlyNameBuilder)
        {
            this.ActorMethodFriendlyNameBuilder = methodFriendlyNameBuilder;
        }

        public static long GetInterfaceMethodKey(uint interfaceId, uint methodId)
        {
            var key = (ulong)methodId;
            key = key | (((ulong)interfaceId) << 32);
            return (long)key;
        }

        internal void ActorRequestProcessingStart()
        {
            var callbacks = this.onActorRequestProcessingStart;
            if (null != callbacks)
            {
                callbacks();
            }
        }

        internal void ActorRequestProcessingFinish(DateTime startTime)
        {
            var processingTime = DateTime.UtcNow - startTime;
            var callbacks = this.onActorRequestProcessingFinish;
            if (null != callbacks)
            {
                callbacks(processingTime);
            }
        }

        internal void ActorRequestDeserializationFinish(DateTime startTime)
        {
            var processingTime = DateTime.UtcNow - startTime;
            var callbacks = this.onActorRequestDeserializationFinish;
            if (null != callbacks)
            {
                callbacks(processingTime);
            }
        }

        internal void ActorResponseSerializationFinish(DateTime startTime)
        {
            var processingTime = DateTime.UtcNow - startTime;
            var callbacks = this.onActorResponseSerializationFinish;
            if (null != callbacks)
            {
                callbacks(processingTime);
            }
        }

        internal void ActorOnActivateAsyncStart(ActorBase actor)
        {
            var diagCtx = actor.DiagnosticsContext;
            diagCtx.OnActivateAsyncStopwatch.Restart();
        }

        internal void ActorOnActivateAsyncFinish(ActorBase actor)
        {
            var diagCtx = actor.DiagnosticsContext;
            var onActivateAsyncStopwatch = diagCtx.OnActivateAsyncStopwatch;
            onActivateAsyncStopwatch.Stop();

            var callbacks = this.onActorOnActivateAsyncFinish;
            if (null != callbacks)
            {
                callbacks(onActivateAsyncStopwatch.Elapsed);
            }

            onActivateAsyncStopwatch.Reset();
        }

        internal void ActorMethodStart(long interfaceMethodKey, ActorBase actor, RemotingListener remotingListener)
        {
            var diagCtx = actor.DiagnosticsContext;
            var mtdEvtArgs = diagCtx.MethodData;
            mtdEvtArgs.ActorId = actor.Id;
            mtdEvtArgs.InterfaceMethodKey = interfaceMethodKey;
            mtdEvtArgs.MethodExecutionTime = null;
            mtdEvtArgs.RemotingListener = remotingListener;
            var methodStopwatch = diagCtx.GetOrCreateActorMethodStopwatch();
            methodStopwatch.Restart();

            var callbacks = this.onActorMethodStart;
            if (null != callbacks)
            {
                callbacks(mtdEvtArgs);
            }

            // Push the stopwatch to the stopwatch stack. Stack is needed for
            // handling reentrancy.
            diagCtx.PushActorMethodStopwatch(methodStopwatch);
        }

        internal void ActorMethodFinish(long interfaceMethodKey, ActorBase actor, Exception e, RemotingListener remotingListener)
        {
            var diagCtx = actor.DiagnosticsContext;
            var mtdEvtArgs = diagCtx.MethodData;

            // Pop the stopwatch from the stopwatch stack.
            var mtdStopwatch = diagCtx.PopActorMethodStopwatch();

            mtdStopwatch.Stop();
            mtdEvtArgs.ActorId = actor.Id;
            mtdEvtArgs.InterfaceMethodKey = interfaceMethodKey;
            mtdEvtArgs.MethodExecutionTime = mtdStopwatch.Elapsed;
            mtdEvtArgs.Exception = e;
            mtdEvtArgs.RemotingListener = remotingListener;
            mtdStopwatch.Reset();

            var callbacks = this.onActorMethodFinish;
            if (null != callbacks)
            {
                callbacks(mtdEvtArgs);
            }
        }

        internal void LoadActorStateStart(ActorBase actor)
        {
            var diagCtx = actor.DiagnosticsContext;
            diagCtx.StateStopwatch.Restart();
        }

        internal void LoadActorStateFinish(ActorBase actor)
        {
            var diagCtx = actor.DiagnosticsContext;
            var stateStopwatch = diagCtx.StateStopwatch;
            stateStopwatch.Stop();

            var callbacks = this.onLoadActorStateFinish;
            if (null != callbacks)
            {
                callbacks(stateStopwatch.Elapsed);
            }

            stateStopwatch.Reset();
        }

        internal void SaveActorStateStart(ActorBase actor)
        {
            var diagCtx = actor.DiagnosticsContext;
            var stateEvtArgs = diagCtx.StateData;
            stateEvtArgs.ActorId = actor.Id;
            stateEvtArgs.OperationTime = null;
            diagCtx.StateStopwatch.Restart();

            var callbacks = this.onSaveActorStateStart;
            if (null != callbacks)
            {
                callbacks(stateEvtArgs);
            }
        }

        internal void SaveActorStateFinish(ActorBase actor)
        {
            var diagCtx = actor.DiagnosticsContext;
            var stateEvtArgs = diagCtx.StateData;
            var stateStopwatch = diagCtx.StateStopwatch;
            stateStopwatch.Stop();
            stateEvtArgs.ActorId = actor.Id;
            stateEvtArgs.OperationTime = stateStopwatch.Elapsed;
            stateStopwatch.Reset();

            var callbacks = this.onSaveActorStateFinish;
            if (null != callbacks)
            {
                callbacks(stateEvtArgs);
            }
        }

        internal DateTime AcquireActorLockStart(ActorBase actor)
        {
            // Use DateTime instead of StopWatch to measure elapsed time. We do this in order to avoid allocating a
            // StopWatch object for each operation that acquires the actor lock.
            var startTime = DateTime.UtcNow;
            Interlocked.Increment(ref actor.DiagnosticsContext.PendingActorMethodCalls);
            return startTime;
        }

        internal void AcquireActorLockFailed(ActorBase actor)
        {
            Interlocked.Decrement(ref actor.DiagnosticsContext.PendingActorMethodCalls);
        }

        internal DateTime AcquireActorLockFinish(ActorBase actor, DateTime actorLockAcquireStartTime)
        {
            // Record the current time
            var currentTime = DateTime.UtcNow;

            // Update number of pending actor method calls
            var diagCtx = actor.DiagnosticsContext;
            var pendingActorMethodCalls = Interlocked.Decrement(ref diagCtx.PendingActorMethodCalls);
            var delta = pendingActorMethodCalls - diagCtx.LastReportedPendingActorMethodCalls;
            diagCtx.LastReportedPendingActorMethodCalls = pendingActorMethodCalls;

            var pendingMtdEvtArgs = diagCtx.PendingMethodDiagnosticData;
            pendingMtdEvtArgs.ActorId = actor.Id;
            pendingMtdEvtArgs.PendingActorMethodCalls = pendingActorMethodCalls;
            pendingMtdEvtArgs.PendingActorMethodCallsDelta = delta;

            var callbacks1 = this.onPendingActorMethodCallsUpdated;
            if (null != callbacks1)
            {
                callbacks1(pendingMtdEvtArgs);
            }

            // Update time taken to acquire actor lock
            var lockAcquireTime = currentTime - actorLockAcquireStartTime;
            var callbacks2 = this.onActorLockAcquired;
            if (null != callbacks2)
            {
                callbacks2(lockAcquireTime);
            }

            return currentTime;
        }

        internal void ReleaseActorLock(DateTime? actorLockHoldStartTime)
        {
            if (actorLockHoldStartTime.HasValue)
            {
                var callbacks = this.onActorLockReleased;
                if (null != callbacks)
                {
                    var lockHoldTime = DateTime.UtcNow - actorLockHoldStartTime.Value;
                    callbacks(lockHoldTime);
                }
            }
        }

        internal void ActorChangeRole(ReplicaRole currentRole, ReplicaRole newRole)
        {
            var callbacks = this.onActorChangeRole;
            if (null != callbacks)
            {
                this.changeRoleDiagnosticData.CurrentRole = currentRole;
                this.changeRoleDiagnosticData.NewRole = newRole;
                callbacks(this.changeRoleDiagnosticData);
            }
        }

        internal void ActorActivated(ActorBase actor)
        {
            var activationEvtArgs = actor.DiagnosticsContext.ActivationDiagnosticData;
            activationEvtArgs.IsActivationEvent = true;
            activationEvtArgs.ActorId = actor.Id;

            var callbacks = this.onActorActivated;
            if (null != callbacks)
            {
                callbacks(activationEvtArgs);
            }
        }

        internal void ActorDeactivated(ActorBase actor)
        {
            var activationEvtArgs = actor.DiagnosticsContext.ActivationDiagnosticData;
            activationEvtArgs.IsActivationEvent = false;
            activationEvtArgs.ActorId = actor.Id;

            var callbacks = this.onActorDeactivated;
            if (null != callbacks)
            {
                callbacks(activationEvtArgs);
            }
        }
    }
}
