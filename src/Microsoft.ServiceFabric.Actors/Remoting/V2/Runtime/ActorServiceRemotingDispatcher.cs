// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime
{
    using System;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Builder;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    /// <summary>
    /// Provides an implementation of <see cref="IServiceRemotingMessageHandler"/> that can dispatch
    /// messages to an actor service and to the actors hosted in the service.
    /// </summary>
    public class ActorServiceRemotingDispatcher : ServiceRemotingMessageDispatcher
    {
        private readonly ActorService actorService;
        private readonly ServiceRemotingCancellationHelper cancellationHelper;


        /// <summary>
        /// Instantiates the ActorServiceRemotingDispatcher that can dispatch messages to an actor service and 
        /// to the actors hosted in the service..
        /// </summary>
        /// <param name="actorService">An actor service instance.</param>
        /// <param name="serviceRemotingRequestMessageBodyFactory"></param>
        public ActorServiceRemotingDispatcher(ActorService actorService,
            IServiceRemotingMessageBodyFactory serviceRemotingRequestMessageBodyFactory)
            : base(GetContext(actorService),
                actorService,
                serviceRemotingRequestMessageBodyFactory)
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
        /// This can be used by user where they know interfaceId and MethodId for the method to dispatch to .
        /// </summary>
        /// <param name="requestContext">Request context that allows getting the callback channel if required.</param>
        /// <param name="requestMessage">Remoting message.</param>
        /// <returns></returns>
        public override Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            IServiceRemotingRequestMessage requestMessage)
        {
            Requires.ThrowIfNull(requestMessage, "requestMessage");
            Requires.ThrowIfNull(requestMessage.GetHeader(), "RequestMessageHeader");

            var messageHeaders = requestMessage.GetHeader();

            if (requestMessage.GetHeader() is IActorRemotingMessageHeaders actorHeaders)
            {
                if (messageHeaders.InterfaceId == Runtime.ActorEventSubscription.InterfaceId)
                {
                    return this.HandleSubscriptionRequestsAsync(
                        requestContext,
                        messageHeaders,
                        requestMessage.GetBody());
                }

                return this.HandleActorMethodDispatchAsync(actorHeaders, requestMessage.GetBody());
            }

            return base.HandleRequestResponseAsync(requestContext, requestMessage);
        }


        /// <summary>
        /// Dispatches the messages received from the client to the actor service methods or the actor methods.
        /// This can be be used  by user as an independent dispatcher like short-circuiting.
        /// </summary>
        /// <param name="requestBody"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="actorDispatchHeaders"></param>
        /// <returns></returns>
        public Task<IServiceRemotingResponseMessageBody> HandleRequestResponseAsync(
            ActorRemotingDispatchHeaders actorDispatchHeaders,
            IServiceRemotingRequestMessageBody requestBody,
            CancellationToken cancellationToken)
        {
            //For Actor Service Requests
            if (!string.IsNullOrEmpty(actorDispatchHeaders.ServiceInterfaceName))
            {
                return base.HandleRequestResponseAsync(actorDispatchHeaders,
                    requestBody,
                    cancellationToken);
            }
            var header = this.CreateActorHeader(actorDispatchHeaders);

            return this.HandleActorMethodDispatchAsync(header, requestBody, cancellationToken);
        }

        private async Task<IServiceRemotingResponseMessageBody> HandleActorMethodDispatchAsync(
            IActorRemotingMessageHeaders actorMessageHeaders, IServiceRemotingRequestMessageBody msgBody,
            CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow;
            IServiceRemotingResponseMessageBody retVal;
            this.actorService.ActorManager.DiagnosticsEventManager.ActorRequestProcessingStart();
            try
            {
                retVal = await this.OnDispatch(actorMessageHeaders, msgBody,
                    cancellationToken);
            }
            finally
            {
                this.actorService.ActorManager.DiagnosticsEventManager.ActorRequestProcessingFinish(startTime);
            }
            return retVal;
        }

        private async Task<IServiceRemotingResponseMessage> HandleActorMethodDispatchAsync(
            IActorRemotingMessageHeaders messageHeaders, IServiceRemotingRequestMessageBody msgBody)
        {
            var startTime = DateTime.UtcNow;
            if (this.IsCancellationRequest(messageHeaders))
            {
                await this.cancellationHelper.CancelRequestAsync(
                    messageHeaders.InterfaceId,
                    messageHeaders.MethodId,
                    messageHeaders.InvocationId);
                return null;
            }
            else
            {
                IServiceRemotingResponseMessageBody retVal;
                this.actorService.ActorManager.DiagnosticsEventManager.ActorRequestProcessingStart();

                var methodDispatcher = this.actorService.MethodDispatcherMapV2.GetDispatcher(messageHeaders.InterfaceId, messageHeaders.MethodId);

                var requestMessage = new ServiceRemotingRequestMessage(messageHeaders, msgBody);
                ServiceRemotingServiceEvents.RaiseReceiveRequest(requestMessage, methodDispatcher.GetMethodName(messageHeaders.MethodId));

                try
                {
                    retVal = await this.cancellationHelper.DispatchRequest(
                        messageHeaders.InterfaceId,
                        messageHeaders.MethodId,
                        messageHeaders.InvocationId,
                        cancellationToken => this.OnDispatch(messageHeaders, msgBody,
                            cancellationToken));
                }
                catch (Exception ex)
                {
                    ServiceRemotingServiceEvents.RaiseExceptionResponse(ex, requestMessage);
                    throw;
                }
                finally
                {
                    this.actorService.ActorManager.DiagnosticsEventManager.ActorRequestProcessingFinish(startTime);
                }

                var response = new ServiceRemotingResponseMessage(new ServiceRemotingResponseMessageHeader(), retVal);

                ServiceRemotingServiceEvents.RaiseSendResponse(response, requestMessage);

                return response;
            }
        }

        private Task<IServiceRemotingResponseMessageBody> OnDispatch(
            IActorRemotingMessageHeaders actorMessageHeaders,
            IServiceRemotingRequestMessageBody requestBody,
            CancellationToken cancellationToken)
        {
            return this.actorService.ActorManager
                .InvokeAsync(
                    actorMessageHeaders.ActorId,
                    actorMessageHeaders.InterfaceId,
                    actorMessageHeaders.MethodId,
                    actorMessageHeaders.CallContext,
                    requestBody,
                    this.GetRemotingMessageBodyFactory(),
                    cancellationToken);
        }


        private IActorRemotingMessageHeaders CreateActorHeader(ActorRemotingDispatchHeaders actorDispatchHeaders)
        {
            if (ActorCodeBuilder.TryGetKnownTypes(actorDispatchHeaders.ActorInterfaceName, out var details))
            {
                var headers = new ActorRemotingMessageHeaders();
                headers.ActorId = actorDispatchHeaders.ActorId;
                headers.InterfaceId = details.Id;
                if (string.IsNullOrEmpty(actorDispatchHeaders.CallContext))
                {
                    headers.CallContext = Helper.GetCallContext();
                }
                else
                {
                    headers.CallContext = actorDispatchHeaders.CallContext;
                }

                if (!details.MethodNames.TryGetValue(actorDispatchHeaders.MethodName, out var headersMethodId))
                {
                    throw new NotSupportedException("This Actor Method is not Supported" + actorDispatchHeaders.MethodName);
                }
                headers.MethodId = headersMethodId;

                return headers;
            }

            throw new NotSupportedException("This Actor Interface is not Supported" + actorDispatchHeaders.ActorInterfaceName);
        }

        private async Task<IServiceRemotingResponseMessage> HandleSubscriptionRequestsAsync(
            IServiceRemotingRequestContext requestContext,
            IServiceRemotingRequestMessageHeader messageHeaders,
            IServiceRemotingRequestMessageBody requestMsgBody)
        {
            var actorHeaders = (IActorRemotingMessageHeaders)messageHeaders;

            if (actorHeaders.MethodId == Runtime.ActorEventSubscription.SubscribeMethodId)
            {
                var castedRequestMsgBody =
                    (EventSubscriptionRequestBody)requestMsgBody.GetParameter(0, "Value",
                        typeof(EventSubscriptionRequestBody));

                await this.actorService.ActorManager
                    .SubscribeAsync(
                        actorHeaders.ActorId,
                        castedRequestMsgBody.eventInterfaceId,
                        new ActorEventSubscriberProxy(
                            castedRequestMsgBody.subscriptionId,
                            requestContext.GetCallBackClient()));

                return null;
            }

            if (messageHeaders.MethodId == Runtime.ActorEventSubscription.UnSubscribeMethodId)
            {
                var castedRequestMsgBody =
                    (Actors.Remoting.EventSubscriptionRequestBody)requestMsgBody.GetParameter(0, "Value",
                        typeof(Actors.Remoting.EventSubscriptionRequestBody));

                await this.actorService.ActorManager
                    .UnsubscribeAsync(
                        actorHeaders.ActorId,
                        castedRequestMsgBody.eventInterfaceId,
                        castedRequestMsgBody.subscriptionId);

                return null;
            }

            throw new MissingMethodException(string.Format(CultureInfo.CurrentCulture, SR.ErrorInvalidMethodId,
                messageHeaders.MethodId));
        }
    }
}
