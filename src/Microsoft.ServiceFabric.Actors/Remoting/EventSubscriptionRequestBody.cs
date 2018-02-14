// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Constants.Namespace)]
    internal class EventSubscriptionRequestBody
    {
        [DataMember(IsRequired = true, Order = 0)] public int eventInterfaceId;
        [DataMember(IsRequired = true, Order = 1)] public Guid subscriptionId;
    }
}
