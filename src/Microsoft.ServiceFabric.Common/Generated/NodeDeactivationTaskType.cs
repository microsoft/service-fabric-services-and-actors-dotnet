// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for NodeDeactivationTaskType.
    /// </summary>
    public enum NodeDeactivationTaskType
    {
        /// <summary>
        /// Indicates the node deactivation task type is invalid. All Service Fabric enumerations have the invalid type. The
        /// value is zero. This value is not used.
        /// </summary>
        Invalid,

        /// <summary>
        /// Specifies the task created by Infrastructure hosting the nodes. The value is 1.
        /// </summary>
        Infrastructure,

        /// <summary>
        /// Specifies the task that was created by the Repair Manager service. The value is 2.
        /// </summary>
        Repair,

        /// <summary>
        /// Specifies that the task was created by using the public API. The value is 3.
        /// </summary>
        Client,
    }
}
