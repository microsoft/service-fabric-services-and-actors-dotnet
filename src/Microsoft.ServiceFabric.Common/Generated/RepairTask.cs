// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repair task, which includes information about what kind of repair was requested, what its progress is,
    /// and what its final result was.
    /// 
    /// This type supports the Service Fabric platform; it is not meant to be used directly from your code.
    /// </summary>
    public partial class RepairTask
    {
        /// <summary>
        /// Initializes a new instance of the RepairTask class.
        /// </summary>
        /// <param name="taskId">The ID of the repair task.</param>
        /// <param name="state">The workflow state of the repair task. Valid initial states are Created, Claimed, and
        /// Preparing. Possible values include: 'Invalid', 'Created', 'Claimed', 'Preparing', 'Approved', 'Executing',
        /// 'Restoring', 'Completed'</param>
        /// <param name="action">The requested repair action. Must be specified when the repair task is created, and is
        /// immutable once set.
        /// </param>
        /// <param name="version">The version of the repair task.
        /// When creating a new repair task, the version must be set to zero.  When updating a repair task,
        /// the version is used for optimistic concurrency checks.  If the version is
        /// set to zero, the update will not check for write conflicts.  If the version is set to a non-zero value, then the
        /// update will only succeed if the actual current version of the repair task matches this value.
        /// </param>
        /// <param name="description">A description of the purpose of the repair task, or other informational details.
        /// May be set when the repair task is created, and is immutable once set.
        /// </param>
        /// <param name="flags">A bitwise-OR of the following values, which gives additional details about the status of the
        /// repair task.
        /// - 1 - Cancellation of the repair has been requested
        /// - 2 - Abort of the repair has been requested
        /// - 4 - Approval of the repair was forced via client request
        /// </param>
        /// <param name="target">The target object determines what actions the system will take to prepare for the impact of
        /// the repair, prior to approving execution of the repair.
        /// May be set when the repair task is created, and is immutable once set.
        /// </param>
        /// <param name="executor">The name of the repair executor. Must be specified in Claimed and later states, and is
        /// immutable once set.</param>
        /// <param name="executorData">A data string that the repair executor can use to store its internal state.</param>
        /// <param name="impact">The impact object determines what actions the system will take to prepare for the impact of
        /// the repair, prior to approving execution of the repair.
        /// Impact must be specified by the repair executor when transitioning to the Preparing state, and is immutable once
        /// set.
        /// </param>
        /// <param name="resultStatus">A value describing the overall result of the repair task execution. Must be specified in
        /// the Restoring and later states, and is immutable once set. Possible values include: 'Invalid', 'Succeeded',
        /// 'Cancelled', 'Interrupted', 'Failed', 'Pending'</param>
        /// <param name="resultCode">A numeric value providing additional details about the result of the repair task
        /// execution.
        /// May be specified in the Restoring and later states, and is immutable once set.
        /// </param>
        /// <param name="resultDetails">A string providing additional details about the result of the repair task execution.
        /// May be specified in the Restoring and later states, and is immutable once set.
        /// </param>
        /// <param name="history">An object that contains timestamps of the repair task's state transitions.
        /// These timestamps are updated by the system, and cannot be directly modified.
        /// </param>
        /// <param name="preparingHealthCheckState">The workflow state of the health check when the repair task is in the
        /// Preparing state. Possible values include: 'NotStarted', 'InProgress', 'Succeeded', 'Skipped', 'TimedOut'
        /// 
        /// Specifies the workflow state of a repair task's health check. This type supports the Service Fabric platform; it is
        /// not meant to be used directly from your code.
        /// </param>
        /// <param name="restoringHealthCheckState">The workflow state of the health check when the repair task is in the
        /// Restoring state. Possible values include: 'NotStarted', 'InProgress', 'Succeeded', 'Skipped', 'TimedOut'
        /// 
        /// Specifies the workflow state of a repair task's health check. This type supports the Service Fabric platform; it is
        /// not meant to be used directly from your code.
        /// </param>
        /// <param name="performPreparingHealthCheck">A value to determine if health checks will be performed when the repair
        /// task enters the Preparing state.</param>
        /// <param name="performRestoringHealthCheck">A value to determine if health checks will be performed when the repair
        /// task enters the Restoring state.</param>
        public RepairTask(
            string taskId,
            State? state,
            string action,
            string version = default(string),
            string description = default(string),
            int? flags = default(int?),
            RepairTargetDescriptionBase target = default(RepairTargetDescriptionBase),
            string executor = default(string),
            string executorData = default(string),
            RepairImpactDescriptionBase impact = default(RepairImpactDescriptionBase),
            ResultStatus? resultStatus = default(ResultStatus?),
            int? resultCode = default(int?),
            string resultDetails = default(string),
            RepairTaskHistory history = default(RepairTaskHistory),
            RepairTaskHealthCheckState? preparingHealthCheckState = default(RepairTaskHealthCheckState?),
            RepairTaskHealthCheckState? restoringHealthCheckState = default(RepairTaskHealthCheckState?),
            bool? performPreparingHealthCheck = default(bool?),
            bool? performRestoringHealthCheck = default(bool?))
        {
            taskId.ThrowIfNull(nameof(taskId));
            state.ThrowIfNull(nameof(state));
            action.ThrowIfNull(nameof(action));
            this.TaskId = taskId;
            this.State = state;
            this.Action = action;
            this.Version = version;
            this.Description = description;
            this.Flags = flags;
            this.Target = target;
            this.Executor = executor;
            this.ExecutorData = executorData;
            this.Impact = impact;
            this.ResultStatus = resultStatus;
            this.ResultCode = resultCode;
            this.ResultDetails = resultDetails;
            this.History = history;
            this.PreparingHealthCheckState = preparingHealthCheckState;
            this.RestoringHealthCheckState = restoringHealthCheckState;
            this.PerformPreparingHealthCheck = performPreparingHealthCheck;
            this.PerformRestoringHealthCheck = performRestoringHealthCheck;
        }

        /// <summary>
        /// Gets the ID of the repair task.
        /// </summary>
        public string TaskId { get; }

        /// <summary>
        /// Gets the version of the repair task.
        /// When creating a new repair task, the version must be set to zero.  When updating a repair task,
        /// the version is used for optimistic concurrency checks.  If the version is
        /// set to zero, the update will not check for write conflicts.  If the version is set to a non-zero value, then the
        /// update will only succeed if the actual current version of the repair task matches this value.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets a description of the purpose of the repair task, or other informational details.
        /// May be set when the repair task is created, and is immutable once set.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the workflow state of the repair task. Valid initial states are Created, Claimed, and Preparing. Possible
        /// values include: 'Invalid', 'Created', 'Claimed', 'Preparing', 'Approved', 'Executing', 'Restoring', 'Completed'
        /// </summary>
        public State? State { get; }

        /// <summary>
        /// Gets a bitwise-OR of the following values, which gives additional details about the status of the repair task.
        /// - 1 - Cancellation of the repair has been requested
        /// - 2 - Abort of the repair has been requested
        /// - 4 - Approval of the repair was forced via client request
        /// </summary>
        public int? Flags { get; }

        /// <summary>
        /// Gets the requested repair action. Must be specified when the repair task is created, and is immutable once set.
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// Gets the target object determines what actions the system will take to prepare for the impact of the repair, prior
        /// to approving execution of the repair.
        /// May be set when the repair task is created, and is immutable once set.
        /// </summary>
        public RepairTargetDescriptionBase Target { get; }

        /// <summary>
        /// Gets the name of the repair executor. Must be specified in Claimed and later states, and is immutable once set.
        /// </summary>
        public string Executor { get; }

        /// <summary>
        /// Gets a data string that the repair executor can use to store its internal state.
        /// </summary>
        public string ExecutorData { get; }

        /// <summary>
        /// Gets the impact object determines what actions the system will take to prepare for the impact of the repair, prior
        /// to approving execution of the repair.
        /// Impact must be specified by the repair executor when transitioning to the Preparing state, and is immutable once
        /// set.
        /// </summary>
        public RepairImpactDescriptionBase Impact { get; }

        /// <summary>
        /// Gets a value describing the overall result of the repair task execution. Must be specified in the Restoring and
        /// later states, and is immutable once set. Possible values include: 'Invalid', 'Succeeded', 'Cancelled',
        /// 'Interrupted', 'Failed', 'Pending'
        /// </summary>
        public ResultStatus? ResultStatus { get; }

        /// <summary>
        /// Gets a numeric value providing additional details about the result of the repair task execution.
        /// May be specified in the Restoring and later states, and is immutable once set.
        /// </summary>
        public int? ResultCode { get; }

        /// <summary>
        /// Gets a string providing additional details about the result of the repair task execution.
        /// May be specified in the Restoring and later states, and is immutable once set.
        /// </summary>
        public string ResultDetails { get; }

        /// <summary>
        /// Gets an object that contains timestamps of the repair task's state transitions.
        /// These timestamps are updated by the system, and cannot be directly modified.
        /// </summary>
        public RepairTaskHistory History { get; }

        /// <summary>
        /// Gets the workflow state of the health check when the repair task is in the Preparing state. Possible values
        /// include: 'NotStarted', 'InProgress', 'Succeeded', 'Skipped', 'TimedOut'
        /// 
        /// Specifies the workflow state of a repair task's health check. This type supports the Service Fabric platform; it is
        /// not meant to be used directly from your code.
        /// </summary>
        public RepairTaskHealthCheckState? PreparingHealthCheckState { get; }

        /// <summary>
        /// Gets the workflow state of the health check when the repair task is in the Restoring state. Possible values
        /// include: 'NotStarted', 'InProgress', 'Succeeded', 'Skipped', 'TimedOut'
        /// 
        /// Specifies the workflow state of a repair task's health check. This type supports the Service Fabric platform; it is
        /// not meant to be used directly from your code.
        /// </summary>
        public RepairTaskHealthCheckState? RestoringHealthCheckState { get; }

        /// <summary>
        /// Gets a value to determine if health checks will be performed when the repair task enters the Preparing state.
        /// </summary>
        public bool? PerformPreparingHealthCheck { get; }

        /// <summary>
        /// Gets a value to determine if health checks will be performed when the repair task enters the Restoring state.
        /// </summary>
        public bool? PerformRestoringHealthCheck { get; }
    }
}
