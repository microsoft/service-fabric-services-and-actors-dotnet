// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the current in-progress upgrade units.
    /// </summary>
    public partial class CurrentUpgradeUnitsProgressInfo
    {
        /// <summary>
        /// Initializes a new instance of the CurrentUpgradeUnitsProgressInfo class.
        /// </summary>
        /// <param name="domainName">The name of the upgrade domain. Not applicable to node-by-node upgrades.</param>
        /// <param name="nodeUpgradeProgressList">List of upgrading nodes and their statuses</param>
        public CurrentUpgradeUnitsProgressInfo(
            string domainName = default(string),
            IEnumerable<NodeUpgradeProgressInfo> nodeUpgradeProgressList = default(IEnumerable<NodeUpgradeProgressInfo>))
        {
            this.DomainName = domainName;
            this.NodeUpgradeProgressList = nodeUpgradeProgressList;
        }

        /// <summary>
        /// Gets the name of the upgrade domain. Not applicable to node-by-node upgrades.
        /// </summary>
        public string DomainName { get; }

        /// <summary>
        /// Gets list of upgrading nodes and their statuses
        /// </summary>
        public IEnumerable<NodeUpgradeProgressInfo> NodeUpgradeProgressList { get; }
    }
}
