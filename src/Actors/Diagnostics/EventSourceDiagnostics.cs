// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Diagnostics;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    sealed class EventSourceDiagnostics : IDiagnostics
    {
        readonly ActorFrameworkEventSource eventSource;
        readonly ServiceContext serviceContext;
        readonly IClock clock;
        readonly string actorType;

        readonly IReadOnlyDictionary<long, ActorMethodInfo> actorMethodInfo;

        internal EventSourceDiagnostics(ActorFrameworkEventSource eventSource, IClock clock, ServiceContext serviceContext, ActorMethodFriendlyNameBuilder nameBuilder, ActorTypeInformation typeInfo)
        {
            this.eventSource = eventSource ?? throw new ArgumentNullException(nameof(eventSource));
            this.serviceContext = serviceContext ?? throw new ArgumentNullException(nameof(serviceContext));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _ = nameBuilder ?? throw new ArgumentNullException(nameof(nameBuilder));
            _ = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            this.actorType = typeInfo.ImplementationType.ToString();
            this.actorMethodInfo = ActorMethodInfoUtil.BuildActorMethodInfo(nameBuilder, typeInfo);
        }

        public void AcquireActorLockFinish(PendingActorMethodDiagnosticData diagnosticData, DateTime startTime)
        {
            if (eventSource.IsPendingMethodCallsEventEnabled())
            {
                eventSource.ActorMethodCallsWaitingForLock(diagnosticData.PendingActorMethodCalls, actorType, diagnosticData.ActorId, serviceContext);
            }
        }

        public void ActorActivated(ActorId actorId)
        {
            eventSource.ActorActivated(actorType, actorId, serviceContext);
        }

        public void ActorChangeRole(ReplicaRole currentRole, ReplicaRole newRole)
        {
            if (newRole == ReplicaRole.Primary)
            {
                eventSource.ReplicaChangeRoleToPrimary(serviceContext);
            }
            else if (currentRole == ReplicaRole.Primary)
            {
                eventSource.ReplicaChangeRoleFromPrimary(serviceContext);
            }
        }

        public void ActorDeactivated(ActorId actorId)
        {
            eventSource.ActorDeactivated(actorType, actorId, serviceContext);
        }

        public void ActorMethodFinish(ActorMethodDiagnosticData actorMethodDiagnosticData, DateTime startTime)
        {
            var methodInfo = actorMethodInfo[actorMethodDiagnosticData.InterfaceMethodKey];

            if (actorMethodDiagnosticData.Exception != null)
            {
                eventSource.ActorMethodThrewException(
                   actorMethodDiagnosticData.Exception.ToString(),
                   TicksSinceStart(startTime),
                   methodInfo.methodName,
                   methodInfo.methodSignature,
                   actorType,
                   actorMethodDiagnosticData.ActorId,
                   serviceContext);
                return;
            }

            if (eventSource.IsActorMethodStopEventEnabled())
            {
                eventSource.ActorMethodStop(
                    TicksSinceStart(startTime),
                    methodInfo.methodName,
                    methodInfo.methodSignature,
                    actorType,
                    actorMethodDiagnosticData.ActorId,
                    serviceContext);
            }
        }

        public void ActorMethodStart(ActorId actorId, long interfaceMethodKey)
        {
            if (eventSource.IsActorMethodStartEventEnabled())
            {
                var methodInfo = actorMethodInfo[interfaceMethodKey];
                eventSource.ActorMethodStart(methodInfo.methodName, methodInfo.methodSignature, actorType, actorId, serviceContext);
            }
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
            // Intentionally left blank, since we don't track
        }

        public void ActorRequestProcessingStart()
        {
            // Intentionally left blank, since we don't track
        }

        public void LoadActorStateFinish(DateTime startTime)
        {
            // Intentionally left blank, since we don't track
        }

        public void LoadActorStateStart()
        {
            // Intentionally left blank, since we don't track
        }

        public void ReleaseActorLock(DateTime startTime)
        {
            // Intentionally left blank, since we don't track
        }

        public void SaveActorStateFinish(ActorId actorId, DateTime startTime)
        {
            if (eventSource.IsActorSaveStateStopEventEnabled())
            {
                eventSource.ActorSaveStateStop(TicksSinceStart(startTime), actorType, actorId, serviceContext);
            }
        }

        public void SaveActorStateStart(ActorId actorId)
        {
            if (eventSource.IsActorSaveStateStartEventEnabled())
            {
                eventSource.ActorSaveStateStart(actorType, actorId, serviceContext);
            }
        }
        private long TicksSinceStart(DateTime startTime)
        {
            return TimeSpan.FromMilliseconds((long)(clock.UtcNow - startTime).TotalMilliseconds).Ticks;
        }
    }
}
