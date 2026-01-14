// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Derived from PropertyBatchInfo. Represents the property batch succeeding. Contains the results of any "Get"
    /// operations in the batch.
    /// </summary>
    public partial class SuccessfulPropertyBatchInfo : PropertyBatchInfo
    {
        /// <summary>
        /// Initializes a new instance of the SuccessfulPropertyBatchInfo class.
        /// </summary>
        /// <param name="properties">A map containing the properties that were requested through any "Get" property batch
        /// operations. The key represents the index of the "Get" operation in the original request, in string form. The value
        /// is the property. If a property is not found, it will not be in the map.</param>
        public SuccessfulPropertyBatchInfo(
            IReadOnlyDictionary<string, PropertyInfo> properties = default(IReadOnlyDictionary<string, PropertyInfo>))
            : base(
                Common.PropertyBatchInfoKind.Successful)
        {
            this.Properties = properties;
        }

        /// <summary>
        /// Gets a map containing the properties that were requested through any "Get" property batch operations. The key
        /// represents the index of the "Get" operation in the original request, in string form. The value is the property. If
        /// a property is not found, it will not be in the map.
        /// </summary>
        public IReadOnlyDictionary<string, PropertyInfo> Properties { get; }
    }
}
