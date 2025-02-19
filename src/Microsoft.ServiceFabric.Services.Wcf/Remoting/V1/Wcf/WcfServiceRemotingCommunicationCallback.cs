// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Wcf
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.V1;

    [Obsolete("This class is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
    internal sealed class WcfServiceRemotingCommunicationCallback : IServiceRemotingCallbackClient
    {
        private readonly IServiceRemotingCallbackContract callbackChannel;

        public WcfServiceRemotingCommunicationCallback(IServiceRemotingCallbackContract callbackChannel)
        {
            this.callbackChannel = callbackChannel;
        }

        public Task<byte[]> RequestResponseAsync(
            ServiceRemotingMessageHeaders messageHeaders,
            byte[] requestBody)
        {
            return this.callbackChannel.RequestResponseAsync(messageHeaders, requestBody);
        }

        public void OneWayMessage(
            ServiceRemotingMessageHeaders messageHeaders,
            byte[] requestBody)
        {
            this.callbackChannel.SendOneWay(messageHeaders, requestBody);
        }
    }
}
