// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The detailed upgrade progress for nodes in the current upgrade domain at the point of failure. Not applicable to
    /// node-by-node upgrades.
    /// </summary>
    public partial class FailedUpgradeDomainProgressObject
    {
        /// <summary>
        /// Initializes a new instance of the FailedUpgradeDomainProgressObject class.
        /// </summary>
        /// <param name="domainName">The name of the upgrade domain</param>
        /// <param name="nodeUpgradeProgressList">List of upgrading nodes and their statuses</param>
        public FailedUpgradeDomainProgressObject(
            string domainName = default(string),
            IEnumerable<NodeUpgradeProgressInfo> nodeUpgradeProgressList = default(IEnumerable<NodeUpgradeProgressInfo>))
        {
            this.DomainName = domainName;
            this.NodeUpgradeProgressList = nodeUpgradeProgressList;
        }

        /// <summary>
        /// Gets the name of the upgrade domain
        /// </summary>
        public string DomainName { get; }

        /// <summary>
        /// Gets list of upgrading nodes and their statuses
        /// </summary>
        public IEnumerable<NodeUpgradeProgressInfo> NodeUpgradeProgressList { get; }
    }
}
