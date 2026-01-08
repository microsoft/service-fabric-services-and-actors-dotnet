// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for NodeStatus.
    /// </summary>
    public enum NodeStatus
    {
        /// <summary>
        /// Indicates the node status is invalid. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates the node is up. The value is 1.
        /// </summary>
        Up,

        /// <summary>
        /// Indicates the node is down. The value is 2.
        /// </summary>
        Down,

        /// <summary>
        /// Indicates the node is in process of being enabled. The value is 3.
        /// </summary>
        Enabling,

        /// <summary>
        /// Indicates the node is in the process of being disabled. The value is 4.
        /// </summary>
        Disabling,

        /// <summary>
        /// Indicates the node is disabled. The value is 5.
        /// </summary>
        Disabled,

        /// <summary>
        /// Indicates the node is unknown. A node would be in Unknown state if Service Fabric does not have authoritative
        /// information about that node. This can happen if the system learns about a node at runtime.The value is 6.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates the node is removed. A node would be in Removed state if NodeStateRemoved API has been called for this
        /// node. In other words, Service Fabric has been informed that the persisted state on the node has been permanently
        /// lost. The value is 7.
        /// </summary>
        Removed,
    }
}
