// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about how many replicas are completed or pending for a specific service during upgrade.
    /// </summary>
    public partial class ServiceUpgradeProgress
    {
        /// <summary>
        /// Initializes a new instance of the ServiceUpgradeProgress class.
        /// </summary>
        /// <param name="serviceName">Name of the Service resource.</param>
        /// <param name="completedReplicaCount">The number of replicas that completes the upgrade in the service.</param>
        /// <param name="pendingReplicaCount">The number of replicas that are waiting to be upgraded in the service.</param>
        public ServiceUpgradeProgress(
            string serviceName = default(string),
            string completedReplicaCount = default(string),
            string pendingReplicaCount = default(string))
        {
            this.ServiceName = serviceName;
            this.CompletedReplicaCount = completedReplicaCount;
            this.PendingReplicaCount = pendingReplicaCount;
        }

        /// <summary>
        /// Gets name of the Service resource.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets the number of replicas that completes the upgrade in the service.
        /// </summary>
        public string CompletedReplicaCount { get; }

        /// <summary>
        /// Gets the number of replicas that are waiting to be upgraded in the service.
        /// </summary>
        public string PendingReplicaCount { get; }
    }
}
