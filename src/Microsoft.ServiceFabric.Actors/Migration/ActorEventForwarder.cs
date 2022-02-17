namespace Microsoft.ServiceFabric.Actors.Migration
{
    using Microsoft.ServiceFabric.Actors.Remoting.V2;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

    internal class ActorEventForwarder : IServiceRemotingCallbackMessageHandler
    {
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
            if (this.actorService.AreActorCallsAllowed)
            {
                this.eventManager.HandleOneWayMessage(requestMessage);
            }

            var actorHeaders = (IActorRemotingMessageHeaders)requestMessage.GetHeader();
            foreach (var entry in this.subscriptionCache.GetSubscriptions(actorHeaders.ActorId, actorHeaders.MethodId).Values)
            {
                entry.SendOneWay(requestMessage);
            }
        }
    }
}
