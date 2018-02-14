// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    internal interface IServiceRemotingMessageHeaderSerializer
    {
        IMessageHeader SerializeRequestHeader(IServiceRemotingRequestMessageHeader serviceRemotingRequestMessageHeader);

        IServiceRemotingRequestMessageHeader DeserializeRequestHeaders(IMessageHeader messageHeader);

        IMessageHeader SerializeResponseHeader(IServiceRemotingResponseMessageHeader serviceRemotingResponseMessageHeader);

        IServiceRemotingResponseMessageHeader DeserializeResponseHeaders(IMessageHeader messageHeader);
    }
}
