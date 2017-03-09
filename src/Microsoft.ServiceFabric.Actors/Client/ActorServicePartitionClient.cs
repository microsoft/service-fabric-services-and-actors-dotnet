// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Common;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    /// <summary>
    /// Specifies the Service partition client for Actor communication
    /// </summary>
    internal class ActorServicePartitionClient : ServiceRemotingPartitionClient, IActorServicePartitionClient
    {
        private readonly ActorId actorId;

        /// <summary>
        /// Instantiates a Partition client for a singleton partitioned service with the given name.
        /// </summary>
        /// <param name="remotingClientFactory">Remoting client factory</param>
        /// <param name="serviceUri">Actor service name</param>
        /// <param name="actorId">Actor id</param>
        /// <param name="listenerName">
        /// By default an actor service has only one listener for clients to connect to and communicate with.
        /// However it is possible to configure an actor service with more than one listeners, the listenerName parameter specifies the name of the listener to connect to.
        /// </param>
        /// <param name="retrySettings">Retry settings for the remote calls made by the partition client.</param>
        public ActorServicePartitionClient(
            IServiceRemotingClientFactory remotingClientFactory,
            Uri serviceUri,
            ActorId actorId,
            string listenerName = null,
            OperationRetrySettings retrySettings = null)
            : base(
                remotingClientFactory, 
                serviceUri, 
                new ServicePartitionKey(actorId.GetPartitionKey()),
                TargetReplicaSelector.Default,
                listenerName,
                retrySettings)
        {
            this.actorId = actorId;
        }

        /// <summary>
        /// Actor id. Actor id is used to identify the partition of the service that this actor
        /// belongs to.
        /// </summary>
        /// <value>actor id</value>
        public ActorId ActorId
        {
            get { return this.actorId; } 
        }

        internal Task SubscribeAsync(int eventInterfaceId, Guid subscriberId)
        {
            var headers = new ActorMessageHeaders
            {
                ActorId = this.actorId,
                InterfaceId = ActorEventSubscription.InterfaceId,
                MethodId = ActorEventSubscription.SubscribeMethodId,
            };

            var serviceMessageHeaders = headers.ToServiceMessageHeaders();
            serviceMessageHeaders.InterfaceId = ActorEventSubscription.InterfaceId;
            serviceMessageHeaders.MethodId = ActorEventSubscription.SubscribeMethodId;

            var msgBody = new ActorMessageBody()
            {
                Value = new EventSubscriptionRequestBody()
                {
                    eventInterfaceId = eventInterfaceId,
                    subscriptionId = subscriberId
                }
            };

            var msgBodyBytes = SerializationUtility.Serialize(ActorEventSubscription.Serializer, msgBody);

            return this.InvokeWithRetryAsync(
                client => client.RequestResponseAsync(serviceMessageHeaders, msgBodyBytes),
                CancellationToken.None);
        }

        internal Task UnsubscribeAsync(int eventInterfaceId, Guid subscriberId)
        {
            var headers = new ActorMessageHeaders
            {
                ActorId = this.actorId,
                InterfaceId = ActorEventSubscription.InterfaceId,
                MethodId = ActorEventSubscription.UnSubscribeMethodId,
            };

            var serviceMessageHeaders = headers.ToServiceMessageHeaders();
            serviceMessageHeaders.InterfaceId = ActorEventSubscription.InterfaceId;
            serviceMessageHeaders.MethodId = ActorEventSubscription.UnSubscribeMethodId;

            var msgBody = new ActorMessageBody()
            {
                Value = new EventSubscriptionRequestBody()
                {
                    eventInterfaceId = eventInterfaceId,
                    subscriptionId = subscriberId
                }
            };

            var msgBodyBytes = SerializationUtility.Serialize(ActorEventSubscription.Serializer, msgBody);

            return this.InvokeWithRetryAsync(
                client => client.RequestResponseAsync(serviceMessageHeaders, msgBodyBytes),
                CancellationToken.None);
        }

        internal Task<byte[]> InvokeAsync(ActorMessageHeaders headers, byte[] requestMsgBody, CancellationToken cancellationToken)
        {
            var serviceMessageHeaders = headers.ToServiceMessageHeaders();
            serviceMessageHeaders.InterfaceId = ActorMessageDispatch.InterfaceId;

            return this.InvokeAsync(
                serviceMessageHeaders,
                requestMsgBody,
                cancellationToken);
        }
    }
}
