// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.FabricTransport.Runtime
{
    using System.Fabric;
    using System.Fabric.Common;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    /// <summary>
    ///     An <see cref="IServiceRemotingListener"/>
    ///     that uses fabric TCP transport to provide remoting of actor and service interfaces for actor
    ///     service.
    /// </summary>
    public class FabricTransportActorServiceRemotingListener : FabricTransportServiceRemotingListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportActorServiceRemotingListener"/> class.
        /// This is a Service Fabric TCP transport based service remoting listener for the specified actor service.
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
            : this(
                GetContext(actorService),
                new ActorServiceRemotingDispatcher(actorService, new DataContractRemotingMessageFactory()),
                SetEndPointResourceName(listenerSettings, actorService))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportActorServiceRemotingListener"/> class.
        /// This is a Service Fabric TCP transport based service remoting listener for the specified actor service.
        /// </summary>
        /// <param name="actorService">
        ///     The implementation of the actor service.
        /// </param>
        /// <param name="serializationProvider">
        /// It is used to serialize deserialize request and response body.
        /// </param>
        /// <param name="listenerSettings">
        ///     The settings to use for the listener.
        /// </param>
        public FabricTransportActorServiceRemotingListener(
            ActorService actorService,
            IServiceRemotingMessageSerializationProvider serializationProvider,
            FabricTransportRemotingListenerSettings listenerSettings = null)
            : this(
                GetContext(actorService),
                new ActorServiceRemotingDispatcher(actorService, serializationProvider.CreateMessageBodyFactory()),
                SetEndPointResourceName(listenerSettings, actorService),
                serializationProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportActorServiceRemotingListener"/> class.
        /// This is a Service Fabric TCP transport based service remoting listener for the specified actor service.
        /// </summary>
        /// <param name="serviceContext">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="messageHandler">
        ///     The handler for processing remoting messages. As the messages are received,
        ///     the listener delivers them to this handler.
        /// </param>
        /// <param name="listenerSettings">Listener Settings.</param>
        /// <param name="serializationProvider">Serialization provider for remoting.</param>
        public FabricTransportActorServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler messageHandler,
            FabricTransportRemotingListenerSettings listenerSettings = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null)
            : base(
                serviceContext,
                messageHandler,
                InitializeSerializerManager(
                   listenerSettings,
                   serializationProvider),
                listenerSettings)
        {
        }

        private static ActorRemotingSerializationManager InitializeSerializerManager(
            FabricTransportRemotingListenerSettings listenerSettings,
            IServiceRemotingMessageSerializationProvider serializationProvider)
        {
            listenerSettings = listenerSettings ?? FabricTransportRemotingListenerSettings.GetDefault();

            return new ActorRemotingSerializationManager(
                serializationProvider,
                new ActorRemotingMessageHeaderSerializer(
                    listenerSettings.HeaderBufferSize,
                    listenerSettings.HeaderMaxBufferCount));
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
                listenerSettings.EndpointResourceName = ActorNameFormat.GetFabricServiceV2EndpointName(
                    actorService.ActorTypeInformation.ImplementationType);
            }

            return listenerSettings;
        }
    }
}
