// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Remoting.FabricTransport.Runtime
{
    using System.Fabric;
    using System.Fabric.Common;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Remoting.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;

    /// <summary>
    ///     An <see cref="Microsoft.ServiceFabric.Services.Remoting.Runtime.IServiceRemotingListener"/> 
    ///     that uses fabric TCP transport to provide remoting of actor and service interfaces for actor 
    ///     service.
    /// </summary>
    public class FabricTransportActorServiceRemotingListener : FabricTransportServiceRemotingListener
    {
        /// <summary>
        ///     Construct a fabric TCP transport based service remoting listener for the specified actor service.
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
        ///     Construct a fabric TCP transport based service remoting listener for the specified actor service.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="messageHandler">
        ///     The handler for processing remoting messages. As the messages are received,
        ///     the listener delivers them to this handler.
        /// </param>
        /// <param name="listenerSettings"></param>
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
                listenerSettings = GetActorListenerSettings(actorService);
            }

            if (listenerSettings.EndpointResourceName.Equals(FabricTransportRemotingListenerSettings.DefaultEndpointResourceName))
            {
                listenerSettings.EndpointResourceName = ActorNameFormat.GetFabricServiceEndpointName(
                    actorService.ActorTypeInformation.ImplementationType);
            }
            return listenerSettings;
        }

        internal static FabricTransportRemotingListenerSettings GetActorListenerSettings(ActorService actorService)
        {
            var sectionName = ActorNameFormat.GetFabricServiceTransportSettingsSectionName(
                actorService.ActorTypeInformation.ImplementationType);
            FabricTransportRemotingListenerSettings listenerSettings;
            var isSucceded = FabricTransportRemotingListenerSettings.TryLoadFrom(sectionName, out listenerSettings);
            if (!isSucceded)
            {
                listenerSettings = FabricTransportRemotingListenerSettings.GetDefault();
            }

            return listenerSettings;
        }
    }
}
