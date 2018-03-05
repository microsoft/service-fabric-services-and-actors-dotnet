// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
    using System.Collections.Generic;

    internal class BasicDataContractSerializationProvider : IServiceRemotingMessageSerializationProvider
    {
        public IServiceRemotingMessageBodyFactory CreateMessageBodyFactory()
        {
            return new DataContractRemotingMessageFactory();
        }

        public IServiceRemotingRequestMessageBodySerializer CreateRequestMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> requestBodyTypes)
        {
            return new BasicDataRequestMessageBodySerializer(requestBodyTypes);
        }

        public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> responseBodyTypes)
        {
            return new BasicDataResponsetMessageBodySerializer(responseBodyTypes);
        }
    }
}
