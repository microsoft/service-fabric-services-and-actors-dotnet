// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a standalone cluster configuration upgrade status.
    /// </summary>
    public partial class ClusterConfigurationUpgradeStatusInfo
    {
        /// <summary>
        /// Initializes a new instance of the ClusterConfigurationUpgradeStatusInfo class.
        /// </summary>
        /// <param name="upgradeState">The state of the upgrade domain. Possible values include: 'Invalid',
        /// 'RollingBackInProgress', 'RollingBackCompleted', 'RollingForwardPending', 'RollingForwardInProgress',
        /// 'RollingForwardCompleted', 'Failed'</param>
        /// <param name="progressStatus">The cluster manifest version.</param>
        /// <param name="configVersion">The cluster configuration version.</param>
        /// <param name="details">The cluster upgrade status details.</param>
        public ClusterConfigurationUpgradeStatusInfo(
            UpgradeState? upgradeState = default(UpgradeState?),
            int? progressStatus = default(int?),
            string configVersion = default(string),
            string details = default(string))
        {
            this.UpgradeState = upgradeState;
            this.ProgressStatus = progressStatus;
            this.ConfigVersion = configVersion;
            this.Details = details;
        }

        /// <summary>
        /// Gets the state of the upgrade domain. Possible values include: 'Invalid', 'RollingBackInProgress',
        /// 'RollingBackCompleted', 'RollingForwardPending', 'RollingForwardInProgress', 'RollingForwardCompleted', 'Failed'
        /// </summary>
        public UpgradeState? UpgradeState { get; }

        /// <summary>
        /// Gets the cluster manifest version.
        /// </summary>
        public int? ProgressStatus { get; }

        /// <summary>
        /// Gets the cluster configuration version.
        /// </summary>
        public string ConfigVersion { get; }

        /// <summary>
        /// Gets the cluster upgrade status details.
        /// </summary>
        public string Details { get; }
    }
}
