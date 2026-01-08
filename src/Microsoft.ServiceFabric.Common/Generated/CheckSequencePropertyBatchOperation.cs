// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Compares the Sequence Number of a property with the SequenceNumber argument.
    /// A property's sequence number can be thought of as that property's version.
    /// Every time the property is modified, its sequence number is increased.
    /// The sequence number can be found in a property's metadata.
    /// The comparison fails if the sequence numbers are not equal.
    /// CheckSequencePropertyBatchOperation is generally used as a precondition for the write operations in the batch.
    /// Note that if one PropertyBatchOperation in a PropertyBatch fails,
    /// the entire batch fails and cannot be committed in a transactional manner.
    /// </summary>
    public partial class CheckSequencePropertyBatchOperation : PropertyBatchOperation
    {
        /// <summary>
        /// Initializes a new instance of the CheckSequencePropertyBatchOperation class.
        /// </summary>
        /// <param name="propertyName">The name of the Service Fabric property.</param>
        /// <param name="sequenceNumber">The expected sequence number.</param>
        public CheckSequencePropertyBatchOperation(
            string propertyName,
            string sequenceNumber)
            : base(
                propertyName,
                Common.PropertyBatchOperationKind.CheckSequence)
        {
            sequenceNumber.ThrowIfNull(nameof(sequenceNumber));
            this.SequenceNumber = sequenceNumber;
        }

        /// <summary>
        /// Gets the expected sequence number.
        /// </summary>
        public string SequenceNumber { get; }
    }
}
