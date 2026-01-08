// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the base type for property operations that can be put into a batch and submitted.
    /// </summary>
    public abstract partial class PropertyBatchOperation
    {
        /// <summary>
        /// Initializes a new instance of the PropertyBatchOperation class.
        /// </summary>
        /// <param name="propertyName">The name of the Service Fabric property.</param>
        /// <param name="kind">The kind of property batch operation, determined by the operation to be performed. The following
        /// are the possible values.</param>
        protected PropertyBatchOperation(
            string propertyName,
            PropertyBatchOperationKind? kind)
        {
            propertyName.ThrowIfNull(nameof(propertyName));
            kind.ThrowIfNull(nameof(kind));
            this.PropertyName = propertyName;
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the name of the Service Fabric property.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the kind of property batch operation, determined by the operation to be performed. The following are the
        /// possible values.
        /// </summary>
        public PropertyBatchOperationKind? Kind { get; }
    }
}
