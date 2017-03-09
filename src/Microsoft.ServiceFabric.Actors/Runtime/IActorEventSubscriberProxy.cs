// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;

    internal interface IActorEventSubscriberProxy
    {
        Guid Id { get; }

        void RaiseEvent(int eventInterfaceId, int methodId, byte[] eventMsgBody);
    }
}