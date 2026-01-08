// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the results of a property batch.
    /// </summary>
    public abstract partial class PropertyBatchInfo
    {
        /// <summary>
        /// Initializes a new instance of the PropertyBatchInfo class.
        /// </summary>
        /// <param name="kind">The kind of property batch info, determined by the results of a property batch. The following
        /// are the possible values.</param>
        protected PropertyBatchInfo(
            PropertyBatchInfoKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the kind of property batch info, determined by the results of a property batch. The following are the possible
        /// values.
        /// </summary>
        public PropertyBatchInfoKind? Kind { get; }
    }
}
