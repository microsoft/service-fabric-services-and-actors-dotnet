// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Wcf.Client
{
    using System.Collections.Generic;
    using System.ServiceModel.Channels;
    using Microsoft.ServiceFabric.Actors.Remoting.Client;
    using Microsoft.ServiceFabric.Actors.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Client;

    /// <summary>
    ///     An <see cref="IServiceRemotingClientFactory"/> that uses
    ///     Windows Communication Foundation to create <see cref="IServiceRemotingClient"/>
    ///     to communicate with an actor service and actors hosted by it, using actor and service interfaces that are remoted via
    ///     <see cref="Microsoft.ServiceFabric.Actors.Remoting.V2.Wcf.Runtime.WcfActorServiceRemotingListener"/>.
    /// </summary>
    public class WcfActorRemotingClientFactory : WcfServiceRemotingClientFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WcfActorRemotingClientFactory"/> class.
        /// </summary>
        /// <param name="callbackClient">
        ///     The callback client that receives the callbacks from the service.
        /// </param>
        public WcfActorRemotingClientFactory(
            IServiceRemotingCallbackMessageHandler callbackClient)
            : this(null, callbackClient)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfActorRemotingClientFactory"/> class.
        /// </summary>
        /// <param name="clientBinding">
        ///     WCF binding to use for the client. If the client binding is null,
        ///     a default client binding is created using
        ///     <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.WcfUtility.CreateTcpClientBinding"/> method
        ///     which creates a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="callbackClient">
        ///     The callback client that receives the callbacks from the service.
        /// </param>
        /// <param name="exceptionHandlers">
        ///     Exception handlers to handle the exceptions encountered in communicating with the service.
        /// </param>
        /// <param name="servicePartitionResolver">
        ///     Service partition resolver to resolve the service endpoints. If not specified, a default
        ///     service partition resolver returned by <see cref="ServicePartitionResolver.GetDefault"/> is used.
        /// </param>
        /// <param name="traceId">
        ///     Id to use in diagnostics traces from this component.
        /// </param>
        /// <param name="serializationProvider">Serialization Provider</param>
        /// <param name="useWrappedMessage">
        /// It indicates whether the remoting method parameters should be wrapped or not before sending it over the wire. When UseWrappedMessage is set to false, parameters  will not be wrapped. When this value is set to true, the parameters will be wrapped.Default value is false.</param>
        /// <remarks>
        ///     This factory uses <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.Client.WcfExceptionHandler"/>,
        ///     <see cref="Microsoft.ServiceFabric.Actors.Remoting.Client.ActorRemotingExceptionHandler"/>, in addition to the
        ///     exception handlers supplied to the constructor.
        /// </remarks>
        public WcfActorRemotingClientFactory(
            Binding clientBinding,
            IServiceRemotingCallbackMessageHandler callbackClient,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            IServicePartitionResolver servicePartitionResolver = null,
            string traceId = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null,
            bool useWrappedMessage = false)
            : base(
                InitializeSerializerManager(serializationProvider, useWrappedMessage),
                clientBinding,
                callbackClient,
                GetExceptionHandlers(exceptionHandlers),
                servicePartitionResolver,
                traceId)
        {
        }

        private static ActorRemotingSerializationManager InitializeSerializerManager(
            IServiceRemotingMessageSerializationProvider serializationProvider,
            bool useWrappedMessage)
        {
            if (serializationProvider == null)
            {
                if (useWrappedMessage)
                {
                    serializationProvider = new ActorRemotingWrappingDataContractSerializationProvider(null);
                }

                serializationProvider = new ActorRemotingDataContractSerializationProvider(null);
            }

            return new ActorRemotingSerializationManager(
                serializationProvider,
                new BasicDataContractActorHeaderSerializer());
        }

        private static IEnumerable<IExceptionHandler> GetExceptionHandlers(IEnumerable<IExceptionHandler> exceptionHandlers)
        {
            var handlers = new List<IExceptionHandler>();
            if (exceptionHandlers != null)
            {
                handlers.AddRange(exceptionHandlers);
            }

            handlers.Add(new ActorRemotingExceptionHandler());
            return handlers;
        }
    }
}
