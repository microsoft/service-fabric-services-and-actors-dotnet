// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Constants.Namespace)]
    internal class EventSubscriptionRequestBody
    {
        [DataMember(IsRequired = true, Order = 0, Name = "eventInterfaceId")]
        public int EventInterfaceId { get; set; }

        [DataMember(IsRequired = true, Order = 1, Name = "subscriptionId")]
        public Guid SubscriptionId { get; set; }
    }
}
