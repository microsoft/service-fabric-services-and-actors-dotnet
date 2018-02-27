// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Wcf.Runtime
{
    using System;
    using System.Fabric;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Remoting.V2;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime;

    /// <summary>
    /// An <see cref="IServiceRemotingListener"/> that uses
    /// Windows Communication Foundation to provide interface remoting for actor services.
    /// </summary>
    public class WcfActorServiceRemotingListener : WcfServiceRemotingListener
    {
        /// <summary>
        /// Constructs a WCF based actor remoting listener.
        /// </summary>
        /// <param name="listenerBinding">WCF binding to use for the listener. If the listener binding is not specified or null,
        /// a default listener binding is created using <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.WcfUtility.CreateTcpListenerBinding"/> method which creates
        /// a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="actorService">The actor service.</param>
        public WcfActorServiceRemotingListener(
            ActorService actorService,
            Binding listenerBinding = null)
            : base(
                GetContext(actorService),
                new ActorServiceRemotingDispatcher(actorService, new DataContractRemotingMessageFactory()),
                new ActorRemotingSerializationManager(
                    new BasicDataContractSerializationProvider(),
                    new BasicDataContractActorHeaderSerializer()),
                listenerBinding,
                ActorNameFormat.GetFabricServiceEndpointName(actorService.ActorTypeInformation.ImplementationType))
        {
        }

        /// <summary>
        /// Constructs a WCF based service remoting listener.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="serviceRemotingMessageHandler">The handler for receiving and processing remoting messages. As the messages are received
        /// the listener delivers the messages to the handler.
        /// </param>
        /// <param name="serializationProvider"></param>
        /// <param name="listenerBinding">WCF binding to use for the listener. If the listener binding is not specified or null,
        /// a default listener binding is created using <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.WcfUtility.CreateTcpListenerBinding"/> method.
        /// </param>
        /// <param name="address">The endpoint address to use for the WCF listener. If not specified or null, the endpoint
        /// address is created using the default endpoint resource named "ServiceEndpoint" defined in the service manifest.
        /// </param>
        public WcfActorServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler serviceRemotingMessageHandler,
            IServiceRemotingMessageSerializationProvider serializationProvider,
            Binding listenerBinding = null,
            EndpointAddress address = null)
            : base(
                serviceContext,
                serviceRemotingMessageHandler,
                new ActorRemotingSerializationManager(
                    serializationProvider ?? new BasicDataContractSerializationProvider(),
                    new BasicDataContractActorHeaderSerializer()),
                listenerBinding,
                address)
        {
        }

        private static ServiceContext GetContext(ActorService actorService)
        {
            if (actorService == null)
            {
                throw new ArgumentNullException("actorService");
            }

            return actorService.Context;
        }
    }
}
