// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ServiceFabric.Actors.Generator;
using Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime;

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Wcf.Runtime
{
    /// <summary>
    /// An <see cref="IServiceRemotingListener"/> that uses
    /// Windows Communication Foundation to provide interface remoting for actor services.
    /// </summary>
    public class WcfActorServiceRemotingListener : WcfServiceRemotingListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WcfActorServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="actorService">The actor service.</param>
        /// <param name="listenerBinding">WCF binding to use for the listener. If the listener binding is not specified or null,
        /// a default listener binding is created using <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.WcfUtility.CreateTcpListenerBinding"/> method which creates
        /// a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="useWrappedMessage">
        ///     It indicates whether the remoting method parameters should be wrapped or not before sending it over the wire. When UseWrappedMessage is set to false,
        ///     parameters will not be wrapped. When this value is set to true, the parameters will be wrapped.Default value is false.
        /// </param>
        [Obsolete]
        public WcfActorServiceRemotingListener(
            ActorService actorService,
            Binding listenerBinding,
            bool useWrappedMessage)
            : this(
                actorService, listenerBinding, useWrappedMessage, null, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfActorServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="actorService">The actor service.</param>
        /// <param name="listenerBinding">WCF binding to use for the listener. If the listener binding is not specified or null,
        /// a default listener binding is created using <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.WcfUtility.CreateTcpListenerBinding"/> method which creates
        /// a <see cref="System.ServiceModel.NetTcpBinding"/> with no security.
        /// </param>
        /// <param name="useWrappedMessage">
        ///     It indicates whether the remoting method parameters should be wrapped or not before sending it over the wire. When UseWrappedMessage is set to false,
        ///     parameters will not be wrapped. When this value is set to true, the parameters will be wrapped.Default value is false.
        /// </param>
        /// <param name="exceptionConvertors">Exception convertors to use for converting exceptions to RemoteException2.</param>
        /// <param name="settings">Settings for the WCF remoting listener.</param>
        public WcfActorServiceRemotingListener(
            ActorService actorService,
            Binding listenerBinding = null,
            bool useWrappedMessage = false,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null,
            WcfRemotingListenerSettings settings = null)
            : base(
                GetContext(actorService),
                new ActorServiceRemotingDispatcher(actorService, GetDefaultRequestMessageFactory(useWrappedMessage)),
                GetDefaultSerializationProvider(null, useWrappedMessage),
                listenerBinding,
                ActorNameFormat.GetFabricServiceEndpointName(actorService.ActorTypeInformation.ImplementationType),
                useWrappedMessage,
                GetDefaultActorConvertors(exceptionConvertors),
                settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfActorServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="serviceRemotingMessageHandler">The handler for receiving and processing remoting messages. As the messages are received
        /// the listener delivers the messages to the handler.
        /// </param>
        /// <param name="serializationProvider">Serialization Provider.</param>
        /// <param name="listenerBinding">WCF binding to use for the listener. If the listener binding is not specified or null,
        /// a default listener binding is created using <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.WcfUtility.CreateTcpListenerBinding"/> method.
        /// </param>
        /// <param name="address">The endpoint address to use for the WCF listener. If not specified or null, the endpoint
        /// address is created using the default endpoint resource named "ServiceEndpoint" defined in the service manifest.
        /// </param>
        /// <param name="useWrappedMessage">
        ///     It indicates whether the remoting method parameters should be wrapped or not before sending it over the wire.
        ///     When UseWrappedMessage is set to false, parameters  will not be wrapped. When this value is set to true, the
        ///     parameters will be wrapped.Default value is false.
        /// </param>
        [Obsolete]
        public WcfActorServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler serviceRemotingMessageHandler,
            IServiceRemotingMessageSerializationProvider serializationProvider,
            Binding listenerBinding,
            EndpointAddress address,
            bool useWrappedMessage)
            : this(
                serviceContext,
                serviceRemotingMessageHandler,
                serializationProvider,
                listenerBinding,
                address,
                useWrappedMessage,
                null,
                null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfActorServiceRemotingListener"/> class.
        /// </summary>
        /// <param name="serviceContext">The context of the service for which the remoting listener is being constructed.</param>
        /// <param name="serviceRemotingMessageHandler">The handler for receiving and processing remoting messages. As the messages are received
        /// the listener delivers the messages to the handler.
        /// </param>
        /// <param name="serializationProvider">Serialization Provider.</param>
        /// <param name="listenerBinding">WCF binding to use for the listener. If the listener binding is not specified or null,
        /// a default listener binding is created using <see cref="Microsoft.ServiceFabric.Services.Communication.Wcf.WcfUtility.CreateTcpListenerBinding"/> method.
        /// </param>
        /// <param name="address">The endpoint address to use for the WCF listener. If not specified or null, the endpoint
        /// address is created using the default endpoint resource named "ServiceEndpoint" defined in the service manifest.
        /// </param>
        /// <param name="useWrappedMessage">
        ///     It indicates whether the remoting method parameters should be wrapped or not before sending it over the wire.
        ///     When UseWrappedMessage is set to false, parameters  will not be wrapped. When this value is set to true, the
        ///     parameters will be wrapped.Default value is false.
        /// </param>
        /// <param name="exceptionConvertors">Exception convertors to use for converting exceptions to RemoteException2.</param>
        /// <param name="settings">Settings for the WCF remoting listener.</param>
        public WcfActorServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler serviceRemotingMessageHandler,
            IServiceRemotingMessageSerializationProvider serializationProvider,
            Binding listenerBinding = null,
            EndpointAddress address = null,
            bool useWrappedMessage = false,
            IEnumerable<IExceptionConvertor> exceptionConvertors = null,
            WcfRemotingListenerSettings settings = null)
            : base(
                serviceContext,
                serviceRemotingMessageHandler,
                new ActorRemotingSerializationManager(
                    GetDefaultSerializationProvider(serializationProvider, useWrappedMessage),
                    new BasicDataContractActorHeaderSerializer()),
                listenerBinding,
                address,
                GetDefaultActorConvertors(exceptionConvertors),
                settings)
        {
        }

        private static IEnumerable<IExceptionConvertor> GetDefaultActorConvertors(IEnumerable<IExceptionConvertor> exceptionConvertors)
        {
            var convertors = new List<IExceptionConvertor>(exceptionConvertors ?? Enumerable.Empty<IExceptionConvertor>())
            {
                new FabricActorExceptionConvertor(),
            };

            return convertors;
        }

        private static IServiceRemotingMessageBodyFactory GetDefaultRequestMessageFactory(bool useWrappedMessage)
        {
            if (useWrappedMessage)
            {
                return new WrappedRequestMessageFactory();
            }

            return new DataContractRemotingMessageFactory();
        }

        private static ServiceContext GetContext(ActorService actorService)
        {
            if (actorService == null)
            {
                throw new ArgumentNullException("actorService");
            }

            return actorService.Context;
        }

        private static IServiceRemotingMessageSerializationProvider GetDefaultSerializationProvider(
          IServiceRemotingMessageSerializationProvider serviceRemotingSerializationProvider,
          bool usewrappedMessage)
        {
            if (serviceRemotingSerializationProvider == null)
            {
                if (usewrappedMessage)
                {
                    return new ActorRemotingWrappingDataContractSerializationProvider(null);
                }

                return new ActorRemotingDataContractSerializationProvider(null);
            }

            return serviceRemotingSerializationProvider;
        }
    }
}
