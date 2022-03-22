// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Migration.Exceptions;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    internal class RequestForwardableRemotingDispatcher : IServiceRemotingMessageHandler, IDisposable
    {
        private static readonly string TraceType = typeof(RequestForwardableRemotingDispatcher).Name;
        private IServiceRemotingMessageHandler actualMessageHandler;
        private ActorService actorService;
        private IRequestForwarder requestForwarder;
        private ServiceRemotingCancellationHelper cancellationHelper;
        private string traceId;

        public RequestForwardableRemotingDispatcher(ActorService actorService, IServiceRemotingMessageHandler actualMessageHandler, IRequestForwarder requestForwarder)
        {
            this.actorService = actorService;
            this.actualMessageHandler = actualMessageHandler;
            this.requestForwarder = requestForwarder;
            this.traceId = this.actorService.Context.TraceId;
            this.cancellationHelper = new ServiceRemotingCancellationHelper(this.actorService.Context.TraceId);
        }

        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
             return this.actualMessageHandler.GetRemotingMessageBodyFactory();
        }

        public void HandleOneWayMessage(IServiceRemotingRequestMessage requestMessage)
        {
            this.actualMessageHandler.HandleOneWayMessage(requestMessage);
        }

        public async Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(IServiceRemotingRequestContext requestContext, IServiceRemotingRequestMessage requestMessage)
        {
            if (this.actorService.IsActorCallToBeForwarded)
            {
                if (requestMessage.GetHeader().TryGetHeaderValue(Runtime.Migration.Constants.ForwardRequestHeaderName, out var val))
                {
                    var errorMsg = $"Both migration services are in Downtime. Retry the call after Downtime phase is completed.";
                    ActorTrace.Source.WriteInfoWithId(
                       TraceType,
                       this.traceId,
                       errorMsg);

                    throw new ActorCallsDisallowedException(errorMsg);
                }

                return await this.requestForwarder.ForwardRequestResponseAsync(requestContext, requestMessage);
            }

            // User could observe ActorCallsDisallowedException in a race situation where service moved to downtime phase after dispatching the request.
            return await this.actualMessageHandler.HandleRequestResponseAsync(requestContext, requestMessage);
        }

        public void Dispose()
        {
            ((IDisposable)this.actualMessageHandler).Dispose();
        }
    }
}
