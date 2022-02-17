// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Actors.Remoting.V2;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.FabricTransport.Client;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    // TODO: Public
    internal class DefaultActorRequestForwarder : IRequestForwarder
    {
        private static readonly string TraceType = typeof(DefaultActorRequestForwarder).Name;
        private ServiceRemotingPartitionClient remotingClient;
        private EventSubscriptionCache eventCache;
        private IServiceRemotingCallbackMessageHandler callbackHandler;
        private ActorService actorService;
        private string traceId;

        internal DefaultActorRequestForwarder(
            ActorService actorService,
            RequestForwarderContext forwardContext,
            string listenerName = null,
            Func<IServiceRemotingCallbackMessageHandler, IServiceRemotingClientFactory> createServiceRemotingClientFactory = null,
            OperationRetrySettings retrySettings = null)
        {
            this.actorService = actorService;
            this.eventCache = new EventSubscriptionCache();
            this.callbackHandler = new ActorEventForwarder(this.actorService, this.eventCache);
            var serviceRemotingClientFactory = createServiceRemotingClientFactory == null
                ? new FabricTransportActorRemotingClientFactory(this.callbackHandler)
                : createServiceRemotingClientFactory.Invoke(this.callbackHandler);

            this.remotingClient = new ServiceRemotingPartitionClient(
                serviceRemotingClientFactory,
                forwardContext.ServiceUri,
                forwardContext.ServicePartitionKey,
                forwardContext.ReplicaSelector,
                listenerName ?? Runtime.Migration.Constants.MigrationListenerName,
                retrySettings);

            this.traceId = forwardContext.TraceId;
        }

        public async Task<IServiceRemotingResponseMessage> ForwardRequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            IServiceRemotingRequestMessage requestMessage)
        {
            ActorId actorId = null;
            if (requestMessage.GetHeader() is IActorRemotingMessageHeaders actorHeaders)
            {
                actorId = ((IActorRemotingMessageHeaders)requestMessage.GetHeader()).ActorId;
                if (actorHeaders.InterfaceId == ActorEventSubscription.InterfaceId)
                {
                    var requestMsgBody = requestMessage.GetBody();
                    if (actorHeaders.MethodId == ActorEventSubscription.SubscribeMethodId)
                    {
                        var castedRequestMsgBody =
                            (EventSubscriptionRequestBody)requestMsgBody.GetParameter(
                                0,
                                "Value",
                                typeof(EventSubscriptionRequestBody));

                        this.eventCache.AddToCache(actorHeaders.ActorId, castedRequestMsgBody.EventInterfaceId, castedRequestMsgBody.SubscriptionId, requestContext.GetCallBackClient());
                    }
                    else if (actorHeaders.MethodId == ActorEventSubscription.UnSubscribeMethodId)
                    {
                        var castedRequestMsgBody =
                            (EventSubscriptionRequestBody)requestMsgBody.GetParameter(
                                0,
                                "Value",
                                typeof(EventSubscriptionRequestBody));

                        this.eventCache.RemoveFromCache(actorHeaders.ActorId, castedRequestMsgBody.EventInterfaceId, castedRequestMsgBody.SubscriptionId);
                    }
                }
            }

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, $"Forwarding actor request - ActorId : {actorId}, MethodName : {requestMessage.GetHeader().MethodName}");

            requestMessage.GetHeader().AddHeader("ActorRequestForwarded", new byte[0]);
            var retVal = await this.remotingClient.InvokeAsync(requestMessage, requestMessage.GetHeader().MethodName, CancellationToken.None);

            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, $"Successfully received response for the forwarded actor request - ActorId : {actorId}, MethodName : {requestMessage.GetHeader().MethodName}");

            return retVal;
        }
    }
}
