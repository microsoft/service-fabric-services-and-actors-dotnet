// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a PropertyBatchOperation that deletes a specified property if it exists.
    /// Note that if one PropertyBatchOperation in a PropertyBatch fails,
    /// the entire batch fails and cannot be committed in a transactional manner.
    /// </summary>
    public partial class DeletePropertyBatchOperation : PropertyBatchOperation
    {
        /// <summary>
        /// Initializes a new instance of the DeletePropertyBatchOperation class.
        /// </summary>
        /// <param name="propertyName">The name of the Service Fabric property.</param>
        public DeletePropertyBatchOperation(
            string propertyName)
            : base(
                propertyName,
                Common.PropertyBatchOperationKind.Delete)
        {
        }
    }
}
