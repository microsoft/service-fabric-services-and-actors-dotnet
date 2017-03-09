// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Actors.Remoting.Description;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    class ActorMethodFriendlyNameBuilder
    {
        private readonly Dictionary<Type, Tuple<int, MethodDescription[]>> actorMethodDescriptions;

        internal ActorMethodFriendlyNameBuilder(ActorTypeInformation actorTypeInformation)
        {
            this.actorMethodDescriptions = new Dictionary<Type, Tuple<int, MethodDescription[]>>();
            foreach (var actorInterfaceType in actorTypeInformation.InterfaceTypes)
            {
                var actorInterfaceDescription = ActorInterfaceDescription.Create(actorInterfaceType);
                this.actorMethodDescriptions[actorInterfaceType] = new Tuple<int, MethodDescription[]>(
                    actorInterfaceDescription.Id,
                    actorInterfaceDescription.Methods);
            }
        }

        internal void GetActorInterfaceMethodDescriptions(Type interfaceType, out int interfaceId,
            out MethodDescription[] actorInterfaceMethodDescriptions)
        {
            interfaceId = this.actorMethodDescriptions[interfaceType].Item1;
            actorInterfaceMethodDescriptions = this.actorMethodDescriptions[interfaceType].Item2;
        }

    }
}