// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a list of property batch operations to be executed. Either all or none of the operations will be
    /// committed.
    /// </summary>
    public partial class PropertyBatchDescriptionList
    {
        /// <summary>
        /// Initializes a new instance of the PropertyBatchDescriptionList class.
        /// </summary>
        /// <param name="operations">A list of the property batch operations to be executed.</param>
        public PropertyBatchDescriptionList(
            IEnumerable<PropertyBatchOperation> operations = default(IEnumerable<PropertyBatchOperation>))
        {
            this.Operations = operations;
        }

        /// <summary>
        /// Gets a list of the property batch operations to be executed.
        /// </summary>
        public IEnumerable<PropertyBatchOperation> Operations { get; }
    }
}
