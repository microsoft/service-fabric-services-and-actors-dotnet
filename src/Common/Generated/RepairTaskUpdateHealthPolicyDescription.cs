// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a request to update the health policy of a repair task.
    /// 
    /// This type supports the Service Fabric platform; it is not meant to be used directly from your code.
    /// </summary>
    public partial class RepairTaskUpdateHealthPolicyDescription
    {
        /// <summary>
        /// Initializes a new instance of the RepairTaskUpdateHealthPolicyDescription class.
        /// </summary>
        /// <param name="taskId">The ID of the repair task to be updated.</param>
        /// <param name="version">The current version number of the repair task. If non-zero, then the request will only
        /// succeed if this value matches the actual current value of the repair task. If zero, then no version check is
        /// performed.</param>
        /// <param name="performPreparingHealthCheck">A boolean indicating if health check is to be performed in the Preparing
        /// stage of the repair task. If not specified the existing value should not be altered. Otherwise, specify the desired
        /// new value.</param>
        /// <param name="performRestoringHealthCheck">A boolean indicating if health check is to be performed in the Restoring
        /// stage of the repair task. If not specified the existing value should not be altered. Otherwise, specify the desired
        /// new value.</param>
        public RepairTaskUpdateHealthPolicyDescription(
            string taskId,
            string version = default(string),
            bool? performPreparingHealthCheck = default(bool?),
            bool? performRestoringHealthCheck = default(bool?))
        {
            taskId.ThrowIfNull(nameof(taskId));
            this.TaskId = taskId;
            this.Version = version;
            this.PerformPreparingHealthCheck = performPreparingHealthCheck;
            this.PerformRestoringHealthCheck = performRestoringHealthCheck;
        }

        /// <summary>
        /// Gets the ID of the repair task to be updated.
        /// </summary>
        public string TaskId { get; }

        /// <summary>
        /// Gets the current version number of the repair task. If non-zero, then the request will only succeed if this value
        /// matches the actual current value of the repair task. If zero, then no version check is performed.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets a boolean indicating if health check is to be performed in the Preparing stage of the repair task. If not
        /// specified the existing value should not be altered. Otherwise, specify the desired new value.
        /// </summary>
        public bool? PerformPreparingHealthCheck { get; }

        /// <summary>
        /// Gets a boolean indicating if health check is to be performed in the Restoring stage of the repair task. If not
        /// specified the existing value should not be altered. Otherwise, specify the desired new value.
        /// </summary>
        public bool? PerformRestoringHealthCheck { get; }
    }
}
