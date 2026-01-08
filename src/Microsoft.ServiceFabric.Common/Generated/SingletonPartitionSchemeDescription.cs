// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the partition scheme of a singleton-partitioned, or non-partitioned service.
    /// </summary>
    public partial class SingletonPartitionSchemeDescription : PartitionSchemeDescription
    {
        /// <summary>
        /// Initializes a new instance of the SingletonPartitionSchemeDescription class.
        /// </summary>
        public SingletonPartitionSchemeDescription()
            : base(
                Common.PartitionScheme.Singleton)
        {
        }
    }
}
