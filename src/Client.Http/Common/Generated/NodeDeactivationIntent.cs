// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for NodeDeactivationIntent.
    /// </summary>
    public enum NodeDeactivationIntent
    {
        /// <summary>
        /// Indicates the node deactivation intent is invalid. All Service Fabric enumerations have the invalid type. The value
        /// is zero. This value is not used.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the node should be paused. The value is 1.
        /// </summary>
        Pause,

        /// <summary>
        /// Indicates that the intent is for the node to be restarted after a short period of time. Service Fabric does not
        /// restart the node, this action is done outside of Service Fabric. The value is 2.
        /// </summary>
        Restart,

        /// <summary>
        /// Indicates that the intent is to reimage the node. Service Fabric does not reimage the node, this action is done
        /// outside of Service Fabric. The value is 3.
        /// </summary>
        RemoveData,

        /// <summary>
        /// Indicates that the node is being decommissioned and is not expected to return. Service Fabric does not decommission
        /// the node, this action is done outside of Service Fabric. The value is 4.
        /// </summary>
        RemoveNode,
    }
}
