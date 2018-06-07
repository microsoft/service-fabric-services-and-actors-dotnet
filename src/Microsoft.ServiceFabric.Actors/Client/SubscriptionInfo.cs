// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Client
{
    using System;

    internal class SubscriptionInfo
    {
        public SubscriptionInfo(Subscriber subscriber)
        {
            this.Subscriber = subscriber;
            this.Id = Guid.NewGuid();
            this.IsActive = true;
        }

        public Guid Id { get; }

        public Subscriber Subscriber { get; }

        public bool IsActive { get; set; }
    }
}
