// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.FabricTransport.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Common;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Migration;
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
        /// <param name="exceptionConvertors">Convertors to convert user exception to service exception.</param>
        /// <param name="requestForwarderFactory">Request forwarder incase migration is ongoing and current service cannot service the request.</param>
        public FabricTransportActorServiceRemotingListener(
            ActorService actorService,
            FabricTransportRemotingListenerSettings listenerSettings = null,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null,
            Func<RequestForwarderContext, IRequestForwarder> requestForwarderFactory = null)
            : this(
                actorService,
                CreateActorRemotingDispatcher(actorService, listenerSettings),
                SetEndPointResourceName(listenerSettings, actorService),
                exceptionConvertors: exceptionConvertors,
                requestForwarderFactory: requestForwarderFactory)
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
        /// <param name="exceptionConvertors">Convertors to convert user exception to service exception.</param>
        /// <param name="requestForwarderFactory">Request forwarder incase migration is ongoing and current service cannot service the request.</param>
        public FabricTransportActorServiceRemotingListener(
            ActorService actorService,
            IServiceRemotingMessageSerializationProvider serializationProvider,
            FabricTransportRemotingListenerSettings listenerSettings = null,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null,
            Func<RequestForwarderContext, IRequestForwarder> requestForwarderFactory = null)
            : this(
                actorService,
                new ActorServiceRemotingDispatcher(actorService, serializationProvider.CreateMessageBodyFactory()),
                SetEndPointResourceName(listenerSettings, actorService),
                serializationProvider,
                exceptionConvertors,
                requestForwarderFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportActorServiceRemotingListener"/> class.
        /// This is a Service Fabric TCP transport based service remoting listener for the specified actor service.
        /// This constructor is deprecated, use <see cref="FabricTransportActorServiceRemotingListener(ActorService, IServiceRemotingMessageHandler, FabricTransportRemotingListenerSettings, IServiceRemotingMessageSerializationProvider, IEnumerable{IExceptionConvertor}, Func{RequestForwarderContext, IRequestForwarder})"/>
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
        [Obsolete("Deprecated, use FabricTransportActorServiceRemotingListener(ActorService, IServiceRemotingMessageHandler, FabricTransportRemotingListenerSettings, IServiceRemotingMessageSerializationProvider)")]
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

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportActorServiceRemotingListener"/> class.
        /// This is a Service Fabric TCP transport based service remoting listener for the specified actor service.
        /// </summary>
        /// <param name="actorService">
        ///     The context of the service for which the remoting listener is being constructed.
        /// </param>
        /// <param name="messageHandler">
        ///     The handler for processing remoting messages. As the messages are received,
        ///     the listener delivers them to this handler.
        /// </param>
        /// <param name="listenerSettings">Listener Settings.</param>
        /// <param name="serializationProvider">Serialization provider for remoting.</param>
        /// <param name="exceptionConvertors">Convertors to convert user exception to service exception.</param>
        /// <param name="requestForwarderFactory">Request forwarder incase migration is ongoing and current service cannot service the request.</param>
        public FabricTransportActorServiceRemotingListener(
            ActorService actorService,
            IServiceRemotingMessageHandler messageHandler,
            FabricTransportRemotingListenerSettings listenerSettings = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null,
            Func<RequestForwarderContext, IRequestForwarder> requestForwarderFactory = null)
            : base(
                GetContext(actorService),
                OverrideMessageHandlerIfRequired(actorService, messageHandler, requestForwarderFactory),
                InitializeSerializerManager(
                   SetEndPointResourceName(listenerSettings, actorService),
                   serializationProvider),
                SetEndPointResourceName(listenerSettings, actorService),
                GetExceptionConvertors(exceptionConvertors))
        {
        }

        private static IServiceRemotingMessageHandler OverrideMessageHandlerIfRequired(ActorService actorService, IServiceRemotingMessageHandler messageHandler, Func<RequestForwarderContext, IRequestForwarder> requestForwarderFactory)
        {
            if (actorService.IsConfiguredForMigration())
            {
                return actorService.MigrationOrchestrator.GetMessageHandler(actorService, messageHandler, requestForwarderFactory);
            }

            return messageHandler;
        }

        private static IEnumerable<IExceptionConvertor> GetExceptionConvertors(IEnumerable<IExceptionConvertor> exceptionConvertors)
        {
            var actorConvertors = new List<IExceptionConvertor>();
            if (exceptionConvertors != null)
            {
                actorConvertors.AddRange(exceptionConvertors);
            }

            actorConvertors.Add(new FabricActorExceptionConvertor());

            return actorConvertors;
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
                    listenerSettings.HeaderMaxBufferCount),
                listenerSettings.UseWrappedMessage);
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
                if (listenerSettings.UseWrappedMessage)
                {
                    listenerSettings.EndpointResourceName = ActorNameFormat.GetFabricServiceWrappedMessageEndpointName(
                        actorService.ActorTypeInformation.ImplementationType);
                }
                else
                {
                    listenerSettings.EndpointResourceName = ActorNameFormat.GetFabricServiceV2EndpointName(
                    actorService.ActorTypeInformation.ImplementationType);
                }
            }

            return listenerSettings;
        }

        private static ActorServiceRemotingDispatcher CreateActorRemotingDispatcher(
            ActorService actorService,
            FabricTransportRemotingListenerSettings listenerSettings)
        {
            if (listenerSettings == null || !listenerSettings.UseWrappedMessage)
            {
                return new ActorServiceRemotingDispatcher(actorService, new DataContractRemotingMessageFactory());
            }

            return new ActorServiceRemotingDispatcher(actorService, new WrappedRequestMessageFactory());
        }
    }
}
