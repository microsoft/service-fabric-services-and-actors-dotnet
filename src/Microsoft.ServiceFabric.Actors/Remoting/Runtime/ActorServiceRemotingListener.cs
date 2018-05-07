// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.Runtime
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting;

    internal static class ActorServiceRemotingListener
    {
#if !DotNetCoreClr
        public static ICommunicationListener CreateActorServiceRemotingListener(
            ActorService actorService)
        {
            var types = new List<Type> { actorService.ActorTypeInformation.ImplementationType };
            types.AddRange(actorService.ActorTypeInformation.InterfaceTypes);

            var provider = ActorRemotingProviderAttribute.GetProvider(types);
            if (provider.RemotingListener.Equals(RemotingListener.V2Listener))
            {
                return provider.CreateServiceRemotingListenerV2(actorService);
            }

            return provider.CreateServiceRemotingListener(actorService);
        }

#endif

    }
}
