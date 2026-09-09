// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base type for stateful service store-specific copy details during replica build.
    /// The Kind property determines the replica kind for this copy detail.
    /// </summary>
    public abstract partial class InbuildReplicaCopyDetail
    {
        /// <summary>
        /// Initializes a new instance of the InbuildReplicaCopyDetail class.
        /// </summary>
        /// <param name="kind">The role of a replica of a stateful service.</param>
        protected InbuildReplicaCopyDetail(
            ReplicaKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the role of a replica of a stateful service.
        /// </summary>
        public ReplicaKind? Kind { get; }
    }
}
