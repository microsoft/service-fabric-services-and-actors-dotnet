// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a Service Fabric property.
    /// </summary>
    public partial class PropertyInfo
    {
        /// <summary>
        /// Initializes a new instance of the PropertyInfo class.
        /// </summary>
        /// <param name="name">The name of the Service Fabric property.</param>
        /// <param name="metadata">The metadata associated with a property, including the property's name.</param>
        /// <param name="value">Describes a Service Fabric property value.</param>
        public PropertyInfo(
            string name,
            PropertyMetadata metadata,
            PropertyValue value = default(PropertyValue))
        {
            name.ThrowIfNull(nameof(name));
            metadata.ThrowIfNull(nameof(metadata));
            this.Name = name;
            this.Metadata = metadata;
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of the Service Fabric property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a Service Fabric property value.
        /// </summary>
        public PropertyValue Value { get; }

        /// <summary>
        /// Gets the metadata associated with a property, including the property's name.
        /// </summary>
        public PropertyMetadata Metadata { get; }
    }
}
