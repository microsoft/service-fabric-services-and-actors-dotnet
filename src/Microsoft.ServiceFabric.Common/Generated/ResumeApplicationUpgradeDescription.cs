// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for resuming an unmonitored manual Service Fabric application upgrade
    /// </summary>
    public partial class ResumeApplicationUpgradeDescription
    {
        /// <summary>
        /// Initializes a new instance of the ResumeApplicationUpgradeDescription class.
        /// </summary>
        /// <param name="upgradeDomainName">The name of the upgrade domain in which to resume the upgrade.</param>
        public ResumeApplicationUpgradeDescription(
            string upgradeDomainName)
        {
            upgradeDomainName.ThrowIfNull(nameof(upgradeDomainName));
            this.UpgradeDomainName = upgradeDomainName;
        }

        /// <summary>
        /// Gets the name of the upgrade domain in which to resume the upgrade.
        /// </summary>
        public string UpgradeDomainName { get; }
    }
}
