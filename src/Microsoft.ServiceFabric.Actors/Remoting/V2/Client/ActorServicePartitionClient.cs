// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

    internal class ActorServicePartitionClient : ServiceRemotingPartitionClient, IActorServicePartitionClient
    {
        private IServiceRemotingMessageBodyFactory messageBodyFactory;

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
            this.ActorId = actorId;
            this.messageBodyFactory = remotingClientFactory.GetRemotingMessageBodyFactory();
        }

        /// <summary>
        /// Gets the Actor id. Actor id is used to identify the partition of the service that this actor
        /// belongs to.
        /// </summary>
        /// <value>actor id</value>
        public ActorId ActorId { get; }

        internal Task SubscribeAsync(int eventInterfaceId, Guid subscriberId)
        {
            var actorRemotingMessageHeaders = new ActorRemotingMessageHeaders
            {
                ActorId = this.ActorId,
                InterfaceId = ActorEventSubscription.InterfaceId,
                MethodId = ActorEventSubscription.SubscribeMethodId,
                MethodName = ActorEventSubscription.SubscribeMethodName,
            };

            var msgBody = new ServiceRemotingRequestMessageBody(1);
            msgBody.SetParameter(0, "Value", new EventSubscriptionRequestBody()
            {
                EventInterfaceId = eventInterfaceId,
                SubscriptionId = subscriberId,
            });

            return this.InvokeWithRetryAsync(
                client => client.RequestResponseAsync(
                    new ServiceRemotingRequestMessage(actorRemotingMessageHeaders, msgBody)),
                CancellationToken.None);
        }

        internal Task UnsubscribeAsync(int eventInterfaceId, Guid subscriberId)
        {
            var headers = new ActorRemotingMessageHeaders
            {
                ActorId = this.ActorId,
                InterfaceId = ActorEventSubscription.InterfaceId,
                MethodId = ActorEventSubscription.UnSubscribeMethodId,
                MethodName = ActorEventSubscription.UnSubscribeMethodName,
            };

            var msgBody = new ServiceRemotingRequestMessageBody(1);

            msgBody.SetParameter(0, "Value", new EventSubscriptionRequestBody()
            {
                EventInterfaceId = eventInterfaceId,
                SubscriptionId = subscriberId,
            });

            return this.InvokeWithRetryAsync(
                client => client.RequestResponseAsync(new ServiceRemotingRequestMessage(headers, msgBody)),
                CancellationToken.None);
        }
    }
}
