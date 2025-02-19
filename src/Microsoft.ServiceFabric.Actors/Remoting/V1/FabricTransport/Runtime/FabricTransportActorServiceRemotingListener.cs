// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1.FabricTransport.Runtime
{
    using System;
    using System.Fabric;
    using System.Fabric.Common;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
    using Microsoft.ServiceFabric.Actors.Remoting.V1.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Runtime;

    /// <summary>
    ///     An <see cref="IServiceRemotingListener"/>
    ///     that uses fabric TCP transport to provide remoting of actor and service interfaces for actor
    ///     service.
    /// </summary>
    [Obsolete("This class is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
    public class FabricTransportActorServiceRemotingListener : FabricTransportServiceRemotingListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportActorServiceRemotingListener"/> class for the specified actor service.
        /// </summary>
        /// <param name="actorService">
        ///     The implementation of the actor service.
        /// </param>
        /// <param name="listenerSettings">
        ///     The settings to use for the listener.
        /// </param>
        public FabricTransportActorServiceRemotingListener(
            ActorService actorService,
            FabricTransportRemotingListenerSettings listenerSettings = null)
            : base(
                GetContext(actorService),
                new ActorServiceRemotingDispatcher(actorService),
                SetEndPointResourceName(listenerSettings, actorService))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportActorServiceRemotingListener"/> class for the specified actor service.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="messageHandler">
        ///     The handler for processing remoting messages. As the messages are received,
        ///     the listener delivers them to this handler.
        /// </param>
        /// <param name="listenerSettings">Settings for creating the listener.</param>
        public FabricTransportActorServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            FabricTransportRemotingListenerSettings listenerSettings = null)
            : base(
                serviceContext,
                messageHandler,
                listenerSettings)
        {
        }

        private static ServiceContext GetContext(ActorService actorService)
        {
            Requires.ThrowIfNull(actorService, "actorService");
            return actorService.Context;
        }

        private static FabricTransportRemotingListenerSettings SetEndPointResourceName(
            FabricTransportRemotingListenerSettings listenerSettings, ActorService actorService)
        {
            if (listenerSettings == null)
            {
                listenerSettings = FabricTransportActorRemotingProviderAttribute.GetActorListenerSettings(actorService);
            }

            if (listenerSettings.EndpointResourceName.Equals(FabricTransportRemotingListenerSettings
                .DefaultEndpointResourceName))
            {
                listenerSettings.EndpointResourceName = ActorNameFormat.GetFabricServiceEndpointName(
                    actorService.ActorTypeInformation.ImplementationType);
            }

            return listenerSettings;
        }
    }
}
