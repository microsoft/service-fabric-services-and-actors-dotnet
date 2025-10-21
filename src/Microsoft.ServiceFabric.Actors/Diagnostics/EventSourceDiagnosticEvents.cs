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
    internal class EventSourceDiagnosticEvents : IDiagnosticEvents
    {
        readonly ServiceContext serviceContext;
        readonly IClock clock;
        readonly string actorType;
        readonly ActorFrameworkEventSource eventSource;
        Dictionary<long, ActorMethodInfo> actorMethodInfo;

        public EventSourceDiagnosticEvents(ActorFrameworkEventSource eventSource, IClock clock, ServiceContext serviceContext, ActorMethodFriendlyNameBuilder nameBuilder, ActorTypeInformation typeInfo)
        {
            this.eventSource = eventSource ?? throw new ArgumentNullException(nameof(eventSource));
            this.serviceContext = serviceContext;
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _ = nameBuilder ?? throw new ArgumentNullException(nameof(nameBuilder));
            _ = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            actorType = typeInfo.ImplementationType.ToString();
        }

        //private void InitializeActorMethodInfo(ActorMethodFriendlyNameBuilder nameBuilder, ActorTypeInformation typeInfo)
        //{
        //    this.actorMethodInfo = new Dictionary<long, ActorMethodInfo>();

        //    foreach (var actorInterfaceType in typeInfo.InterfaceTypes)
        //    {
        //        nameBuilder.GetActorInterfaceMethodDescriptionsV2(actorInterfaceType, out var interfaceId, out var actorInterfaceMethodDescriptions);
        //        foreach (var actorInterfaceMethodDescription in actorInterfaceMethodDescriptions)
        //        {
        //            var methodInfo = actorInterfaceMethodDescription.MethodInfo;
        //            var ami = new ActorMethodInfo()
        //            {
        //                MethodName = string.Concat(methodInfo.DeclaringType.Name, ".", methodInfo.Name),
        //                MethodSignature = actorInterfaceMethodDescription.MethodInfo.ToString(),
        //            };

        //            var key =
        //                DiagnosticsEventManager.GetInterfaceMethodKey(
        //                    (uint)interfaceId,
        //                    (uint)actorInterfaceMethodDescription.Id);
        //            actorMethodInfo[key] = ami;
        //        }
        //    }
        //}

        public void AcquireActorLockFailed(DiagnosticsManagerActorContext diagnosticContext)
        {
            // Intentionally left blank, since we don't track
        }

        public void AcquireActorLockFinish(PendingActorMethodDiagnosticData diagnosticData, DateTime startTime)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void ActorChangeRole(ReplicaRole currentRole, ReplicaRole newRole)
        {
            throw new NotImplementedException();
        }

        public void ActorDeactivated(ActorId actorId)
        {
            throw new NotImplementedException();
        }

        public void ActorMethodFinish(DateTime startTime, ActorId actorId, long interfaceMethodKey, Exception e, RemotingListenerVersion remotingListener)
        {
            throw new NotImplementedException();
        }

        public void ActorMethodStart(ActorId actorId, long interfaceMethodKey, RemotingListenerVersion remotingListener)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SaveActorStateStart(ActorId actorId)
        {
            throw new NotImplementedException();
        }
    }
}
