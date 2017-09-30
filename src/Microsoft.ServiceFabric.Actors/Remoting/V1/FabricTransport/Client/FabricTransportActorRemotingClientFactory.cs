// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1.FabricTransport.Client
{
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Actors.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
    using Microsoft.ServiceFabric.Services.Remoting.V1;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime;

    /// <summary>
    /// An <see cref="IServiceRemotingClientFactory"/> that uses
    /// Fabric TCP transport to create <see cref="IServiceRemotingClient"/> that communicate with 
    /// actors over interfaces that are remoted via 
    /// <see cref="FabricTransportServiceRemotingListener"/>.
    /// </summary>
    public class FabricTransportActorRemotingClientFactory : FabricTransportServiceRemotingClientFactory
    {
        /// <summary>
        /// Constructs a fabric transport based actor remoting client factory.
        /// </summary>
        /// <param name="callbackClient">
        ///     The callback client that receives the callbacks from the service.
        /// </param>
        public FabricTransportActorRemotingClientFactory(
            IServiceRemotingCallbackClient callbackClient)
            : this(FabricTransportRemotingSettings.GetDefault(), callbackClient)
        {
        }

        /// <summary>
        /// Constructs a fabric transport based actor remoting client factory.
        /// </summary>
        /// <param name="fabricTransportRemotingSettings">
        ///     The settings for the fabric transport. If the settings are not provided or null, default settings 
        ///     with no security.
        /// </param>
        /// <param name="callbackClient">
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
        public FabricTransportActorRemotingClientFactory(
            FabricTransportRemotingSettings fabricTransportRemotingSettings,
            IServiceRemotingCallbackClient callbackClient,
            IServicePartitionResolver servicePartitionResolver = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            string traceId = null) :
            base(
                fabricTransportRemotingSettings,
                callbackClient,
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
    }
}