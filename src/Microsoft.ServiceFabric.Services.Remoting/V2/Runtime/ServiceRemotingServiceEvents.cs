// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    using System;

    /// <summary>
    /// Defines lifecycle events for service runtime.
    /// </summary>
    internal static class ServiceRemotingServiceEvents
    {
        /// <summary>
        /// This event is raised right after a request is received.
        /// </summary>
        public static event EventHandler ReceiveRequest;

        /// <summary>
        /// This event is raised right before a response is sent against a service request.
        /// </summary>
        public static event EventHandler SendResponse;

        /// <summary>
        /// Call this method to raise ReceiveRequest event.
        /// </summary>
        /// <param name="request">The request object that is received.</param>
        /// <param name="methodName"></param>
        internal static void RaiseReceiveRequest(IServiceRemotingRequestMessage request, string methodName)
        {
            var receiveRequest = ReceiveRequest;

            if (receiveRequest != null)
            {
                receiveRequest(null, // sender is null for static events.
                    new ServiceRemotingRequestEventArgs(request, methodName));
            }
        }

        /// <summary>
        /// Call this method to raise SendResponse event.
        /// </summary>
        /// <param name="response">The response being sent.</param>
        /// <param name="request">The original request for which the response is being sent.</param>
        internal static void RaiseSendResponse(IServiceRemotingResponseMessage response, IServiceRemotingRequestMessage request)
        {
            var sendResponse = SendResponse;

            if (sendResponse != null)
            {
                sendResponse(null, // sender is null for static events
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
            var sendResponse = SendResponse;

            if (sendResponse != null)
            {
                sendResponse(null, // sender is null for static events
                    new ServiceRemotingFailedResponseEventArgs(ex, request));
            }
        }
    }
}