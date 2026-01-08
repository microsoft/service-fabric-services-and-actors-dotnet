// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a request for forced approval of a repair task.
    /// 
    /// This type supports the Service Fabric platform; it is not meant to be used directly from your code.
    /// </summary>
    public partial class RepairTaskApproveDescription
    {
        /// <summary>
        /// Initializes a new instance of the RepairTaskApproveDescription class.
        /// </summary>
        /// <param name="taskId">The ID of the repair task.</param>
        /// <param name="version">The current version number of the repair task. If non-zero, then the request will only
        /// succeed if this value matches the actual current version of the repair task. If zero, then no version check is
        /// performed.</param>
        public RepairTaskApproveDescription(
            string taskId,
            string version = default(string))
        {
            taskId.ThrowIfNull(nameof(taskId));
            this.TaskId = taskId;
            this.Version = version;
        }

        /// <summary>
        /// Gets the ID of the repair task.
        /// </summary>
        public string TaskId { get; }

        /// <summary>
        /// Gets the current version number of the repair task. If non-zero, then the request will only succeed if this value
        /// matches the actual current version of the repair task. If zero, then no version check is performed.
        /// </summary>
        public string Version { get; }
    }
}
