// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for DeactivationIntent.
    /// </summary>
    public enum DeactivationIntent
    {
        /// <summary>
        /// Indicates that the node should be paused. You might specify this setting to debug replicas that run on the node.
        /// The value is 1.
        /// </summary>
        Pause,

        /// <summary>
        /// Indicates that the intent is for the node to be restarted after a short period of time. You might specify this
        /// setting when a node restart is required for installing a patch. The value is 2.
        /// </summary>
        Restart,

        /// <summary>
        /// Indicates the intent is for the node to remove data. You might specify this setting when the hard disk is being
        /// re-imaged. The value is 3.
        /// </summary>
        RemoveData,

        /// <summary>
        /// Indicates that the data on the node is to be permanently lost. You might specify this setting when the node is
        /// being removed from the cluster. The value is 4.
        /// </summary>
        RemoveNode,
    }
}
