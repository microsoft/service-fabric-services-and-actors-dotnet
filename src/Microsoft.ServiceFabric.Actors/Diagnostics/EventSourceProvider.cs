// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Description;

    internal class EventSourceProvider
    {
        private readonly ActorTypeInformation actorTypeInformation;
        private readonly string actorType;
        private readonly ServiceContext serviceContext;
        private readonly ActorFrameworkEventSource writer;
        private Dictionary<long, ActorMethodInfo> actorMethodInfo;

        internal EventSourceProvider(ServiceContext serviceContext, ActorTypeInformation actorTypeInformation)
        {
            this.serviceContext = serviceContext;
            this.actorTypeInformation = actorTypeInformation;
            this.actorType = actorTypeInformation.ImplementationType.ToString();

            this.writer = ActorFrameworkEventSource.Writer;
        }

        internal ActorTypeInformation ActorTypeInformation => this.actorTypeInformation;

        internal void RegisterWithDiagnosticsEventManager(DiagnosticsEventManager diagnosticsEventManager)
        {
            this.InitializeActorMethodInfo(diagnosticsEventManager);

            diagnosticsEventManager.OnActorChangeRole += this.OnActorChangeRole;
            diagnosticsEventManager.OnActorActivated += this.OnActorActivated;
            diagnosticsEventManager.OnActorDeactivated += this.OnActorDeactivated;
            diagnosticsEventManager.OnActorMethodStart += this.OnActorMethodStart;
            diagnosticsEventManager.OnActorMethodFinish += this.OnActorMethodFinish;
            diagnosticsEventManager.OnSaveActorStateStart += this.OnSaveActorStateStart;
            diagnosticsEventManager.OnSaveActorStateFinish += this.OnSaveActorStateFinish;
            diagnosticsEventManager.OnPendingActorMethodCallsUpdated += this.OnPendingActorMethodCallsUpdated;
        }

        internal virtual void InitializeActorMethodInfo(DiagnosticsEventManager diagnosticsEventManager)
        {
            this.actorMethodInfo = new Dictionary<long, ActorMethodInfo>();

            foreach (var actorInterfaceType in this.ActorTypeInformation.InterfaceTypes)
            {
                diagnosticsEventManager.ActorMethodFriendlyNameBuilder.GetActorInterfaceMethodDescriptions(
                    actorInterfaceType, out var interfaceId, out var actorInterfaceMethodDescriptions);
                this.InitializeActorMethodInfo(actorInterfaceMethodDescriptions, interfaceId, this.actorMethodInfo);
            }
        }

        internal void InitializeActorMethodInfo(
            MethodDescription[] actorInterfaceMethodDescriptions,
            int interfaceId,
            Dictionary<long, ActorMethodInfo> actorMethodInfos)
        {
            foreach (var actorInterfaceMethodDescription in actorInterfaceMethodDescriptions)
            {
                var methodInfo = actorInterfaceMethodDescription.MethodInfo;
                var ami = new ActorMethodInfo()
                {
                    MethodName = string.Concat(methodInfo.DeclaringType.Name, ".", methodInfo.Name),
                    MethodSignature = actorInterfaceMethodDescription.MethodInfo.ToString(),
                };

                var key =
                    DiagnosticsEventManager.GetInterfaceMethodKey(
                        (uint)interfaceId,
                        (uint)actorInterfaceMethodDescription.Id);
                actorMethodInfos[key] = ami;
            }
        }

        internal virtual ActorMethodInfo GetActorMethodInfo(long key, RemotingListenerVersion remotingListener)
        {
            var methodInfo = this.actorMethodInfo[key];
            return methodInfo;
        }

        private void OnActorChangeRole(ChangeRoleDiagnosticData changeRoleData)
        {
            if (changeRoleData.NewRole == ReplicaRole.Primary)
            {
                this.writer.ReplicaChangeRoleToPrimary(this.serviceContext);
            }
            else if (changeRoleData.CurrentRole == ReplicaRole.Primary)
            {
                this.writer.ReplicaChangeRoleFromPrimary(this.serviceContext);
            }
        }

        private void OnActorActivated(ActivationDiagnosticData activationData)
        {
            var actorId = activationData.ActorId;
            this.writer.ActorActivated(
                this.actorType,
                actorId,
                this.serviceContext);
        }

        private void OnActorDeactivated(ActivationDiagnosticData activationData)
        {
            var actorId = activationData.ActorId;
            this.writer.ActorDeactivated(
                this.actorType,
                actorId,
                this.serviceContext);
        }

        private void OnActorMethodStart(ActorMethodDiagnosticData methodData)
        {
            if (this.writer.IsActorMethodStartEventEnabled())
            {
                var actorId = methodData.ActorId;
                var methodInfo = this.GetActorMethodInfo(methodData.InterfaceMethodKey, methodData.RemotingListener);
                this.writer.ActorMethodStart(
                    methodInfo.MethodName,
                    methodInfo.MethodSignature,
                    this.actorType,
                    actorId,
                    this.serviceContext);
            }
        }

        private void OnActorMethodFinish(ActorMethodDiagnosticData methodData)
        {
            if (methodData.Exception == null)
            {
                if (this.writer.IsActorMethodStopEventEnabled())
                {
                    var actorId = methodData.ActorId;
                    var methodInfo = this.GetActorMethodInfo(methodData.InterfaceMethodKey, methodData.RemotingListener);
                    this.writer.ActorMethodStop(
                        methodData.MethodExecutionTime.Value.Ticks,
                        methodInfo.MethodName,
                        methodInfo.MethodSignature,
                        this.actorType,
                        actorId,
                        this.serviceContext);
                }
            }
            else
            {
                var actorId = methodData.ActorId;
                var methodInfo = this.GetActorMethodInfo(methodData.InterfaceMethodKey, methodData.RemotingListener);
                this.writer.ActorMethodThrewException(
                    methodData.Exception.ToString(),
                    methodData.MethodExecutionTime.Value.Ticks,
                    methodInfo.MethodName,
                    methodInfo.MethodSignature,
                    this.actorType,
                    actorId,
                    this.serviceContext);
            }
        }

        private void OnPendingActorMethodCallsUpdated(PendingActorMethodDiagnosticData pendingMethodData)
        {
            if (this.writer.IsPendingMethodCallsEventEnabled())
            {
                var actorId = pendingMethodData.ActorId;
                this.writer.ActorMethodCallsWaitingForLock(
                    pendingMethodData.PendingActorMethodCalls,
                    this.actorType,
                    actorId,
                    this.serviceContext);
            }
        }

        private void OnSaveActorStateStart(ActorStateDiagnosticData stateData)
        {
            if (this.writer.IsActorSaveStateStartEventEnabled())
            {
                var actorId = stateData.ActorId;
                this.writer.ActorSaveStateStart(
                    this.actorType,
                    actorId,
                    this.serviceContext);
            }
        }

        private void OnSaveActorStateFinish(ActorStateDiagnosticData stateData)
        {
            if (this.writer.IsActorSaveStateStopEventEnabled())
            {
                var actorId = stateData.ActorId;
                this.writer.ActorSaveStateStop(
                    stateData.OperationTime.Value.Ticks,
                    this.actorType,
                    actorId,
                    this.serviceContext);
            }
        }

        internal class ActorMethodInfo
        {
            internal string MethodName { get; set; }

            internal string MethodSignature { get; set; }
        }
    }
}
