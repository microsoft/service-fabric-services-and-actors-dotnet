// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for NodeStatusFilter.
    /// </summary>
    public enum NodeStatusFilter
    {
        /// <summary>
        /// This filter value will match all of the nodes excepts the ones with status as Unknown or Removed.
        /// </summary>
        Default,

        /// <summary>
        /// This filter value will match all of the nodes.
        /// </summary>
        All,

        /// <summary>
        /// This filter value will match nodes that are Up.
        /// </summary>
        Up,

        /// <summary>
        /// This filter value will match nodes that are Down.
        /// </summary>
        Down,

        /// <summary>
        /// This filter value will match nodes that are in the process of being enabled with status as Enabling.
        /// </summary>
        Enabling,

        /// <summary>
        /// This filter value will match nodes that are in the process of being disabled with status as Disabling.
        /// </summary>
        Disabling,

        /// <summary>
        /// This filter value will match nodes that are Disabled.
        /// </summary>
        Disabled,

        /// <summary>
        /// This filter value will match nodes whose status is Unknown. A node would be in Unknown state if Service Fabric does
        /// not have authoritative information about that node. This can happen if the system learns about a node at runtime.
        /// </summary>
        Unknown,

        /// <summary>
        /// This filter value will match nodes whose status is Removed. These are the nodes that are removed from the cluster
        /// using the RemoveNodeState API.
        /// </summary>
        Removed,
    }
}
