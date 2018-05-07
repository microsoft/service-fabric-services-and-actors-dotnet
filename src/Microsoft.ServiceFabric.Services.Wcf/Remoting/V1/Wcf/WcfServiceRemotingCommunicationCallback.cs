// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Wcf
{
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.V1;

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
