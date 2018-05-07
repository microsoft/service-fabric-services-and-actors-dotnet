// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Client
{
    using System;

    /// <summary>
    /// Defines lifecycle events for service clients (caller side events).
    /// </summary>
    internal static class ServiceRemotingClientEvents
    {
        /// <summary>
        /// This event is raised just before a service remoting request is sent.
        /// </summary>
        public static event EventHandler SendRequest;

        /// <summary>
        /// This event is raised right after a response to a service remoting request is received.
        /// </summary>
        public static event EventHandler ReceiveResponse;

        /// <summary>
        /// Call this method to raise the SendRequest event.
        /// </summary>
        /// <param name="request">The actual request that is being sent.</param>
        /// <param name="serviceUri">Uri of the service being called.</param>
        /// <param name="methodName">Method name on the service that is being called.</param>
        internal static void RaiseSendRequest(IServiceRemotingRequestMessage request, Uri serviceUri, string methodName)
        {
            var sendRequest = SendRequest;

            if (sendRequest != null)
            {
                sendRequest(
                    null, // sender is null for static events.
                    new ServiceRemotingRequestEventArgs(request, serviceUri, methodName));
            }
        }

        /// <summary>
        /// Call this method to raise the ReceiveResponse event
        /// </summary>
        /// <param name="response">The actual response that is received.</param>
        /// <param name="request">The original request against which the response is generated.</param>
        internal static void RaiseRecieveResponse(IServiceRemotingResponseMessage response, IServiceRemotingRequestMessage request)
        {
            var receiveResponse = ReceiveResponse;

            if (receiveResponse != null)
            {
                receiveResponse(
                    null, // sender is null for static events
                    new ServiceRemotingResponseEventArgs(response, request));
            }
        }

        /// <summary>
        /// Call this method to raise SendResponseEvent in case of a failed request.
        /// </summary>
        /// <param name="ex">The exception being sent.</param>
        /// <param name="request">The original request that resulted in the exception.</param>
        internal static void RaiseExceptionResponse(Exception ex, IServiceRemotingRequestMessage request)
        {
            var receiveResponse = ReceiveResponse;

            if (receiveResponse != null)
            {
                receiveResponse(
                    null, // sender is null for static events
                    new ServiceRemotingFailedResponseEventArgs(ex, request));
            }
        }
    }
}
