// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the node deactivation. This information is valid for a node that is undergoing deactivation or
    /// has already been deactivated.
    /// </summary>
    public partial class NodeDeactivationInfo
    {
        /// <summary>
        /// Initializes a new instance of the NodeDeactivationInfo class.
        /// </summary>
        /// <param name="nodeDeactivationIntent">The intent or the reason for deactivating the node. Following are the possible
        /// values for it. Possible values include: 'Invalid', 'Pause', 'Restart', 'RemoveData', 'RemoveNode'</param>
        /// <param name="nodeDeactivationStatus">The status of node deactivation operation. Following are the possible values.
        /// Possible values include: 'None', 'SafetyCheckInProgress', 'SafetyCheckComplete', 'Completed'</param>
        /// <param name="nodeDeactivationTask">List of tasks representing the deactivation operation on the node.</param>
        /// <param name="pendingSafetyChecks">List of pending safety checks</param>
        public NodeDeactivationInfo(
            NodeDeactivationIntent? nodeDeactivationIntent = default(NodeDeactivationIntent?),
            NodeDeactivationStatus? nodeDeactivationStatus = default(NodeDeactivationStatus?),
            IEnumerable<NodeDeactivationTask> nodeDeactivationTask = default(IEnumerable<NodeDeactivationTask>),
            IEnumerable<SafetyCheckWrapper> pendingSafetyChecks = default(IEnumerable<SafetyCheckWrapper>))
        {
            this.NodeDeactivationIntent = nodeDeactivationIntent;
            this.NodeDeactivationStatus = nodeDeactivationStatus;
            this.NodeDeactivationTask = nodeDeactivationTask;
            this.PendingSafetyChecks = pendingSafetyChecks;
        }

        /// <summary>
        /// Gets the intent or the reason for deactivating the node. Following are the possible values for it. Possible values
        /// include: 'Invalid', 'Pause', 'Restart', 'RemoveData', 'RemoveNode'
        /// </summary>
        public NodeDeactivationIntent? NodeDeactivationIntent { get; }

        /// <summary>
        /// Gets the status of node deactivation operation. Following are the possible values. Possible values include: 'None',
        /// 'SafetyCheckInProgress', 'SafetyCheckComplete', 'Completed'
        /// </summary>
        public NodeDeactivationStatus? NodeDeactivationStatus { get; }

        /// <summary>
        /// Gets list of tasks representing the deactivation operation on the node.
        /// </summary>
        public IEnumerable<NodeDeactivationTask> NodeDeactivationTask { get; }

        /// <summary>
        /// Gets list of pending safety checks
        /// </summary>
        public IEnumerable<SafetyCheckWrapper> PendingSafetyChecks { get; }
    }
}
