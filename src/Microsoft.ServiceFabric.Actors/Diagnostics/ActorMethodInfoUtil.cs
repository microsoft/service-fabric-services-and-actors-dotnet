// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Description;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    static class ActorMethodInfoUtil
    {
        internal static long GetInterfaceMethodKey(uint interfaceId, uint methodId)
        {
            var key = (ulong)methodId;
            key = key | (ulong)interfaceId << 32;
            return (long)key;
        }

        internal static IReadOnlyDictionary<long, ActorMethodInfo> BuildActorMethodInfo(ActorMethodFriendlyNameBuilder nameBuilder, ActorTypeInformation typeInfo)
        {
            var actorMethodInfos = new Dictionary<long, ActorMethodInfo>();

            foreach (Type actorInterfaceType in typeInfo.InterfaceTypes)
            {
                nameBuilder.GetActorInterfaceMethodDescriptionsV2(actorInterfaceType, out var interfaceId, out var actorInterfaceMethodDescriptions);
                foreach (MethodDescription actorInterfaceMethodDescription in actorInterfaceMethodDescriptions)
                {
                    var methodInfo = actorInterfaceMethodDescription.MethodInfo;
                    var actorMethodInfo = new ActorMethodInfo(string.Concat(methodInfo.DeclaringType.Name, ".", methodInfo.Name), methodInfo.ToString());

                    actorMethodInfos[GetInterfaceMethodKey((uint)interfaceId, (uint)actorInterfaceMethodDescription.Id)] = actorMethodInfo;
                }
            }

            return new ReadOnlyDictionary<long, ActorMethodInfo>(actorMethodInfos);
        }
    }
}
