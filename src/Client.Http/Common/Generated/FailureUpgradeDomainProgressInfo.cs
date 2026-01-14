// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the upgrade domain progress at the time of upgrade failure.
    /// </summary>
    public partial class FailureUpgradeDomainProgressInfo
    {
        /// <summary>
        /// Initializes a new instance of the FailureUpgradeDomainProgressInfo class.
        /// </summary>
        /// <param name="domainName">The name of the upgrade domain</param>
        /// <param name="nodeUpgradeProgressList">List of upgrading nodes and their statuses</param>
        public FailureUpgradeDomainProgressInfo(
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
