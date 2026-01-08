// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents information about an operation in a terminal state (Completed or Faulted).
    /// </summary>
    public partial class InvokeDataLossResult
    {
        /// <summary>
        /// Initializes a new instance of the InvokeDataLossResult class.
        /// </summary>
        /// <param name="errorCode">If OperationState is Completed, this is 0.  If OperationState is Faulted, this is an error
        /// code indicating the reason.</param>
        /// <param name="selectedPartition">This class returns information about the partition that the user-induced operation
        /// acted upon.</param>
        public InvokeDataLossResult(
            int? errorCode = default(int?),
            SelectedPartition selectedPartition = default(SelectedPartition))
        {
            this.ErrorCode = errorCode;
            this.SelectedPartition = selectedPartition;
        }

        /// <summary>
        /// Gets if OperationState is Completed, this is 0.  If OperationState is Faulted, this is an error code indicating the
        /// reason.
        /// </summary>
        public int? ErrorCode { get; }

        /// <summary>
        /// Gets this class returns information about the partition that the user-induced operation acted upon.
        /// </summary>
        public SelectedPartition SelectedPartition { get; }
    }
}
