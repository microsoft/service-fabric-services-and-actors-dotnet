// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1.Wcf.Client
{
    using System.Collections.Generic;
    using System.ServiceModel.Channels;
    using Microsoft.ServiceFabric.Actors.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Wcf.Client;

    /// <summary>
    ///     An <see cref="IServiceRemotingClientFactory"/> that uses
    ///     Windows Communication Foundation to create <see cref="IServiceRemotingClient"/>
    ///     to communicate with an actor service and actors hosted by it, using actor and service interfaces that are remoted via
    ///     <see cref="Microsoft.ServiceFabric.Actors.Remoting.V1.Wcf.Runtime.WcfActorServiceRemotingListener"/>.
    /// </summary>
    public class WcfActorRemotingClientFactory : WcfServiceRemotingClientFactory
    {
        /// <summary>
        ///     Constructs a WCF based actor remoting factory.
        /// </summary>
        /// <param name="callbackClient">
        ///     The callback client that receives the callbacks from the service.
        /// </param>
        public WcfActorRemotingClientFactory(
            IServiceRemotingCallbackClient callbackClient)
            : this(null, callbackClient)
        {
        }

        /// <summary>
        ///     Constructs a WCF based actor remoting factory.
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
        /// <remarks>
        ///     This factory uses <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.Client.WcfExceptionHandler"/>,
        ///     <see cref="Microsoft.ServiceFabric.Services.Remoting.Client.ServiceRemotingExceptionHandler"/> and
        ///     <see cref="Microsoft.ServiceFabric.Actors.Remoting.Client.ActorRemotingExceptionHandler"/>, in addition to the
        ///     exception handlers supplied to the constructor.
        /// </remarks>
        public WcfActorRemotingClientFactory(
            Binding clientBinding,
            IServiceRemotingCallbackClient callbackClient,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            IServicePartitionResolver servicePartitionResolver = null,
            string traceId = null) :
            base(
                clientBinding,
                callbackClient,
                GetExceptionHandlers(exceptionHandlers),
                servicePartitionResolver,
                traceId)
        {
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
