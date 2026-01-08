// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for resuming a cluster upgrade.
    /// </summary>
    public partial class ResumeClusterUpgradeDescription
    {
        /// <summary>
        /// Initializes a new instance of the ResumeClusterUpgradeDescription class.
        /// </summary>
        /// <param name="upgradeDomain">The next upgrade domain for this cluster upgrade.</param>
        public ResumeClusterUpgradeDescription(
            string upgradeDomain)
        {
            upgradeDomain.ThrowIfNull(nameof(upgradeDomain));
            this.UpgradeDomain = upgradeDomain;
        }

        /// <summary>
        /// Gets the next upgrade domain for this cluster upgrade.
        /// </summary>
        public string UpgradeDomain { get; }
    }
}
