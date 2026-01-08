// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Derived from PropertyBatchInfo. Represents the property batch failing. Contains information about the specific
    /// batch failure.
    /// </summary>
    public partial class FailedPropertyBatchInfo : PropertyBatchInfo
    {
        /// <summary>
        /// Initializes a new instance of the FailedPropertyBatchInfo class.
        /// </summary>
        /// <param name="errorMessage">The error message of the failed operation. Describes the exception thrown due to the
        /// first unsuccessful operation in the property batch.</param>
        /// <param name="operationIndex">The index of the unsuccessful operation in the property batch.</param>
        public FailedPropertyBatchInfo(
            string errorMessage = default(string),
            int? operationIndex = default(int?))
            : base(
                Common.PropertyBatchInfoKind.Failed)
        {
            this.ErrorMessage = errorMessage;
            this.OperationIndex = operationIndex;
        }

        /// <summary>
        /// Gets the error message of the failed operation. Describes the exception thrown due to the first unsuccessful
        /// operation in the property batch.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets the index of the unsuccessful operation in the property batch.
        /// </summary>
        public int? OperationIndex { get; }
    }
}
