// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2
{
    internal class ServiceRemotingRequestMessage : IServiceRemotingRequestMessage
    {
        private readonly IServiceRemotingRequestMessageHeader header;
        private readonly IServiceRemotingRequestMessageBody msgBody;

        public ServiceRemotingRequestMessage(IServiceRemotingRequestMessageHeader header, IServiceRemotingRequestMessageBody msgBody)
        {
            this.header = header;
            this.msgBody = msgBody;
        }

        public IServiceRemotingRequestMessageHeader GetHeader()
        {
            return this.header;
        }

        public IServiceRemotingRequestMessageBody GetBody()
        {
            return this.msgBody;
        }
    }
}
