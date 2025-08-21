// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class adds extensions methods to initialize <see cref="ServicePartitionResolver"/>
    /// </summary>
    public static class ServicePartitionResolverExtensions
    {
        /// <summary>
        /// An extension method that disables the notification for this particular instance of <see cref="ServicePartitionResolver"/>
        /// By default, service partition Resolver register for notification.There is a cache of service endpoints in the client that gets updated by notifications and
        /// this same cache is used to satisfy complaint based resolution requests.
        /// If you interested in only complain based resolution, then you could  use this extension to disable it.
        /// </summary>
        /// <param name="partitionResolver">ServicePartitionResolver to disable notification for.</param>
        /// <returns>ServicePartitionResolver</returns>
        public static ServicePartitionResolver DisableNotification(
          this ServicePartitionResolver partitionResolver)
        {
            partitionResolver.UseNotification = false;
            return partitionResolver;
        }
    }
}
