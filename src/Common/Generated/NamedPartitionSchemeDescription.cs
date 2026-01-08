// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the named partition scheme of the service.
    /// </summary>
    public partial class NamedPartitionSchemeDescription : PartitionSchemeDescription
    {
        /// <summary>
        /// Initializes a new instance of the NamedPartitionSchemeDescription class.
        /// </summary>
        /// <param name="count">The number of partitions.</param>
        /// <param name="names">Array of size specified by the ‘Count’ parameter, for the names of the partitions.</param>
        public NamedPartitionSchemeDescription(
            int? count,
            IEnumerable<string> names)
            : base(
                Common.PartitionScheme.Named)
        {
            count.ThrowIfNull(nameof(count));
            names.ThrowIfNull(nameof(names));
            this.Count = count;
            this.Names = names;
        }

        /// <summary>
        /// Gets the number of partitions.
        /// </summary>
        public int? Count { get; }

        /// <summary>
        /// Gets array of size specified by the ‘Count’ parameter, for the names of the partitions.
        /// </summary>
        public IEnumerable<string> Names { get; }
    }
}
