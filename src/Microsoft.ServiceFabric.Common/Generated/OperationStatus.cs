// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains the OperationId, OperationState, and OperationType for user-induced operations.
    /// </summary>
    public partial class OperationStatus
    {
        /// <summary>
        /// Initializes a new instance of the OperationStatus class.
        /// </summary>
        /// <param name="operationId">A GUID that identifies a call to this API.  This is also passed into the corresponding
        /// GetProgress API.</param>
        /// <param name="state">The state of the operation. Possible values include: 'Invalid', 'Running', 'RollingBack',
        /// 'Completed', 'Faulted', 'Cancelled', 'ForceCancelled'</param>
        /// <param name="type">The type of the operation. Possible values include: 'Invalid', 'PartitionDataLoss',
        /// 'PartitionQuorumLoss', 'PartitionRestart', 'NodeTransition'</param>
        public OperationStatus(
            Guid? operationId = default(Guid?),
            OperationState? state = default(OperationState?),
            OperationType? type = default(OperationType?))
        {
            this.OperationId = operationId;
            this.State = state;
            this.Type = type;
        }

        /// <summary>
        /// Gets a GUID that identifies a call to this API.  This is also passed into the corresponding GetProgress API.
        /// </summary>
        public Guid? OperationId { get; }

        /// <summary>
        /// Gets the state of the operation. Possible values include: 'Invalid', 'Running', 'RollingBack', 'Completed',
        /// 'Faulted', 'Cancelled', 'ForceCancelled'
        /// </summary>
        public OperationState? State { get; }

        /// <summary>
        /// Gets the type of the operation. Possible values include: 'Invalid', 'PartitionDataLoss', 'PartitionQuorumLoss',
        /// 'PartitionRestart', 'NodeTransition'
        /// </summary>
        public OperationType? Type { get; }
    }
}
