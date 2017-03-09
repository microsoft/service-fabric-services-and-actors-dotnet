// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Remoting.Description
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    internal class ActorInterfaceDescription : InterfaceDescription
    {
        private ActorInterfaceDescription(Type actorInterfaceType)
            : base("actor", actorInterfaceType, MethodReturnCheck.EnsureReturnsTask)
        {
        }

        public static ActorInterfaceDescription Create(Type actorInterfaceType)
        {
            EnsureActorInterface(actorInterfaceType);
            return new ActorInterfaceDescription(actorInterfaceType);
        }

        private static void EnsureActorInterface(Type actorInterfaceType)
        {
            if (!actorInterfaceType.GetTypeInfo().IsInterface)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                        SR.ErrorNotAnActorInterface_InterfaceCheck,
                        actorInterfaceType.FullName,
                        typeof(IActor).FullName),
                    "actorInterfaceType");
            }

            var nonActorParentInterface = actorInterfaceType.GetNonActorParentType();
            if (nonActorParentInterface != null)
            {
                if (nonActorParentInterface == actorInterfaceType)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture,
                            SR.ErrorNotAnActorInterface_DerivationCheck1,
                            actorInterfaceType.FullName,
                            typeof(IActor).FullName),
                        "actorInterfaceType");
                }
                else
                {
                    throw new ArgumentException(
                       string.Format(CultureInfo.CurrentCulture,
                           SR.ErrorNotAnActorInterface_DerivationCheck1,
                           actorInterfaceType.FullName,
                           nonActorParentInterface.FullName,
                           typeof(IActor).FullName),
                       "actorInterfaceType");
                }
            }
        }
    }
}