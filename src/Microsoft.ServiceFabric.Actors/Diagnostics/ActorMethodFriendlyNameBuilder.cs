// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Actors.Remoting.Description;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    internal class ActorMethodFriendlyNameBuilder
    {
        private readonly Dictionary<Type, ActorInterfaceDescription> actorMethodDescriptions;
        private readonly Dictionary<Type, ActorInterfaceDescription> actorMethodDescriptionsV2;

        internal ActorMethodFriendlyNameBuilder(ActorTypeInformation actorTypeInformation)
        {
            this.actorMethodDescriptions = new Dictionary<Type, ActorInterfaceDescription>();
            this.actorMethodDescriptionsV2 = new Dictionary<Type, ActorInterfaceDescription>();

            foreach (var actorInterfaceType in actorTypeInformation.InterfaceTypes)
            {
                var actorInterfaceDescription = ActorInterfaceDescription.Create(actorInterfaceType);
                this.actorMethodDescriptions[actorInterfaceType] = actorInterfaceDescription;
                var actorInterfaceDescriptionV2 = ActorInterfaceDescription.CreateUsingCRCId(actorInterfaceType);
                this.actorMethodDescriptionsV2[actorInterfaceType] = actorInterfaceDescriptionV2;
            }
        }

        internal void GetActorInterfaceMethodDescriptions(
            Type interfaceType,
            out int interfaceId,
            out MethodDescription[] actorInterfaceMethodDescriptions)
        {
            interfaceId = this.actorMethodDescriptions[interfaceType].Id;
            actorInterfaceMethodDescriptions = this.actorMethodDescriptions[interfaceType].Methods;
        }

        internal void GetActorInterfaceMethodDescriptionsV2(
            Type interfaceType,
            out int interfaceId,
            out MethodDescription[] actorInterfaceMethodDescriptions)
        {
            interfaceId = this.actorMethodDescriptionsV2[interfaceType].Id;
            actorInterfaceMethodDescriptions = this.actorMethodDescriptionsV2[interfaceType].Methods;
        }
    }
}
