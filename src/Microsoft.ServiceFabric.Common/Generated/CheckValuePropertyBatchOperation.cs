// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a PropertyBatchOperation that compares the value of the property with the expected value.
    /// The CheckValuePropertyBatchOperation is generally used as a precondition for the write operations in the batch.
    /// Note that if one PropertyBatchOperation in a PropertyBatch fails,
    /// the entire batch fails and cannot be committed in a transactional manner.
    /// </summary>
    public partial class CheckValuePropertyBatchOperation : PropertyBatchOperation
    {
        /// <summary>
        /// Initializes a new instance of the CheckValuePropertyBatchOperation class.
        /// </summary>
        /// <param name="propertyName">The name of the Service Fabric property.</param>
        /// <param name="value">The expected property value.</param>
        public CheckValuePropertyBatchOperation(
            string propertyName,
            PropertyValue value)
            : base(
                propertyName,
                Common.PropertyBatchOperationKind.CheckValue)
        {
            value.ThrowIfNull(nameof(value));
            this.Value = value;
        }

        /// <summary>
        /// Gets the expected property value.
        /// </summary>
        public PropertyValue Value { get; }
    }
}
