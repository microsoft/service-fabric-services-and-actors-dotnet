// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    sealed class EventSourceDiagnosticEvents : IDiagnosticEvents
    {
        readonly ServiceContext serviceContext;
        readonly IClock clock;
        readonly string actorType;
        readonly ActorFrameworkEventSource eventSource;
        readonly Dictionary<long, ActorMethodInfo> actorMethodInfo;

        internal EventSourceDiagnosticEvents(ActorFrameworkEventSource eventSource, IClock clock, ServiceContext serviceContext, ActorMethodFriendlyNameBuilder nameBuilder, ActorTypeInformation typeInfo)
        {
            this.eventSource = eventSource ?? throw new ArgumentNullException(nameof(eventSource));
            this.serviceContext = serviceContext ?? throw new ArgumentNullException(nameof(serviceContext));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _ = nameBuilder ?? throw new ArgumentNullException(nameof(nameBuilder));
            _ = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            this.actorType = typeInfo.ImplementationType.ToString();
            this.actorMethodInfo = InitializeActorMethodInfo(nameBuilder, typeInfo);
        }

        private Dictionary<long, ActorMethodInfo> InitializeActorMethodInfo(ActorMethodFriendlyNameBuilder nameBuilder, ActorTypeInformation typeInfo)
        {
            var actorMethodInfo = new Dictionary<long, ActorMethodInfo>();

            foreach (var actorInterfaceType in typeInfo.InterfaceTypes)
            {
                nameBuilder.GetActorInterfaceMethodDescriptionsV2(actorInterfaceType, out var interfaceId, out var actorInterfaceMethodDescriptions);
                foreach (var actorInterfaceMethodDescription in actorInterfaceMethodDescriptions)
                {
                    var methodInfo = new ActorMethodInfo(actorInterfaceMethodDescription.MethodInfo);

                    actorMethodInfo[Util.GetInterfaceMethodKey((uint)interfaceId, (uint)actorInterfaceMethodDescription.Id)] = methodInfo;
                }
            }
            return actorMethodInfo;
        }

        public void AcquireActorLockFailed(DiagnosticsManagerActorContext diagnosticContext)
        {
            // Intentionally left blank, since we don't track
        }

        public void AcquireActorLockFinish(PendingActorMethodDiagnosticData diagnosticData, DateTime startTime)
        {
            if (this.eventSource.IsPendingMethodCallsEventEnabled())
            {
                this.eventSource.ActorMethodCallsWaitingForLock(diagnosticData.PendingActorMethodCalls, this.actorType, diagnosticData.ActorId, this.serviceContext);
            }
        }

        public void AcquireActorLockFinishPreProcess(DiagnosticsManagerActorContext diagnosticContext, DateTime startTime, ActorId actorId)
        {
            // Intentionally left blank, since we don't track
        }

        public void AcquireActorLockStart(DiagnosticsManagerActorContext diagnosticContext)
        {
            // Intentionally left blank, since we don't track
        }

        public void ActorActivated(ActorId actorId)
        {
            this.eventSource.ActorActivated(this.actorType, actorId, this.serviceContext);
        }

        public void ActorChangeRole(ReplicaRole currentRole, ReplicaRole newRole)
        {
            if (newRole == ReplicaRole.Primary)
            {
                this.eventSource.ReplicaChangeRoleToPrimary(this.serviceContext);
            }
            else if (currentRole == ReplicaRole.Primary)
            {
                this.eventSource.ReplicaChangeRoleFromPrimary(this.serviceContext);
            }
        }

        public void ActorDeactivated(ActorId actorId)
        {
            this.eventSource.ActorDeactivated(this.actorType, actorId, this.serviceContext);
        }

        public void ActorMethodFinish(DateTime startTime, ActorId actorId, long interfaceMethodKey, Exception e, RemotingListenerVersion remotingListener)
        {
            var methodInfo = this.actorMethodInfo[interfaceMethodKey];

            if (e != null)
            {
                this.eventSource.ActorMethodThrewException(
                   e.ToString(),
                   TicksSinceStart(startTime),
                   methodInfo.MethodName,
                   methodInfo.MethodSignature,
                   this.actorType,
                   actorId,
                   this.serviceContext);
                return;
            }

            if (this.eventSource.IsActorMethodStopEventEnabled())
            {
                this.eventSource.ActorMethodStop(
                    TicksSinceStart(startTime),
                    methodInfo.MethodName,
                    methodInfo.MethodSignature,
                    this.actorType,
                    actorId,
                    this.serviceContext);
            }
        }

        public void ActorMethodStart(ActorId actorId, long interfaceMethodKey, RemotingListenerVersion remotingListener)
        {
            if (this.eventSource.IsActorMethodStartEventEnabled())
            {
                var methodInfo = this.actorMethodInfo[interfaceMethodKey];
                this.eventSource.ActorMethodStart(methodInfo.MethodName, methodInfo.MethodSignature, this.actorType, actorId, this.serviceContext);
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
            if (this.eventSource.IsActorSaveStateStopEventEnabled())
            {
                this.eventSource.ActorSaveStateStop(TicksSinceStart(startTime), this.actorType, actorId, this.serviceContext);
            }
        }

        public void SaveActorStateStart(ActorId actorId)
        {
            if (this.eventSource.IsActorSaveStateStartEventEnabled())
            {
                this.eventSource.ActorSaveStateStart(this.actorType, actorId, this.serviceContext);
            }
        }
        private long TicksSinceStart(DateTime startTime)
        {
            return TimeSpan.FromMilliseconds((long)(clock.UtcNow - startTime).TotalMilliseconds).Ticks;
        }
    }
}
