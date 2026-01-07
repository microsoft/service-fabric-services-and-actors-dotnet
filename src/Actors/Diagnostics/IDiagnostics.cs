// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal interface IDiagnostics
    {
        void ActorRequestProcessingStart();

        void ActorRequestProcessingFinish(DateTime startTime);

        void ActorOnActivateAsyncStart();

        void ActorOnActivateAsyncFinish(DateTime startTime);

        void ActorMethodStart(ActorId actorId, long interfaceMethodKey);

        void ActorMethodFinish(ActorMethodDiagnosticData actorMethodDiagnosticData, DateTime startTime);

        void LoadActorStateStart();

        void LoadActorStateFinish(DateTime startTime);

        void SaveActorStateStart(ActorId actorId);

        void SaveActorStateFinish(ActorId actorId, DateTime startTime);

        void AcquireActorLockFinish(PendingActorMethodDiagnosticData diagnosticData, DateTime startTime);

        void ReleaseActorLock(DateTime startTime);

        void ActorChangeRole(ReplicaRole currentRole, ReplicaRole newRole);

        void ActorActivated(ActorId actorId);

        void ActorDeactivated(ActorId actorId);
    }
}
