// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a PropertyBatchOperation that compares the Boolean existence of a property with the Exists argument.
    /// The PropertyBatchOperation operation fails if the property's existence is not equal to the Exists argument.
    /// The CheckExistsPropertyBatchOperation is generally used as a precondition for the write operations in the batch.
    /// Note that if one PropertyBatchOperation in a PropertyBatch fails,
    /// the entire batch fails and cannot be committed in a transactional manner.
    /// </summary>
    public partial class CheckExistsPropertyBatchOperation : PropertyBatchOperation
    {
        /// <summary>
        /// Initializes a new instance of the CheckExistsPropertyBatchOperation class.
        /// </summary>
        /// <param name="propertyName">The name of the Service Fabric property.</param>
        /// <param name="exists">Whether or not the property should exist for the operation to pass.</param>
        public CheckExistsPropertyBatchOperation(
            string propertyName,
            bool? exists)
            : base(
                propertyName,
                Common.PropertyBatchOperationKind.CheckExists)
        {
            exists.ThrowIfNull(nameof(exists));
            this.Exists = exists;
        }

        /// <summary>
        /// Gets whether or not the property should exist for the operation to pass.
        /// </summary>
        public bool? Exists { get; }
    }
}
