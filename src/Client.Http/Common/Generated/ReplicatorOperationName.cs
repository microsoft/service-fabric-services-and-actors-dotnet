// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ReplicatorOperationName.
    /// </summary>
    public enum ReplicatorOperationName
    {
        /// <summary>
        /// Default value if the replicator is not yet ready.
        /// </summary>
        Invalid,

        /// <summary>
        /// Replicator is not running any operation from Service Fabric perspective.
        /// </summary>
        None,

        /// <summary>
        /// Replicator is opening.
        /// </summary>
        Open,

        /// <summary>
        /// Replicator is in the process of changing its role.
        /// </summary>
        ChangeRole,

        /// <summary>
        /// Due to a change in the replica set, replicator is being updated with its Epoch.
        /// </summary>
        UpdateEpoch,

        /// <summary>
        /// Replicator is closing.
        /// </summary>
        Close,

        /// <summary>
        /// Replicator is being aborted.
        /// </summary>
        Abort,

        /// <summary>
        /// Replicator is handling the data loss condition, where the user service may potentially be recovering state from an
        /// external source.
        /// </summary>
        OnDataLoss,

        /// <summary>
        /// Replicator is waiting for a quorum of replicas to be caught up to the latest state.
        /// </summary>
        WaitForCatchup,

        /// <summary>
        /// Replicator is in the process of building one or more replicas.
        /// </summary>
        Build,
    }
}
