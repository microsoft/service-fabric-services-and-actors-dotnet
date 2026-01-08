// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes how the service is partitioned.
    /// </summary>
    public abstract partial class PartitionSchemeDescription
    {
        /// <summary>
        /// Initializes a new instance of the PartitionSchemeDescription class.
        /// </summary>
        /// <param name="partitionScheme">Enumerates the ways that a service can be partitioned.</param>
        protected PartitionSchemeDescription(
            PartitionScheme? partitionScheme)
        {
            partitionScheme.ThrowIfNull(nameof(partitionScheme));
            this.PartitionScheme = partitionScheme;
        }

        /// <summary>
        /// Gets enumerates the ways that a service can be partitioned.
        /// </summary>
        public PartitionScheme? PartitionScheme { get; }
    }
}
