// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a Service Fabric property value.
    /// </summary>
    public abstract partial class PropertyValue
    {
        /// <summary>
        /// Initializes a new instance of the PropertyValue class.
        /// </summary>
        /// <param name="kind">The kind of property, determined by the type of data. Following are the possible values.</param>
        protected PropertyValue(
            PropertyValueKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the kind of property, determined by the type of data. Following are the possible values.
        /// </summary>
        public PropertyValueKind? Kind { get; }
    }
}
