// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Services.Remoting.V2;

internal class WrappedRequestMessageFactory : IServiceRemotingMessageBodyFactory
{
    public IServiceRemotingRequestMessageBody CreateRequest(
        string interfaceName,
        string methodName,
        int numberOfParameters,
        object wrappedRequestObject)
    {
        return new WrappedRemotingMessageBody()
        {
            Value = wrappedRequestObject,
        };
    }

    public IServiceRemotingResponseMessageBody CreateResponse(
        string interfaceName,
        string methodName,
        object wrappedResponseObject)
    {
        return new WrappedRemotingMessageBody()
        {
            Value = wrappedResponseObject,
        };
    }
}
