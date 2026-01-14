// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a PropertyBatchOperation that gets the specified property if it exists.
    /// Note that if one PropertyBatchOperation in a PropertyBatch fails,
    /// the entire batch fails and cannot be committed in a transactional manner.
    /// </summary>
    public partial class GetPropertyBatchOperation : PropertyBatchOperation
    {
        /// <summary>
        /// Initializes a new instance of the GetPropertyBatchOperation class.
        /// </summary>
        /// <param name="propertyName">The name of the Service Fabric property.</param>
        /// <param name="includeValue">Whether or not to return the property value with the metadata.
        /// True if values should be returned with the metadata; False to return only property metadata.
        /// </param>
        public GetPropertyBatchOperation(
            string propertyName,
            bool? includeValue = false)
            : base(
                propertyName,
                Common.PropertyBatchOperationKind.Get)
        {
            this.IncludeValue = includeValue;
        }

        /// <summary>
        /// Gets whether or not to return the property value with the metadata.
        /// True if values should be returned with the metadata; False to return only property metadata.
        /// </summary>
        public bool? IncludeValue { get; }
    }
}
