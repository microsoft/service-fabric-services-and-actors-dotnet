// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Service state summary of Service Fabric Upgrade Orchestration Service.
    /// </summary>
    public partial class UpgradeOrchestrationServiceStateSummary
    {
        /// <summary>
        /// Initializes a new instance of the UpgradeOrchestrationServiceStateSummary class.
        /// </summary>
        /// <param name="currentCodeVersion">The current code version of the cluster.</param>
        /// <param name="currentManifestVersion">The current manifest version of the cluster.</param>
        /// <param name="targetCodeVersion">The target code version of  the cluster.</param>
        /// <param name="targetManifestVersion">The target manifest version of the cluster.</param>
        /// <param name="pendingUpgradeType">The type of the pending upgrade of the cluster.</param>
        public UpgradeOrchestrationServiceStateSummary(
            string currentCodeVersion = default(string),
            string currentManifestVersion = default(string),
            string targetCodeVersion = default(string),
            string targetManifestVersion = default(string),
            string pendingUpgradeType = default(string))
        {
            this.CurrentCodeVersion = currentCodeVersion;
            this.CurrentManifestVersion = currentManifestVersion;
            this.TargetCodeVersion = targetCodeVersion;
            this.TargetManifestVersion = targetManifestVersion;
            this.PendingUpgradeType = pendingUpgradeType;
        }

        /// <summary>
        /// Gets the current code version of the cluster.
        /// </summary>
        public string CurrentCodeVersion { get; }

        /// <summary>
        /// Gets the current manifest version of the cluster.
        /// </summary>
        public string CurrentManifestVersion { get; }

        /// <summary>
        /// Gets the target code version of  the cluster.
        /// </summary>
        public string TargetCodeVersion { get; }

        /// <summary>
        /// Gets the target manifest version of the cluster.
        /// </summary>
        public string TargetManifestVersion { get; }

        /// <summary>
        /// Gets the type of the pending upgrade of the cluster.
        /// </summary>
        public string PendingUpgradeType { get; }
    }
}
