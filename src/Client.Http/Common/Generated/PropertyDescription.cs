// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Description of a Service Fabric property.
    /// </summary>
    public partial class PropertyDescription
    {
        /// <summary>
        /// Initializes a new instance of the PropertyDescription class.
        /// </summary>
        /// <param name="propertyName">The name of the Service Fabric property.</param>
        /// <param name="value">Describes a Service Fabric property value.</param>
        /// <param name="customTypeId">The property's custom type ID. Using this property, the user is able to tag the type of
        /// the value of the property.</param>
        public PropertyDescription(
            string propertyName,
            PropertyValue value,
            string customTypeId = default(string))
        {
            propertyName.ThrowIfNull(nameof(propertyName));
            value.ThrowIfNull(nameof(value));
            this.PropertyName = propertyName;
            this.Value = value;
            this.CustomTypeId = customTypeId;
        }

        /// <summary>
        /// Gets the name of the Service Fabric property.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the property's custom type ID. Using this property, the user is able to tag the type of the value of the
        /// property.
        /// </summary>
        public string CustomTypeId { get; }

        /// <summary>
        /// Gets a Service Fabric property value.
        /// </summary>
        public PropertyValue Value { get; }
    }
}
