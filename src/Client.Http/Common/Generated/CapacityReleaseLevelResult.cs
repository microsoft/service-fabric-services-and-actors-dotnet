// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The current capacity release level for the cluster.
    /// </summary>
    public partial class CapacityReleaseLevelResult
    {
        /// <summary>
        /// Initializes a new instance of the CapacityReleaseLevelResult class.
        /// </summary>
        /// <param name="level">The current capacity release level. Possible values include: 'None', 'Minor', 'Major'
        /// 
        /// The level of capacity release applied to the cluster.
        /// </param>
        public CapacityReleaseLevelResult(
            CapacityReleaseLevel? level)
        {
            level.ThrowIfNull(nameof(level));
            this.Level = level;
        }

        /// <summary>
        /// Gets the current capacity release level. Possible values include: 'None', 'Minor', 'Major'
        /// 
        /// The level of capacity release applied to the cluster.
        /// </summary>
        public CapacityReleaseLevel? Level { get; }
    }
}
