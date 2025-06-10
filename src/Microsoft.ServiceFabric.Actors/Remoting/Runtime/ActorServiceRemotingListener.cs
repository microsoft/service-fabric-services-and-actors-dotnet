// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace Microsoft.ServiceFabric.Actors.Remoting.Runtime
{
    internal static class ActorServiceRemotingListener
    {
#if !NET
        public static ICommunicationListener CreateActorServiceRemotingListener(
            ActorService actorService)
        {
            var types = new List<Type> { actorService.ActorTypeInformation.ImplementationType };
            types.AddRange(actorService.ActorTypeInformation.InterfaceTypes);

            var provider = ActorRemotingProviderAttribute.GetProvider(types);
            return provider.CreateServiceRemotingListeners().ElementAt(0).Value(actorService);
        }
#endif

    }
}
