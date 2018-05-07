// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    internal class ServiceRemotingResponseMessage : IServiceRemotingResponseMessage
    {
        private readonly IServiceRemotingResponseMessageHeader header;
        private readonly IServiceRemotingResponseMessageBody msgBody;

        public ServiceRemotingResponseMessage(
            IServiceRemotingResponseMessageHeader header,
            IServiceRemotingResponseMessageBody msgBody)
        {
            this.header = header;
            this.msgBody = msgBody;
        }

        public IServiceRemotingResponseMessageHeader GetHeader()
        {
            return this.header;
        }

        public IServiceRemotingResponseMessageBody GetBody()
        {
            return this.msgBody;
        }
    }
}
