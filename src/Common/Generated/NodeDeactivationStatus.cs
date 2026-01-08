// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for NodeDeactivationStatus.
    /// </summary>
    public enum NodeDeactivationStatus
    {
        /// <summary>
        /// No status is associated with the task. The value is zero.
        /// </summary>
        None,

        /// <summary>
        /// When a node is deactivated Service Fabric performs checks to ensure that the operation is safe to proceed to ensure
        /// availability of the service and reliability of the state. This value indicates that one or more safety checks are
        /// in progress. The value is 1.
        /// </summary>
        SafetyCheckInProgress,

        /// <summary>
        /// When a node is deactivated Service Fabric performs checks to ensure that the operation is safe to proceed to ensure
        /// availability of the service and reliability of the state. This value indicates that all safety checks have been
        /// completed. The value is 2.
        /// </summary>
        SafetyCheckComplete,

        /// <summary>
        /// The task is completed. The value is 3.
        /// </summary>
        Completed,
    }
}
