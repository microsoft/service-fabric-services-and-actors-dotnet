// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal interface IDiagnosticEvents
    {
        void ActorRequestProcessingStart();

        void ActorRequestProcessingFinish(DateTime startTime);

        void ActorOnActivateAsyncStart();

        void ActorOnActivateAsyncFinish(DateTime startTime);

        void ActorMethodStart(ActorId actorId, long interfaceMethodKey, RemotingListenerVersion remotingListener);

        void ActorMethodFinish(DateTime startTime, ActorId actorId, long interfaceMethodKey, Exception e, RemotingListenerVersion remotingListener);

        void LoadActorStateStart();

        void LoadActorStateFinish(DateTime startTime);

        void SaveActorStateStart(ActorId actorId);

        void SaveActorStateFinish(ActorId actorId, DateTime startTime);

        void AcquireActorLockStart(DiagnosticsManagerActorContext diagnosticContext);

        void AcquireActorLockFailed(DiagnosticsManagerActorContext diagnosticContext);

        void AcquireActorLockFinishPreProcess(DiagnosticsManagerActorContext diagnosticContext, DateTime startTime, ActorId actorId);

        void AcquireActorLockFinish(PendingActorMethodDiagnosticData diagnosticData, DateTime startTime);

        void ReleaseActorLock(DateTime startTime);

        void ActorChangeRole(ReplicaRole currentRole, ReplicaRole newRole);

        void ActorActivated(ActorId actorId);

        void ActorDeactivated(ActorId actorId);
    }
}
