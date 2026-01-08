// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a base class for primary or secondary replicator status.
    /// Contains information about the service fabric replicator like the replication/copy queue utilization, last
    /// acknowledgement received timestamp, etc.
    /// </summary>
    public abstract partial class ReplicatorStatus
    {
        /// <summary>
        /// Initializes a new instance of the ReplicatorStatus class.
        /// </summary>
        /// <param name="kind">The role of a replica of a stateful service.</param>
        protected ReplicatorStatus(
            ReplicaRole? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the role of a replica of a stateful service.
        /// </summary>
        public ReplicaRole? Kind { get; }
    }
}
