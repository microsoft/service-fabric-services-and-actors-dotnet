// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Diagnostics;


    internal class ActorServiceFactory
    {
        private readonly ActorTypeInformation actorTypeInformation;
        private readonly ActorMethodFriendlyNameBuilder methodFriendlyNameBuilder;
        private readonly Func<StatefulServiceContext, ActorTypeInformation, ActorService> actorServiceFactory;

        public ActorServiceFactory(
            ActorTypeInformation actorTypeInformation,
            ActorMethodFriendlyNameBuilder methodFriendlyNameBuilder,
            Func<StatefulServiceContext, ActorTypeInformation, ActorService> actorServiceFactory)
        {
            this.actorTypeInformation = actorTypeInformation;
            this.methodFriendlyNameBuilder = methodFriendlyNameBuilder;
            this.actorServiceFactory = actorServiceFactory;
        }

        public ActorService CreateActorService(StatefulServiceContext context)
        {
            var serviceReplica = this.actorServiceFactory.Invoke(context, this.actorTypeInformation);

            // Initialize here so that service can set function in constructor.
            serviceReplica.StateProvider.Initialize(this.actorTypeInformation);

            serviceReplica.InitializeInternal(this.methodFriendlyNameBuilder);

            return serviceReplica;
        }
    }
}
