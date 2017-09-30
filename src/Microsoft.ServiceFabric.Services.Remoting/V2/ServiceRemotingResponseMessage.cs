// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    class ServiceRemotingResponseMessage : IServiceRemotingResponseMessage
    {
        private readonly IServiceRemotingResponseMessageHeader header;
        private readonly IServiceRemotingResponseMessageBody msgBody;

        public ServiceRemotingResponseMessage(IServiceRemotingResponseMessageHeader header,
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
