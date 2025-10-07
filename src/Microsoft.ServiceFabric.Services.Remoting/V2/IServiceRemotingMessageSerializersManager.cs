// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    internal interface IServiceRemotingMessageSerializersManager
    {
        IServiceRemotingMessageSerializationProvider GetSerializationProvider();
        IServiceRemotingMessageHeaderSerializer GetHeaderSerializer();
        IServiceRemotingRequestMessageBodySerializer GetRequestBodySerializer(int interfaceId);
        IServiceRemotingResponseMessageBodySerializer GetResponseBodySerializer(int interfaceId);
    }
}
