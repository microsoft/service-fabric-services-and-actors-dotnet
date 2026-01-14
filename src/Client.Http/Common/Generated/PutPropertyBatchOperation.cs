// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Puts the specified property under the specified name.
    /// Note that if one PropertyBatchOperation in a PropertyBatch fails,
    /// the entire batch fails and cannot be committed in a transactional manner.
    /// </summary>
    public partial class PutPropertyBatchOperation : PropertyBatchOperation
    {
        /// <summary>
        /// Initializes a new instance of the PutPropertyBatchOperation class.
        /// </summary>
        /// <param name="propertyName">The name of the Service Fabric property.</param>
        /// <param name="value">Describes a Service Fabric property value.</param>
        /// <param name="customTypeId">The property's custom type ID. Using this property, the user is able to tag the type of
        /// the value of the property.</param>
        public PutPropertyBatchOperation(
            string propertyName,
            PropertyValue value,
            string customTypeId = default(string))
            : base(
                propertyName,
                Common.PropertyBatchOperationKind.Put)
        {
            value.ThrowIfNull(nameof(value));
            this.Value = value;
            this.CustomTypeId = customTypeId;
        }

        /// <summary>
        /// Gets a Service Fabric property value.
        /// </summary>
        public PropertyValue Value { get; }

        /// <summary>
        /// Gets the property's custom type ID. Using this property, the user is able to tag the type of the value of the
        /// property.
        /// </summary>
        public string CustomTypeId { get; }
    }
}
