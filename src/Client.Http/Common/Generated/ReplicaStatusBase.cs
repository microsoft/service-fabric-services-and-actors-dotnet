// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the replica.
    /// </summary>
    public abstract partial class ReplicaStatusBase
    {
        /// <summary>
        /// Initializes a new instance of the ReplicaStatusBase class.
        /// </summary>
        /// <param name="kind">The role of a replica of a stateful service.</param>
        protected ReplicaStatusBase(
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
