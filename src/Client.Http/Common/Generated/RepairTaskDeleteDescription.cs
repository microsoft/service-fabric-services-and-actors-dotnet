// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a request to delete a completed repair task.
    /// 
    /// This type supports the Service Fabric platform; it is not meant to be used directly from your code.
    /// </summary>
    public partial class RepairTaskDeleteDescription
    {
        /// <summary>
        /// Initializes a new instance of the RepairTaskDeleteDescription class.
        /// </summary>
        /// <param name="taskId">The ID of the completed repair task to be deleted.</param>
        /// <param name="version">The current version number of the repair task. If non-zero, then the request will only
        /// succeed if this value matches the actual current version of the repair task. If zero, then no version check is
        /// performed.</param>
        public RepairTaskDeleteDescription(
            string taskId,
            string version = default(string))
        {
            taskId.ThrowIfNull(nameof(taskId));
            this.TaskId = taskId;
            this.Version = version;
        }

        /// <summary>
        /// Gets the ID of the completed repair task to be deleted.
        /// </summary>
        public string TaskId { get; }

        /// <summary>
        /// Gets the current version number of the repair task. If non-zero, then the request will only succeed if this value
        /// matches the actual current version of the repair task. If zero, then no version check is performed.
        /// </summary>
        public string Version { get; }
    }
}
