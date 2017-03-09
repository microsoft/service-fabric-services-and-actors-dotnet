// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Remoting.Runtime
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal static class ActorServiceRemotingListener 
    {
        public static ICommunicationListener CreateActorServiceRemotingListener(
            ActorService actorService)
        {
            var types = new List<Type> { actorService.ActorTypeInformation.ImplementationType };
            types.AddRange(actorService.ActorTypeInformation.InterfaceTypes);

            var provider = ActorRemotingProviderAttribute.GetProvider(types);
            return provider.CreateServiceRemotingListener(actorService);
        }
    }
}
