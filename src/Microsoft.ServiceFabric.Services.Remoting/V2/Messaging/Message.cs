// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Messaging
{
    internal class Message : IMessage
    {
        private readonly IMessageBody messageBody;
        private readonly IMessageHeader messageHeader;

        public Message(IMessageHeader messageHeader, IMessageBody messageBody)
        {
            this.messageBody = messageBody;
            this.messageHeader = messageHeader;
        }

        public void Dispose()
        {
            this.messageHeader.Dispose();
            this.messageBody.Dispose();
        }

        public IMessageHeader GetHeader()
        {
            return this.messageHeader;
        }

        public IMessageBody GetBody()
        {
            return this.messageBody;
        }
    }
}