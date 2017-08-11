﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Diagnostics;
    using Microsoft.ServiceFabric.Actors.Remoting.Runtime;

    internal class ActorServiceFactory
    {
        private readonly ActorTypeInformation actorTypeInformation;
        private readonly ActorMethodDispatcherMap methodDispatcherMap;
        private readonly ActorMethodFriendlyNameBuilder methodFriendlyNameBuilder;

        private readonly Func<StatefulServiceContext,
            ActorTypeInformation,
            Func<
                Func<ActorService,
                    ActorId,
                    ActorBase>,
                IActorActivator>,
            ActorService> actorServiceFactory;
        
        private readonly Func<
            Func<ActorService,
                ActorId,
                ActorBase>,
            IActorActivator> actorActivatorFactory;
        
        public ActorServiceFactory(
            ActorTypeInformation actorTypeInformation,
            ActorMethodFriendlyNameBuilder methodFriendlyNameBuilder,
            Func<StatefulServiceContext, 
                 ActorTypeInformation,
                 Func<Func<ActorService,ActorId,ActorBase>,IActorActivator>,
                 ActorService> actorServiceFactory,
            ActorMethodDispatcherMap methodDispatcherMap = null,
            Func<Func<ActorService,ActorId,ActorBase>,IActorActivator> actorActivatorFactory = null)
        {
            this.actorTypeInformation = actorTypeInformation;
            this.methodDispatcherMap = methodDispatcherMap;
            this.methodFriendlyNameBuilder = methodFriendlyNameBuilder;
            this.actorServiceFactory = actorServiceFactory;
            this.actorActivatorFactory = actorActivatorFactory;
        }

        public ActorService CreateActorService(StatefulServiceContext context)
        {
            var serviceReplica = this.actorServiceFactory
                .Invoke(context, 
                    this.actorTypeInformation,
                    this.actorActivatorFactory
                );

            // Initialize here so that service can set function in constructor.
            serviceReplica.StateProvider.Initialize(this.actorTypeInformation);
            
            serviceReplica.InitializeInternal(this.methodDispatcherMap, this.methodFriendlyNameBuilder);

            return serviceReplica;
        }
    }
}
