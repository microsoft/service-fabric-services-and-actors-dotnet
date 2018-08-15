// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

    public class DataContractRemotingMessageFactory : IServiceRemotingMessageBodyFactory
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

        public IServiceRemotingRequestMessageBody CreateRequest(string interfaceName, string methodName, int numberOfParameters, object wrappedRequest)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return new ServiceRemotingRequestMessageBody(numberOfParameters);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

        public IServiceRemotingResponseMessageBody CreateResponse(string interfaceName, string methodName, object wrappedResponse)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return new ServiceRemotingResponseMessageBody();
        }
    }
}
