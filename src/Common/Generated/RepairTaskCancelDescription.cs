// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a request to cancel a repair task.
    /// 
    /// This type supports the Service Fabric platform; it is not meant to be used directly from your code.
    /// </summary>
    public partial class RepairTaskCancelDescription
    {
        /// <summary>
        /// Initializes a new instance of the RepairTaskCancelDescription class.
        /// </summary>
        /// <param name="taskId">The ID of the repair task.</param>
        /// <param name="version">The current version number of the repair task. If non-zero, then the request will only
        /// succeed if this value matches the actual current version of the repair task. If zero, then no version check is
        /// performed.</param>
        /// <param name="requestAbort">_True_ if the repair should be stopped as soon as possible even if it has already
        /// started executing. _False_ if the repair should be cancelled only if execution has not yet started.</param>
        public RepairTaskCancelDescription(
            string taskId,
            string version = default(string),
            bool? requestAbort = default(bool?))
        {
            taskId.ThrowIfNull(nameof(taskId));
            this.TaskId = taskId;
            this.Version = version;
            this.RequestAbort = requestAbort;
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

        /// <summary>
        /// Gets _True_ if the repair should be stopped as soon as possible even if it has already started executing. _False_
        /// if the repair should be cancelled only if execution has not yet started.
        /// </summary>
        public bool? RequestAbort { get; }
    }
}
