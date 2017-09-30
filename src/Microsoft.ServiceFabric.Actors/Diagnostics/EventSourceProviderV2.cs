// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System.Collections.Generic;
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    class EventSourceProviderV2 : EventSourceProvider
    {
        private readonly Dictionary<long, ActorMethodInfo> actorMethodInfoV2;

        internal EventSourceProviderV2(ServiceContext serviceContext, ActorTypeInformation actorTypeInformation)
            : base(serviceContext,
                actorTypeInformation)
        {
            this.actorMethodInfoV2 = new Dictionary<long, ActorMethodInfo>();
        }


        internal override void InitializeActorMethodInfo(DiagnosticsEventManager diagnosticsEventManager)
        {
            foreach (var actorInterfaceType in this.actorTypeInformation.InterfaceTypes)
            {
                int interfaceId;
                MethodDescription[] actorInterfaceMethodDescriptions;
                diagnosticsEventManager.ActorMethodFriendlyNameBuilder.GetActorInterfaceMethodDescriptionsV2(
                    actorInterfaceType, out interfaceId, out actorInterfaceMethodDescriptions);
                this.InitializeActorMethodInfo(actorInterfaceMethodDescriptions, interfaceId, this.actorMethodInfoV2);
            }
            base.InitializeActorMethodInfo(diagnosticsEventManager);
        }


        internal override ActorMethodInfo GetActorMethodInfo(long key,RemotingListener remotingListener)
        {
            if (remotingListener.Equals(RemotingListener.V2Listener))
            {
                return this.actorMethodInfoV2[key];
            }
            
            return base.GetActorMethodInfo(key,remotingListener);
        }
    }
}