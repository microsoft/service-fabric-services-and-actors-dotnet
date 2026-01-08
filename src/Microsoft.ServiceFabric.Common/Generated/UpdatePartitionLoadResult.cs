// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Specifies result of updating load for specified partitions. The output will be ordered based on the partition ID.
    /// </summary>
    public partial class UpdatePartitionLoadResult
    {
        /// <summary>
        /// Initializes a new instance of the UpdatePartitionLoadResult class.
        /// </summary>
        /// <param name="partitionId">Id of the partition.</param>
        /// <param name="partitionErrorCode">If OperationState is Completed - this is 0.  If OperationState is Faulted - this
        /// is an error code indicating the reason.</param>
        public UpdatePartitionLoadResult(
            PartitionId partitionId = default(PartitionId),
            int? partitionErrorCode = default(int?))
        {
            this.PartitionId = partitionId;
            this.PartitionErrorCode = partitionErrorCode;
        }

        /// <summary>
        /// Gets id of the partition.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets if OperationState is Completed - this is 0.  If OperationState is Faulted - this is an error code indicating
        /// the reason.
        /// </summary>
        public int? PartitionErrorCode { get; }
    }
}
