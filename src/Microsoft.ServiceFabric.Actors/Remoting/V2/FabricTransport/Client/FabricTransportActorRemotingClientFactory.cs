// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.FabricTransport.Client
{
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Actors.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;

    /// <summary>
    /// An <see cref="IServiceRemotingClientFactory"/> that uses
    /// Fabric TCP transport to create <see cref="IServiceRemotingClient"/> that communicate with
    /// actors over interfaces that are remoted via
    /// <see cref="FabricTransportServiceRemotingListener"/>.
    /// </summary>
    public class FabricTransportActorRemotingClientFactory : FabricTransportServiceRemotingClientFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportActorRemotingClientFactory"/> class.
        /// </summary>
        /// <param name="callbackMessageHandler">
        ///     The callback client that receives the callbacks from the service.
        /// </param>
        public FabricTransportActorRemotingClientFactory(
            IServiceRemotingCallbackMessageHandler callbackMessageHandler)
            : this(FabricTransportRemotingSettings.GetDefault(), callbackMessageHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricTransportActorRemotingClientFactory"/> class.
        /// </summary>
        /// <param name="fabricTransportRemotingSettings">
        ///     The settings for the fabric transport. If the settings are not provided or null, default settings
        ///     with no security.
        /// </param>
        /// <param name="callbackMessageHandler">
        ///     The callback client that receives the callbacks from the service.
        /// </param>
        /// <param name="servicePartitionResolver">
        ///     Service partition resolver to resolve the service endpoints. If not specified, a default
        ///     service partition resolver returned by <see cref="ServicePartitionResolver.GetDefault"/> is used.
        /// </param>
        /// <param name="exceptionHandlers">
        ///     Exception handlers to handle the exceptions encountered in communicating with the actor.
        /// </param>
        /// <param name="traceId">
        ///     Id to use in diagnostics traces from this component.
        /// </param>
        /// <param name="serializationProvider">This is used to serialize remoting request/response.</param>
        public FabricTransportActorRemotingClientFactory(
            FabricTransportRemotingSettings fabricTransportRemotingSettings,
            IServiceRemotingCallbackMessageHandler callbackMessageHandler = null,
            IServicePartitionResolver servicePartitionResolver = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            string traceId = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null)
            : base(
                 IntializeSerializationManager(
                serializationProvider,
                fabricTransportRemotingSettings),
                 fabricTransportRemotingSettings,
                 callbackMessageHandler,
                 servicePartitionResolver,
                 GetExceptionHandlers(exceptionHandlers),
                 traceId)
        {
        }

        private static IEnumerable<IExceptionHandler> GetExceptionHandlers(
            IEnumerable<IExceptionHandler> exceptionHandlers)
        {
            var handlers = new List<IExceptionHandler>();
            if (exceptionHandlers != null)
            {
                handlers.AddRange(exceptionHandlers);
            }

            handlers.Add(new ActorRemotingExceptionHandler());
            return handlers;
        }

        private static ActorRemotingSerializationManager IntializeSerializationManager(
            IServiceRemotingMessageSerializationProvider serializationProvider,
            FabricTransportRemotingSettings settings)
        {
            settings = settings ?? FabricTransportRemotingSettings.GetDefault();

            return new ActorRemotingSerializationManager(
                serializationProvider,
                new ActorRemotingMessageHeaderSerializer(settings.HeaderBufferSize, settings.HeaderMaxBufferCount),
                settings.UseWrappedMessage);
        }
    }
}
