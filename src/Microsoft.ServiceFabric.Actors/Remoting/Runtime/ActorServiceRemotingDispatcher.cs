// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Remoting.Runtime
{
    using System;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Services.Common;
    using System.Globalization;

    /// <summary>
    /// Provides an implementation of <see cref="IServiceRemotingMessageHandler"/> that can dispatch
    /// messages to an actor service and to the actors hosted in the service.
    /// </summary>
    public class ActorServiceRemotingDispatcher : ServiceRemotingDispatcher
    {
        private readonly ActorService actorService;
        private readonly ServiceRemotingCancellationHelper cancellationHelper;

        /// <summary>
        /// Instantiates the ActorServiceRemotingDispatcher that can dispatch messages to an actor service and 
        /// to the actors hosted in the service..
        /// </summary>
        /// <param name="actorService">An actor service instance.</param>
        public ActorServiceRemotingDispatcher(ActorService actorService)
            : base(GetContext(actorService), actorService)
        {
            this.actorService = actorService;
            this.cancellationHelper = new ServiceRemotingCancellationHelper(actorService.Context.TraceId);
        }

        private static ServiceContext GetContext(ActorService actorService)
        {
            Requires.ThrowIfNull(actorService, "actorService");
            return actorService.Context;
        }

        /// <summary>
        /// Dispatches the messages received from the client to the actor service methods or the actor methods.
        /// </summary>
        /// <param name="requestContext">Request context that allows getting the callback channel if required.</param>
        /// <param name="messageHeaders">Service remoting message headers</param>
        /// <param name="requestBodyBytes">serialized request body of the remoting message.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. 
        /// The result of the Task is the serialized response body.
        /// </returns>
        public override Task<byte[]> RequestResponseAsync(
            IServiceRemotingRequestContext requestContext, 
            ServiceRemotingMessageHeaders messageHeaders, 
            byte[] requestBodyBytes)
        {
            if (messageHeaders.InterfaceId == ActorMessageDispatch.InterfaceId)
            {
                return this.HandleActorMethodDispatchAsync(messageHeaders, requestBodyBytes);
            }

            if (messageHeaders.InterfaceId == ActorEventSubscription.InterfaceId)
            {
                return this.HandleSubscriptionRequestsAsync(requestContext, messageHeaders, requestBodyBytes);
            }

            return base.RequestResponseAsync(requestContext, messageHeaders, requestBodyBytes);
        }

        private async Task<byte[]> HandleSubscriptionRequestsAsync(
            IServiceRemotingRequestContext requestContext,
            ServiceRemotingMessageHeaders messageHeaders,
            byte[] requestMsgBodyBytes)
        {
            ActorMessageHeaders actorMessageHeaders;
            if (!ActorMessageHeaders.TryFromServiceMessageHeaders(messageHeaders, out actorMessageHeaders))
            {
                throw new SerializationException(Actors.SR.ErrorActorMessageHeadersDeserializationFailed);
            }

            if (actorMessageHeaders.MethodId == ActorEventSubscription.SubscribeMethodId)
            {
                var requestMsgBody = (ActorMessageBody)SerializationUtility.Deserialize(ActorEventSubscription.Serializer, requestMsgBodyBytes);
                var castedRequestMsgBody = (EventSubscriptionRequestBody)requestMsgBody.Value;

                await this.actorService.ActorManager.SubscribeAsync(
                    actorMessageHeaders.ActorId,
                    castedRequestMsgBody.eventInterfaceId,
                    new ActorEventSubscriberProxy(
                        castedRequestMsgBody.subscriptionId,
                        requestContext.GetCallbackClient()));

                return null;
            }

            if (actorMessageHeaders.MethodId == ActorEventSubscription.UnSubscribeMethodId)
            {
                var requestMsgBody = (ActorMessageBody) SerializationUtility.Deserialize(ActorEventSubscription.Serializer, requestMsgBodyBytes);
                var castedRequestMsgBody = (EventSubscriptionRequestBody)requestMsgBody.Value;

                await this.actorService.ActorManager.UnsubscribeAsync(
                    actorMessageHeaders.ActorId,
                    castedRequestMsgBody.eventInterfaceId,
                    castedRequestMsgBody.subscriptionId);

                return null;
            }

            throw new MissingMethodException(string.Format(CultureInfo.CurrentCulture, Actors.SR.ErrorInvalidMethodId, actorMessageHeaders.MethodId));
        }

        private async Task<byte[]> HandleActorMethodDispatchAsync(
            ServiceRemotingMessageHeaders messageHeaders,
            byte[] requestMsgBodyBytes)
        {
            var startTime = DateTime.UtcNow;
            ActorMessageHeaders actorMessageHeaders;
            if (!ActorMessageHeaders.TryFromServiceMessageHeaders(messageHeaders, out actorMessageHeaders))
            {

                throw new SerializationException(Actors.SR.ErrorActorMessageHeadersDeserializationFailed);
                    
            }

            if (this.cancellationHelper.IsCancellationRequest(messageHeaders))
            {
                return await this.cancellationHelper.CancelRequestAsync(
                    actorMessageHeaders.InterfaceId,
                    actorMessageHeaders.MethodId,
                    messageHeaders.InvocationId);
            }
            else
            {
                byte[] retVal;
                this.actorService.ActorManager.DiagnosticsEventManager.ActorRequestProcessingStart();
                try
                {
                    retVal = await this.cancellationHelper.DispatchRequestAsync(
                        actorMessageHeaders.InterfaceId,
                        actorMessageHeaders.MethodId,
                        messageHeaders.InvocationId,
                        cancellationToken => this.OnDispatch(actorMessageHeaders, requestMsgBodyBytes, cancellationToken));
                }
                finally
                {
                    this.actorService.ActorManager.DiagnosticsEventManager.ActorRequestProcessingFinish(startTime);
                }
                return retVal;
            }
        }

        private Task<byte[]> OnDispatch(
          ActorMessageHeaders actorMessageHeaders,
          byte[] requestBodyBytes,
          CancellationToken cancellationToken)
        {
            return this.actorService.ActorManager.InvokeAsync(
                actorMessageHeaders.ActorId,
                actorMessageHeaders.InterfaceId,
                actorMessageHeaders.MethodId,
                actorMessageHeaders.CallContext,
                requestBodyBytes,
                cancellationToken);
        }

    }
}
