// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.Description
{
    using System;
    using System.Globalization;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Description;

    internal class ActorEventInterfaceDescription : InterfaceDescription
    {
        private ActorEventInterfaceDescription(Type actorEventInterfaceType, bool useCRCIdForGeneration)
            : base("actorEvent", actorEventInterfaceType, useCRCIdForGeneration, MethodReturnCheck.EnsureReturnsVoid)
        {
        }

        public static ActorEventInterfaceDescription Create(Type actorEventInterfaceType)
        {
            EnsureActorEventInterface(actorEventInterfaceType);
            return new ActorEventInterfaceDescription(actorEventInterfaceType, false);
        }

        public static ActorEventInterfaceDescription CreateUsingCRCId(Type actorEventInterfaceType)
        {
            EnsureActorEventInterface(actorEventInterfaceType);
            return new ActorEventInterfaceDescription(actorEventInterfaceType, true);
        }

        private static void EnsureActorEventInterface(Type actorEventInterfaceType)
        {
            if ((actorEventInterfaceType.GetInterfaces().Length != 1) ||
                (actorEventInterfaceType.GetInterfaces()[0] != typeof(IActorEvents)))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ErrorEventInterfaceMustBeIActorEvents,
                        actorEventInterfaceType.FullName,
                        typeof(IActorEvents)),
                    "actorEventInterfaceType");
            }
        }
    }
}
