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
    using Microsoft.ServiceFabric.Actors.Remoting.V2.FabricTransport.Client;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    /// <summary>
    /// Default implementation for actor request forwarding scenario.
    /// </summary>
    public class DefaultActorRequestForwarder : IRequestForwarder
    {
        private static readonly string TraceType = typeof(DefaultActorRequestForwarder).Name;
        private ServiceRemotingPartitionClient remotingClient;
        private EventSubscriptionCache eventCache;
        private IServiceRemotingCallbackMessageHandler callbackHandler;
        private ActorService actorService;
        private string traceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultActorRequestForwarder"/> class
        /// </summary>
        /// <param name="actorService">Actor service.</param>
        /// <param name="requestForwarderContext">Context for request forwarder.</param>
        /// <param name="listenerName">Remote service listener name. If specified null, "Migration Listener" will be used as default.</param>
        /// <param name="createServiceRemotingClientFactory">Factory method to create remoting communication client factory.</param>
        /// <param name="retrySettings">Retry settings for the remote object calls made by proxy.</param>
        public DefaultActorRequestForwarder(
            ActorService actorService,
            RequestForwarderContext requestForwarderContext,
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
                requestForwarderContext.ServiceUri,
                requestForwarderContext.ServicePartitionKey,
                requestForwarderContext.ReplicaSelector,
                listenerName ?? Runtime.Migration.Constants.MigrationListenerName,
                retrySettings);

            this.traceId = requestForwarderContext.TraceId;
        }

        /// <inheritdoc/>
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

            try
            {
                requestMessage.GetHeader().AddHeader("ActorRequestForwarded", new byte[0]);
                var retVal = await this.remotingClient.InvokeAsync(requestMessage, requestMessage.GetHeader().MethodName, CancellationToken.None);
                ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, $"Successfully received response for the forwarded actor request - ActorId : {actorId}, MethodName : {requestMessage.GetHeader().MethodName}");

                return retVal;
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteErrorWithId(TraceType, this.traceId, $"Error encountered while forwarding actor request - ActorId : {actorId}, MethodName : {requestMessage.GetHeader().MethodName}, Exception : {e}");
                throw e;
            }
        }
    }
}
