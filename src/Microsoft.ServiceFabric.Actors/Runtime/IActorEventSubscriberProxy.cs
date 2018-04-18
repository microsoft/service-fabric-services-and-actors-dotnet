// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    /// <summary>
    /// Contains methods for SubscriberProxy for actor events.
    /// </summary>
    internal interface IActorEventSubscriberProxy
    {
        /// <summary>
        /// Gets the subscription Id.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the value indicating the remoting stack for server/listener when using remoting provider attribuite to determine the remoting client.
        /// </summary>
        RemotingListener RemotingListener { get; }

#if !DotNetCoreClr
        /// <summary>
        /// Raises the event.
        /// </summary>
        /// <param name="eventInterfaceId">Id of event interface.</param>
        /// <param name="methodId">Id of method in event interface.</param>
        /// <param name="eventMsgBody">Message body for the event.</param>
        void RaiseEvent(int eventInterfaceId, int methodId, byte[] eventMsgBody);

#endif

        // V2 Stack Api

        /// <summary>
        /// Raises the event.
        /// </summary>
        /// <param name="eventInterfaceId">Id of event interface.</param>
        /// <param name="methodId">Id of method in event interface.</param>
        /// <param name="eventMsgBody">Message body for the event.</param>
        void RaiseEvent(int eventInterfaceId, int methodId, IServiceRemotingRequestMessageBody eventMsgBody);

        /// <summary>
        /// Gets the factory for creating remoting request body and response body objects.
        /// </summary>
        /// <returns>Factory for creating remoting request body and response body objects.</returns>
        IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory();
    }
}
