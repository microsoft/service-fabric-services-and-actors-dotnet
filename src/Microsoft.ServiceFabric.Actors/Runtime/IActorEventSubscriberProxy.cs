// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    internal interface IActorEventSubscriberProxy
    {
        Guid Id { get; }

        RemotingListener RemotingListener { get; }

#if !DotNetCoreClr
        void RaiseEvent(int eventInterfaceId, int methodId, byte[] eventMsgBody);
#endif
        //V2 Stack Api
        void RaiseEvent(int eventInterfaceId, int methodId, IServiceRemotingRequestMessageBody eventMsgBody);

        IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory();
    }
}
