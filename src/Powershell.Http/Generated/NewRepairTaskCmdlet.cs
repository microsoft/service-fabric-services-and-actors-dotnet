// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Creates a new repair task.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFRepairTask")]
    public partial class NewRepairTaskCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Node flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_Node__Node_")]
        public SwitchParameter Node { get; set; }

        /// <summary>
        /// Gets or sets TaskId. The ID of the repair task.
        /// </summary>
        [Parameter(Mandatory = true, Position = 2)]
        public string TaskId { get; set; }

        /// <summary>
        /// Gets or sets State. The workflow state of the repair task. Valid initial states are Created, Claimed, and
        /// Preparing. Possible values include: 'Invalid', 'Created', 'Claimed', 'Preparing', 'Approved', 'Executing',
        /// 'Restoring', 'Completed'
        /// </summary>
        [Parameter(Mandatory = true, Position = 3)]
        public State? State { get; set; }

        /// <summary>
        /// Gets or sets Action. The requested repair action. Must be specified when the repair task is created, and is
        /// immutable once set.
        /// </summary>
        [Parameter(Mandatory = true, Position = 4)]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets Version. The version of the repair task.
        /// When creating a new repair task, the version must be set to zero.  When updating a repair task,
        /// the version is used for optimistic concurrency checks.  If the version is
        /// set to zero, the update will not check for write conflicts.  If the version is set to a non-zero value, then the
        /// update will only succeed if the actual current version of the repair task matches this value.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets Description. A description of the purpose of the repair task, or other informational details.
        /// May be set when the repair task is created, and is immutable once set.
        /// </summary>
        [Parameter(Mandatory = false, Position = 6)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Flags. A bitwise-OR of the following values, which gives additional details about the status of the
        /// repair task.
        /// - 1 - Cancellation of the repair has been requested
        /// - 2 - Abort of the repair has been requested
        /// - 4 - Approval of the repair was forced via client request
        /// </summary>
        [Parameter(Mandatory = false, Position = 7)]
        public int? Flags { get; set; }

        /// <summary>
        /// Gets or sets NodeNames. The list of nodes targeted by a repair action.
        /// </summary>
        [Parameter(Mandatory = false, Position = 8, ParameterSetName = "_Node__Node_")]
        public IEnumerable<string> NodeNames { get; set; }

        /// <summary>
        /// Gets or sets Executor. The name of the repair executor. Must be specified in Claimed and later states, and is
        /// immutable once set.
        /// </summary>
        [Parameter(Mandatory = false, Position = 9)]
        public string Executor { get; set; }

        /// <summary>
        /// Gets or sets ExecutorData. A data string that the repair executor can use to store its internal state.
        /// </summary>
        [Parameter(Mandatory = false, Position = 10)]
        public string ExecutorData { get; set; }

        /// <summary>
        /// Gets or sets NodeImpactList. The list of nodes impacted by a repair action and their respective expected impact.
        /// </summary>
        [Parameter(Mandatory = false, Position = 11, ParameterSetName = "_Node__Node_")]
        public IEnumerable<NodeImpact> NodeImpactList { get; set; }

        /// <summary>
        /// Gets or sets ResultStatus. A value describing the overall result of the repair task execution. Must be specified in
        /// the Restoring and later states, and is immutable once set. Possible values include: 'Invalid', 'Succeeded',
        /// 'Cancelled', 'Interrupted', 'Failed', 'Pending'
        /// </summary>
        [Parameter(Mandatory = false, Position = 12)]
        public ResultStatus? ResultStatus { get; set; }

        /// <summary>
        /// Gets or sets ResultCode. A numeric value providing additional details about the result of the repair task
        /// execution.
        /// May be specified in the Restoring and later states, and is immutable once set.
        /// </summary>
        [Parameter(Mandatory = false, Position = 13)]
        public int? ResultCode { get; set; }

        /// <summary>
        /// Gets or sets ResultDetails. A string providing additional details about the result of the repair task execution.
        /// May be specified in the Restoring and later states, and is immutable once set.
        /// </summary>
        [Parameter(Mandatory = false, Position = 14)]
        public string ResultDetails { get; set; }

        /// <summary>
        /// Gets or sets CreatedUtcTimestamp. The time when the repair task entered the Created state.
        /// </summary>
        [Parameter(Mandatory = false, Position = 15)]
        public DateTime? CreatedUtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets ClaimedUtcTimestamp. The time when the repair task entered the Claimed state.
        /// </summary>
        [Parameter(Mandatory = false, Position = 16)]
        public DateTime? ClaimedUtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets PreparingUtcTimestamp. The time when the repair task entered the Preparing state.
        /// </summary>
        [Parameter(Mandatory = false, Position = 17)]
        public DateTime? PreparingUtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets ApprovedUtcTimestamp. The time when the repair task entered the Approved state
        /// </summary>
        [Parameter(Mandatory = false, Position = 18)]
        public DateTime? ApprovedUtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets ExecutingUtcTimestamp. The time when the repair task entered the Executing state
        /// </summary>
        [Parameter(Mandatory = false, Position = 19)]
        public DateTime? ExecutingUtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets RestoringUtcTimestamp. The time when the repair task entered the Restoring state
        /// </summary>
        [Parameter(Mandatory = false, Position = 20)]
        public DateTime? RestoringUtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets CompletedUtcTimestamp. The time when the repair task entered the Completed state
        /// </summary>
        [Parameter(Mandatory = false, Position = 21)]
        public DateTime? CompletedUtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets PreparingHealthCheckStartUtcTimestamp. The time when the repair task started the health check in the
        /// Preparing state.
        /// </summary>
        [Parameter(Mandatory = false, Position = 22)]
        public DateTime? PreparingHealthCheckStartUtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets PreparingHealthCheckEndUtcTimestamp. The time when the repair task completed the health check in the
        /// Preparing state.
        /// </summary>
        [Parameter(Mandatory = false, Position = 23)]
        public DateTime? PreparingHealthCheckEndUtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets RestoringHealthCheckStartUtcTimestamp. The time when the repair task started the health check in the
        /// Restoring state.
        /// </summary>
        [Parameter(Mandatory = false, Position = 24)]
        public DateTime? RestoringHealthCheckStartUtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets RestoringHealthCheckEndUtcTimestamp. The time when the repair task completed the health check in the
        /// Restoring state.
        /// </summary>
        [Parameter(Mandatory = false, Position = 25)]
        public DateTime? RestoringHealthCheckEndUtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets PreparingHealthCheckState. The workflow state of the health check when the repair task is in the
        /// Preparing state. Possible values include: 'NotStarted', 'InProgress', 'Succeeded', 'Skipped', 'TimedOut'
        /// 
        /// Specifies the workflow state of a repair task's health check. This type supports the Service Fabric platform; it is
        /// not meant to be used directly from your code.
        /// </summary>
        [Parameter(Mandatory = false, Position = 26)]
        public RepairTaskHealthCheckState? PreparingHealthCheckState { get; set; }

        /// <summary>
        /// Gets or sets RestoringHealthCheckState. The workflow state of the health check when the repair task is in the
        /// Restoring state. Possible values include: 'NotStarted', 'InProgress', 'Succeeded', 'Skipped', 'TimedOut'
        /// 
        /// Specifies the workflow state of a repair task's health check. This type supports the Service Fabric platform; it is
        /// not meant to be used directly from your code.
        /// </summary>
        [Parameter(Mandatory = false, Position = 27)]
        public RepairTaskHealthCheckState? RestoringHealthCheckState { get; set; }

        /// <summary>
        /// Gets or sets PerformPreparingHealthCheck. A value to determine if health checks will be performed when the repair
        /// task enters the Preparing state.
        /// </summary>
        [Parameter(Mandatory = false, Position = 28)]
        public bool? PerformPreparingHealthCheck { get; set; }

        /// <summary>
        /// Gets or sets PerformRestoringHealthCheck. A value to determine if health checks will be performed when the repair
        /// task enters the Restoring state.
        /// </summary>
        [Parameter(Mandatory = false, Position = 29)]
        public bool? PerformRestoringHealthCheck { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            RepairTargetDescriptionBase repairTargetDescriptionBase = null;
            if (this.Node.IsPresent)
            {
                repairTargetDescriptionBase = new NodeRepairTargetDescription(
                    nodeNames: this.NodeNames);
            }

            RepairImpactDescriptionBase repairImpactDescriptionBase = null;
            if (this.Node.IsPresent)
            {
                repairImpactDescriptionBase = new NodeRepairImpactDescription(
                    nodeImpactList: this.NodeImpactList);
            }

            var repairTaskHistory = new RepairTaskHistory(
            createdUtcTimestamp: this.CreatedUtcTimestamp,
            claimedUtcTimestamp: this.ClaimedUtcTimestamp,
            preparingUtcTimestamp: this.PreparingUtcTimestamp,
            approvedUtcTimestamp: this.ApprovedUtcTimestamp,
            executingUtcTimestamp: this.ExecutingUtcTimestamp,
            restoringUtcTimestamp: this.RestoringUtcTimestamp,
            completedUtcTimestamp: this.CompletedUtcTimestamp,
            preparingHealthCheckStartUtcTimestamp: this.PreparingHealthCheckStartUtcTimestamp,
            preparingHealthCheckEndUtcTimestamp: this.PreparingHealthCheckEndUtcTimestamp,
            restoringHealthCheckStartUtcTimestamp: this.RestoringHealthCheckStartUtcTimestamp,
            restoringHealthCheckEndUtcTimestamp: this.RestoringHealthCheckEndUtcTimestamp);

            var repairTask = new RepairTask(
            taskId: this.TaskId,
            state: this.State,
            action: this.Action,
            version: this.Version,
            description: this.Description,
            flags: this.Flags,
            target: repairTargetDescriptionBase,
            executor: this.Executor,
            executorData: this.ExecutorData,
            impact: repairImpactDescriptionBase,
            resultStatus: this.ResultStatus,
            resultCode: this.ResultCode,
            resultDetails: this.ResultDetails,
            history: repairTaskHistory,
            preparingHealthCheckState: this.PreparingHealthCheckState,
            restoringHealthCheckState: this.RestoringHealthCheckState,
            performPreparingHealthCheck: this.PerformPreparingHealthCheck,
            performRestoringHealthCheck: this.PerformRestoringHealthCheck);

            var result = this.ServiceFabricClient.Repairs.CreateRepairTaskAsync(
                repairTask: repairTask,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            if (result != null)
            {
                this.WriteObject(this.FormatOutput(result));
            }
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            return output;
        }
    }
}
