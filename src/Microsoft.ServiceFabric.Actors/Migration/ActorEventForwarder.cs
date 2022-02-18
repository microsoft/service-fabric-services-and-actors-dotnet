// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using Microsoft.ServiceFabric.Actors.Remoting.V2;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

    internal class ActorEventForwarder : IServiceRemotingCallbackMessageHandler
    {
        private static readonly string TraceType = typeof(ActorEventForwarder).Name;
        private ActorService actorService;
        private string traceId;
        private EventSubscriptionCache subscriptionCache;
        private ActorEventSubscriberManager eventManager = ActorEventSubscriberManager.Instance;

        public ActorEventForwarder(ActorService actorService, EventSubscriptionCache subscriptionCache)
        {
            this.actorService = actorService;
            this.subscriptionCache = subscriptionCache;
            this.traceId = this.actorService.Context.TraceId;
        }

        public void HandleOneWayMessage(IServiceRemotingRequestMessage requestMessage)
        {
            if (this.actorService.IsActorCallToBeForwarded)
            {
                var actorHeaders = (IActorRemotingMessageHeaders)requestMessage.GetHeader();
                foreach (var entry in this.subscriptionCache.GetSubscriptions(actorHeaders.ActorId, actorHeaders.MethodId).Values)
                {
                    ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, $"Forwarding actor event message - ActorId : {actorHeaders.ActorId}, MethodName : {requestMessage.GetHeader().MethodName}");
                    try
                    {
                        entry.SendOneWay(requestMessage);
                    }
                    catch (Exception e)
                    {
                        ActorTrace.Source.WriteErrorWithId(TraceType, this.traceId, $"Error encountered while forwarding actor event message - ActorId : {actorHeaders.ActorId}, MethodName : {requestMessage.GetHeader().MethodName}, Exception : {e}");
                        throw e;
                    }

                    ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, $"Successfully forwarded actor event message - ActorId : {actorHeaders.ActorId}, MethodName : {requestMessage.GetHeader().MethodName}");
                }

                return;
            }

            this.eventManager.HandleOneWayMessage(requestMessage);
        }
    }
}
